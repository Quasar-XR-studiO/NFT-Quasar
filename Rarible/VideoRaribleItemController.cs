using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rarible;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using ZBoom.Common.SpatialMap;

public class VideoRaribleItemController : BaseRaribleItemController
{
    public VideoPlayer VideoPlayer;
    public string UrlVideo = "";

    //public NftImage NftImage;
    public GameObject Image;
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
        //m_AspectRatioFitter = NftImage.GetComponent<AspectRatioFitter>();

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
        Image.SetActive(false);
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

        Image.SetActive(false);
        VideoPlayer.url = originalContent.url;
        VideoPlayer.errorReceived += (source, message) =>
        {
            //NftImage.SetActive(false);
            //ErrorView.SetActive(true);
            //ProgressView.SetActive(false);
            
            Image.SetActive(false);
            ErrorPanel.SetActive(false);
            ProgressPanel.SetActive(true);
            TryDownloadAndPlayVideo(raribleItem, originalContent);
        };
        VideoPlayer.prepareCompleted += source =>
        {
            Image.SetActive(true);
            ErrorPanel.SetActive(false);
            ProgressPanel.SetActive(false);
            VideoPlayer.Play();
        };
        VideoPlayer.Prepare();
        /*
        try
        {
            VideoPlayer.Prepare();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        */

        /*
         
        if (m_TextureLoader == null)
        {
            m_TextureLoader = new TextureLoader();
        }
         
        StartCoroutine(m_TextureLoader.LoadTexture(originalContent.url, new IResultListener<Sprite>()
        {
            OnSuccess = (sprite, message) =>
            {
                ProgressView.SetActive(false);
                if (sprite.texture.width > sprite.texture.height)
                {
                    m_AspectRatioFitter.aspectRatio = 10f;
                }
                else
                {
                    m_AspectRatioFitter.aspectRatio = 0.1f;
                }

                NftImage.sprite = sprite;
            },
            OnError = errorMessage =>
            {
                ProgressView.SetActive(false);
                ErrorView.SetActive(true);
            }
        }));
        */
    }

    public void TryDownloadAndPlayVideo(RaribleItem raribleItem, Content originalContent)
    {
        StartCoroutine(DownloadAndPlayVideo(originalContent.url, "video.mp4", true));
    }

    IEnumerator DownloadAndPlayVideo(string videoUrl, string saveFileName, bool overwriteVideo)
    {
        string saveDir = Path.Combine(Application.persistentDataPath, saveFileName);

        //Play back Directory
        string playbackDir = saveDir;
#if UNITY_IPHONE
        playbackDir = "file://" + saveDir;
#endif

        bool downloadSuccess = false;
        byte[] vidData = null;

        string[] persistantData = Directory.GetFiles(Application.persistentDataPath);
        if (persistantData.Contains(playbackDir) && !overwriteVideo)
        {
            Debug.Log("Video already exist. Playing it now");
            PlayVideo(playbackDir);
            yield break;
        }
        else if (persistantData.Contains(playbackDir) && overwriteVideo)
        {
            Debug.Log("Video already exist [but] we are [Re-downloading] it");
            yield return DownloadData(videoUrl, (status, dowloadData) =>
            {
                downloadSuccess = status;
                vidData = dowloadData;
            });
        }
        else
        {
            Debug.Log("Video Does not exist. Downloading video");
            yield return DownloadData(videoUrl, (status, dowloadData) =>
            {
                downloadSuccess = status;
                vidData = dowloadData;
            });
        }

        if (downloadSuccess)
        {
            SaveVideoFile(saveDir, vidData);
            PlayVideo(playbackDir);
        }
    }

    void PlayVideo(string path)
    {
        VideoPlayer.source = VideoSource.VideoClip;
        VideoPlayer.url = path;
        VideoPlayer.errorReceived += (source, message) =>
        {
            Image.SetActive(false);
            ErrorPanel.SetActive(true);
            ProgressPanel.SetActive(false);
        };
        VideoPlayer.prepareCompleted += source =>
        {
            Image.SetActive(true);
            ErrorPanel.SetActive(false);
            ProgressPanel.SetActive(false);
            VideoPlayer.Play();
        };
        VideoPlayer.Prepare();
    }

    IEnumerator DownloadData(string videoUrl, Action<bool, byte[]> result)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(videoUrl);
        webRequest.Send();

        while (!webRequest.isDone)
        {
            Debug.Log("Downloading: " + webRequest.downloadProgress);
            yield return null;
        }

        if (webRequest.isNetworkError)
        {
            Debug.Log("Error while downloading Video: " + webRequest.error);
            yield break; 
        }

        Debug.Log("Video Downloaded");
        result(!webRequest.isNetworkError, webRequest.downloadHandler.data);
    }

    private bool SaveVideoFile(string saveDir, byte[] vidData)
    {
        try
        {
            FileStream stream = new FileStream(saveDir, FileMode.Create);
            stream.Write(vidData, 0, vidData.Length);
            stream.Close();
            Debug.Log("Video Downloaded to: " + saveDir.Replace("/", "\\"));
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Error while saving Video File: " + e.Message);
        }

        return false;
    }


    public void Play()
    {
        if (VideoPlayer.isPrepared)
        {
            VideoPlayer.Play();
        }
    }

    public void Pause()
    {
        if (VideoPlayer.isPrepared)
        {
            VideoPlayer.Stop();
        }
    }
}