using System;
using AllArt.Solana.Nft;
using Rarible;
using SpatialMap_SparseSpatialMap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class SolanaNftPropItemController : MonoBehaviour
    {
        [HideInInspector] public Nft SolanaNft;

        [HideInInspector] public SolanaNftGridController SolanaNftGridController;
        [HideInInspector] public SolanaNftOwnedGridController SolanaNftOwnedGridController;
        public Image ImageNft;
        public ProGifPlayerTexture2D ProGifPlayerTexture2D;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI TypeText;
        
        [HideInInspector] public bool IsSelected = false;

        public event Action SelectEvent;
        public event Action DeselectEvent;


        private void Awake()
        {
            SolanaNftGridController = GetComponentInParent<SolanaNftGridController>();
            SolanaNftOwnedGridController = GetComponentInParent<SolanaNftOwnedGridController>();
            Deselect();
        }

        private void Start()
        {
        }

        public void SetData(Nft solanaNft)
        {
            SolanaNft = solanaNft;
            //ImageNft.color = Color.black;
            NameText.text = solanaNft.metaplexData.data.json.name;
            TypeText.text = solanaNft.metaplexData.data.json.properties.category;

            var files = solanaNft.metaplexData.data.json.properties.files;
            var urlImage = solanaNft.metaplexData.data.json.image;
            if (files.Count > 0 && files[0].type.Contains("gif") || urlImage.Contains("ext=gif"))
            {
                ProGifPlayerTexture2D.Play(urlImage, true);
                //ProGifPlayerTexture2D.Play(urlImage, false);
                ProGifPlayerTexture2D.OnTexture2DCallback = (texture) =>
                {
                    /*
                    Rect rect = new Rect(0, 0, texture.width, texture.height);
                    Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
                    ImageNft.sprite = sprite;
                    ImageNft.color = Color.white;
                    */
                };

                ProGifPlayerTexture2D.OnFirstFrame += frame =>
                {
                    Texture2D texture = frame.gifTexture.GetTexture2D();
                    Rect rect = new Rect(0, 0, frame.width, frame.height);
                    Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
                    ImageNft.sprite = sprite;
                    ImageNft.color = Color.white;
                    TypeText.text = "gif";
                    
                    ResizeImage();
                    ProGifPlayerTexture2D.Stop();
                };
            }
            else
            {
                Texture2D texture = solanaNft.metaplexData.nftImage.file;
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
                ImageNft.sprite = sprite;
                ImageNft.color = Color.white;
                
                ResizeImage();
            }
        }

        private void ResizeImage()
        {
            AspectRatioFitter aspectRatioFitter = ImageNft.GetComponent<AspectRatioFitter>();
            if (aspectRatioFitter != null)
            {
                if (ImageNft.sprite.texture.width > ImageNft.sprite.texture.height)
                {
                    aspectRatioFitter.aspectRatio = 10f;
                }
                else
                {
                    aspectRatioFitter.aspectRatio = 0.1f;
                }
            }
        }

        public void Select()
        {
            if (!IsSelected)
            {
                IsSelected = true;

                if (SolanaNftGridController != null)
                {
                    SolanaNftGridController.Select(this);
                }
                
                if (SolanaNftOwnedGridController != null)
                {
                    SolanaNftOwnedGridController.Select(this);
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