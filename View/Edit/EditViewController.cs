using System;
using Michsky.UI.ModernUIPack;
using System.Collections.Generic;
using easyar;
using SpatialMap_SparseSpatialMap;
using UnityEngine;

namespace ZBoom.Common.SpatialMap
{
    public class EditViewController : MonoBehaviour
    {
        public SpatialMapGameObjectController SpatialMapGameObjectController;
        public GameObject ProgressPanel;
        public GameObject ErrorPanel;
        public SwitchManager DenseSpatialMapSwitch;
        public SwitchManager OcclustionSwitch;
        public Material MeshMaterial;

        private MapSession m_MapSession;
        private MapSession.MapData m_MapData;
        private SpatialMapData m_SpatialMapData;
        private SparseSpatialMapController m_SparseSpatialMapController;
        private DenseSpatialMapBuilderFrameFilter m_DenseSpatialMapBuilderFrameFilter;
        private bool m_IsTipsOn;

        private GameObject m_DownloadedMesh = null;

        private void Awake()
        {
            SpatialMapGameObjectController.CreateObjectEvent += (gameObj) =>
            {
                if (gameObj)
                {
                    gameObj.transform.parent = m_MapData.Controller.transform;
                    m_MapData.Props.Add(gameObj);
                }
            };
            SpatialMapGameObjectController.DeleteObjectEvent += (gameObj) =>
            {
                if (gameObj)
                {
                    m_MapData.Props.Remove(gameObj);
                }
            };
        }

        private void OnEnable()
        {
            m_IsTipsOn = false;
            UseDenseSpatialMap(DenseSpatialMapSwitch.isOn);
        }

        public void SetMapSession(MapSession session)
        {
            m_MapData = session.Maps[0];
            m_MapSession = session;
            SpatialMapGameObjectController.SetMapSession(session);

            m_DenseSpatialMapBuilderFrameFilter =
                m_MapSession.ARSession.GetComponentInChildren<DenseSpatialMapBuilderFrameFilter>(true);
            if (m_DenseSpatialMapBuilderFrameFilter != null)
            {
                m_DenseSpatialMapBuilderFrameFilter.gameObject.SetActive(true);
                m_DenseSpatialMapBuilderFrameFilter.RenderMesh = true;

                UseDenseSpatialMap(DenseSpatialMapSwitch.isOn);
            }

            m_DownloadedMesh = null;
        }

        public void SetSpatialMap(SpatialMapData spatialMapData, SparseSpatialMapController sparseSpatialMapController)
        {
            m_SpatialMapData = spatialMapData;
            m_SparseSpatialMapController = sparseSpatialMapController;
        }

