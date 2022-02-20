using System;
using Michsky.UI.ModernUIPack;
using System.Collections.Generic;
using easyar;
using SpatialMap_SparseSpatialMap;
using UnityEngine;

namespace ZBoom.Common.SpatialMap
{
    public class PreviewViewController : MonoBehaviour
    {
        public SwitchManager MeshOcclustionSwitch;
        public SwitchManager PrimitiveOcclustionSwitch;
        public Material MeshMaterial;

        private MapSession m_MapSession;
        private List<SpatialMapData> m_SpatialMaps = new List<SpatialMapData>();
        private List<SparseSpatialMapController> m_SparseSpatialMapControllers = new List<SparseSpatialMapController>();
        private List<GameObject> m_DownloadedMeshes = new List<GameObject>();

        private void Awake()
        {
        }

        private void OnEnable()
        {
        }

        public void SetMapSession(MapSession session)
        {
            m_MapSession = session;
            m_SpatialMaps.Clear();
            m_SparseSpatialMapControllers.Clear();
            m_DownloadedMeshes.Clear();
        }

        public void SetSpatialMaps(List<SpatialMapData> spatialMapData, List<SparseSpatialMapController> sparseSpatialMapController)
        {
            m_SpatialMaps = spatialMapData;
        }

        public void LocalizeSpatialMap(SpatialMapData spatialMapData, SparseSpatialMapController sparseSpatialMapController)
        {
            if (!m_SpatialMaps.Exists(data => data.id.Equals(spatialMapData.id)))
            {
                m_SparseSpatialMapControllers.Add(sparseSpatialMapController);
                m_SpatialMaps.Add(spatialMapData);
                if (spatialMapData.mesh.fullUrlMesh.Length > 1)
                {
                    ImportMesh(spatialMapData, sparseSpatialMapController.gameObject);
                }
                
                UsePrimitiveOcclusion(MeshOcclustionSwitch.isOn);
            }
        }

        public void ImportMesh(SpatialMapData spatialMapData, GameObject parent)
        {
            ViewManager.Instance.LoadMesh(spatialMapData, new IResultListener<GameObject>()
            {
                OnSuccess = (meshGameObject, message) =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        meshGameObject.transform.parent = parent.transform;

                        meshGameObject.transform.localPosition = new Vector3(
                            spatialMapData.mesh.position[0],
                            spatialMapData.mesh.position[1],
                            spatialMapData.mesh.position[2]);

                        meshGameObject.transform.localRotation = new Quaternion(
                            spatialMapData.mesh.rotation[0],
                            spatialMapData.mesh.rotation[1],
                            spatialMapData.mesh.rotation[2],
                            spatialMapData.mesh.rotation[3]);

                        meshGameObject.transform.localScale = new Vector3(
                            spatialMapData.mesh.scale[0],
                            spatialMapData.mesh.scale[1],
                            spatialMapData.mesh.scale[2]);

                        var meshes = meshGameObject.GetComponentsInChildren<MeshRenderer>();
                        foreach (var meshRenderer in meshes)
                        {
                            meshRenderer.material = MeshMaterial;
                        }
                        
                        m_DownloadedMeshes.Add(meshGameObject);
                        UseMeshOcclusion(MeshOcclustionSwitch.isOn);
                    });
                },
                OnError = errorMessage => { }
            });
        }

        public void UseMeshOcclusion(bool useOcclusion)
        {
            foreach (var downloadedMesh in m_DownloadedMeshes)
            {
                if (useOcclusion)
                {
                    downloadedMesh.gameObject.SetActive(true);
                }
                else
                {
                    downloadedMesh.gameObject.SetActive(false);
                }
            }
        }
        
        public void UsePrimitiveOcclusion(bool useOcclusion)
        {
            var primitives = FindObjectsOfType<OcclusionFigureController>(true);
            foreach (var primitiveMesh in primitives)
            {
                if (useOcclusion)
                {
                    primitiveMesh.gameObject.SetActive(true);
                }
                else
                {
                    primitiveMesh.gameObject.SetActive(false);
                }
            }
        }
    }
}