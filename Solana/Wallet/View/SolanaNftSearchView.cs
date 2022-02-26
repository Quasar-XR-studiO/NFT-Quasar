using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using AllArt.Solana.Nft;
using Michsky.UI.ModernUIPack;
using Rarible;
using TMPro;
using UnityEngine;
using ZBoom.Common.SpatialMap;
using ZBoom.Solana;

public class SolanaNftSearchView : MonoBehaviour
{
    public SpatialMapGameObjectController SpatialMapGameObjectController;
    public GameObject MainPanel;
    
    public TMP_InputField InputField;
    public GameObject ProgressPanel;
    public GameObject ErrorPanel;

    private void Start()
    {
    }

    private void Update()
    {
    }
    
    public void Paste()
    {
        InputField.text = UniClipboard.GetText();
        InputField.ForceLabelUpdate();
    }

    public void Search()
    {
        ProgressPanel.SetActive(true);
        
        string valueInput = InputField.text;
        Uri uriInput;
        if (Uri.TryCreate(valueInput, UriKind.Absolute, out uriInput))
        {
            string[] segments = uriInput.Segments;
            if (segments != null && segments.Length > 0)
            {
                int size = segments.Length;
                string idToken = segments[size - 1];
                idToken = idToken.Replace("/", "");
                GetNftItem(idToken);
            }
            else
            {
                ProgressPanel.SetActive(false);
                ErrorPanel.SetActive(true);
            }
        }
        else
        {
            string idToken = valueInput;
            GetNftItem(idToken);
        }
    }
    
    public async void GetNftItem(string id)
    {
        //NFTProData proNft = await Nft.TryGetNftPro(id, SimpleWallet.Instance.ActiveRpcClient);
        
        Nft nft = await Nft.TryGetNftData(id,
            SimpleWallet.Instance.ActiveRpcClient, true, false);

        ProgressPanel.SetActive(false);
        ErrorPanel.SetActive(false);
        
        if (nft != null)
        {
            MainPanel.SetActive(false);
            SpatialMapGameObjectController.SelectSolanaNftTemplate(nft);
        }
        else
        {
            ErrorPanel.SetActive(true);
        }
        
    }
}