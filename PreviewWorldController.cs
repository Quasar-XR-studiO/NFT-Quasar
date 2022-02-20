using easyar;
using System;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using SpatialMap_SparseSpatialMap;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class PreviewWorldController : MonoBehaviour
    {
        [SerializeField] public SliderManager WindSliderManager;
        [SerializeField] public bool UseShields = true;

        public GameObject EasyARSession;
        public SpatialMapFirebaseController FirebaseController;
        public SparseSpatialMapController MapControllerPrefab;

        public GameObject ProgressPanel;
        public GameObject ErrorPanel;

        private GameObject m_EasyarObject;
        private ARSession m_Session;
        private VIOCameraDeviceUnion m_VioCamera;
        private SparseSpatialMapWorkerFrameFilter m_MapFrameFilter;
        private List<MapMeta> m_SelectedMaps = new List<MapMeta>();
        private MapSession m_MapSession;


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void ImportSampleStreamingAssets()
        {
            FileUtil.ImportSampleStreamingAssets();
        }
#endif

        private void Awake()
        {
        }

        private void Start()
        {
            GetSpatialMaps();
        }

        private void Update()
        {
        }

        private void OnDestroy()
        {
            DestroySession();
        }

        private void CreateSession()
        {
            m_EasyarObject = Instantiate(EasyARSession);
            m_EasyarObject.SetActive(true);
            m_Session = m_EasyarObject.GetComponent<ARSession>();
            m_VioCamera = m_EasyarObject.GetComponentInChildren<VIOCameraDeviceUnion>();
            m_MapFrameFilter = m_EasyarObject.GetComponentInChildren<SparseSpatialMapWorkerFrameFilter>();

            m_MapSession = new MapSession(m_Session, m_MapFrameFilter, m_SelectedMaps);
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

        public void SetActive()
        {
        }


        public void GetSpatialMaps()
        {
            ProgressPanel.SetActive(true);
            ErrorPanel.SetActive(false);

            FirebaseController.GetSpatialMap(new IResultListener<List<SpatialMapData>>()
            {
                OnError = errorMessage =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        ProgressPanel.SetActive(false);
                        ErrorPanel.SetActive(true);
                    });
                },
                OnSuccess = (spatialMaps, message) =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
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

                                props.Add(propInfo);
                            }

                            MapMeta mapMeta = new MapMeta(spatialMapInfo, props);
                            m_SelectedMaps.Add(mapMeta);
                        }

                        ProgressPanel.SetActive(false);
                        ErrorPanel.SetActive(false);


                        Clear();
                        CreateSession();
                        //m_MapSession.LoadMapMeta(MapControllerPrefab, false);

                        m_MapSession.LoadMapMeta(
                            MapControllerPrefab,
                            false,
                            //objectCreatedEvent: ModifyObject,
                            //mapLoadEvent:PreviewMapLoad,
                            mapLocalized: (sparseSpatialMapController) => SetActive()
                        );
                    });
                }
            });
        }

        public void Clear()
        {
        }
    }
}