using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZBoom.Solana
{
    public interface IWalletView 
    {
        public WalletViewManager ViewManager { get; set; }
        public void ShowView(object data = null);
        public void HideView();    
    }

}
