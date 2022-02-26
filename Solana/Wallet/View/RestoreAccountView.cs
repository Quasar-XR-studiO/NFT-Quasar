using Newtonsoft.Json;
using SFB;
using Solnet.Wallet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Solana
{
    [RequireComponent(typeof(TxtFileLoader))]
    public class RestoreAccountView : SimpleWalletView
    {
        public TMP_InputField MnemonicInput;
        public Button RestoreButton;
        public Button MnemonicLoadButton;
        public TMP_InputField PasswordInput;

        private string[] m_Paths;
        private string m_Path;
        private string m_LoadedMnemonics;

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
        }

        private void Start()
        {
            /*
            RestoreButton.onClick.AddListener(() =>
            {
                CreateNewAccount();
            });
            */

            //MnemonicLoadButton.onClick.AddListener(LoadMnemonicsFromFile);
        }
        
        public void PasteMnemonic()
        {
            MnemonicInput.text = UniClipboard.GetText();
        }

        public void CreateNewAccount()
        {
            if (string.IsNullOrEmpty(PasswordInput.text))
            {
                ShowNotification(ERROR_MESSAGE, "Enter a password.");
                return;
            }
            
            if (string.IsNullOrEmpty(MnemonicInput.text))
            {
                ShowNotification(ERROR_MESSAGE, "Enter a seed phrase.");
                return;
            }
            
            SimpleWallet.Instance.DeleteWalletAndClearKey();
            
            SimpleWallet.Instance.SavePlayerPrefs(SimpleWallet.Instance.PasswordKey, PasswordInput.text);

            Wallet keypair = SimpleWallet.Instance.GenerateWalletWithMenmonic(MnemonicInput.text);
            if (keypair != null)
            {
                //ViewManager.ShowScreen(this, "wallet_screen");
                
                string publicKey = SimpleWallet.Instance.wallet.GetAccount(0).GetPublicKey;
                SimpleWallet.Instance.SavePlayerPrefs(SimpleWallet.Instance.PublicKeyKey, publicKey);
                ViewManager.ShowWalletView();
            }
            else
            {
                ShowNotification(ERROR_MESSAGE, "Keywords are not in a valid format.");
            }
        }

        public override void ShowView(object data = null)
        {
            base.ShowView();

            //MnemonicText.text = "margin toast sheriff air tank liar tuna oyster cake tell trial more rebuild ostrich sick once palace uphold fall faculty clap slam job pitch";
            //MnemonicText.text = "gym basket dizzy chest pact rubber canvas staff around shadow brain purchase hello parent digital degree window version still rather measure brass lock arrest";
            MnemonicInput.text = String.Empty;
            PasswordInput.text = String.Empty;

            gameObject.SetActive(true);
        }

        public override void HideView()
        {
            base.HideView();
            gameObject.SetActive(false);
        }

        public void LoadMnemonicsFromFile()
        {
            try
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                 UploadFile(gameObject.name, "OnFileUpload", ".txt", false);
#elif UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_STANDALONE
                m_Paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "txt", false);
                m_Path = m_Paths[0];
                m_LoadedMnemonics = File.ReadAllText(m_Path);
#elif UNITY_ANDROID || UNITY_IPHONE
                string txt;
                txt = NativeFilePicker.ConvertExtensionToFileType("txt");
                NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
		            {
			            if (path == null)
				            Debug.Log("Operation cancelled");
			            else
			            {
                            m_LoadedMnemonics = File.ReadAllText(path);
                        }
		            }, new string[] { txt });
		        Debug.Log("Permission result: " + permission);
#endif

#if UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IPHONE
                ResolveMnemonicsByType();
#endif
            }

            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        private void ResolveMnemonicsByType()
        {
            if (!string.IsNullOrEmpty(m_LoadedMnemonics))
            {
                if (SimpleWallet.Instance.StorageMethod == StorageMethod.JSON)
                {
                    try
                    {
                        JSONDeserialization();
                    }
                    catch
                    {
                        try
                        {
                            SimpleTxtDeserialization();
                        }
                        catch
                        {
                            return;
                        }
                    }
                }
                else if (SimpleWallet.Instance.StorageMethod == StorageMethod.SimpleTxt)
                {
                    try
                    {
                        SimpleTxtDeserialization();
                    }
                    catch
                    {
                        try
                        {
                            JSONDeserialization();
                        }
                        catch
                        {
       
                            return;
                        }
                    }
                }
            }

            void JSONDeserialization()
            {
                MnemonicsModel mnemonicsModel = JsonConvert.DeserializeObject<MnemonicsModel>(m_LoadedMnemonics);
                string deserializedMnemonics = string.Join(" ", mnemonicsModel.Mnemonics);
                MnemonicInput.text = deserializedMnemonics;
            }

            void SimpleTxtDeserialization()
            {
                MnemonicInput.text = m_LoadedMnemonics;
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

            MainThreadDispatcher.Instance().Enqueue(() => { m_LoadedMnemonics = loader.text; });
            MainThreadDispatcher.Instance().Enqueue(() => { ResolveMnemonicsByType(); });           
        }
        */
    }
}
