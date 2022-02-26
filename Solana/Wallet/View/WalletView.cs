using Solnet.Rpc.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using SFB;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text;
using System;
using AllArt.Solana.Nft;

namespace ZBoom.Solana
{
    [RequireComponent(typeof(TxtFileLoader))]
    public class WalletView : SimpleWalletView
    {
        public TextMeshProUGUI BalanceText;
        public TextMeshProUGUI WalletPublicKeyText;
        public Transform TokenContainer;
        public TokenItem TokenItemPrefab;

        [HideInInspector] public List<TokenItem> TokenItems = new List<TokenItem>();

        private TxtFileLoader m_TxtFileLoader;
        private CancellationTokenSource m_StopTask;

        private string m_MnemonicsFileTitle = "Mnemonics";
        private string m_PrivateKeyFileTitle = "PrivateKey";

        private void Start()
        {
            m_TxtFileLoader = GetComponent<TxtFileLoader>();
            
            WebSocketActions.WebSocketAccountSubscriptionAction += (bool value) => 
            {
                MainThreadDispatcher.Instance().Enqueue(() =>
                {
                    UpdateWalletBalanceDisplay();
                });
            };
            
            WebSocketActions.CloseWebSocketConnectionAction += DisconnectToWebSocket;

            m_TxtFileLoader.TxtSavedAction += SaveMnemonicsToFile;
            m_TxtFileLoader.TxtSavedAction += SavePrivateKeyToFile;

            m_StopTask = new CancellationTokenSource();
        }

        public void Refresh()
        {
            UpdateWalletBalanceDisplay();
            GetOwnedTokenAccounts();
        }

        public void Logout()
        {
            SimpleWallet.Instance.DeleteWalletAndClearKey();
            ViewManager.ShowStartView();
        }
        
        public void SaveMnemonics()
        {
            m_TxtFileLoader.SaveTxt(m_MnemonicsFileTitle,
                SimpleWallet.Instance.LoadPlayerPrefs(SimpleWallet.Instance.MnemonicsKey), false);
        }

        public void SavePrivateKey()
        {
            m_TxtFileLoader.SaveTxt(m_PrivateKeyFileTitle, SimpleWallet.Instance.privateKey, false);
        }

        public void SavePrivateKeyToFile(string path, string key, string fileTitle)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (fileTitle != m_PrivateKeyFileTitle)
            {
                return;
            }

            if (path != string.Empty)
            {
                File.WriteAllText(path, key);
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(key);
                //DownloadFile(gameObject.name, "OnFileDownload", m_PrivateKeyFileTitle + ".txt", bytes, bytes.Length);
            }
        }

        public void SaveMnemonicsToFile(string path, string mnemonics, string fileTitle)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (fileTitle != m_MnemonicsFileTitle)
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
                    //DownloadFile(gameObject.name, "OnFileDownload", m_MnemonicsFileTitle + ".txt", bytes, bytes.Length);
                }
            }
            else if (SimpleWallet.Instance.StorageMethodReference == StorageMethod.SimpleTxt)
            {
                if(path != string.Empty)
                    File.WriteAllText(path, mnemonics);
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(mnemonics);
                    //DownloadFile(gameObject.name, "OnFileDownload", m_MnemonicsFileTitle + ".txt", bytes, bytes.Length);
                }
            }
        }

        public void TransitionToTransfer(object data = null)
        {
            ViewManager.ShowTransferView(data);
            //ViewManager.ShowScreen(this, "transfer_screen", data);
        }

        private async void UpdateWalletBalanceDisplay()
        {
            if (SimpleWallet.Instance.wallet is null)
            {
                return;
            }

            double sol = await SimpleWallet.Instance.GetSolAmmount(SimpleWallet.Instance.wallet.GetAccount(0));
            MainThreadDispatcher.Instance().Enqueue(() =>
            {
                BalanceText.text = $"{sol}";
            });
        }

        private void DisconnectToWebSocket()
        {
            MainThreadDispatcher.Instance().Enqueue(() =>
            {
                ViewManager.ShowScreen(this, "login_screen");
            });
            
            MainThreadDispatcher.Instance().Enqueue(() =>
            {
                SimpleWallet.Instance.DeleteWalletAndClearKey();
            });
        }

        public override void ShowView(object data = null)
        {
            base.ShowView();
            gameObject.SetActive(true);
            
            string publicKey = SimpleWallet.Instance.LoadPlayerPrefs(SimpleWallet.Instance.PublicKeyKey);
            if (string.IsNullOrEmpty(publicKey))
            {
                WalletPublicKeyText.gameObject.SetActive(false);
            }
            else
            {
                WalletPublicKeyText.gameObject.SetActive(true);
                WalletPublicKeyText.text = publicKey;
            }

            GetOwnedTokenAccounts();
            UpdateWalletBalanceDisplay();
        }

        public override void HideView()
        {
            base.HideView();
            gameObject.SetActive(false);
        }

        public async void GetOwnedTokenAccounts()
        {
            ClearTokens();
            TokenAccount[] result = await SimpleWallet.Instance.GetOwnedTokenAccounts(SimpleWallet.Instance.wallet.GetAccount(0));

            if (result != null && result.Length > 0)
            {
                int itemIndex = 0;
                foreach (TokenAccount item in result)
                {
                    if (float.Parse(item.Account.Data.Parsed.Info.TokenAmount.Amount) > 0)
                    {
                        Nft nft = await Nft.TryGetNftData(item.Account.Data.Parsed.Info.Mint,
                            SimpleWallet.Instance.ActiveRpcClient, false, false);

                        //Task<AllArt.Solana.Nft.Nft> t = Task.Run<AllArt.Solana.Nft.Nft>( async () => {
                        //    return await AllArt.Solana.Nft.Nft.TryGetNftData(item.Account.Data.Parsed.Info.Mint, SimpleWallet.Instance.ActiveRpcClient, false);
                        //}, m_StopTask.Token);

                        //Debug.Log("new");
                        //AllArt.Solana.Nft.Nft nft = t.Result;

                        TokenItem tokenItem = Instantiate(TokenItemPrefab, TokenContainer);
                        tokenItem.InitializeData(item, this, nft);
                        TokenItems.Add(tokenItem);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (m_StopTask is null)
            {
                return;
            }
            m_StopTask.Cancel();
        }

        private void ClearTokens()
        {
            foreach (TokenItem token in TokenItems)
            {
                Destroy(token.gameObject);
            }
            TokenItems.Clear();
        }

        /*
        //
        // WebGL
        //
        [DllImport("__Internal")]
        private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

        // Called from browser
        public void OnFileDownload()
        {
            
        }
        */
    }
}
