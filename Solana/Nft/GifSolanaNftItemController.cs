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

public class GifSolanaNftItemController : InfoSolanaNftItemController
{
    public ProGifPlayerTexture2D ProGifPlayerTexture2D;
    public RawImage NftImage;

    protected override void Awake()
    {
        base.Awake();
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

        FileData fileData = nft.metaplexData.data.nftFile;
        if (fileData != null)
        {
            ProGifPlayerTexture2D.Play(fileData.uri, true);
            ProGifPlayerTexture2D.OnTexture2DCallback = (texture) =>
            {
                /*
                float k = (float) texture.width / (float) texture.height;
                float imageWidth = NftImage.rectTransform.rect.width;
                float imageHeight = imageWidth / k;
                NftImage.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);
                NftImage.color = Color.white;
                */

                NftImage.texture = texture;
            };

            ProGifPlayerTexture2D.OnFirstFrame += frame =>
            {
                float k = (float) frame.width / (float) frame.height;
                float imageWidth = NftImage.rectTransform.rect.width;
                float imageHeight = imageWidth / k;
                NftImage.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);
                NftImage.color = Color.white;

                ProgressView.SetActive(false);
            };
        }
    }
}