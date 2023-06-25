using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;
using Monke.Networking;
using Monke.UI;
using Monke.Gameplay.Character;
using Unity.Netcode;

namespace Monke.GameState 
{
    /// <summary>
    /// Match State reflects a Connected State where Players choose cards for their Characters.
    /// Each Player will choose a card from a pool
    /// </summary>
    [RequireComponent(typeof(NetcodeHooks))]
    public class ClientMatchState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.Match; } }

        [SerializeField] NetcodeHooks m_NetcodeHooks;

        [SerializeField] CardPanel m_CardPanel;


        void OnClientConnectedCallback(ulong clientId)
        {
            //Update Serverside list of CharacterTurnQueue
            Debug.Log("Client Connected");
        }
        [ServerRpc(RequireOwnership = false)] void UpdateTurnQueueRpc(ServerRpcParams serverRpcParams)
        {

        }

    void OnClientDisconnectedCallback(ulong clientId){
            //Update Serverside list of CharacterTurnQueue
            Debug.LogError("Player Disconnected during MatchState");
        }
        protected override void Awake()
        {
            base.Awake();
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkDespawn;
        }
        void OnNetworkSpawn()
        {
           
             if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
                return;
            }
             Debug.Log("OnClientConnected Registered");
            MonkeNetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            MonkeNetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
               
        }
        void OnNetworkDespawn()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
            }
            MonkeNetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            MonkeNetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;

        }
    }
}
