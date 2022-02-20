using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Rarible;
using UnityEngine;
using UnityEngine.UI;

public class RaribleContentCreator : MonoBehaviour
{
    public BaseRaribleItemController ImageRariblePrefab;
    public BaseRaribleItemController GifRariblePrefab;
    public BaseRaribleItemController VideoRariblePrefab;
    public BaseRaribleItemController AudioRariblePrefab;
    public BaseRaribleItemController ModelRariblePrefab;
    public BaseRaribleItemController UnknownRariblePrefab;

    //private const string m_ImageRegex = @"[^\s]+(\.(jpg|jpeg|png|gif|bmp))$";
    //private const string m_ImageRegex = @"^.*\.(jpg|JPG|gif|GIF|doc|DOC|pdf|PDF).*$";
    private Regex m_ImageRegex;
    private Regex m_ModelRegex;
    private Regex m_VideoRegex;

    private void Start()
    {
        m_ImageRegex = new Regex(@"^.*\.(jpg|jpeg|png|gif|bmp)$");
        m_ModelRegex = new Regex(@"^.*\.(obj|fbx|gltf|glb|stl|ply|zip)$");
        m_VideoRegex = new Regex(@"^.*\.(mp4|mov|avi)$");
    }

    private void Update()
    {
    }

    public void Create(RaribleItem itemRarible)
    {
        Meta meta = itemRarible.meta;
        List<Content> contents = meta.content;
        Content originalContent = null;
        int size = contents.Count;
        if (size > 0)
        {
            foreach (var content in contents)
            {
                if (content.GetRepresentationType() == Content.TypeRepresentation.ORIGINAL)
                {
                    originalContent = content;
                    break;
                }
            }

            if (originalContent != null)
            {
                switch (originalContent.GetType())
                {
                    case Content.TypeContent.UNKNOWN:
                        break;
                    case Content.TypeContent.IMAGE:
                        CreateImage(itemRarible, originalContent);
                        break;
                    case Content.TypeContent.VIDEO:
                        CreateVideo(itemRarible, originalContent);
                        break;
                    case Content.TypeContent.AUDIO:
                        CreateAudio(itemRarible, originalContent);
                        break;
                    case Content.TypeContent.MODEL:
                        CreateModel(itemRarible, originalContent);
                        break;
                    case Content.TypeContent.AR:
                        CreateAR(itemRarible, originalContent);
                        break;
                }
            }
        }
    }

    public BaseRaribleItemController GetRaribleItemPrefabByContentType(RaribleItem itemRarible)
    {
        Meta meta = itemRarible.meta;
        List<Content> contents = meta.content;
        Content originalContent = null;
        int size = contents.Count;
        BaseRaribleItemController raribleItemController = null;
        if (size > 0)
        {
            foreach (var content in contents)
            {
                if (content.GetRepresentationType() == Content.TypeRepresentation.ORIGINAL)
                {
                    originalContent = content;
                    //break;
                }
            }

            if (originalContent == null)
            {
                originalContent = contents[0];
            }

            if (originalContent != null)
            {
                switch (originalContent.GetType())
                {
                    case Content.TypeContent.UNKNOWN:
                        raribleItemController = null;
                        break;
                    case Content.TypeContent.IMAGE:
                        raribleItemController = Instantiate(ImageRariblePrefab);
                        break;
                    case Content.TypeContent.VIDEO:
                        //raribleItemController = Instantiate(VideoRariblePrefab);
                        raribleItemController = Instantiate(ModelRariblePrefab);
                        break;
                    case Content.TypeContent.AUDIO:
                        raribleItemController = Instantiate(AudioRariblePrefab);
                        break;
                    case Content.TypeContent.MODEL:
                        raribleItemController = Instantiate(ModelRariblePrefab);
                        break;
                    case Content.TypeContent.AR:
                        raribleItemController = null;
                        break;
                }
            }
        }

        if (raribleItemController != null)
        {
            raribleItemController.OriginalContent = originalContent;
            raribleItemController.RaribleItem = itemRarible;
        }

        return raribleItemController;
    }

