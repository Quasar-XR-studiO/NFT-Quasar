using System.Collections.Generic;
using SpatialMap_SparseSpatialMap;
using UnityEngine;

namespace ZBoom.Common.SpatialMap
{
    public class ScanEditWithGridViewController : MonoBehaviour
    {
        public SpatialMapGameObjectController PropDragger;
        public GameObject Tips;

        private MapSession.MapData mapData;
        private bool isTipsOn;

        private void Awake()
        {
            PropDragger.CreateObjectEvent += (gameObj) =>
            {
                if (gameObj)
                {
                    gameObj.transform.parent = mapData.Controller.transform;
                    mapData.Props.Add(gameObj);
                }
            };
            PropDragger.DeleteObjectEvent += (gameObj) =>
            {
                if (gameObj)
                {
                    mapData.Props.Remove(gameObj);
                }
            };
        }

        private void OnEnable()
        {
            Tips.SetActive(false);
            isTipsOn = false;
        }

        public void SetMapSession(MapSession session)
        {
            mapData = session.Maps[0];
            PropDragger.SetMapSession(session);
        }

        public void ShowTips()
        {
            isTipsOn = !isTipsOn;
            Tips.SetActive(isTipsOn);
        }

        public void Save()
        {
            if (mapData == null)
            {
                return;
            }

            var propInfos = new List<MapMeta.PropInfo>();

            foreach (var prop in mapData.Props)
            {
                var position = prop.transform.localPosition;
                var rotation = prop.transform.localRotation;
                var scale = prop.transform.localScale;

                propInfos.Add(new MapMeta.PropInfo()
                {
                    Name = prop.name,
                    Position = new float[3] { position.x, position.y, position.z },
                    Rotation = new float[4] { rotation.x, rotation.y, rotation.z, rotation.w },
                    Scale = new float[3] { scale.x, scale.y, scale.z }
                });
            }
            mapData.Meta.Props = propInfos;

            MapMetaManager.Save(mapData.Meta);
        }
    }
}
