using AllArt.Solana.Nft;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Solana
{
    public class TransferView : SimpleWalletView
    {
        public TextMeshProUGUI OwnedAmmountText;
        public TextMeshProUGUI NftTitleText;
        public TMP_InputField PublicKeyInput;
        public TMP_InputField AmmountInput;
        public Button transferButton;
        public RawImage NftImage;
        public GameObject ParentNftImage;

        private TokenAccount m_TransferTokenAccount;
        private double m_OwnedSolAmmount;

        private void Start()
        {
            
        }

        public void TryTransfer()
        {
            if (m_TransferTokenAccount == null)
            {
                if (CheckInput())
                {
                    TransferSol();
                }
            }
            else
            {
                if (CheckInput())
                {
                    TransferToken();
                }
            }
        }

        private async void TransferSol()
        {
            RequestResult<string> result =
                await SimpleWallet.Instance.TransferSol(PublicKeyInput.text, long.Parse(AmmountInput.text));
            HandleResponse(result);
        }

       private bool CheckInput()
        {
            if (string.IsNullOrEmpty(AmmountInput.text))
            {
                ShowNotification(ERROR_MESSAGE, "Please input transfer ammount");
                return false;
            }

            if (string.IsNullOrEmpty(PublicKeyInput.text))
            {
                ShowNotification(ERROR_MESSAGE, "Please enter receiver public key");
                return false;
            }

            if (m_TransferTokenAccount == null)
            {
                if (long.Parse(AmmountInput.text) > (long) (m_OwnedSolAmmount * 1000000000))
                {
                    ShowNotification(ERROR_MESSAGE, "Not enough funds for transaction.");
                    return false;
                }
            }
            else
            {
                if (long.Parse(AmmountInput.text) > long.Parse(OwnedAmmountText.text))
                {
                    ShowNotification(ERROR_MESSAGE, "Not enough funds for transaction.");
                    return false;
                }
            }

            //ShowNotification(SUCCESS_MESSAGE, "Operation is completed successfully.");

            return true;
        }

        private async void TransferToken()
        {
            RequestResult<string> result = await SimpleWallet.Instance.TransferToken(
                m_TransferTokenAccount.pubkey,
                PublicKeyInput.text,
                SimpleWallet.Instance.wallet.GetAccount(0),
                m_TransferTokenAccount.Account.Data.Parsed.Info.Mint,
                long.Parse(AmmountInput.text));

            HandleResponse(result);
        }

        private void HandleResponse(RequestResult<string> result)
        {
            if (result.Result == null)
            {
                ShowNotification(SUCCESS_MESSAGE, result.Reason);
            }
            else
            {
                
            }
        }

        public async override void ShowView(object data = null)
        {
            base.ShowView();

            ResetInputFileds();
            await PopulateInfoFileds(data);
            gameObject.SetActive(true);
        }

        private async System.Threading.Tasks.Task PopulateInfoFileds(object data)
        {
            ParentNftImage.gameObject.SetActive(false);
            NftTitleText.gameObject.SetActive(false);
            OwnedAmmountText.gameObject.SetActive(false);
            if (data != null && data.GetType().Equals(typeof(TokenAccount)))
            {
                this.m_TransferTokenAccount = (TokenAccount) data;
                OwnedAmmountText.text = $"{m_TransferTokenAccount.Account.Data.Parsed.Info.TokenAmount.Amount}";
            }
            else if (data != null && data.GetType().Equals(typeof(Nft)))
            {
                NftTitleText.gameObject.SetActive(true);
                NftImage.gameObject.SetActive(true);
                Nft nft = (Nft) data;
                NftTitleText.text = $"{nft.metaplexData.data.name}";
                ParentNftImage.gameObject.SetActive(true);
                NftImage.texture = nft.metaplexData.nftImage.file;
            }
            else
            {
                m_OwnedSolAmmount =
                    await SimpleWallet.Instance.GetSolAmmount(SimpleWallet.Instance.wallet.GetAccount(0));
                OwnedAmmountText.text = $"{m_OwnedSolAmmount}";
            }
        }

        private void ResetInputFileds()
        {
            AmmountInput.text = "";
            PublicKeyInput.text = "";
        }

        public override void HideView()
        {
            base.HideView();
            this.m_TransferTokenAccount = null;
            gameObject.SetActive(false);
        }
    }
}