    public Content GetMainContent(RaribleItem itemRarible)
    {
        Meta meta = itemRarible.meta;
        List<Content> contents = meta.content;
        Content originalContent = null;
        int contentSize = contents.Count;

        if (contentSize > 0)
        {
            originalContent = contents[0];

            Content modelContent = contents.FindLast(content => m_ModelRegex.IsMatch(content.url));
            if (modelContent != null)
            {
                originalContent = modelContent;
            }
            else
            {
                Content videoContent = contents.FindLast(content =>
                    (content.GetType() == Content.TypeContent.VIDEO || m_VideoRegex.IsMatch(content.url))
                    && content.mimeType.Contains("video")
                );

                if (videoContent != null)
                {
                    originalContent = videoContent;
                }
                else
                {
                    foreach (var currentContent in contents)
                    {
                        if (currentContent.GetType() == Content.TypeContent.IMAGE ||
                            (currentContent.GetType() == Content.TypeContent.VIDEO &&
                             currentContent.mimeType.Contains("image")))
                        {
                            if (originalContent == null)
                            {
                                originalContent = currentContent;
                            }
                            else
                            {
                                int originalContentRepresentationType = (int) originalContent.GetRepresentationType();
                                int currentContentRepresentationType = (int) currentContent.GetRepresentationType();
                                if (currentContentRepresentationType > originalContentRepresentationType)
                                {
                                    originalContent = currentContent;
                                }
                            }
                        }
                    }
                }
            }
        }

        return originalContent;
    }

    public BaseRaribleItemController GetRaribleItemPrefab(RaribleItem itemRarible)
    {
        Meta meta = itemRarible.meta;
        List<Content> contents = meta.content;
        Content originalContent = null;
        int contentSize = contents.Count;
        BaseRaribleItemController raribleItemController = null;

        if (contentSize > 0)
        {
            Content.TypeContent mainContentType = Content.TypeContent.UNKNOWN;
            originalContent = contents[0];

            Content modelContent = contents.FindLast(content => m_ModelRegex.IsMatch(content.url));
            if (modelContent != null)
            {
                mainContentType = Content.TypeContent.MODEL;
                originalContent = modelContent;
            }
            else
            {
                Content videoContent = contents.FindLast(content =>
                    (content.GetType() == Content.TypeContent.VIDEO || m_VideoRegex.IsMatch(content.url))
                    && content.mimeType.Contains("video")
                );

                if (videoContent != null)
                {
                    mainContentType = Content.TypeContent.VIDEO;
                    originalContent = videoContent;
                }
                else
                {
                    foreach (var currentContent in contents)
                    {
                        if (currentContent.GetType() == Content.TypeContent.IMAGE ||
                            (currentContent.GetType() == Content.TypeContent.VIDEO &&
                             currentContent.mimeType.Contains("image")))
                        {
                            if (currentContent.mimeType.Contains("gif"))
                            {
                                mainContentType = Content.TypeContent.GIF;
                            }
                            else
                            {
                                mainContentType = Content.TypeContent.IMAGE;
                            }

                            if (originalContent == null)
                            {
                                originalContent = currentContent;
                            }
                            else
                            {
                                int originalContentRepresentationType = (int) originalContent.GetRepresentationType();
                                int currentContentRepresentationType = (int) currentContent.GetRepresentationType();
                                if (currentContentRepresentationType > originalContentRepresentationType)
                                {
                                    originalContent = currentContent;
                                }
                            }
                        }
                    }
                }
            }

            switch (mainContentType)
            {
                case Content.TypeContent.UNKNOWN:
                    raribleItemController = Instantiate(UnknownRariblePrefab);
                    raribleItemController.name = UnknownRariblePrefab.name; 
                    break;
                case Content.TypeContent.IMAGE:
                    raribleItemController = Instantiate(ImageRariblePrefab);
                    raribleItemController.name = ImageRariblePrefab.name; 
                    break;
                case Content.TypeContent.GIF:
                    raribleItemController = Instantiate(GifRariblePrefab);
                    raribleItemController.name = GifRariblePrefab.name; 
                    break;
                case Content.TypeContent.VIDEO:
                    raribleItemController = Instantiate(VideoRariblePrefab);
                    raribleItemController.name = VideoRariblePrefab.name; 
                    break;
                case Content.TypeContent.MODEL:
                    raribleItemController = Instantiate(ModelRariblePrefab);
                    raribleItemController.name = ModelRariblePrefab.name; 
                    break;
                case Content.TypeContent.AUDIO:
                    raribleItemController = Instantiate(UnknownRariblePrefab);
                    raribleItemController.name = UnknownRariblePrefab.name; 
                    break;
                case Content.TypeContent.AR:
                    raribleItemController = Instantiate(UnknownRariblePrefab);
                    raribleItemController.name = UnknownRariblePrefab.name; 
                    break;
            }
        }

        if (raribleItemController != null)
        {
            raribleItemController.OriginalContent = originalContent;
            raribleItemController.RaribleItem = itemRarible;
        }

        return raribleItemController;
    }

    public void CreateImage(RaribleItem itemRarible, Content content)
    {
    }

    public void CreateVideo(RaribleItem itemRarible, Content content)
    {
    }

    public void CreateAudio(RaribleItem itemRarible, Content content)
    {
    }

    public void CreateModel(RaribleItem itemRarible, Content content)
    {
    }

    public void CreateAR(RaribleItem itemRarible, Content content)
    {
        //TODO
    }
}