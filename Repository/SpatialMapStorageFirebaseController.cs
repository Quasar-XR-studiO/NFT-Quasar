using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Storage;
using Parabox.Stl;
using UnityEngine.Networking;

namespace ZBoom.Common.SpatialMap
{
    public class SpatialMapStorageFirebaseController : MonoBehaviour
    {
        public static string STORAGE_BASE_URL = "gs://karandash-education.appspot.com";
        
        private const string MESSAGE_ERROR = "ERROR";
        private const string MESSAGE_SUCCESS = "SUCCESS";

        //[SerializeField] private string m_SpatialMapsReferenceName = "spatial_maps";
        private StorageReference m_StorageReference;
        private FirebaseStorage m_Storage;

        private string m_LocalFilePath;
        private string m_ServerFilePath = "Maps";

        private void Awake()
        {
            m_LocalFilePath = FileHelper.GetDownloadedMapPath();

            m_Storage = FirebaseStorage.DefaultInstance;
            m_StorageReference = m_Storage.GetReferenceFromUrl(STORAGE_BASE_URL);
        }

        public void UploadFile(byte[] data, string name, IResultListener<string> resultListener)
        {
            string fullName = GetServerFilePath(name);
            StorageReference mapReference = m_StorageReference.Child(fullName);

            mapReference
                .PutBytesAsync(data)
                .ContinueWith((Task<StorageMetadata> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());
                        resultListener.OnError(MESSAGE_ERROR);
                    }
                    else
                    {
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;

                        resultListener.OnSuccess(metadata.Path, MESSAGE_SUCCESS);
                        Debug.Log("Finished uploading...");
                        Debug.Log("md5 hash = " + md5Hash);
                    }
                });
        }

        public void UploadFile(string path, string name, IResultListener<string> resultListener)
        {
            string fullName = "maps/" + name;
            StorageReference mapReference = m_StorageReference.Child("fullName");

            mapReference
                .PutFileAsync(path)
                .ContinueWith((Task<StorageMetadata> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());
                        resultListener.OnError(MESSAGE_ERROR);
                    }
                    else
                    {
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;

                        resultListener.OnSuccess(metadata.Path, MESSAGE_SUCCESS);
                        Debug.Log("Finished uploading...");
                        Debug.Log("md5 hash = " + md5Hash);
                    }
                });
        }

        public void RemoveFile(string name, IResultListener<string> resultListener)
        {
            string path = FileHelper.GetLocalFilePath(name);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string fullName = GetServerFilePath(name);
            StorageReference mapReference = m_StorageReference.Child(fullName);

            mapReference
                .DeleteAsync()
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        resultListener.OnSuccess(MESSAGE_SUCCESS, MESSAGE_SUCCESS);
                        Debug.Log("File deleted successfully.");
                    }
                    else
                    {
                        resultListener.OnError(MESSAGE_ERROR);
                    }
                });
        }

        public void DownloadUrlFile(string url, string name, IResultListener<string> resultListener)
        {
            StorageReference mapReference = m_Storage.GetReferenceFromUrl(url);

            mapReference
                .GetDownloadUrlAsync()
                .ContinueWith(task =>
                {
                    if (!task.IsFaulted && !task.IsCanceled)
                    {
                        Debug.Log("Download URL: " + task.Result);
                        resultListener.OnSuccess(task.Result.ToString(), MESSAGE_SUCCESS);
                    }
                    else
                    {
                        resultListener.OnError(MESSAGE_ERROR);
                    }
                });
        }

        public void DownloadFile(string url, string name, IResultListener<GameObject> resultListener)
        {
            string path = FileHelper.GetLocalFilePath(name);
            if (File.Exists(path))
            {
                Debug.Log("Found file locally, loading...");
                var mapGameObject = LoadModel(path);
                resultListener.OnSuccess(mapGameObject, MESSAGE_SUCCESS);
                return;
            }

            StartCoroutine(GetFileRequest(url, name, (UnityWebRequest req) =>
            {
                if (req.isNetworkError || req.isHttpError)
                {
                    resultListener.OnError(MESSAGE_ERROR);
                    Debug.Log($"{req.error} : {req.downloadHandler.text}");
                }
                else
                {
                    var mapGameObject = LoadModel(path);
                    resultListener.OnSuccess(mapGameObject, MESSAGE_SUCCESS);
                }
            }));
        }

        public void DownloadUrlAndFile(string url, string name, IResultListener<GameObject> resultListener)
        {
            string path = FileHelper.GetLocalFilePath(name);
            Debug.Log("EditView Path " + path);
            if (File.Exists(path))
            {
                Debug.Log("EditView Found file locally, loading...");
                var mapGameObject = LoadModel(path);
                resultListener.OnSuccess(mapGameObject, MESSAGE_SUCCESS);
                return;
            }
            
            StorageReference mapReference = m_Storage.GetReferenceFromUrl(url);

            mapReference
                .GetDownloadUrlAsync()
                .ContinueWith(task =>
                {
                    if (!task.IsFaulted && !task.IsCanceled)
                    {
                        Debug.Log("EditView Download URL: " + task.Result);
                        string fullUrl = task.Result.ToString();
                       
                        StartCoroutine(GetFileRequest(fullUrl, name, (UnityWebRequest req) =>
                        {
                            if (req.isNetworkError || req.isHttpError)
                            {
                                resultListener.OnError(MESSAGE_ERROR);
                                Debug.Log($"{req.error} : {req.downloadHandler.text}");
                            }
                            else
                            {
                                var mapGameObject = LoadModel(path);
                                resultListener.OnSuccess(mapGameObject, MESSAGE_SUCCESS);
                            }
                        }));
                    }
                    else
                    {
                        resultListener.OnError(MESSAGE_ERROR);
                    }
                });
        }

        IEnumerator GetFileRequest(string url, string name, Action<UnityWebRequest> callback)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                req.downloadHandler = new DownloadHandlerFile(FileHelper.GetLocalFilePath(name));
                yield return req.SendWebRequest();
                callback(req);
            }
        }

        private GameObject LoadModel(string path)
        {
            Mesh[] meshes = Importer.Import(path);
            var rootGameObject = new GameObject("MapRoot");
            for (var i = 0; i < meshes.Length; i++)
            {
                var mesh = meshes[i];
                var newGameObject = new GameObject($"Map_{i}");
                var meshFilter = newGameObject.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                var meshRenderer = newGameObject.AddComponent<MeshRenderer>();
                newGameObject.transform.parent = rootGameObject.transform;
            }

            return rootGameObject;
        }

        private string GetServerFilePath(string name)
        {
            return $"{m_ServerFilePath}/{name}";
        }
    }
}