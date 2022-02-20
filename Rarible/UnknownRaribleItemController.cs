using System.Collections;
using System.Collections.Generic;
using Rarible;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZBoom.Common.SpatialMap;

public class UnknownRaribleItemController : BaseRaribleItemController
{
    public TextMeshProUGUI TitleTextView;
    public TextMeshProUGUI DescriptionTextView;

    public GameObject ProgressPanel;
    public GameObject ErrorPanel;

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

    private void Clear()
    {
        ProgressPanel.SetActive(false);
        ErrorPanel.SetActive(false);
    }

    public override void Create()
    {
        base.Create();
    }

    public override void Create(RaribleItem raribleItem, Content originalContent)
    {
        base.Create(raribleItem, originalContent);
        TitleTextView.text = RaribleItem.meta.name;
        DescriptionTextView.text = RaribleItem.meta.description;
        
        ErrorPanel.SetActive(false);
        ProgressPanel.SetActive(true);
    }
}