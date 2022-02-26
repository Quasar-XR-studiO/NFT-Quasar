using Solnet.Wallet;
using System.Collections;
using System.Collections.Generic;
using AllArt.Solana;
using UnityEngine;

namespace ZBoom.Solana
{
    public enum StorageMethod { JSON, SimpleTxt }
    public class SimpleWallet : WalletBaseComponent
    {
        public StorageMethod StorageMethod;
        
        public static SimpleWallet Instance;
        public readonly string StorageMethodStateKey = "StorageMethodKey";

        public override void Awake()
        {           
            base.Awake();
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            ChangeState(StorageMethod.ToString());
            if (PlayerPrefs.HasKey(StorageMethodStateKey))
            {
                string storageMethodString = LoadPlayerPrefs(StorageMethodStateKey);

                if (storageMethodString != StorageMethod.ToString())
                {
                    storageMethodString = StorageMethod.ToString();
                    ChangeState(storageMethodString);
                }

                if (storageMethodString == StorageMethod.JSON.ToString())
                    StorageMethodReference = StorageMethod.JSON;
                else if (storageMethodString == StorageMethod.SimpleTxt.ToString())
                    StorageMethodReference = StorageMethod.SimpleTxt;
            }
            else
            {
                StorageMethodReference = StorageMethod.SimpleTxt;
            }
        }

        private void ChangeState(string state)
        {
            SavePlayerPrefs(StorageMethodStateKey, StorageMethod.ToString());
        }

        public StorageMethod StorageMethodReference
        {
            get { return StorageMethod; }
            set { StorageMethod = value; ChangeState(StorageMethod.ToString()); }
        }

    }
}
