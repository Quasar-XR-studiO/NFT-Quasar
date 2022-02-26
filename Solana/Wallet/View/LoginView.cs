using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Collections;
using SFB;
using System.Collections.Generic;
using System;
using UltimateClean;

namespace ZBoom.Solana
{
    [RequireComponent(typeof(TxtFileLoader))]
    public class LoginView : SimpleWalletView
    {
        public TMP_InputField PasswordInput;

        public TextMeshProUGUI WalletKeyText;

        //public Button CreateNewWalletBtn;
        //public Button LoginToWalletBtn;
        public Button LoginButton;

        private string m_Password;
        private string m_Mnemonics;
        private string m_Path;
        private SimpleWallet m_SimpleWallet;
        private Cypher m_Cypher;
        private string m_PubKey;
        private string[] m_Strings;
        private string m_LoadedKey;

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            PasswordInput.text = String.Empty;
        }

        private void Start()
        {
            m_Cypher = new Cypher();
            m_SimpleWallet = SimpleWallet.Instance;
            PasswordInput.text = "";

            PasswordInput.onSubmit.AddListener(delegate { Login(); });
            //LoginButton.onClick.AddListener(Login);
        }

        public override void ShowView(object data = null)
        {
            base.ShowView(data);
            string publicKey = SimpleWallet.Instance.LoadPlayerPrefs(SimpleWallet.Instance.PublicKeyKey);
            if (string.IsNullOrEmpty(publicKey))
            {
                WalletKeyText.gameObject.SetActive(false);
            }
            else
            {
                WalletKeyText.gameObject.SetActive(true);
                WalletKeyText.text = publicKey;
            }
        }

        public void Login()
        {
            if (string.IsNullOrEmpty(PasswordInput.text))
            {
                ShowNotification(ERROR_MESSAGE, "Enter a password.");
                return;
            }
        
            if (m_SimpleWallet.LoginCheckMnemonicAndPassword(PasswordInput.text))
            {
                SimpleWallet.Instance.GenerateWalletWithMenmonic(
                    m_SimpleWallet.LoadPlayerPrefs(m_SimpleWallet.MnemonicsKey));
                MainThreadDispatcher.Instance().Enqueue(() =>
                {
                    m_SimpleWallet.StartWebSocketConnection();
                });
                //ViewManager.ShowScreen(this, "wallet_screen");
                ViewManager.ShowWalletView();
                gameObject.SetActive(false);
            }
            else
            { 
                ShowNotification(ERROR_MESSAGE, "Authentication error. Incorrect name or seed phrase.");
                PasswordInput.text = string.Empty;
            }
        }

        /*
        //
        // WebGL
        //
        [DllImport("__Internal")]
        private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

        // Called from browser
        public void OnFileUpload(string url)
        {
            StartCoroutine(OutputRoutine(url));
        }

        private IEnumerator OutputRoutine(string url)
        {
            var loader = new WWW(url);
            yield return loader;
            m_LoadedKey = loader.text;

            //LoginWithPrivateKeyCallback();
        }
        */
    }
}