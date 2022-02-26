using System;
using Rarible;
using SpatialMap_SparseSpatialMap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class RariblePropItemController : MonoBehaviour
    {
        [HideInInspector] public RaribleItem RaribleItem;
        [HideInInspector] public RaribleGridController RaribleGridController;
        public Image Icon;
        public Image Background;
        public TextMeshProUGUI NameText;
        [HideInInspector] public bool IsSelected = false;

        private TextureLoader m_TextureLoader;
        public PropCollection.Templet Templet { get; private set; }
        public event Action SelectEvent;
        public event Action DeselectEvent;

        private void Awake()
        {
            RaribleGridController = GetComponentInParent<RaribleGridController>();
            Deselect();
        }

        private void Start()
        {
        }

        public void SetData(RaribleItem raribleItem)
        {
            RaribleItem = raribleItem;
            Icon.color = Color.black;
            if (NameText != null)
            {
                NameText.text = raribleItem.meta.name;
            }

            string url = "";
            foreach (var content in raribleItem.meta.content)
            {
                if (content.GetType() == Content.TypeContent.IMAGE && !content.mimeType.Contains("gif"))
                {
                    url = content.url;
                }
            }

            if (url.Length > 2)
            {
                m_TextureLoader = new TextureLoader();

                StartCoroutine(m_TextureLoader.LoadTexture(url, new IResultListener<Sprite>()
                {
                    OnSuccess = (sprite, message) =>
                    {
                        Icon.color = Color.white;
                        Icon.sprite = sprite;
                    },
                    OnError = errorMessage =>
                    {
                    }
                }));
            }
        }

        public void Select()
        {
            if (!IsSelected)
            {
                IsSelected = true;

                if (RaribleGridController != null)
                {
                    RaribleGridController.Select(this);
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

                //SolanaNftGridController.Deselect(this);

                if (DeselectEvent != null)
                {
                    DeselectEvent();
                }
            }
        }
    }
}