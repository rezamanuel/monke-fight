using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;
using Monke.Networking;
using Monke.Cards;
using Monke.Gameplay.Character;
using Monke.Gameplay;
using Unity.Netcode;
using VContainer;

namespace Monke.GameState
{
    /// <summary>
    /// Match State reflects a Connected State where Players choose cards for their Characters.
    /// Each Player will choose a card from a pool
    /// </summary>
    [RequireComponent(typeof(NetcodeHooks), typeof(NetworkMatchLogic))]
    public class ServerMatchState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.Match; } }
        public bool queueStarted { private set; get; } = false;
        [SerializeField] ClientMatchState clientMatchState;
        [SerializeField] NetcodeHooks m_NetcodeHooks;
        [SerializeField] NetworkMatchLogic networkMatchLogic;
        List<NetworkClient> m_ClientTurnQueue;
        [SerializeField] NetworkObject MatchUI;


        protected override void Awake()
        {
            base.Awake();
            networkMatchLogic = GetComponent<NetworkMatchLogic>();
            m_ClientTurnQueue = new List<NetworkClient>();
            clientMatchState = GetComponent<ClientMatchState>();
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkDespawn;

            if (MonkeNetworkManager.Singleton.IsServer)
            {
                networkMatchLogic.OnClientConnected += OnClientConnected;
                networkMatchLogic.OnCardSelected += OnCardSelected;
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_NetcodeHooks)
            {
                m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
                m_NetcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            }
                networkMatchLogic.OnClientConnected -= OnClientConnected;
                networkMatchLogic.OnCardSelected -= OnCardSelected;
            
           
            
        }
         /// <summary>
        /// Draws Cards into Character Card Inventory, Spawns UI for them thru NetworkManager.
        /// </summary>
        void StartPlayerTurn(NetworkClient client){
            //Enable mouse for player in charge
            Debug.Log("Player " + client.ClientId + " Turn started");
            ServerCharacter server_character = client.PlayerObject.GetComponentInChildren<ServerCharacter>();
            server_character.m_CharacterCardInventory.DrawCards(5);
            Debug.Log("Cards Drawn: " + server_character.m_CharacterCardInventory.m_DrawnCards.Count);
            networkMatchLogic.DisplayCardsClientRpc(server_character.m_CharacterCardInventory.m_DrawnCards.ToArray());
            networkMatchLogic.SetControlClientRpc(client.ClientId);
            
        }

        /// <summary>
        /// Prompts end of turn for current player in the Queue.
        /// </summary>
        /// <param name="chosenCardIndex"></param>
        void OnCardSelected(CardID chosenCardID){
            var current_player = m_ClientTurnQueue[0];
            ServerCharacter server_character = current_player.PlayerObject.GetComponentInChildren<ServerCharacter>();
            server_character.m_CharacterCardInventory.PlayCard(chosenCardID);
            networkMatchLogic.SetControlClientRpc(ulong.MaxValue); //removes control of player who just selected card
            StartCoroutine(EndPlayerTurn(current_player));
        }
        /// <summary>
        /// Cleans up Character Card Inventory
        /// </summary>
        IEnumerator EndPlayerTurn(NetworkClient client){
            yield return new WaitForSeconds(2);
            ServerCharacter server_character = client.PlayerObject.GetComponentInChildren<ServerCharacter>();
            server_character.m_CharacterCardInventory.ClearDrawnCards();
            networkMatchLogic.ClearCardsClientRpc();
            m_ClientTurnQueue.Remove(client);
            CheckForPendingTurns();

        }

        void CheckForPendingTurns(){
            // if there's more  characters left in the turn queue, start next one.
            Debug.Log("checking for pending turns, queue count: " + m_ClientTurnQueue.Count);
            if(m_ClientTurnQueue.Count > 0){
                StartPlayerTurn(m_ClientTurnQueue[0]);
            }
            else{
                Debug.Log("FIGHT!!!");
                SceneLoaderWrapper.Instance.QueueNextScene("Fight");
                SceneLoaderWrapper.Instance.UnloadScene();
                
            }
        }

        void OnClientConnected(ulong clientId){
            // add client to turn queue
            Debug.Log("client connected, queue count: " + m_ClientTurnQueue.Count);
            if(!queueStarted){
                m_ClientTurnQueue.Add(MonkeNetworkManager.Singleton.ConnectedClients[clientId]);
                // if MonkeNetworkManager has at least 2 players connected, start turns
                // Wait for 
                if(m_ClientTurnQueue.Count >1){
                    queueStarted = true;
                    NetworkClient nextplayer = m_ClientTurnQueue[0];
                    StartPlayerTurn(nextplayer);
                }
            }
        }

        void OnNetworkSpawn()
        {
            if (!MonkeNetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }
            
        }
        void OnNetworkDespawn()
        {
            if (!MonkeNetworkManager.Singleton.IsServer)
            {
                return;
            }
        }
    }
}
