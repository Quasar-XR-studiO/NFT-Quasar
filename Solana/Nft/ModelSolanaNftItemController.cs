using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AllArt.Solana.Nft;
using Rarible;
using TMPro;
using TriLibCore.Samples;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ZBoom.Common.SpatialMap;

public class ModelSolanaNftItemController : InfoSolanaNftItemController
{
    public GameObject InfoCanvas;
    public ModelDownloader ModelDownloader;
    public TextMeshProUGUI ProgressTextView;
    
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
        
        FileData fileData = nft.metaplexData.data.nftFile;
        if (fileData != null)
        {
            ProgressView.SetActive(true);
            
            ModelDownloader.Download(
                fileData.uri,
                gameObject,
                onError: error =>
                {
                    ErrorView.SetActive(true);
                    ProgressView.SetActive(false);
                },
                onProgress: (context, progress) =>
                {
                    ProgressTextView.text = (progress * 100) + " %";
                    if (progress >= 1f)
                    {
                        InfoCanvas.gameObject.SetActive(false);
                        ErrorView.SetActive(false);
                        ProgressView.SetActive(false);

                        var animation = context.RootGameObject.GetComponent<Animation>();
                        if (animation != null)
                        {
                            animation.Play();
                        }
                    }
                },
                onLoad: context => { },
                onMaterialsLoad: context =>
                {
                    //context.RootGameObject.transform.localScale /= 100f;
                    context.RootGameObject.transform.localScale /= 50f;
                });
        }
    }
}