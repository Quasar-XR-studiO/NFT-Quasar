using System.Collections.Generic;
using SpatialMap_SparseSpatialMap;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class PropController : MonoBehaviour
    {
        public PropItemController PropItemPrefab;
        public SpatialMapGameObjectController SpatialMapGameObjectController;

        private List<PropItemController> m_PropItemControllers = new List<PropItemController>();
        private int m_SelectedPosition = -1;

        private void Start()
        {
            foreach (var templet in PropCollection.Instance.Templets)
            {
                var scanPropCellController = Instantiate(PropItemPrefab, transform);
                scanPropCellController.SetData(templet);
                m_PropItemControllers.Add(scanPropCellController);
            }
            
            //AddPanel.SetActive(true);
            //TapPanel.SetActive(false);
        }

        public void Select()
        {
            PropItemController propItemController = m_PropItemControllers[0];
            
            if (m_SelectedPosition != -1)
            {
                m_PropItemControllers[m_SelectedPosition].Deselect();
                if (propItemController == m_PropItemControllers[m_SelectedPosition])
                {
                    m_SelectedPosition = -1;
                }
                else
                {
                    //AddPanel.SetActive(false);
                    //TapPanel.SetActive(true);

                    m_SelectedPosition = 0;
                    SpatialMapGameObjectController.SelectTemplate(propItemController);
                }
            }
            else
            {
                //AddPanel.SetActive(false);
                //TapPanel.SetActive(true);

                m_SelectedPosition = 0;
                SpatialMapGameObjectController.SelectTemplate(propItemController);
            }
        }

        public void Deselect(PropItemController propItemController = null)
        {
            //AddPanel.SetActive(true);
            //TapPanel.SetActive(false);
            
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
            //AddPanel.SetActive(true);
            //TapPanel.SetActive(false);
            
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