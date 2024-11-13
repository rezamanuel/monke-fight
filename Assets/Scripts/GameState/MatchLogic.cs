
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
        [SerializeField] Monke.Infrastructure.GameConfig m_GameConfig;

        protected void Awake()
        {
            
            if(NetworkManager.Singleton.IsServer){
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }
        override public void OnDestroy()
        {
            if(NetworkManager.Singleton.IsServer){
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
        override public void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }
            m_ClientTurnQueue = new List<NetworkClient>();
            
        }
       
        void OnClientConnected(ulong clientId){
            // add client to turn queue
            if(!gameStarted){
                 foreach(var client in NetworkManager.Singleton.ConnectedClients.Values){
                        if(!m_ClientTurnQueue.Contains(client)){
                            m_ClientTurnQueue.Add(client);
                            Debug.Log("Client added to queue: " + client.ClientId);
                            // if NetworkManager has at least 2 players connected, load CardSelect.
                            if(m_ClientTurnQueue.Count > 1){
                                if(m_GameConfig.skipCardSelect){
                                    SceneLoaderWrapper.Instance.LoadScene("Fight", true, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                                }
                                else{
                                    SceneLoaderWrapper.Instance.LoadScene("CardSelect", true, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                                }
                            }
                        }
                    }
            }
        }

        override public void OnNetworkDespawn()
        {
             if (!NetworkManager.Singleton.IsServer)
            {
                return;
            } 
           
        }
    }

}