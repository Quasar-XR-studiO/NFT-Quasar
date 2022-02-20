using System.Collections.Generic;
using SpatialMap_SparseSpatialMap;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class PrefabGridController : MonoBehaviour
    {
        public PropGroupItemController PropGroupItemPrefab;
        public PropItemController PropItemPrefab;
        public SpatialMapGameObjectController SpatialMapGameObjectController;

        private List<PropItemController> m_PropItemControllers = new List<PropItemController>();
        private int m_SelectedPosition = -1;

        private void Start()
        {
            /*
            foreach (var templet in PropCollection.Instance.Templets)
            {
                var scanPropCellController = Instantiate(PropItemPrefab, transform);
                scanPropCellController.SetMapMeta(templet);
                m_PropItemControllers.Add(scanPropCellController);
            }
            */
            
            foreach (var group in PropCollection.Instance.TempletGroups)
            {
                var propGroupItemPrefab = Instantiate(PropGroupItemPrefab, transform);
                propGroupItemPrefab.Title.text = group.Name;
                
                foreach (var templet in group.Templets)
                {
                    var propCellController = Instantiate(PropItemPrefab, propGroupItemPrefab.Grid.transform);
                    propCellController.SetData(templet);
                    m_PropItemControllers.Add(propCellController);
                }
            }
        }

        public void Select(PropItemController propItemController)
        {
            if (m_SelectedPosition != -1)
            {
                m_PropItemControllers[m_SelectedPosition].Deselect();
                if (propItemController == m_PropItemControllers[m_SelectedPosition])
                {
                    m_SelectedPosition = -1;
                }
                else
                {
                    m_SelectedPosition = m_PropItemControllers.IndexOf(propItemController);
                    SpatialMapGameObjectController.SelectTemplate(propItemController);
                }
            }
            else
            {
                m_SelectedPosition = m_PropItemControllers.IndexOf(propItemController);
                SpatialMapGameObjectController.SelectTemplate(propItemController);
            }
        }

        public void Deselect(PropItemController propItemController = null)
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