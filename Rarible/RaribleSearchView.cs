using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Michsky.UI.ModernUIPack;
using Rarible;
using TMPro;
using UnityEngine;
using ZBoom.Common.SpatialMap;

public class RaribleSearchView : MonoBehaviour
{
    public SpatialMapGameObjectController SpatialMapGameObjectController;
    public GameObject MainPanel;
    
    public TMP_InputField UrlInputField;
    public TMP_InputField IdInputField;
    public HorizontalSelector BlockchainSelector;
    public GameObject ProgressPanel;
    public GameObject ErrorPanel;

    private void Start()
    {
    }

    private void Update()
    {
    }

    public void GetItemByUrl()
    {
        // TODO: validate and regex url 
        string urlInput = UrlInputField.text;
        Uri uriInput;
        if (Uri.TryCreate(urlInput, UriKind.Absolute, out uriInput))
        {
            string[] segments = uriInput.Segments;
            if (segments != null && segments.Length > 0)
            {
                int size = segments.Length;
                string idToken = segments[size - 1];
                idToken = idToken.Replace("/", "");
                string nameBlockchain = segments[size - 2];
                nameBlockchain = nameBlockchain.Replace("/", "");
                if (nameBlockchain.Equals("token", StringComparison.OrdinalIgnoreCase))
                {
                    nameBlockchain = "Ethereum";
                }

                nameBlockchain = nameBlockchain.ToUpper();
                string id = $"{nameBlockchain}:{idToken}";

                DownloadRaribleItem(id);
            }
            else
            {
                ShowError(true);
            }
        }
        else
        {
            ShowError(true);
        }
    }

    public void GetItemById()
    {
        string blockchainName = BlockchainSelector.itemList[BlockchainSelector.index].itemTitle;
        blockchainName = blockchainName.ToUpper();
        
        string idInput = IdInputField.text;
        string id = $"{blockchainName}:{idInput}";
        DownloadRaribleItem(id);
    }

    private void DownloadRaribleItem(string id)
    {
        ShowError(false);
        ShowProgress(true);

        ViewManager.Instance.GetRaribleItem(id, new IResultListener<RaribleItem>()
        {
            OnSuccess = (raribleItem, message) =>
            {
                ShowProgress(false);
                ShowError(false);

                MainPanel.SetActive(false);
                SpatialMapGameObjectController.SelectNftTemplate(raribleItem);
            },
            OnError = errorMessage =>
            {
                ProgressPanel.gameObject.SetActive(false);
                ShowProgress(false);
                ShowError(true);
            }
        });
    }
    
    private void ShowError(bool isShow)
    {
        if (isShow)
        {
            ErrorPanel.SetActive(true);
        }
        else
        {
            ErrorPanel.SetActive(false);
        }
    }

    private void ShowProgress(bool isShow)
    {
        if (isShow)
        {
            ProgressPanel.SetActive(true);
        }
        else
        {
            ProgressPanel.SetActive(false);
        }
    }

}