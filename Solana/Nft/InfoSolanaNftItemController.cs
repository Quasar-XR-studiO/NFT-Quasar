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

public class InfoSolanaNftItemController : BaseSolanaNftItemController
{
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI MintText;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI TypeText;

    public GameObject ProgressView;
    public GameObject ErrorView;

    protected override void Awake()
    {
        base.Awake();
        Clear();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected void Clear()
    {
        ProgressView.SetActive(false);
        ErrorView.SetActive(false);
    }

    public override void Create(Nft nft)
    {
        base.Create(nft);
        string name = nft.metaplexData.data.json.name;
        string description = nft.metaplexData.data.json.description;
        string category = nft.metaplexData.data.json.properties.category;

        if (string.IsNullOrEmpty(name))
        {
            TitleText.gameObject.SetActive(false);
        }
        else
        {
            TitleText.gameObject.SetActive(true);
            TitleText.text = name;
        }

        if (string.IsNullOrEmpty(description))
        {
            DescriptionText.gameObject.SetActive(false);
        }
        else
        {
            DescriptionText.gameObject.SetActive(true);
            DescriptionText.text = description;
        }

        if (string.IsNullOrEmpty(category))
        {
            TypeText.gameObject.SetActive(false);
        }
        else
        {
            TypeText.gameObject.SetActive(true);
            //TypeText.gameObject.SetActive(false);
            TypeText.text = category;
        }

        MintText.gameObject.SetActive(true);
        //MintText.gameObject.SetActive(false);
        MintText.text = nft.metaplexData.mint;
    }
}