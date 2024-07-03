using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;
using Monke.Networking;
using Monke.Gameplay.Character;
using Unity.Netcode;
using Monke.Infrastructure;
using Monke.Gameplay.ClientPlayer;
using UnityEngine.iOS;
using Monke.UI;
using System;
namespace Monke.GameState 
{
    /// <summary>
    /// FightState means the Players are currently actively fighting!
    /// </summary>
    [RequireComponent(typeof(NetcodeHooks))]
    public class FightState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.Match; } }

        [SerializeField] NetcodeHooks m_NetcodeHooks;
        [SerializeField]
        [Tooltip("A collection of locations for spawning players")]
        private Transform[] m_PlayerSpawnPoints;
        
        // has the first player spawned?
        // need to figure out how to make this consistent even when the first client to connect is not the server.
   
        protected override void Awake()
        {
            base.Awake();
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkDespawn;
            SceneLoaderWrapper.Instance.OnClientSynchronized += OnClientSynchronized;
            if(NetworkManager.Singleton.IsServer){
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            }
        }
        protected override void OnDestroy()
        {
            m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkDespawn;
            SceneLoaderWrapper.Instance.OnClientSynchronized -= OnClientSynchronized;
            if(NetworkManager.Singleton.IsServer){
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
        void OnClientConnected(ulong clientId){
            var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            player.GetComponent<ServerCharacter>().InitializeCharacter();
        }


        void OnClientSynchronized()
        {
            // tell all player network objects to initialize client character
            
             if(NetworkManager.Singleton.IsClient)
            {
                Debug.Log( "Client is synchronized and fighting, enabling input!");
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClientPlayerInput>().SetEnabled(true);
                if (NetworkManager.Singleton.IsServer)
                {
                    // has the first player spawned?
                    // need to figure out how to make this consistent even when the first client to connect is not the server.
                    NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = m_PlayerSpawnPoints[0].position;
                }
                else
                {
                    NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = m_PlayerSpawnPoints[1].position;
                }
            }
        }

        
        void OnNetworkSpawn()
        {
        }

        void OnNetworkDespawn()
        {
          
        }
    }
}
