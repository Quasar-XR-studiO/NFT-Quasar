using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SpatialMap_SparseSpatialMap;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class SpatialMapGridController : MonoBehaviour
    {
        public SpatialMapItemController SpatialMapItemPrefab;

        private RectTransform m_RectTransform;
        private List<SpatialMapItemController> m_MapCellControllers = new List<SpatialMapItemController>();

        public int MapCellCount => m_MapCellControllers.Count;

        private void OnEnable()
        {
            /*
            SetMapMeta(MapMetaManager.LoadAll());
            */
        }

        private void Start()
        {
            /*
            m_RectTransform = GetComponent<RectTransform>();
            var cellWidth = m_RectTransform.rect.width * 0.9f;
            var padding = (int) (cellWidth * 0.1f);
            */
        }

        private void Update()
        {
        }

        private void OnDisable()
        {
            /*
            foreach (var mapCellController in m_MapCellControllers)
            {
                if (mapCellController)
                {
                    Destroy(mapCellController.gameObject);
                }
            }

            m_MapCellControllers.Clear();
            */
            
            foreach (var mapCellController in m_MapCellControllers)
            {
                mapCellController.Cancel();
            }
        }

        public void ClearAll()
        {
            // Notice:
            //   a) When clear both map cache and map list,
            //      load map will not trigger a download (cache is build when upload),
            //      and statistical request count will not be increased in a subsequent load (when edit or preview).
            //   b) When clear map cache only,
            //      load map after clear (only the first time each map) will trigger a download,
            //      and statistical request count will be increased in a subsequent load (when edit or preview).
            //      Map cache is used after a successful download and will be cleared if SparseSpatialMapManager.clear is called or app uninstalled.
            //
            // More about the statistical request count and limitations for different subscription mode can be found at EasyAR website.

            if (!SpatialMap_SparseSpatialMap.ViewManager.Instance.MainViewRecycleBinClearMapCacheOnly)
            {
                foreach (var mapCellController in m_MapCellControllers)
                {
                    if (mapCellController)
                    {
                        MapMetaManager.Delete(mapCellController.MapMeta);
                        Destroy(mapCellController.gameObject);
                    }
                }

                m_MapCellControllers.Clear();
            }

            MapSession.ClearCache();
            OnMapCellChange();

            if (!SpatialMap_SparseSpatialMap.ViewManager.Instance.MainViewRecycleBinClearMapCacheOnly)
            {
                easyar.GUIPopup.EnqueueMessage(
                    "DELETED: {(Sample) Meta Data, Map Cache}" + Environment.NewLine +
                    "NOT DELETED: {Map on Server}" + Environment.NewLine +
                    "Use web develop center to manage maps on server", 5);
            }
            else
            {
                easyar.GUIPopup.EnqueueMessage(
                    "DELETED: {Map Cache}" + Environment.NewLine +
                    "NOT DELETED: {Map on Server, (Sample) Meta Data}" + Environment.NewLine +
                    "Use web develop center to manage maps on server", 5);
            }
        }

        private void OnMapCellChange()
        {
            ViewManager.Instance.SelectMaps(m_MapCellControllers
                    .Where(mapCellController => mapCellController && mapCellController.IsSelected)
                    .Select(mapCellController => mapCellController.MapMeta)
                    .ToList());
        }

        public void SetData(List<MapMeta> mapMetas, List<SpatialMapData> spatialMaps)
        {
            foreach (var mapCellController in m_MapCellControllers)
            {
                Destroy(mapCellController.gameObject);
            }
            m_MapCellControllers.Clear();

            for (var i = 0; i < mapMetas.Count; i++)
            {
                var meta = mapMetas[i];
                var spatialMap = spatialMaps[i];
                var mapCellController = Instantiate(SpatialMapItemPrefab, transform);
                mapCellController.SetMapMeta(meta);
                mapCellController.SetSpatialMapData(spatialMap);
                mapCellController.PointerDownEvent += OnMapCellChange;
                mapCellController.DeleteEvent += () =>
                {
                    if (m_MapCellControllers.Remove(mapCellController))
                    {
                        MapMetaManager.Delete(mapCellController.MapMeta);
                        Destroy(mapCellController.gameObject);
                        OnMapCellChange();

                        /*
                        easyar.GUIPopup.EnqueueMessage(
                            "DELETED: {(Sample) Meta Data}" + Environment.NewLine +
                            "NOT DELETED: {Map Cache, Map on Server}" + Environment.NewLine +
                            "Use recycle bin button to delete map cache" + Environment.NewLine +
                            "Use web develop center to manage maps on server", 5);
                        */

                        ViewManager.Instance.RemoveSpatialMap(meta.Map.ID, new IResultListener<SpatialMapData>()
                        {
                        });
                        
                        ViewManager.Instance.RemoveMesh(spatialMap, new IResultListener<string>()
                        {
                        });
                    }
                };
                m_MapCellControllers.Add(mapCellController);
            }
        }
    }
}