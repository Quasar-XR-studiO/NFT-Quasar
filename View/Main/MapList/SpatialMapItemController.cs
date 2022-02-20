using System;
using SpatialMap_SparseSpatialMap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class SpatialMapItemController : MonoBehaviour
    {
        [SerializeField] public Button DeleteButton;
        [SerializeField] public Button EditButton;

        [SerializeField] private Color m_TextSelectedColor;
        [SerializeField] private Color m_TextDefaultColor;
        [SerializeField] private Color m_BackgroundSelectedColor;
        [SerializeField] private Color m_BackgroundDefaultColor;
        
        [SerializeField] private TMP_Text m_NameText;
        [SerializeField] private TMP_Text m_DateText;
        [SerializeField] private Image m_BackgroundImage;

        public event Action PointerDownEvent;
        public event Action DeleteEvent;
        public event Action EditEvent;

        public bool IsSelected { get; private set; }
        
        public MapMeta MapMeta { get; private set; }
        public SpatialMapData SpatialMapData { get; private set; }

        public void Awake()
        {
            Cancel();
        }

        public void SetMapMeta(MapMeta meta)
        {
            MapMeta = meta;
        }
        
        public void SetSpatialMapData(SpatialMapData spatialMapData)
        {
            SpatialMapData = spatialMapData;
            
            m_NameText.text = SpatialMapData.name;
            if (spatialMapData.updatedDate.Length > 1)
            {
                m_DateText.text = SpatialMapData.updatedDate;
            }
            else
            {
                m_DateText.gameObject.SetActive(false);
            }
        }

        public void OnPointerDown()
        {
            if (IsSelected)
            {
                Cancel();
            }
            else
            {
                Select();
            }

            if (PointerDownEvent != null)
            {
                PointerDownEvent();
            }
        }

        public virtual void OnDelete()
        {
            //ViewManager.Instance.RemoveMesh();
        
            if (DeleteEvent != null)
            {
                DeleteEvent();
            }
        }
        
        public virtual void OnEdit()
        {
            ViewManager.Instance.LoadEditView();
            
            if (EditEvent != null)
            {
                EditEvent();
            }
        }

        public void Select()
        {
            IsSelected = true;
            
            m_BackgroundImage.color = m_BackgroundSelectedColor;
            m_NameText.color = m_TextSelectedColor;
            
            DeleteButton.gameObject.SetActive(true);
            //EditButton.gameObject.SetActive(true);
        }

        public void Cancel()
        {
            IsSelected = false;
            
            m_BackgroundImage.color = m_BackgroundDefaultColor;
            m_NameText.color = m_TextDefaultColor;
            
            DeleteButton.gameObject.SetActive(false);
            EditButton.gameObject.SetActive(false);
        }
    }
}