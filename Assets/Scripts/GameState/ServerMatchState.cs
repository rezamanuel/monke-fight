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
    public class ServerMatchState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.Match; } }

        [SerializeField] NetcodeHooks m_NetcodeHooks;

        [SerializeField] CardPanel m_CardPanel;

        [SerializeField] List<NetworkClient> m_ClientTurnQueue;

        /// <summary>
        /// Draws Cards into Character Card Inventory, Spawns UI for them thru NetworkManager.
        /// </summary>
        /// <param name="serverCharacter"></param>
        void StartPlayerTurn(NetworkClient client){
            //Enable mouse for player in charge
            Debug.Log(client.ClientId + " Turn started");
        }

        /// <summary>
        /// Cleans up Character Card Inventory, Despawns/Destroys UI for discarded cards thru NetworkManager.
        /// </summary>
        /// <param name="serverCharacter"></param>
        void EndPlayerTurn(NetworkClient client){
            //Disable Mouse for player in charge
        }

        void CheckForPendingTurns(){
            // if there's more characters left in the turn queue, start next one.

            //else, Progress to FightState.
        }

        void CheckForPlayersConnected(){
            // if MonkeNetworkManager has at least 2 players connected, start turns
            if(m_ClientTurnQueue.Count >1){
                NetworkClient nextplayer = m_ClientTurnQueue[0];
                StartPlayerTurn(nextplayer);
                m_ClientTurnQueue.Remove(nextplayer);
            }
        }

        [ServerRpc(RequireOwnership = false)] void UpdateTurnQueueRpc(ServerRpcParams serverRpcParams)
        {

        }
        void OnClientConnectedCallback(ulong clientId){
            //Update Serverside list of CharacterTurnQueue
            Debug.Log("Client Connected");
            m_ClientTurnQueue.Add(MonkeNetworkManager.Singleton.ConnectedClients[clientId]);
            
            CheckForPlayersConnected();
        }
        void OnClientDisconnectedCallback(ulong clientId){
            //Update Serverside list of CharacterTurnQueue
            m_ClientTurnQueue.Remove(MonkeNetworkManager.Singleton.ConnectedClients[clientId]);
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
             if (!MonkeNetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            } 
                MonkeNetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
                MonkeNetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
        }
        void OnNetworkDespawn()
        {
             if (!MonkeNetworkManager.Singleton.IsServer)
            {
                return;
            } 
            MonkeNetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            MonkeNetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;

        }
    }
}
