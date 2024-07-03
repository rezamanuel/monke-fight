using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;
using Monke.Networking;
using Monke.Cards;
using Monke.Gameplay.Character;
using Unity.Netcode;

namespace Monke.GameState
{
    /// <summary>
    /// Match State reflects a Connected State where Players choose cards for their Characters.
    /// Each Player will choose a card from a pool
    /// </summary>
    [RequireComponent(typeof(NetcodeHooks), typeof(CardSelectLogic))]
    public class ServerCardSelectState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.Match; } }
        public bool queueStarted { private set; get; } = false;
        [SerializeField] ClientCardSelectState clientMatchState;
        [SerializeField] NetcodeHooks m_NetcodeHooks;
        [SerializeField] CardSelectLogic cardSelectLogic;
        List<NetworkClient> m_ClientTurnQueue;
        [SerializeField] NetworkObject MatchUI;
        


        protected override void Awake()
        {
            base.Awake();
            cardSelectLogic = GetComponent<CardSelectLogic>();
            m_ClientTurnQueue = new List<NetworkClient>();
            clientMatchState = GetComponent<ClientCardSelectState>();
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkDespawn;

            if (MonkeNetworkManager.Singleton.IsServer)
            {
                SceneLoaderWrapper.Instance.OnClientSynchronized += OnClientSynchronized;
                cardSelectLogic.OnCardSelected += OnCardSelected;
            }
            SceneLoaderWrapper.Instance.OnClientSynchronized += OnClientSynchronized;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_NetcodeHooks)
            {
                m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
                m_NetcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            }
                if (MonkeNetworkManager.Singleton.IsServer)
            {
                SceneLoaderWrapper.Instance.OnClientSynchronized -= OnClientSynchronized;
                cardSelectLogic.OnCardSelected -= OnCardSelected;
            }
        }

        void OnClientSynchronized(){
            if(MonkeNetworkManager.Singleton.IsServer){
                if(!queueStarted){
                    queueStarted = true;
                    Debug.Log("Queue Started");
                    if(m_ClientTurnQueue.Count < MonkeNetworkManager.Singleton.ConnectedClients.Count){
                    foreach(var client in MonkeNetworkManager.Singleton.ConnectedClients.Values){
                        if(!m_ClientTurnQueue.Contains(client)){
                            m_ClientTurnQueue.Add(client);
                            Debug.Log("Client added to queue: " + client.ClientId);
                            if(m_ClientTurnQueue.Count > 1){
                                StartPlayerTurn(m_ClientTurnQueue[0]);
                            }
                        }
                    }
                }
                }
                else{
                    Debug.Log("Queue already started");
                }
                
            } 
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
            cardSelectLogic.DisplayCardsClientRpc(server_character.m_CharacterCardInventory.m_DrawnCards.ToArray());
            cardSelectLogic.SetControlClientRpc(client.ClientId);
            
        }

        /// <summary>
        /// Prompts end of turn for current player in the Queue.
        /// </summary>
        /// <param name="chosenCardIndex"></param>
        void OnCardSelected(CardID chosenCardID){
            var current_player = m_ClientTurnQueue[0];
            ServerCharacter server_character = current_player.PlayerObject.GetComponentInChildren<ServerCharacter>();
            server_character.m_CharacterCardInventory.PlayCard(chosenCardID);
            cardSelectLogic.SetControlClientRpc(ulong.MaxValue); //removes control of player who just selected card
            StartCoroutine(EndPlayerTurn(current_player));
        }
        /// <summary>
        /// Cleans up Character Card Inventory
        /// </summary>
        IEnumerator EndPlayerTurn(NetworkClient client){
            yield return new WaitForSeconds(2);
            ServerCharacter server_character = client.PlayerObject.GetComponentInChildren<ServerCharacter>();
            server_character.m_CharacterCardInventory.ClearDrawnCards();
            cardSelectLogic.ClearCardsClientRpc();
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
