using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Rarible;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ZBoom.Common.SpatialMap;

public class ImageRaribleItemController : BaseRaribleItemController
{
    public Image Image;
    public TextMeshProUGUI TitleTextView;
    public TextMeshProUGUI DescriptionTextView;
    public TextMeshProUGUI TypeTextView;

    public GameObject ProgressPanel;
    public GameObject ErrorPanel;

    private TextureLoader m_TextureLoader;
    private AspectRatioFitter m_AspectRatioFitter;
    
    protected override void Awake()
    {
        base.Awake();
        m_TextureLoader = new TextureLoader();
        m_AspectRatioFitter = Image.GetComponent<AspectRatioFitter>();
        
        Clear();
        //TryDownloadAndShow("https://ipfs://Qma3F5J3DsuAG6d15wS3DsHKAHxMTvrRi5gEYdEcUVPsms");
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
        StartCoroutine(m_TextureLoader.LoadTexture(originalContent.url, new IResultListener<Sprite>()
        {
            OnSuccess = (sprite, message) =>
            {
                ProgressPanel.SetActive(false);
                if (sprite.texture.width > sprite.texture.height)
                {
                    m_AspectRatioFitter.aspectRatio = 10f;
                }
                else
                {
                    m_AspectRatioFitter.aspectRatio = 0.1f;
                }

                Image.color = Color.white;
                Image.sprite = sprite;
            },
            OnError = errorMessage =>
            {
                ProgressPanel.SetActive(false);
                ErrorPanel.SetActive(true);
            }
        }));
    }
    
    public void TryDownloadAndShow(string url)
    {
        string savePath = Path.Combine(Application.persistentDataPath, "data");
        savePath = Path.Combine(savePath, "Images");
        savePath = Path.Combine(savePath, "NftImage.png");
        DownloadImage(url,savePath);
    }
    
    public void DownloadImage(string url, string pathToSaveImage)
    {
        StartCoroutine(DownloadImageCoroutine(url,pathToSaveImage));
    }

    private IEnumerator DownloadImageCoroutine(string url, string savePath)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
            }
            else
            {
                Debug.Log("Success");
                Texture myTexture = DownloadHandlerTexture.GetContent(uwr);
                byte[] results = uwr.downloadHandler.data;
                SaveImage(savePath, results);

                StartCoroutine(m_TextureLoader.LoadTexture(savePath, new IResultListener<Sprite>()
                {
                    OnSuccess = (sprite, message) =>
                    {
                        ProgressPanel.SetActive(false);
                        if (sprite.texture.width > sprite.texture.height)
                        {
                            m_AspectRatioFitter.aspectRatio = 10f;
                        }
                        else
                        {
                            m_AspectRatioFitter.aspectRatio = 0.1f;
                        }

                        Image.color = Color.white;
                        Image.sprite = sprite;
                    },
                    OnError = errorMessage =>
                    {
                        ProgressPanel.SetActive(false);
                        ErrorPanel.SetActive(true);
                    }
                }));
            }
        }
    }
    
    private void SaveImage(string path, byte[] imageBytes)
    {
        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            File.WriteAllBytes(path, imageBytes);
            Debug.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    private byte[] LoadImage(string path)
    {
        byte[] dataByte = null;

        //Exit if Directory or File does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Debug.LogWarning("Directory does not exist");
            return null;
        }

        if (!File.Exists(path))
        {
            Debug.Log("File does not exist");
            return null;
        }

        try
        {
            dataByte = File.ReadAllBytes(path);
            Debug.Log("Loaded Data from: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Load Data from: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }

        return dataByte;
    }
}