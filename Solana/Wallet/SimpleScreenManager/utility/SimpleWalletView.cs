using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZBoom.Solana
{
    public class SimpleWalletView : MonoBehaviour, IWalletView
    {
        public const string ERROR_MESSAGE = "Error";
        public const string SUCCESS_MESSAGE = "Success";
    
        public WalletViewManager ViewManager { get; set; }

        public virtual void HideView()
        {
            gameObject.SetActive(false);
        }

        public virtual void ShowView(object data = null)
        {
            gameObject.SetActive(true);
        }

        public virtual void ShowNotification(string title, string description)
        {
            ViewManager.ShowNotification(title, description);
        }
    }
}
