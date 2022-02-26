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
using UnityEngine.Video;
using ZBoom.Common.SpatialMap;

public class VideoSolanaNftItemController : InfoSolanaNftItemController
{
    public RenderTexture RenderTexturePrefab;
    public VideoPlayer VideoPlayer;
    public RawImage NftImage;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        RenderTexture renderTexture = new RenderTexture(RenderTexturePrefab.width, RenderTexturePrefab.height, RenderTexturePrefab.depth, RenderTexturePrefab.format);
        NftImage.texture = renderTexture;
        VideoPlayer.targetTexture = renderTexture;
    }

    public override void Create(Nft nft)
    {
        base.Create(nft);
        FileData fileData = nft.metaplexData.data.nftFile;
        if (fileData != null)
        {
            ProgressView.SetActive(true);
            
            VideoPlayer.url = fileData.uri;
            VideoPlayer.errorReceived += (source, message) =>
            {
                ErrorView.SetActive(true);
                ProgressView.SetActive(false);
            };
            VideoPlayer.prepareCompleted += source =>
            {
                float k = (float) source.width / (float) source.height;
                float imageWidth = NftImage.rectTransform.rect.width;
                float imageHeight = imageWidth / k;
                NftImage.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);
                NftImage.RecalculateClipping();
                
                ErrorView.SetActive(false);
                ProgressView.SetActive(false);
                VideoPlayer.Play();
            };
            VideoPlayer.Prepare();
        }
    }
}