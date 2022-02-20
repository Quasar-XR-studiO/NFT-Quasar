using easyar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Parabox.Stl;
using SpatialMap_SparseSpatialMap;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityFBXExporter;

namespace ZBoom.Common.SpatialMap
{
    public class CreateViewController : MonoBehaviour
    {
        public GameObject UploadPopup;
        public GameObject ProgressBar;
        public TMP_InputField MapNameInput;
        public Button UploadEasyArButton;
        public Button UploadServerDataButton;
        public Button UploadServerMeshButton;
        public Button UploadCancelButton;
        public Button SaveButton;
        public Button SnapshotButton;
        public RawImage PreviewImage;

        public bool IsSaveMesh = false;

        private DenseSpatialMapBuilderFrameFilter m_DenseSpatialMapBuilderFrameFilter;
        private MapSession m_MapSession;

        private string m_MapName = null;
        private string m_MapId = null;
        private string m_FullPath = null;
        private string m_NameMesh = null;
        private string m_UrlMesh = null;
        private Texture2D m_CapturedImage;

        private void Start()
        {
            if (IsSaveMesh)
            {
                string path = FileHelper.GetDownloadedMapPath();
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        private void OnEnable()
        {
            SaveButton.gameObject.SetActive(true);
            SaveButton.interactable = false;

            StopUploadUI();
            UploadPopup.gameObject.SetActive(false);

            var buttonEasyArText = UploadEasyArButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonEasyArText.text = "Сохранить";

            var buttonServerDataText = UploadServerDataButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonServerDataText.text = "Сохранить";

            var buttonServerMeshText = UploadServerMeshButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonServerMeshText.text = "Сохранить";
        }

        private void Update()
        {
            if ((m_MapSession.MapWorker.LocalizedMap != null &&
                 m_MapSession.MapWorker.LocalizedMap.PointCloud.Count >= 20) && !Application.isEditor)
            {
                /*
                if (IsSaveMesh) 
                {
                    if (m_DenseSpatialMapBuilderFrameFilter.MeshBlocks.Count > 8)
                    {
                        SaveButton.interactable = true;
                    }
                    else
                    {
                        SaveButton.interactable = false;
                    }
                }
                else
                {
                    SaveButton.interactable = true;
                }
                */
                SaveButton.interactable = true;
            }
            else
            {
                SaveButton.interactable = false;
            }
        }

        private void OnDestroy()
        {
            if (m_CapturedImage)
            {
                Destroy(m_CapturedImage);
            }
        }

        public void SetMapSession(MapSession session)
        {
            m_MapSession = session;

            if (IsSaveMesh)
            {
                m_DenseSpatialMapBuilderFrameFilter = m_MapSession.ARSession.GetComponentInChildren<DenseSpatialMapBuilderFrameFilter>(true);
                if (m_DenseSpatialMapBuilderFrameFilter != null)
                {
                    m_DenseSpatialMapBuilderFrameFilter.gameObject.SetActive(true);
                }
            }

            Clear();
        }

        private void Clear()
        {
            m_MapName = null;
            m_MapId = null;
            m_FullPath = null;
            m_NameMesh = null;
            m_UrlMesh = null;
        }

        public void Save()
        {
            SaveButton.gameObject.SetActive(false);
            UploadPopup.gameObject.SetActive(true);
            //MapNameInput.text = m_MapName = "Map_" + DateTime.Now.ToString("yyyy-MM-dd_HHmm");
            MapNameInput.text = m_MapName = "Карта: " + DateHelper.GetCurrentDate();
            m_MapSession.MapWorker.enabled = false;
            if (IsSaveMesh)
            {
                m_DenseSpatialMapBuilderFrameFilter.Builder.stop();
                m_DenseSpatialMapBuilderFrameFilter.enabled = false;
            }
            Snapshot();
        }

        public void Snapshot()
        {
            var oneShot = Camera.main.gameObject.AddComponent<OneShot>();
            oneShot.Shot(true, (texture) =>
            {
                if (m_CapturedImage)
                {
                    Destroy(m_CapturedImage);
                }

                m_CapturedImage = texture;
                PreviewImage.texture = m_CapturedImage;
            });
        }

        public void OnMapNameChange(string name)
        {
            UploadEasyArButton.interactable = !string.IsNullOrEmpty(name);
            m_MapName = name;
        }

        public void Upload()
        {
            UploadEasyArButton.gameObject.SetActive(true);
            UploadServerDataButton.gameObject.SetActive(false);
            UploadServerMeshButton.gameObject.SetActive(false);

            using (var buffer = easyar.Buffer.wrapByteArray(m_CapturedImage.GetRawTextureData()))
            using (var image =
                new easyar.Image(buffer, PixelFormat.RGB888, m_CapturedImage.width, m_CapturedImage.height))
            {
                m_MapSession.MapWorker.MapUnload += (controller, info, arg3, arg4) =>
                {
                    if (info != null)
                    {
                    }
                };

                m_MapSession.MapWorker.BuilderMapController.MapHost += (map, isSuccessful, error) =>
                {
                    if (isSuccessful)
                    {
                        m_MapId = map.ID;
                    }
                    else
                    {
                    }
                };

                m_MapSession.Save(m_MapName, image);
            }

            StartUploadUI();
            StartCoroutine(Saving());
        }

        private IEnumerator Saving()
        {
            while (m_MapSession.IsSaving)
            {
                yield return 0;
            }

            if (m_MapSession.Saved)
            {
                if (IsSaveMesh)
                {
                    if (m_DenseSpatialMapBuilderFrameFilter.MeshBlocks.Count > 10)
                    {
                        SaveMesh();
                    }
                    else
                    {
                        SaveDataServer();
                    }
                }
                else
                {
                    SaveDataServer();
                }
                //Debug.Log("ViewManager: 1 " + m_MapSession.MapWorker.BuilderMapController.);
                //ViewManager.Instance.LoadEditView(m_MapSession, m_MapSession.MapWorker.BuilderMapController.MapInfoSource.ID);
                //ViewManager.Instance.LoadEditView(m_MapSession, m_MapId);
            }
            else
            {
                StopUploadUI();
                ShowErrorEasyAR();
            }
            //StopUploadUI();
        }

        public void SaveMesh()
        {
            //GameObject exportMap = GameObject.Find("DenseSpatialMapRoot");
            GameObject exportMap = GameObject.Find("WorldRoot");
            if (exportMap != null)
            {
                m_NameMesh = m_MapId.Replace("-", "");
                m_NameMesh = $"{m_NameMesh}.{FileHelper.EXT}";

                m_FullPath = FileHelper.GetLocalFilePath(m_NameMesh);
                Debug.Log("Karandash.CreateViewController: FullPath = " + m_FullPath);
                if (Exporter.Export(m_FullPath,
                    new GameObject[] {exportMap}, FileType.Binary)
                )
                {
                    Debug.Log("Karandash.CreateViewController: FullPath = " + "SAVED");
                    SaveMeshServer();
                }
            }
            else
            {
                Debug.Log("Karandash.CreateViewController: FullPath = " + "NULL");
                SaveDataServer();
            }
        }

        public void SaveMeshServer()
        {
            ViewManager.Instance.UploadMesh(File.ReadAllBytes(m_FullPath), m_NameMesh, new IResultListener<string>()
            {
                OnSuccess = (url, message) =>
                {
                    m_UrlMesh = url;
                    SaveDataServer();
                },
                OnError = (errorMessage) =>
                {
                    StopUploadUI();
                    ShowErrorServerMesh();
                }
            });
        }

        public void SaveMeshTest()
        {
            string path = FileHelper.GetDownloadedMapPath();
            //GameObject exportMap = GameObject.Find("DenseSpatialMapRoot");
            GameObject exportMap = GameObject.Find("WorldRoot");

            /*
            FBXExporter.ExportGameObjToFBX(exportMap, path + ".fbx", false, false);

            if (Exporter.Export(path + "_ASCII.stl",
                new GameObject[] {exportMap}, FileType.Ascii)
            )
            {
                
            }
            */

            string name = $"{m_MapId}.{FileHelper.EXT}";
            string fullPath = FileHelper.GetLocalFilePath(name);

            if (Exporter.Export(fullPath,
                new GameObject[] {exportMap}, FileType.Binary)
            )
            {
                ViewManager.Instance.UploadMesh(File.ReadAllBytes(fullPath), name, new IResultListener<string>()
                {
                    OnSuccess = (url, message) => { },
                    OnError = (message) => { }
                });
            }
        }

        public void SaveDataServer()
        {
            SpatialMapData spatialMapData = new SpatialMapData();
            spatialMapData.id = m_MapId;
            spatialMapData.name = m_MapName;
            if (m_UrlMesh != null)
            {
                GameObject exportMap = GameObject.Find("WorldRoot");
       
                if (exportMap != null && m_UrlMesh.Length > 1)
                {
                    spatialMapData.mesh.shortUrlMesh = m_UrlMesh;
                    spatialMapData.mesh.fullUrlMesh = SpatialMapStorageFirebaseController.STORAGE_BASE_URL + "/" + m_UrlMesh;
                    spatialMapData.mesh.nameMesh = m_NameMesh;
                    
                    var position = exportMap.transform.position;
                    var rotation = exportMap.transform.rotation;
                    var scale = exportMap.transform.localScale;

                    spatialMapData.mesh.position = new float[3] {position.x, position.y, position.z};
                    spatialMapData.mesh.rotation = new float[4] {rotation.x, rotation.y, rotation.z, rotation.w};
                    spatialMapData.mesh.scale = new float[3] {scale.x, scale.y, scale.z};
                }
            }

            string date = DateHelper.GetCurrentDate();
            spatialMapData.createdDate = date;
            spatialMapData.updatedDate = date;

            ViewManager.Instance.SaveSpatialMap(spatialMapData, new IResultListener<SpatialMapData>()
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

        private void ShowErrorEasyAR()
        {
            UploadEasyArButton.gameObject.SetActive(true);
            UploadServerDataButton.gameObject.SetActive(false);
            UploadServerMeshButton.gameObject.SetActive(false);

            var buttonText = UploadEasyArButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "Повторить";
        }

        private void ShowErrorServerData()
        {
            UploadEasyArButton.gameObject.SetActive(false);
            UploadServerDataButton.gameObject.SetActive(true);
            UploadServerMeshButton.gameObject.SetActive(true);

            var buttonText = UploadServerDataButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "Повторить";
        }

        private void ShowErrorServerMesh()
        {
            UploadEasyArButton.gameObject.SetActive(false);
            UploadServerDataButton.gameObject.SetActive(false);
            UploadServerMeshButton.gameObject.SetActive(true);

            var buttonText = UploadServerMeshButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "Повторить";
        }

        private void StartUploadUI()
        {
            UploadEasyArButton.interactable = false;
            UploadServerDataButton.interactable = false;
            UploadServerMeshButton.interactable = false;
            MapNameInput.interactable = false;
            UploadCancelButton.interactable = false;
            SnapshotButton.interactable = false;
            ProgressBar.SetActive(true);
        }

        private void StopUploadUI()
        {
            UploadEasyArButton.interactable = true;
            UploadServerDataButton.interactable = true;
            UploadServerMeshButton.interactable = true;
            MapNameInput.interactable = true;
            UploadCancelButton.interactable = true;
            SnapshotButton.interactable = true;
            ProgressBar.SetActive(false);
        }
    }
}