using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;

namespace ZBoom.Solana
{
    public class WalletViewManager : MonoBehaviour
    {
        //public SimpleWalletView[] Views;
        public SimpleWalletView StartWalletView;
        public SimpleWalletView LoginWalletView;
        public SimpleWalletView RestoreWalletView;
        public SimpleWalletView CreateWalletView;
        public SimpleWalletView WalletView;
        public SimpleWalletView TransferWalletView;
        public SimpleWalletView ReceiveWalletView;
        public NotificationManager Notification;

        private Dictionary<string, SimpleWalletView> m_ScreensDict = new Dictionary<string, SimpleWalletView>();
        private SimpleWalletView m_CurrentView = null;
        
        private void Start()
        {
            HideAll();
            
            StartWalletView.ViewManager = this;
            LoginWalletView.ViewManager = this;
            RestoreWalletView.ViewManager = this;
            CreateWalletView.ViewManager = this;
            WalletView.ViewManager = this;
            TransferWalletView.ViewManager = this;
            ReceiveWalletView.ViewManager = this;

            m_CurrentView = StartWalletView;
            ShowStartView();
        }

        private void SetupScreen(SimpleWalletView screen)
        {
            screen.gameObject.SetActive(false);
            m_ScreensDict.Add(screen.gameObject.name, screen);
            screen.ViewManager = this;
        }

        public void ShowScreen(SimpleWalletView curScreen, SimpleWalletView screen)
        {
            curScreen.HideView();
            screen.ShowView();
        }

        public void ShowScreen(SimpleWalletView curScreen, int index)
        {
            curScreen.HideView();
            //Views[index].ShowView();
        }

        public void ShowScreen(SimpleWalletView curScreen, string name, object data = null)
        {
            curScreen.HideView();
            m_ScreensDict[name].ShowView(data);
        }

        private void HideCurrentView()
        {
            LoginWalletView.HideView();
            RestoreWalletView.HideView();
            CreateWalletView.HideView();
            WalletView.HideView();
            TransferWalletView.HideView();
        }
        
        private void HideAll()
        {
            LoginWalletView.HideView();
            RestoreWalletView.HideView();
            CreateWalletView.HideView();
            WalletView.HideView();
            TransferWalletView.HideView();
        }
        
        public void ShowStartView()
        {
            m_CurrentView.HideView();
            m_CurrentView = StartWalletView;
            m_CurrentView.ShowView();
        }

        public void ShowCreateView()
        {
            m_CurrentView.HideView();
            m_CurrentView = CreateWalletView;
            m_CurrentView.ShowView();
        }

        public void ShowRestoreView()
        {
            m_CurrentView.HideView();
            m_CurrentView = RestoreWalletView;
            m_CurrentView.ShowView();
        }

        public void ShowLoginView()
        {
            m_CurrentView.HideView();
            m_CurrentView = LoginWalletView;
            m_CurrentView.ShowView();
        }

        public void ShowWalletView()
        {
            m_CurrentView.HideView();
            m_CurrentView = WalletView;
            m_CurrentView.ShowView();
        }

        public void ShowTransferView()
        {
            m_CurrentView.HideView();
            m_CurrentView = TransferWalletView;
            m_CurrentView.ShowView();
        }
        
        public void ShowTransferView(object data)
        {
            m_CurrentView.HideView();
            m_CurrentView = TransferWalletView;
            m_CurrentView.ShowView(data);
        }
        
        public void ShowReceiveView()
        {
            m_CurrentView.HideView();
            m_CurrentView = ReceiveWalletView;
            m_CurrentView.ShowView();
        }

        public void ShowNotification(string title, string description)
        {
            if (Notification != null)
            {
                Notification.title = title;
                Notification.description = description;

                Notification.UpdateUI();
                Notification.OpenNotification();
                Notification.UpdateUI();
            }
        }
    }
}