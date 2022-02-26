using System;
using SpatialMap_SparseSpatialMap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class PropItemController : MonoBehaviour
    {
        [HideInInspector] public PropGridController PropGridController;
        [HideInInspector] public PrefabGridController PrefabGridController;
        public Image Icon;
        public Image Background;
        public TextMeshProUGUI NameText;
        [HideInInspector] public bool IsSelected = false;

        public PropCollection.Templet Templet { get; private set; }
        public event Action SelectEvent;
        public event Action DeselectEvent;

        [SerializeField] private Color m_DefaultColor;
        [SerializeField] private Color m_SelectedColor;
        [SerializeField] private float m_DefaultScale = 1.0f;
        [SerializeField] private float m_SelectedScale = 1.05f;

        private void Awake()
        {
            PropGridController = GetComponentInParent<PropGridController>();
            PrefabGridController = GetComponentInParent<PrefabGridController>();
            Deselect();
        }

        public void SetData(PropCollection.Templet templet)
        {
            Templet = templet;
            Icon.sprite = templet.Icon;
            if (NameText != null)
            {
                NameText.text = templet.Name;
            }
        }

        public void SwitchSelected()
        {
            IsSelected = !IsSelected;

            if (IsSelected)
            {
                Select();
            }
            else
            {
                Deselect();
            }
        }

        public void Select()
        {
            if (!IsSelected)
            {
                IsSelected = true;

                Background.color = m_SelectedColor;
                //NameText.fontStyle = FontStyles.Bold;
                Background.transform.localScale = new Vector3(m_SelectedScale, m_SelectedScale, m_SelectedScale);

                if (PropGridController != null)
                {
                    PropGridController.Select(this);
                }

                if (PrefabGridController != null)
                {
                    PrefabGridController.Select(this);
                }

                if (SelectEvent != null)
                {
                    SelectEvent();
                }
            }
        }

        public void Deselect()
        {
            if (IsSelected)
            {
                IsSelected = false;

                Background.color = m_DefaultColor;
                //NameText.fontStyle = FontStyles.Normal;
                Background.transform.localScale = new Vector3(m_DefaultScale, m_DefaultScale, m_DefaultScale);

                //SolanaNftGridController.Deselect(this);

                if (DeselectEvent != null)
                {
                    DeselectEvent();
                }
            }
        }
    }
}