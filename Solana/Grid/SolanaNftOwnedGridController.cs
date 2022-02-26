using System.Collections.Generic;
using AllArt.Solana.Nft;
using Rarible;
using Solnet.Rpc.Models;
using SpatialMap_SparseSpatialMap;
using UnityEngine;
using UnityEngine.UI;
using ZBoom.Solana;

namespace ZBoom.Common.SpatialMap
{
    public class SolanaNftOwnedGridController : MonoBehaviour
    {
        public SolanaNftPropItemController PropItemPrefab;
        public SpatialMapGameObjectController SpatialMapGameObjectController;

        private List<SolanaNftPropItemController> m_PropItemControllers = new List<SolanaNftPropItemController>();
        private int m_SelectedPosition = -1;

        private void Start()
        {
            //GetNfts();
        }

        public async void GetNfts()
        {
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

                        SolanaNftPropItemController newNftToken = Instantiate(PropItemPrefab, transform);
                        newNftToken.SolanaNft = nft;
                        newNftToken.SetData(nft);
                        m_PropItemControllers.Add(newNftToken);
                    }
                }
            }
        }

        public void Refresh()
        {
            foreach (var propItemController in m_PropItemControllers)
            {
                Destroy(propItemController.gameObject);
            }
            m_PropItemControllers.Clear();
            GetNfts();
        }

        public async void GetNft(string nftId)
        {
            Nft nft = await Nft.TryGetNftData(nftId,
                SimpleWallet.Instance.ActiveRpcClient, true, true);

            SolanaNftPropItemController newNftToken = Instantiate(PropItemPrefab, transform);
            newNftToken.SolanaNft = nft;
            newNftToken.SetData(nft);
            m_PropItemControllers.Add(newNftToken);
        }

        public void Select(SolanaNftPropItemController propItemController)
        {
            SpatialMapGameObjectController.SelectSolanaNftTemplate(propItemController.SolanaNft);
        }

        public void SelectSolanaNft(Nft solanaNft)
        {
            SpatialMapGameObjectController.SelectSolanaNftTemplate(solanaNft);
        }

        public void Deselect()
        {
            if (m_SelectedPosition != -1)
            {
                m_PropItemControllers[m_SelectedPosition].Deselect();
            }

            m_SelectedPosition = -1;
        }

        private void Update()
        {
        }
    }
}