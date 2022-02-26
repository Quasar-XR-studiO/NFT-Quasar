using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AllArt.Solana.Nft;
using Rarible;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ZBoom.Common.SpatialMap;
using ZBoom.Solana;

public class ImageSolanaNftItemController : InfoSolanaNftItemController
{
    public Image NftImage;

    private TextureLoader m_TextureLoader;
    private AspectRatioFitter m_AspectRatioFitter;

    protected override void Awake()
    {
        base.Awake();
        m_TextureLoader = new TextureLoader();
        m_AspectRatioFitter = NftImage.GetComponent<AspectRatioFitter>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Create(Nft nft)
    {
        base.Create(nft);

        ProgressView.SetActive(true);
        GetTexture(nft);
        /*
        //MainThreadDispatcher.Instance().Enqueue(() =>
        if (nft.metaplexData.nftImage != null)
        {

            Texture2D texture = nft.metaplexData.nftImage.file;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);

            float k = (float) texture.width / (float) texture.height;
            float imageWidth = NftImage.rectTransform.rect.width;
            float imageHeight = imageWidth / k;
            NftImage.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);
            NftImage.sprite = sprite;
            NftImage.color = Color.white;
            NftImage.CalculateLayoutInputHorizontal();
            NftImage.CalculateLayoutInputVertical();

            ProgressView.SetActive(false);
        }
        else
        {
            
        }
        */
    }

    public async void GetTexture(Nft nft)
    {
        var nftImage = nft.metaplexData.nftImage;
        Texture2D texture = null;
        if (nftImage != null && nftImage.file != null)
        {
            texture = nft.metaplexData.nftImage.file;
        }
        else
        {
            Nft localNft = await Nft.TryGetNftData(nft.metaplexData.mint,
                SimpleWallet.Instance.ActiveRpcClient, true);
            
            texture = localNft.metaplexData.nftImage.file;
        }

        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);

        float k = (float) texture.width / (float) texture.height;
        float imageWidth = NftImage.rectTransform.rect.width;
        float imageHeight = imageWidth / k;
        NftImage.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);
        NftImage.sprite = sprite;
        NftImage.color = Color.white;
        NftImage.CalculateLayoutInputHorizontal();
        NftImage.CalculateLayoutInputVertical();

        ProgressView.SetActive(false);
    }
}