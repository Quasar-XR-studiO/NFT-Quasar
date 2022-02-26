using Newtonsoft.Json;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using AllArt.Solana;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Solana
{
    public class StartView : SimpleWalletView
    {
        public Button LoginButton;
        public Button CreateButton;
        public Button RestoreButton;
        
        private SimpleWallet m_SimpleWallet;

        private void Awake()
        {
            m_SimpleWallet = SimpleWallet.Instance;
        }

        private void OnEnable(){
        
        }

        public override void HideView()
        {
            base.HideView();
        }

        public override void ShowView(object data = null)
        {
            base.ShowView(data);
            if (m_SimpleWallet == null)
            {
                m_SimpleWallet = SimpleWallet.Instance;
            }
            string mnemonic = m_SimpleWallet.LoadPlayerPrefs(m_SimpleWallet.MnemonicsKey);
            if (string.IsNullOrEmpty(mnemonic))
            {
                LoginButton.interactable = false;
            }
            else
            {
                LoginButton.interactable = true;
            }
        }
    } 
}
