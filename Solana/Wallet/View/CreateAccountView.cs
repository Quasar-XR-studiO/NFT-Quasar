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
    [RequireComponent(typeof(TxtFileLoader))]
    public class CreateAccountView : SimpleWalletView
    {
        public TMP_InputField MnemonicText;
        public Button CreateButton;
        //public Button RestoreButton;
        public Button SaveMnemonicButton;
        public TMP_InputField PasswordInput;
        //public TextMeshProUGUI ErrorPasswordText;

        private TxtFileLoader m_TxtFileLoader;
        private string m_MnemonicFileTitle = "Mnemonics";
        private string m_PrivateKeyFileTitle = "PrivateKey";

        private void Start()
        {
            m_TxtFileLoader = GetComponent<TxtFileLoader>();
            // Example:
            // "margin toast sheriff air tank liar tuna oyster cake tell trial more rebuild ostrich sick once palace uphold fall faculty clap slam job pitch";
            MnemonicText.text = WalletKeyPair.GenerateNewMnemonic();

            //CreateButton.onClick.AddListener(() => { CreateNewAccount(); });

            //SaveMnemonicButton.onClick.AddListener(() => { SaveMnemonicToFile(); });

            m_TxtFileLoader.TxtSavedAction += SaveMnemonicsToTxtFile;
        }

        private void OnEnable()
        {
            //ErrorPasswordText.gameObject.SetActive(false);
            MnemonicText.interactable = false;
            MnemonicText.text = WalletKeyPair.GenerateNewMnemonic();
        }

        public void CopyMnemonic()
        {
            UniClipboard.SetText(MnemonicText.text);
        }
        
        public void SaveMnemonicToFile()
        {
            m_TxtFileLoader.SaveTxt(m_MnemonicFileTitle, MnemonicText.text, false);
        }
        
        public void CreateNewAccount()
        {
            MainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (string.IsNullOrEmpty(PasswordInput.text))
                {
                    //ErrorPasswordText.gameObject.SetActive(true);
                    //ErrorPasswordText.text = "Need Password!";
                    
                    ShowNotification(ERROR_MESSAGE, "Enter a password.");
                    return;
                }
                
                SimpleWallet.Instance.DeleteWalletAndClearKey();

                SimpleWallet.Instance.SavePlayerPrefs(SimpleWallet.Instance.PasswordKey, PasswordInput.text);
                try
                {
                    SimpleWallet.Instance.GenerateWalletWithMenmonic(MnemonicText.text);
                    string mnemonics = MnemonicText.text;
                    //ViewManager.ShowScreen(this, "wallet_screen");
                    
                    string publicKey = SimpleWallet.Instance.wallet.GetAccount(0).GetPublicKey;
                    SimpleWallet.Instance.SavePlayerPrefs(SimpleWallet.Instance.PublicKeyKey, publicKey);

                    ViewManager.ShowTransferView();
                    
                    //ErrorPasswordText.gameObject.SetActive(false);
                }
                catch (Exception ex)
                {
                    PasswordInput.gameObject.SetActive(true);
                    PasswordInput.text = ex.ToString();
                }
            });
        }

        private void SaveMnemonicsToTxtFile(string path, string mnemonics, string fileTitle)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (fileTitle != m_MnemonicFileTitle)
            {
                return;
            }

            if (SimpleWallet.Instance.StorageMethodReference == StorageMethod.JSON)
            {
                List<string> mnemonicsList = new List<string>();

                string[] splittedStringArray = mnemonics.Split(' ');
                foreach (string stringInArray in splittedStringArray)
                {
                    mnemonicsList.Add(stringInArray);
                }
                MnemonicsModel mnemonicsModel = new MnemonicsModel
                {
                    Mnemonics = mnemonicsList
                };

                if (path != string.Empty)
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(mnemonicsModel));
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mnemonicsModel));
                    //DownloadFile(gameObject.name, "OnFileDownload", m_MnemonicFileTitle + ".txt", bytes, bytes.Length);
                }
            }
            else if (SimpleWallet.Instance.StorageMethodReference == StorageMethod.SimpleTxt)
            {
                if (path != string.Empty)
                    File.WriteAllText(path, mnemonics);
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(mnemonics);
                    //DownloadFile(gameObject.name, "OnFileDownload", m_MnemonicFileTitle + ".txt", bytes, bytes.Length);
                }
            }
        }
        
        /*
        //
        // WebGL
        //
        [DllImport("__Internal")]
        private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

        // Called from browser
        private void OnFileDownload()
        {

        }
        */
    } 
}
