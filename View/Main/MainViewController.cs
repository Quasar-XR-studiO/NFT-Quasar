using System.Collections;
using System.Collections.Generic;
using SpatialMap_SparseSpatialMap;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class MainViewController : MonoBehaviour
    {
        public Button EditButton;
        public Button PreviewButton;
        public Button CreateButton;
        public Button ScanButton;
        public Button RemoveButton;
        public SpatialMapGridController SpatialMapGridController;

        public GameObject ProgressPanel;
        public GameObject ErrorPanel;

        private void OnEnable()
        {
            StopAllCoroutines();
        }

        public void EnableEdit(bool enable)
        {
            EditButton.interactable = enable;
        }

        public void EnablePreview(bool enable)
        {
            PreviewButton.interactable = enable;
        }
        
        public void EnableScan(bool enable)
        {
            ScanButton.interactable = enable;
        }
        
        public void EnableRemove(bool enable)
        {
            RemoveButton.interactable = enable;
        }

        public void SetData(List<MapMeta> mapMetas, List<SpatialMapData> spatialMaps)
        {
            SpatialMapGridController.SetData(mapMetas, spatialMaps);

        }
        
        public void ShowError(bool isShow)
        {
            if (isShow)
            {
                ErrorPanel.SetActive(true);                
            }
            else
            {
                ErrorPanel.SetActive(false);                
            }
        }
        
        public void ShowLoading(bool isShow)
        {
            if (isShow)
            {
                ProgressPanel.SetActive(true);                
            }
            else
            {
                ProgressPanel.SetActive(false);                
            }
        }
    }
}
