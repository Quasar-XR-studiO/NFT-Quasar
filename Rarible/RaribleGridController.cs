using System.Collections.Generic;
using Rarible;
using SpatialMap_SparseSpatialMap;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class RaribleGridController : MonoBehaviour
    {
        public RariblePropItemController PropItemPrefab;
        public SpatialMapGameObjectController SpatialMapGameObjectController;

        private List<RariblePropItemController> m_PropItemControllers = new List<RariblePropItemController>();
        private int m_SelectedPosition = -1;

        private void Start()
        {
            ViewManager.Instance.GetRaribleItems(50, new IResultListener<RaribleCollection>()
            {
                OnSuccess = (collection, message) =>
                {
                    foreach (var raribleItem in collection.items)
                    {
                        if (raribleItem.meta.content.Count > 0)
                        {
                            var newRaribleItem = Instantiate(PropItemPrefab, transform);
                            newRaribleItem.RaribleItem = raribleItem;
                            newRaribleItem.SetData(raribleItem);
                            m_PropItemControllers.Add(newRaribleItem);
                        }
                    }
                },
                OnError = message => { }
            });
            
        }

        public void Select(RariblePropItemController propItemController)
        {
            SpatialMapGameObjectController.SelectNftTemplate(propItemController.RaribleItem);
        }

        public void SelectNft(RaribleItem raribleItem)
        {
            SpatialMapGameObjectController.SelectNftTemplate(raribleItem);
        }

        public void Deselect()
        {
            if (m_SelectedPosition != -1)
            {
                m_PropItemControllers[m_SelectedPosition].Deselect();
            }

            m_SelectedPosition = -1;

            /*
            if (propItemController != null)
            {
                propItemController.Deselect();
            }
            */

            //SpatialMapGameObjectController.DeselectTemplate();
        }

        public void DeselectAll()
        {
            foreach (var scanPropCellController in m_PropItemControllers)
            {
                scanPropCellController.Deselect();
            }

            m_SelectedPosition = -1;
        }

        private void Update()
        {
        }
    }
}