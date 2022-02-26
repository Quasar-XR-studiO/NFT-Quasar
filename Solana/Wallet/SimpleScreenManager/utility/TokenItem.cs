using AllArt.Solana.Nft;
using Solnet.Rpc.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Solana
{
    public class TokenItem : MonoBehaviour
    {
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI PublicText;
        public TextMeshProUGUI AmmountText;
        public Image PreviewImage;
        public AspectRatioFitter AspectRatioFitter;

        private TokenAccount m_TokenAccount;
        private Nft m_Nft;
        private SimpleWalletView m_ParentView;

        private void Awake()
        {
            
        }

        private void Start()
        {
            
        }

        public void InitializeData(TokenAccount tokenAccount, SimpleWalletView screen, Nft nftData = null)
        {
            m_ParentView = screen;
            m_TokenAccount = tokenAccount;
            if (nftData != null)
            {
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { m_Nft = nftData; });
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { AmmountText.text = ""; });
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { PublicText.text = nftData.metaplexData.data.name; });
                m_Nft = nftData;
                AmmountText.text = "";
                NameText.text = nftData.metaplexData.data.name;
                PublicText.text = nftData.metaplexData.mint;

                if (PreviewImage != null)
                {
                    MainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        //NftImage.texture = nftData.metaplexData.NftImage.file;
                        
                        Texture2D texture = nftData.metaplexData.nftImage.file;
                        Rect rect = new Rect(0, 0, texture.width, texture.height);
                        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
                        PreviewImage.sprite = sprite;
                        
                        if (sprite.texture.width > sprite.texture.height)
                        {
                            AspectRatioFitter.aspectRatio = 10f;
                        }
                        else
                        {
                            AspectRatioFitter.aspectRatio = 0.1f;
                        }
                    });
                    //NftImage.texture = nftData.metaplexData.NftImage.file;
                }
            }
            else
            {
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { AmmountText.text = m_TokenAccount.Account.Data.Parsed.Info.TokenAmount.Amount.ToString(); });
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { NftImage.gameObject.SetActive(false); });
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { PublicText.text = m_TokenAccount.Account.Data.Parsed.Info.Mint; });
                AmmountText.text = tokenAccount.Account.Data.Parsed.Info.TokenAmount.Amount.ToString();

                if (PreviewImage is null)
                {
                    return;
                }

                PreviewImage.gameObject.SetActive(false);
                PublicText.text = tokenAccount.Account.Data.Parsed.Info.Mint;
            }
        }

        public void TransferAccount()
        {
            if (m_Nft != null)
            {
                m_ParentView.ViewManager.ShowTransferView(m_Nft);
                //m_ParentView.ViewManager.ShowScreen(m_ParentView, "transfer_screen", m_Nft);
            }
            else
            {
                m_ParentView.ViewManager.ShowTransferView(m_TokenAccount);
            }
        }
    }
}
