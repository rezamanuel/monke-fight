using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;
using Monke.Networking;
using Monke.Cards;
using Monke.Gameplay.Character;
using Unity.Netcode;
using HeathenEngineering.Events;

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
        [SerializeField] NetcodeHooks m_NetcodeHooks;
        [SerializeField] CardSelectLogic cardSelectLogic;
        List<NetworkClient> m_ClientTurnQueue;
        


        protected override void Awake()
        {
            
            base.Awake();
            cardSelectLogic = GetComponent<CardSelectLogic>();
            m_ClientTurnQueue = new List<NetworkClient>();
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;

            
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_NetcodeHooks)
            {
                m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
                m_NetcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            }
                
        }
        /// <summary>
        /// temporary fix for the server not being able to hook into the scene event
        /// </summary>
        /// <param name="sceneEvent"></param>
        IEnumerator waitForSceneEvent(){
            yield return new WaitForSeconds(1);
            cardSelectLogic.OnCardSelected += OnCardSelected;
            OnLoadEventCompleted();
        }
        
        // note: this doesn't work, because the server needs to spawn the Server Card Select State as it is loading the scene,
        // so when we hook into the spawn event, it's already too late, and OnLoadEventCompleted is never called.

        // I might be able to fix this by creating a guaranteed delivery event that is sent by the server to the clients in the MatchLogic script.
        // this is basically a queue of events that are sent to the server as it loads the scene, and then the server will execute them once everyone is connected.
        // this way, the server can send a guaranteed delivery event to this script so we can guarantee code running inside of onSceneEvent.
        void onSceneEvent(SceneEvent sceneEvent){
            if(sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted){
                OnLoadEventCompleted();
            }
        }
        void OnLoadEventCompleted(){
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
            Debug.Log("Card Selected");
            var current_player = m_ClientTurnQueue[0];
            ServerCharacter server_character = current_player.PlayerObject.GetComponentInChildren<ServerCharacter>();
            server_character.m_CharacterCardInventory.PlayCard(chosenCardID);
            cardSelectLogic.SetControlClientRpc(ulong.MaxValue); //removes control of player who just selected card
            StartCoroutine(EndPlayerTurn(current_player));
            Debug.Log("Card Selected: " + chosenCardID.ToString() + " by Player " + current_player.ClientId);
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
            if(MonkeNetworkManager.Singleton.IsServer){
                StartCoroutine(waitForSceneEvent());
            }
            // TODO: guarantee delivery event for onSceneEvent
            // if (MonkeNetworkManager.Singleton.IsServer)
            // {
            //     MonkeNetworkManager.Singleton.SceneManager.OnSceneEvent += onSceneEvent;
            //     cardSelectLogic.OnCardSelected += OnCardSelected;
            // }
            
        }
        void OnNetworkDespawn()
        {
            // if (MonkeNetworkManager.Singleton.IsServer)
            // {
            //    MonkeNetworkManager.Singleton.SceneManager.OnSceneEvent -= onSceneEvent;
            //     cardSelectLogic.OnCardSelected -= OnCardSelected;
            // }
            if(MonkeNetworkManager.Singleton.IsServer){
                cardSelectLogic.OnCardSelected -= OnCardSelected;
            }
        }
    }
}
