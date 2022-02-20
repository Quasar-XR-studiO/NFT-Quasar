using System;
using System.Collections;
using Michsky.UI.ModernUIPack;
using System.Collections.Generic;
using System.IO;
using easyar;
using Parabox.Stl;
using SpatialMap_SparseSpatialMap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class ScanViewController : MonoBehaviour
    {
        public GameObject UploadPopup;
        public GameObject ProgressBar;

        public Button UploadServerDataButton;
        public Button UploadServerMeshButton;
        public Button UploadCancelButton;
        public Button SaveButton;

        private MapSession m_MapSession;
        private MapSession.MapData m_MapData;
        private SpatialMapData m_SpatialMapData;
        private DenseSpatialMapBuilderFrameFilter m_DenseSpatialMapBuilderFrameFilter;
        private SparseSpatialMapController m_SparseSpatialMapController;

        private string m_MapName = null;
        private string m_MapId = null;
        private string m_FullPath = null;
        private string m_NameMesh = null;
        private string m_UrlMesh = null;

        private void Awake()
        {
        }

        private void Start()
        {
            /*
            string path = FileHelper.GetDownloadedMapPath();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            */
        }

        private void OnEnable()
        {
            SaveButton.gameObject.SetActive(true);
            SaveButton.interactable = true;

            StopUploadUI();
            UploadPopup.gameObject.SetActive(false);

            var buttonServerDataText = UploadServerDataButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonServerDataText.text = "Повторить";

            var buttonServerMeshText = UploadServerMeshButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonServerMeshText.text = "Повторить";
        }

        private void Update()
        {
            /*
            if (m_DenseSpatialMapBuilderFrameFilter.MeshBlocks.Count > 8)
            {
                SaveButton.interactable = true;
            }
            else
            {
                SaveButton.interactable = false;
            }
            */
        }

        public void SetMapSession(MapSession session)
        {
            m_MapSession = session;
            m_MapData = session.Maps[0];
        }

        public void SetSpatialMap(SpatialMapData spatialMapData, SparseSpatialMapController sparseSpatialMapController)
        {
            m_SpatialMapData = spatialMapData;
            m_MapId = m_SpatialMapData.id;
            m_SparseSpatialMapController = sparseSpatialMapController;
        }

        public void InitDenseSpatialMap()
        {
            m_DenseSpatialMapBuilderFrameFilter =
                m_MapSession.ARSession.GetComponentInChildren<DenseSpatialMapBuilderFrameFilter>(true);
            if (m_DenseSpatialMapBuilderFrameFilter != null)
            {
                m_DenseSpatialMapBuilderFrameFilter.gameObject.SetActive(true);
                m_DenseSpatialMapBuilderFrameFilter.RenderMesh = true;
            }
        }

        public void MapLocalize()
        {
        }

        public void Save()
        {
            if (m_MapData == null)
            {
                return;
            }

            UploadPopup.gameObject.SetActive(true);
            StartUploadUI();

            m_DenseSpatialMapBuilderFrameFilter.Builder.stop();
            m_DenseSpatialMapBuilderFrameFilter.enabled = false;

            SaveMesh();

            //ViewManager.Instance.SaveSpatialMap(spatialMapData);
        }

        public void SaveMesh()
        {
            if (m_DenseSpatialMapBuilderFrameFilter.MeshBlocks.Count > 10)
            {
                GameObject exportMap = GameObject.Find("WorldRoot");
                exportMap.transform.SetParent(m_SparseSpatialMapController.transform);
                //exportMap.transform.SetParent(m_SparseSpatialMapController.transform, true);

                if (exportMap != null)
                {
                    m_NameMesh = m_MapId.Replace("-", "");
                    m_NameMesh = $"{m_NameMesh}.{FileHelper.EXT}";
                    m_FullPath = FileHelper.GetLocalFilePath(m_NameMesh);
                    if (Exporter.Export(m_FullPath,
                        new GameObject[] {exportMap}, FileType.Binary)
                    )
                    {
                        SaveMeshServer();
                        return;
                    }
                }
            }

            StopUploadUI();
            gameObject.SetActive(false);
            ViewManager.Instance.LoadMainView();
            //SaveDataServer();
        }

        public void SaveMeshServer()
        {
            ViewManager.Instance.UploadMesh(File.ReadAllBytes(m_FullPath), m_NameMesh, new IResultListener<string>()
            {
                OnSuccess = (url, message) =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        m_UrlMesh = url;
                        SaveDataServer();
                    });
                },
                OnError = (errorMessage) =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        StopUploadUI();
                        ShowErrorServerMesh();
                    });
                }
            });
        }

        public void SaveDataServer()
        {
            if (m_UrlMesh != null)
            {
                m_SpatialMapData.mesh.shortUrlMesh = m_UrlMesh;
                m_SpatialMapData.mesh.fullUrlMesh =
                    SpatialMapStorageFirebaseController.STORAGE_BASE_URL + "/" + m_UrlMesh;
                m_SpatialMapData.mesh.nameMesh = m_NameMesh;

                GameObject exportMap = GameObject.Find("WorldRoot");

                if (exportMap != null)
                {
                    var position = exportMap.transform.localPosition;
                    var rotation = exportMap.transform.localRotation;
                    var scale = exportMap.transform.localScale;

                    m_SpatialMapData.mesh.position = new float[3] {position.x, position.y, position.z};
                    m_SpatialMapData.mesh.rotation = new float[4] {rotation.x, rotation.y, rotation.z, rotation.w};
                    m_SpatialMapData.mesh.scale = new float[3] {scale.x, scale.y, scale.z};
                }
            }

            string date = DateHelper.GetCurrentDate();
            m_SpatialMapData.updatedDate = date;

            ViewManager.Instance.SaveSpatialMap(m_SpatialMapData, new IResultListener<SpatialMapData>()
            {
                OnSuccess = (spatialMapData, message) =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        StopUploadUI();
                        gameObject.SetActive(false);
                        ViewManager.Instance.LoadMainView();
                    });
                },

                OnError = (errorMessage) =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        StopUploadUI();
                        ShowErrorServerData();
                    });
                }
            });
        }

        private void ShowErrorServerData()
        {
            UploadServerDataButton.gameObject.SetActive(true);
            UploadServerMeshButton.gameObject.SetActive(true);

            var buttonText = UploadServerDataButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "Повторить";
        }

        private void ShowErrorServerMesh()
        {
            UploadServerDataButton.gameObject.SetActive(false);
            UploadServerMeshButton.gameObject.SetActive(true);

            var buttonText = UploadServerMeshButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "Повторить";
        }

        private void StartUploadUI()
        {
            UploadServerDataButton.interactable = false;
            UploadServerMeshButton.interactable = false;
            UploadCancelButton.interactable = false;
            ProgressBar.SetActive(true);
        }

        private void StopUploadUI()
        {
            UploadServerDataButton.interactable = true;
            UploadServerMeshButton.interactable = true;
            UploadCancelButton.interactable = true;
            ProgressBar.SetActive(false);
        }
    }
}