using System.Collections;
using System.Collections.Generic;
using Rarible;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZBoom.Common.SpatialMap;

public class GifRaribleItemController : BaseRaribleItemController
{
    //public RawImage Image;
    public UniGifImage UniGifImage;
    public TextMeshProUGUI TitleTextView;
    public TextMeshProUGUI DescriptionTextView;
    public TextMeshProUGUI TypeTextView;

    public GameObject ProgressPanel;
    public GameObject ErrorPanel;

    private TextureLoader m_TextureLoader;
    //private AspectRatioFitter m_AspectRatioFitter;
    
    protected override void Awake()
    {
        base.Awake();
        m_TextureLoader = new TextureLoader();
        //m_AspectRatioFitter = Image.GetComponent<AspectRatioFitter>();
        
        //Clear();
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
        if (m_TextureLoader == null)
        {
            m_TextureLoader = new TextureLoader();
        }

        TitleTextView.text = RaribleItem.meta.name;
        DescriptionTextView.text = RaribleItem.meta.description;
        TypeTextView.text = originalContent.type;
        
        ErrorPanel.SetActive(false);
        ProgressPanel.SetActive(true);
        
        StartCoroutine(UniGifImage.SetGifFromUrlCoroutine(originalContent.url, true, new IResultListener<string>()
        {
            OnSuccess = (sprite, message) =>
            {
                ProgressPanel.SetActive(false);
            },
            OnError = errorMessage =>
            {
                ProgressPanel.SetActive(false);
                ErrorPanel.SetActive(true);
            }
        }));
    }
}