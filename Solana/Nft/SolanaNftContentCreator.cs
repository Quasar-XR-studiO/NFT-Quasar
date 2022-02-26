using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AllArt.Solana.Nft;
using Rarible;
using UnityEngine;
using UnityEngine.UI;

public class SolanaNftContentCreator : MonoBehaviour
{
    public enum NftType
    {
        UNKNOWN = 0,
        IMAGE,
        GIF,
        VIDEO,
        MODEL
    }

    public BaseSolanaNftItemController UnknownNftPrefab;
    public BaseSolanaNftItemController ImageNftPrefab;
    public BaseSolanaNftItemController VideoNftPrefab;
    public BaseSolanaNftItemController GifNftPrefab;
    public BaseSolanaNftItemController ModelNftPrefab;

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

    public BaseSolanaNftItemController GetNftPrefab(Nft solanaNft)
    {
        NftType mainNftType = NftType.UNKNOWN;
        FileData mainFile = null;
        List<FileData> files = solanaNft.metaplexData.data.json.properties.files;

        foreach (var currentFile in files)
        {
            NftType currentNftType = NftType.UNKNOWN;
            if (currentFile.type.Contains("image"))
            {
                currentNftType = NftType.IMAGE;
            }

            if (currentFile.type.Contains("gif"))
            {
                currentNftType = NftType.GIF;
            }

            if (currentFile.type.Contains("video"))
            {
                currentNftType = NftType.VIDEO;
            }

            if (currentFile.type.Contains("model"))
            {
                currentNftType = NftType.MODEL;
            }

            if (currentNftType > mainNftType)
            {
                mainNftType = currentNftType;
                mainFile = currentFile;
            }
        }

        if (mainNftType == NftType.UNKNOWN)
        {
            string urlImage = solanaNft.metaplexData.data.json.image;
            if (!string.IsNullOrEmpty(urlImage))
            {
                mainFile = new FileData();
                mainFile.uri = urlImage;
                if (urlImage.Contains("ext=gif"))
                {
                    mainFile.type = "image/gif";
                    mainNftType = NftType.GIF;
                }
                else
                {
                    mainFile.type = "image/png";
                    mainNftType = NftType.IMAGE;
                }
            }
        }

        solanaNft.metaplexData.data.nftFile = mainFile;

        BaseSolanaNftItemController nftItemController = null;

        switch (mainNftType)
        {
            case NftType.UNKNOWN:
                nftItemController = Instantiate(UnknownNftPrefab);
                nftItemController.name = UnknownNftPrefab.name;
                break;
            case NftType.IMAGE:
                nftItemController = Instantiate(ImageNftPrefab);
                nftItemController.name = ImageNftPrefab.name;
                break;
            case NftType.GIF:
                nftItemController = Instantiate(GifNftPrefab);
                nftItemController.name = GifNftPrefab.name;
                break;
            case NftType.VIDEO:
                nftItemController = Instantiate(VideoNftPrefab);
                nftItemController.name = VideoNftPrefab.name;
                break;
            case NftType.MODEL:
                nftItemController = Instantiate(ModelNftPrefab);
                nftItemController.name = ModelNftPrefab.name;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (nftItemController != null)
        {
            nftItemController.Nft = solanaNft;
        }

        return nftItemController;
    }
}