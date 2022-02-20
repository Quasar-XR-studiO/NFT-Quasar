using easyar;
using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using Rarible;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class ViewManager : MonoBehaviour
    {
        public static ViewManager Instance;

        public GameObject EasyARSession;
        public SpatialMapFirebaseController RealtimeFirebaseController;
        public SpatialMapStorageFirebaseController StorageFirebaseController;
        public RaribleApiController RaribleApiController;
        public SparseSpatialMapController MapControllerPrefab;
        public MainViewController MainView;
        public CreateViewController CreateView;
        public ScanViewController ScanView;
        public EditViewController EditView;
        public PreviewViewController Preview;
        public Text Status;
        public SwitchManager CloudPointSwitch;
        public bool MainViewRecycleBinClearMapCacheOnly;
        public RaribleContentCreator RaribleContentCreator;
        public FloorController FloorController;

        public bool ShowStatus = false;

        private GameObject m_EasyarObject;
        private ARSession m_Session;
        private VIOCameraDeviceUnion m_VioCamera;
        private SparseSpatialMapWorkerFrameFilter m_MapFrameFilter;
        private List<MapMeta> m_SelectedMaps = new List<MapMeta>();
        private List<SpatialMapData> m_SpatialMaps = new List<SpatialMapData>();
        private MapSession m_MapSession;
        private string m_DeviceModel = string.Empty;


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void ImportSampleStreamingAssets()
        {
            FileUtil.ImportSampleStreamingAssets();
        }
#endif

        private void Awake()
        {
            Instance = this;
            MainView.gameObject.SetActive(false);
            CreateView.gameObject.SetActive(false);
            EditView.gameObject.SetActive(false);
            Preview.gameObject.SetActive(false);
        }

        private void Start()
        {
            LoadMainView();
        }

        private void Update()
        {
            if (m_Session)
            {
                if (ShowStatus)
                {
                    Status.text = $"Device Model: {SystemInfo.deviceModel} {m_DeviceModel}" + Environment.NewLine +
                                  "VIO Device" + Environment.NewLine +
                                  "\tType: " +
                                  ((m_Session.Assembly != null && m_Session.Assembly.FrameSource)
                                      ? m_Session.Assembly.FrameSource.GetType().ToString().Replace("easyar.", "")
                                          .Replace("FrameSource", "")
                                      : "-") + Environment.NewLine +
                                  "\tTracking Status: " + m_Session.TrackingStatus + Environment.NewLine +
                                  "CenterMode: " + m_Session.CenterMode + Environment.NewLine +
                                  "Sparse Spatial Map" + Environment.NewLine +
                                  "\tWorking Mode: " + m_MapFrameFilter.WorkingMode + Environment.NewLine +
                                  "\tLocalization Mode: " + m_MapFrameFilter.LocalizerConfig.LocalizationMode +
                                  Environment.NewLine +
                                  "Localized Map" + Environment.NewLine +
                                  "\tName: " +
                                  (m_MapFrameFilter.LocalizedMap == null
                                      ? "-"
                                      : (m_MapFrameFilter.LocalizedMap.MapInfo == null
                                          ? "-"
                                          : m_MapFrameFilter.LocalizedMap.MapInfo.Name)) + Environment.NewLine +
                                  "\tID: " + (m_MapFrameFilter.LocalizedMap == null
                                      ? "-"
                                      : (m_MapFrameFilter.LocalizedMap.MapInfo == null
                                          ? "-"
                                          : m_MapFrameFilter.LocalizedMap.MapInfo.ID)) + Environment.NewLine +
                                  "\tPoint Cloud Count: " + (m_MapFrameFilter.LocalizedMap == null
                                      ? "-"
                                      : m_MapFrameFilter.LocalizedMap.PointCloud.Count.ToString());
                }

                if (m_MapFrameFilter.LocalizedMap == null)
                {
                    CloudPointSwitch.gameObject.SetActive(false);
                }
                else
                {
                    CloudPointSwitch.gameObject.SetActive(true);
                }
            }
            else
            {
                Status.text = string.Empty;
            }
        }

        private void OnDestroy()
        {
            DestroySession();
        }

        public void SelectMaps(List<MapMeta> metas)
        {
            m_SelectedMaps = metas;
            MainView.EnablePreview(m_SelectedMaps.Count > 0);
            MainView.EnableEdit(m_SelectedMaps.Count == 1);
            MainView.EnableScan(m_SelectedMaps.Count == 1);
            MainView.EnableRemove(m_SelectedMaps.Count > 0);
        }

        public void LoadMainView(bool isLoadData = true)
        {
            DestroySession();
            SelectMaps(new List<MapMeta>());
            MainView.gameObject.SetActive(true);
            if (isLoadData)
            {
                GetSpatialMaps();
            }
        }

        public void LoadCreateView()
        {
            CreateSession();
            m_MapSession.SetupMapBuilder(MapControllerPrefab);
            CreateView.SetMapSession(m_MapSession);
            CreateView.gameObject.SetActive(true);
        }

        public void LoadScanView()
        {
            var spatialMapData = m_SpatialMaps.Find(data => data.id.Equals(m_SelectedMaps[0].Map.ID));
            if (spatialMapData != null)
            {
                CreateSession();
                ScanView.gameObject.SetActive(true);

                m_MapSession.LoadMapMeta(
                    MapControllerPrefab,
                    true,
                    false,
                    mapLoadEvent: (sparseSpatialMapController, mapInfo) =>
                    {
                        ScanView.SetSpatialMap(spatialMapData, sparseSpatialMapController);
                    },
                    mapLocalized: (sparseSpatialMapController) => { ScanView.MapLocalize(); }
                );

                ScanView.SetMapSession(m_MapSession);
                ScanView.InitDenseSpatialMap();
                CloudPointSwitch.isOn = true;
            }
        }

        public void LoadEditView()
        {
            var spatialMapData = m_SpatialMaps.Find(data => data.id.Equals(m_SelectedMaps[0].Map.ID));
            if (spatialMapData != null)
            {
                CreateSession();
                m_MapSession.LoadMapMeta(
                    MapControllerPrefab,
                    true,
                    objectCreatedEvent: (sparseSpatialMapController, createdObject, propInfo) =>
                    {
                        ObjectAR objectAR = createdObject.GetComponent<ObjectAR>();
                        if (objectAR != null)
                        {
                            objectAR.SetState(ObjectAR.State.EDIT);
                        }
                    },
                    mapLoadEvent: (sparseSpatialMapController, info) =>
                    {
                        EditView.SetSpatialMap(spatialMapData, sparseSpatialMapController);
                    },
                    mapLocalized: (sparseSpatialMapController) =>
                    {
                        if (spatialMapData.mesh.fullUrlMesh.Length > 1)
                        {
                            EditView.ImportMesh(sparseSpatialMapController.gameObject);
                        }
                        
                        StartCoroutine(CreateWithDelay(sparseSpatialMapController, spatialMapData));
                    }
                );

                EditView.SetMapSession(m_MapSession);
                EditView.gameObject.SetActive(true);
                CloudPointSwitch.isOn = true;
            }
        }

        public void LoadEditView(MapSession mapSession, string id)
        {
            if (mapSession != null)
            {
                m_MapSession = mapSession;
                CreateView.gameObject.SetActive(false);
                Debug.Log("ViewManager: 1");
                Debug.Log("ViewManager: 2");

                var mapData = new MapSession.MapData();
                var mapMeta = new MapMeta(m_MapSession.MapWorker.BuilderMapController.MapInfo,
                    new List<MapMeta.PropInfo>());
                //Debug.Log("ViewManager: 2 1 " + m_MapSession.MapWorker.BuilderMapController.MapInfo.Name); //NULL
                var mapMeta1 = MapMetaManager.LoadAll().Find(item => item.Map.ID.Equals(id));

                Debug.Log("ViewManager: 2 2 " + mapMeta1.Map.Name);
                //mapData.Meta = mapMeta;
                //mapData.Controller = MapControllerPrefab;
                //mapData.Props = new List<GameObject>();
                //m_MapSession.Maps.Add(mapData);

                m_MapSession.Maps = new List<MapSession.MapData>();
                //m_MapSession.Maps[0] = new MapSession.MapData() { Meta = mapMeta };
                m_MapSession.Maps.Add(new MapSession.MapData() {Meta = mapMeta1});

                //m_MapSession.LoadMapMeta(m_MapSession.MapWorker.BuilderMapController);
                m_MapSession.LoadMapMeta(MapControllerPrefab, true);
                Debug.Log("ViewManager: 2 5");

                EditView.SetMapSession(m_MapSession);
                Debug.Log("ViewManager: 3");
                EditView.gameObject.SetActive(true);
                CloudPointSwitch.isOn = true;
            }
            else
            {
                LoadEditView();
            }
        }

        public void LoadPreviewView()
        {
            CreateSession();
            m_MapSession.LoadMapMeta(
                MapControllerPrefab,
                false,
                objectCreatedEvent: (sparseSpatialMapController, createdObject, propInfo) =>
                {
                    ObjectAR objectAR = createdObject.GetComponent<ObjectAR>();
                    if (objectAR != null)
                    {
                        objectAR.SetState(ObjectAR.State.PREVIEW);
                    }
                },
                mapLoadEvent: (sparseSpatialMapController, info) => { },
                mapLocalized: (sparseSpatialMapController) =>
                {
                    var spatialMapData =
                        m_SpatialMaps.Find(data => data.id.Equals(sparseSpatialMapController.MapInfo.ID));
                    if (spatialMapData.mesh.fullUrlMesh.Length > 1)
                    {
                        Preview.LocalizeSpatialMap(spatialMapData, sparseSpatialMapController);
                    }

                    StartCoroutine(CreateWithDelay(sparseSpatialMapController, spatialMapData));
                    /*
                    ObjectAR[] arObjects = sparseSpatialMapController.GetComponentsInChildren<ObjectAR>(true);
                    for (int i = 0; i < arObjects.Length; i++)
                    {
                        ObjectAR objectAR = arObjects[i];
                        SpatialMapItemData spatialMapItemData = spatialMapData.mapItems[i];
                        //if (spatialMapItemData.isNft)
                        {
                            BaseRaribleItemController raribleItemController =
                                objectAR.GetComponent<BaseRaribleItemController>();
                            if (raribleItemController != null)
                            {
                                Content mainContent = RaribleContentCreator.GetMainContent(spatialMapItemData.nft);
                                //raribleItemController.Create(spatialMapItemData.nft, mainContent);
                                //StartCoroutine(CreateWithDelay(raribleItemController, spatialMapItemData.nft, mainContent));
                                raribleItemController.CreateWithDelay(spatialMapItemData.nft, mainContent);
                                
                                Star
                            }
                        }
                    }
                    */
                }
                //objectCreatedEvent:ModifyObject
                //mapLoadEvent:PreviewMapLoad,
                //mapLocalized:() => AdminSlamWorldController.SetActive()
            );
            Preview.SetMapSession(m_MapSession);
            Preview.gameObject.SetActive(true);
        }
        
        private IEnumerator CreateWithDelay(
            SparseSpatialMapController sparseSpatialMapController,
            SpatialMapData spatialMapData)
        {
            yield return new WaitForSeconds(1f);
            ObjectAR[] arObjects = sparseSpatialMapController.GetComponentsInChildren<ObjectAR>(true);
            for (int i = 0; i < arObjects.Length; i++)
            {
                ObjectAR objectAR = arObjects[i];
                SpatialMapItemData spatialMapItemData = spatialMapData.mapItems[i];
                int count = 0;
                while (!objectAR.gameObject.activeSelf)
                {
                    count++;
                    if (count > 5)
                    {
                        break;
                    }
                    else
                    {
                        yield return new WaitForSeconds(1f);
                    }
                }

                //if (spatialMapItemData.isNft)
                if (objectAR.gameObject.activeSelf)
                {
                    BaseRaribleItemController raribleItemController =
                        objectAR.GetComponent<BaseRaribleItemController>();
                    if (raribleItemController != null)
                    {
                        Content mainContent = RaribleContentCreator.GetMainContent(spatialMapItemData.nft);
                        raribleItemController.Create(spatialMapItemData.nft, mainContent);
                    }
                }
            }
        }

        private IEnumerator CreateWithDelay1(BaseRaribleItemController raribleItemController, RaribleItem raribleItem,
            Content content)
        {
            yield return new WaitForSeconds(1f);
            if (raribleItemController.gameObject.activeSelf)
            {
                raribleItemController.Create(raribleItem, content);
            }
            else
            {
                //StartCoroutine(CreateWithDelay(raribleItemController, raribleItem, content));
            }
        }

        private void PreviewMapLoad(SparseSpatialMapController.SparseSpatialMapInfo map)
        {
            //AdminSlamWorldController.SetActive();
        }

        public void ShowParticle(bool show)
        {
            if (m_MapSession == null)
            {
                return;
            }

            foreach (var map in m_MapSession.Maps)
            {
                if (map.Controller)
                {
                    map.Controller.ShowPointCloud = show;
                }
            }
        }

        private void CreateSession()
        {
            m_EasyarObject = Instantiate(EasyARSession);
            m_EasyarObject.SetActive(true);
            m_Session = m_EasyarObject.GetComponent<ARSession>();
            m_VioCamera = m_EasyarObject.GetComponentInChildren<VIOCameraDeviceUnion>();
            m_MapFrameFilter = m_EasyarObject.GetComponentInChildren<SparseSpatialMapWorkerFrameFilter>();

            //TODO: INIT MAP SESSION

            //m_MapSession = new MapSession(m_MapFrameFilter, m_SelectedMaps);
            m_MapSession = new MapSession(m_Session, m_MapFrameFilter, m_SelectedMaps);

            if (FloorController != null)
            {
                FloorController.Reset(m_VioCamera);
            }
        }

        private void DestroySession()
        {
            if (m_MapSession != null)
            {
                m_MapSession.Dispose();
                m_MapSession = null;
            }

            if (m_EasyarObject)
            {
                Destroy(m_EasyarObject);
            }
        }

        #region Firebase RealtimeDb

        public void GetSpatialMaps()
        {
            MainView.ShowError(false);
            MainView.ShowLoading(true);

            RealtimeFirebaseController.GetSpatialMap(new IResultListener<List<SpatialMapData>>()
            {
                OnError = errorMessage =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        MainView.ShowLoading(false);
                        MainView.ShowError(true);
                    });
                },
                OnSuccess = (spatialMaps, message) =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        m_SelectedMaps.Clear();

                        List<MapMeta> mapMetas = new List<MapMeta>();
                        foreach (var spatialMapData in spatialMaps)
                        {
                            SparseSpatialMapController.SparseSpatialMapInfo spatialMapInfo =
                                new SparseSpatialMapController.SparseSpatialMapInfo();
                            spatialMapInfo.Name = spatialMapData.name;
                            spatialMapInfo.ID = spatialMapData.id;

                            List<MapMeta.PropInfo> props = new List<MapMeta.PropInfo>();

                            foreach (var spatialMapItemData in spatialMapData.mapItems)
                            {
                                MapMeta.PropInfo propInfo = new MapMeta.PropInfo();
                                propInfo.Name = spatialMapItemData.id;

                                propInfo.Position = spatialMapItemData.position;
                                propInfo.Rotation = spatialMapItemData.rotation;
                                propInfo.Scale = spatialMapItemData.scale;

                                /*
                                propInfo.Position = new float[3]
                                {
                                    spatialMapItemData.positionX,
                                    spatialMapItemData.positionY,
                                    spatialMapItemData.positionZ
                                };

                                propInfo.Rotation = new float[3]
                                {
                                    spatialMapItemData.rotationX,
                                    spatialMapItemData.rotationY,
                                    spatialMapItemData.rotationZ
                                };

                                propInfo.Scale = new float[3]
                                {
                                    spatialMapItemData.scaleX,
                                    spatialMapItemData.scaleY,
                                    spatialMapItemData.scaleZ
                                };
                                */


                                props.Add(propInfo);
                            }

                            MapMeta mapMeta = new MapMeta(spatialMapInfo, props);
                            mapMetas.Add(mapMeta);
                        }

                        m_SpatialMaps = spatialMaps;
                        MainView.SetData(mapMetas, spatialMaps);

                        MainView.ShowLoading(false);
                        MainView.ShowError(false);
                    });
                }
            });
        }

        public void SaveSpatialMap(SpatialMapData spatialMapData)
        {
            RealtimeFirebaseController.SaveSpatialMap(spatialMapData, new IResultListener<SpatialMapData>()
            {
                OnSuccess = (spatialMapData, message) => { UnityMainThreadDispatcher.Instance().Enqueue(() => { }); },
                OnError = (errorMessage) => { UnityMainThreadDispatcher.Instance().Enqueue(() => { }); }
            });
        }

        public void SaveSpatialMap(SpatialMapData spatialMapData, IResultListener<SpatialMapData> resultListener)
        {
            RealtimeFirebaseController.SaveSpatialMap(spatialMapData, resultListener);
        }

        public void RemoveSpatialMap(string idSpatialMap, IResultListener<SpatialMapData> resultListener)
        {
            RealtimeFirebaseController.RemoveSpatialMap(idSpatialMap, resultListener);
        }

        #endregion

        #region Storage Firebase

        public void UploadMesh(byte[] data, string name, IResultListener<string> resultListener)
        {
            StorageFirebaseController.UploadFile(data, name, resultListener);
        }

        public void LoadMesh(SpatialMapData spatialMapData, IResultListener<GameObject> resultListener)
        {
            StorageFirebaseController.DownloadUrlAndFile(spatialMapData.mesh.fullUrlMesh, spatialMapData.mesh.nameMesh,
                resultListener);
        }

        public void RemoveMesh(SpatialMapData spatialMapData, IResultListener<string> resultListener)
        {
            StorageFirebaseController.RemoveFile(spatialMapData.mesh.nameMesh, resultListener);
        }

        #endregion

        #region Rarible

        public void GetRaribleItem(string id, IResultListener<RaribleItem> resultListener)
        {
            RaribleApiController.GetItemById(id, resultListener);
        }
        
        public void GetRaribleItems(int size, IResultListener<RaribleCollection> resultListener)
        {
            RaribleApiController.GetItems(resultListener, size);
        }

        #endregion
    }
}