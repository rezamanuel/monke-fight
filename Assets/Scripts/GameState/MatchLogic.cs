
using Monke.Networking;
using UnityEngine;
using Monke.Utilities;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using Monke.Gameplay.ClientPlayer;

namespace Monke.GameState
{
    /// <summary>
    /// Logic for Match Scene (Server Side)
    /// </summary>
    public class MatchLogic : NetworkBehaviour
    {
        List<NetworkClient> m_ClientTurnQueue;
        public bool gameStarted { private set; get; } = false;

        protected void Awake()
        {
            Debug.Log("MatchLogic Awake");
            
            if(MonkeNetworkManager.Singleton.IsServer){
                MonkeNetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }
        override public void OnDestroy()
        {
            if(MonkeNetworkManager.Singleton.IsServer){
                MonkeNetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
        override public void OnNetworkSpawn()
        {
            Debug.Log("MatchLogic OnNetworkSpawn1");
            if (!MonkeNetworkManager.Singleton.IsServer)
            {
                return;
            }
            m_ClientTurnQueue = new List<NetworkClient>();
            
            Debug.Log("MatchLogic OnNetworkSpawn");
        }
       
        void OnClientConnected(ulong clientId){
            // add client to turn queue
            if(!gameStarted){
                 foreach(var client in MonkeNetworkManager.Singleton.ConnectedClients.Values){
                        if(!m_ClientTurnQueue.Contains(client)){
                            m_ClientTurnQueue.Add(client);
                            Debug.Log("Client added to queue: " + client.ClientId);
                            // if MonkeNetworkManager has at least 2 players connected, load CardSelect.
                            if(m_ClientTurnQueue.Count > 1){
                                SceneLoaderWrapper.Instance.LoadScene("CardSelect", true, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                            }
                        }
                    }
            }
        }

        override public void OnNetworkDespawn()
        {
             if (!MonkeNetworkManager.Singleton.IsServer)
            {
                return;
            } 
           
        }
    }

}