        public void ImportMesh(GameObject parent)
        {
            if (m_DownloadedMesh == null)
            {
                ViewManager.Instance.LoadMesh(m_SpatialMapData, new IResultListener<GameObject>()
                {
                    OnSuccess = (meshGameObject, message) =>
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            m_DownloadedMesh = meshGameObject;
                            m_DownloadedMesh.transform.parent = parent.transform;

                            m_DownloadedMesh.transform.localPosition = new Vector3(
                                m_SpatialMapData.mesh.position[0],
                                m_SpatialMapData.mesh.position[1],
                                m_SpatialMapData.mesh.position[2]);

                            m_DownloadedMesh.transform.localRotation = new Quaternion(
                                m_SpatialMapData.mesh.rotation[0],
                                m_SpatialMapData.mesh.rotation[1],
                                m_SpatialMapData.mesh.rotation[2],
                                m_SpatialMapData.mesh.rotation[3]);

                            m_DownloadedMesh.transform.localScale = new Vector3(
                                m_SpatialMapData.mesh.scale[0],
                                m_SpatialMapData.mesh.scale[1],
                                m_SpatialMapData.mesh.scale[2]);

                            var meshes = m_DownloadedMesh.GetComponentsInChildren<MeshRenderer>();
                            foreach (var meshRenderer in meshes)
                            {
                                meshRenderer.material = MeshMaterial;
                            }

                            UseOcclusion(OcclustionSwitch.isOn);
                        });
                    },
                    OnError = errorMessage => { }
                });
            }
        }

        public void Save(bool isSaveToMetaManager = false)
        {
            if (m_MapData == null)
            {
                return;
            }

            ShowLoading(true);
            ShowError(false);

            List<MapMeta.PropInfo> propInfos = new List<MapMeta.PropInfo>();
            SpatialMapData spatialMapData = new SpatialMapData()
            {
                id = m_MapData.Meta.Map.ID,
                name = m_MapData.Meta.Map.Name
            };

            foreach (var prop in m_MapData.Props)
            {
                var propName = prop.name;
                var position = prop.transform.localPosition;
                var rotation = prop.transform.localRotation;
                var eulerRotation = prop.transform.localRotation.eulerAngles;
                var scale = prop.transform.localScale;

                propInfos.Add(new MapMeta.PropInfo()
                {
                    Name = propName,
                    Position = new float[3] {position.x, position.y, position.z},
                    Rotation = new float[4] {rotation.x, rotation.y, rotation.z, rotation.w},
                    Scale = new float[3] {scale.x, scale.y, scale.z}
                });

                SpatialMapItemData spatialMapItemData = new SpatialMapItemData()
                {
                    id = propName,
                    name = propName,
                    position = new float[3] {position.x, position.y, position.z},
                    rotation = new float[4] {rotation.x, rotation.y, rotation.z, rotation.w},
                    eulerRotation = new float[3] {eulerRotation.x, eulerRotation.y, eulerRotation.z},
                    scale = new float[3] {scale.x, scale.y, scale.z}
                };

                BaseRaribleItemController raribleItemController = prop.GetComponent<BaseRaribleItemController>();
                if (raribleItemController != null)
                {
                    spatialMapItemData.isNft = true;
                    spatialMapItemData.nft = raribleItemController.RaribleItem;
                }

                spatialMapData.mapItems.Add(spatialMapItemData);
            }

            m_MapData.Meta.Props = propInfos;
            if (isSaveToMetaManager)
            {
                MapMetaManager.Save(m_MapData.Meta);
            }

            string date = DateHelper.GetCurrentDate();
            spatialMapData.updatedDate = date;

            ViewManager.Instance.SaveSpatialMap(spatialMapData, new IResultListener<SpatialMapData>()
            {
                OnSuccess = (spatialMapData, message) =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        ShowLoading(false);
                        ShowError(false);

                        gameObject.SetActive(false);
                        ViewManager.Instance.LoadMainView();
                    });
                },

                OnError = (errorMessage) =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        ShowLoading(false);
                        ShowError(true);

                        //ShowError();
                    });
                }
            });

            //ViewManager.Instance.SaveSpatialMap(spatialMapData);
        }

        public void ShowError(bool isShow)
        {
            if (isShow)
            {
                ErrorPanel.SetActive(true);
            }
            else
            {
                ErrorPanel.SetActive(false);
            }
        }

        public void ShowLoading(bool isShow)
        {
            if (isShow)
            {
                ProgressPanel.SetActive(true);
            }
            else
            {
                ProgressPanel.SetActive(false);
            }
        }

        public void UseDenseSpatialMap(bool useDenseSpatialMap)
        {
            if (m_DenseSpatialMapBuilderFrameFilter)
            {
                if (useDenseSpatialMap)
                {
                    m_DenseSpatialMapBuilderFrameFilter.RenderMesh = true;
                    m_DenseSpatialMapBuilderFrameFilter.UseCollider = true;
                }
                else
                {
                    m_DenseSpatialMapBuilderFrameFilter.RenderMesh = false;
                    m_DenseSpatialMapBuilderFrameFilter.UseCollider = false;
                }
            }
        }

        public void UseOcclusion(bool useOcclusion)
        {
            if (m_DownloadedMesh != null)
            {
                if (useOcclusion)
                {
                    m_DownloadedMesh.gameObject.SetActive(true);
                }
                else
                {
                    m_DownloadedMesh.gameObject.SetActive(false);
                }
            }
        }
    }
}