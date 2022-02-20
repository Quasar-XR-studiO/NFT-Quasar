using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using Firebase.Extensions;

namespace ZBoom.Common.SpatialMap
{
    public class SpatialMapFirebaseController : MonoBehaviour
    {
        private const string MESSAGE_ERROR = "ERROR";
        private const string MESSAGE_SUCCESS = "SUCCESS";

        [SerializeField] private string m_SpatialMapsReferenceName = "spatial_maps";
        //[SerializeField] private string m_SpatialMapsReferenceName = "spatial_maps";
        private DatabaseReference m_DatabaseReference;

        private void Awake()
        {
            m_DatabaseReference = FirebaseDatabase.DefaultInstance.GetReference(m_SpatialMapsReferenceName);
        }

        public void GetSpatialMap(IResultListener<List<SpatialMapData>> resultListener)
        {
            m_DatabaseReference
                .GetValueAsync()
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        resultListener.OnError(MESSAGE_ERROR);
                    }
                    else
                    {
                        if (task.IsCompleted)
                        {
                            List<SpatialMapData> mapContainers = new List<SpatialMapData>();
                            DataSnapshot snapshot = task.Result;
                            if (snapshot.ChildrenCount > 0)
                            {
                                var childs = snapshot.Children.GetEnumerator();
                                while (childs.MoveNext())
                                {
                                    string json = childs.Current.GetRawJsonValue();
                                    SpatialMapData spatialMapData =
                                        JsonUtility.FromJson<SpatialMapData>(json);

                                    mapContainers.Add(spatialMapData);
                                }

                                resultListener.OnSuccess(mapContainers, MESSAGE_SUCCESS);
                            }
                            else
                            {
                                resultListener.OnSuccess(mapContainers, MESSAGE_ERROR);
                            }
                        }
                        else
                        {
                            resultListener.OnError(MESSAGE_ERROR);
                        }
                    }
                });
        }

        public void SaveSpatialMap(SpatialMapData spatialMapData,
            IResultListener<SpatialMapData> resultListener)
        {
            m_DatabaseReference
                .Child(spatialMapData.id)
                .SetRawJsonValueAsync(JsonUtility.ToJson(spatialMapData))
                .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            resultListener.OnError(MESSAGE_ERROR);
                        }

                        if (task.IsCompleted)
                        {
                            resultListener.OnSuccess(spatialMapData, MESSAGE_SUCCESS);
                        }
                        else
                        {
                            resultListener.OnError(MESSAGE_ERROR);
                        }
                    }
                );
        }
        
        public void RemoveSpatialMap(string idSpatialMap,
            IResultListener<SpatialMapData> resultListener)
        {
            m_DatabaseReference
                .Child(idSpatialMap)
                .RemoveValueAsync()
                .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            resultListener.OnError(MESSAGE_ERROR);
                        }

                        if (task.IsCompleted)
                        {
                            resultListener.OnSuccess(null, MESSAGE_SUCCESS);
                        }
                        else
                        {
                            resultListener.OnError(MESSAGE_ERROR);
                        }
                    }
                );
        }


        private void Start()
        {
        }

        private void Update()
        {
        }
    }
}