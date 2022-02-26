using System.Collections;
using System.Collections.Generic;
using Rarible;
using TMPro;
using TriLibCore.Samples;
using UnityEngine;
using UnityEngine.UI;
using ZBoom.Common.SpatialMap;

public class ModelRaribleItemController : BaseRaribleItemController
{
    //public NftImage NftImage;
    public TextMeshProUGUI TitleTextView;
    public TextMeshProUGUI DescriptionTextView;
    public TextMeshProUGUI TypeTextView;
    public TextMeshProUGUI ProgressTextView;

    public GameObject InfoCanvas;
    public GameObject ProgressPanel;
    public GameObject ErrorPanel;

    public ModelDownloader ModelDownloader;
    
    //private TextureLoader m_TextureLoader;
    //private AspectRatioFitter m_AspectRatioFitter;
    
    protected override void Awake()
    {
        base.Awake();
        //m_TextureLoader = new TextureLoader();
        //m_AspectRatioFitter = NftImage.GetComponent<AspectRatioFitter>();
        ModelDownloader = GetComponent<ModelDownloader>();
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
        TypeTextView.text = originalContent.type;
        
        ErrorPanel.SetActive(false);
        ProgressPanel.SetActive(true);
      
        ModelDownloader.Download(
            originalContent.url, 
            gameObject,
            onError: error =>
            {
                ErrorPanel.SetActive(true);
                ProgressPanel.SetActive(false);
            },
            onProgress: (context, progress) =>
            {
                ProgressTextView.text = (progress * 100) + " %";
                if (progress >= 1f)
                {
                    InfoCanvas.gameObject.SetActive(false);
                    ErrorPanel.SetActive(false);
                    ProgressPanel.SetActive(false);
                    
                    var animation = context.RootGameObject.GetComponent<Animation>();
                    if (animation != null)
                    {
                        animation.Play();
                    }
                }
            },
            onLoad: context =>
            {
                
            },
            onMaterialsLoad: context =>
            {
                //context.RootGameObject.transform.localScale /= 100f;
                context.RootGameObject.transform.localScale /= 10f;
            });
    }
}