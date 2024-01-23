using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thirdweb;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using System;

namespace Hoco.Samples.Runtime
{

    public class PlayerManager : MonoBehaviour
    {
        public bool IsLoggedIn { get; private set; } = false;
        private LivePlayerData m_localPlayerData = new LivePlayerData();
        //Just a simple implementation of a singleton
        public static PlayerManager Instance { get; private set; }

        [SerializeField]
        private Prefab_ThirdwebConnect walletController = null;
        private void Awake()
        {
            if(walletController == null)
            {
                walletController = FindObjectOfType<Prefab_ThirdwebConnect>();
                Debug.LogWarning("WalletController is null, please assign it in the inspector.");
            }

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Debug.LogWarning("More than 1 PlayerManager instance was found, removing this one.");
                Destroy(this.gameObject);
                return;
            }
            //Listen for the wallet controller to connect
            walletController.onConnected.AddListener(OnConnected);
        }

        private void OnDestroy()
        {
            walletController.onConnected.RemoveListener(OnConnected);
        }

        private void OnConnected(string connectedAddress)
        {
            //User successfully connected their wallet and we have their address, we store it in our local player data
            m_localPlayerData.PlayerData.PlayerAddress = connectedAddress;
            ConnectToCloud().Forget();
        }

        private async UniTaskVoid ConnectToCloud()
        {
            var livePlayerData = await LivePlayerData.GetByAddress(m_localPlayerData.PlayerData.PlayerAddress);
            Debug.Log(string.Format("Returning Player: {0}\n{1}", !string.IsNullOrEmpty(livePlayerData.Id)?"YES":"NO", JsonConvert.SerializeObject(livePlayerData, Formatting.Indented)));
            if(string.IsNullOrEmpty(livePlayerData.Id))//New Player
            {
                //PlayerData does not exist in the DB, we create it
                bool success = await LivePlayerData.Create(m_localPlayerData.PlayerData);
                Debug.Log(string.Format("Creating PlayerData {0} for {1}", success, m_localPlayerData.PlayerData.PlayerAddress));
                livePlayerData = await LivePlayerData.GetByAddress(m_localPlayerData.PlayerData.PlayerAddress);
            }
            m_localPlayerData = livePlayerData;
            Debug.Log(string.Format("Got PlayerData for {0}\n{1}", m_localPlayerData.PlayerData.PlayerAddress, JsonConvert.SerializeObject(m_localPlayerData, Formatting.Indented)));
            IsLoggedIn = true;
        }
    }

}