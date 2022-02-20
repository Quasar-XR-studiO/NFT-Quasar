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
using Rarible;
using UnityEngine.Networking;

namespace ZBoom.Common.SpatialMap
{
    public class RaribleApiController : MonoBehaviour
    {
        public const string MESSAGE_ERROR = "ERROR";
        public const string MESSAGE_SUCCESS = "SUCCESS";

        [SerializeField] private string m_DefaultItemId = "ETHEREUM:0x60f80121c31a0d46b5279700f9df786054aa5ee5:1017528";
        private string m_UrlGetItemById = "https://api.rarible.org/v0.1/items/";
        private string m_UrlGetItems = "https://api.rarible.org/v0.1/items/all";

        private void Start()
        {
        }

        private void Update()
        {
        }

        // https://api.rarible.org/v0.1/doc#operation/getItemById
        public void GetItemById()
        {
            GetItemById(m_DefaultItemId, new IResultListener<RaribleItem>());
        }

        public void GetItemById(string itemId, IResultListener<RaribleItem> resultListener)
        {
            string url = $"{m_UrlGetItemById}{itemId}";
            StartCoroutine(GetRequestItemById(url, resultListener));
        }

        private IEnumerator GetRequestItemById(string url, IResultListener<RaribleItem> resultListener)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                Debug.Log(request.error);
                resultListener.OnError($"{MESSAGE_ERROR} / {request.error}");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                string jsonData = request.downloadHandler.text;
                //fix
                jsonData = jsonData.Replace("@", "");
                RaribleItem raribleItem = JsonUtility.FromJson<RaribleItem>(jsonData);
                resultListener.OnSuccess(raribleItem, MESSAGE_SUCCESS);
            }
        }

        public void GetItems()
        {
            string url = m_UrlGetItems + "?size=" + "20";
            StartCoroutine(GetRequestItems(url, null));
        }
        
        public void GetItems(IResultListener<RaribleCollection> resultListener, int size = 20)
        {
            string url = m_UrlGetItems + "?size=" + size;
            StartCoroutine(GetRequestItems(url, resultListener));
        }
        
        private IEnumerator GetRequestItems(string url, IResultListener<RaribleCollection> resultListener)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                Debug.Log(request.error);
                resultListener.OnError($"{MESSAGE_ERROR} / {request.error}");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                string jsonData = request.downloadHandler.text;
                jsonData = jsonData.Replace("@", "");
                RaribleCollection raribleCollection = JsonUtility.FromJson<RaribleCollection>(jsonData);
                resultListener.OnSuccess(raribleCollection, MESSAGE_SUCCESS);
            }
        }
    }
}