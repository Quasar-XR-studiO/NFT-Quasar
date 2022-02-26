using AllArt.Solana;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ZBoom.Solana
{
    public class ReceiveView : SimpleWalletView
    {
        public Button AirdropButton;
        public TextMeshProUGUI WalletPublicKeyText;
        public RawImage QrCodeImage;

        private void Start()
        {
            //AirdropButton.onClick.AddListener(RequestAirdrop());
        }

        private static UnityAction RequestAirdrop()
        {
            return async () =>
            {
                await SimpleWallet.Instance.RequestAirdrop(SimpleWallet.Instance.wallet.GetAccount(0));
            };
        }

        public override void ShowView(object data = null)
        {
            base.ShowView();
            gameObject.SetActive(true);

            CheckAndToggleAirdrop();

            GenerateQR();
            WalletPublicKeyText.text = SimpleWallet.Instance.wallet.GetAccount(0).GetPublicKey;
        }

        private void CheckAndToggleAirdrop()
        {
            if (SimpleWallet.Instance.ClientSource != WalletBaseComponent.EClientUrlSource.EMainnet)
            {
                AirdropButton.gameObject.SetActive(true);
            }
            else
            {
                AirdropButton.gameObject.SetActive(false);
            }
        }

        private void GenerateQR()
        {
            Texture2D tex =
                QRGenerator.GenerateQRTexture(SimpleWallet.Instance.wallet.GetAccount(0).GetPublicKey, 256, 256);
            QrCodeImage.texture = tex;
        }

        public override void HideView()
        {
            base.HideView();
            gameObject.SetActive(false);
        }
    }
}