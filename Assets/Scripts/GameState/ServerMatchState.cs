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
        

        protected override void Awake()
        {
            base.Awake();
            networkMatchLogic = GetComponent<NetworkMatchLogic>();
            m_ClientTurnQueue = new List<NetworkClient>();
            networkMatchLogic.OnClientConnected += OnClientConnected;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkDespawn;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            networkMatchLogic.OnClientConnected -= OnClientConnected;
            if (m_NetcodeHooks)
            {
                m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
                m_NetcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            }
        }
         /// <summary>
        /// Draws Cards into Character Card Inventory, Spawns UI for them thru NetworkManager.
        /// </summary>
        /// <param name="serverCharacter"></param>
        public void StartPlayerTurn(NetworkClient client){
            //Enable mouse for player in charge
            Debug.Log("Player " + client.ClientId + " Turn started");
            ServerCharacter server_character = client.PlayerObject.GetComponentInChildren<ServerCharacter>();
            server_character.m_CharacterCardInventory.DrawCards(5);
            List<GameObject> spawned_cards = new List<GameObject>();
            foreach(CardID c_id in server_character.m_CharacterCardInventory.m_DrawnCards){
                GameObject card_prefab = GameDataSource.Instance.GetCardPrototypeByID(c_id).m_UICardPrefab;
                GameObject card_go = Instantiate(card_prefab) as GameObject;
                card_go.GetComponent<NetworkObject>().SpawnWithOwnership(client.ClientId, true);
                spawned_cards.Add(card_go);
            }
            clientMatchState.OnDrawCards(spawned_cards);


        }

        /// <summary>
        /// Cleans up Character Card Inventory, Despawns/Destroys UI for discarded cards thru NetworkManager.
        /// </summary>
        /// <param name="serverCharacter"></param>
        public void EndPlayerTurn(NetworkClient client){
            //Disable Mouse for player in charge
        }

        public void CheckForPendingTurns(){
            // if there's more characters left in the turn queue, start next one.

            //else, Progress to FightState.
        }

        public void OnClientConnected(ulong clientId){
            // add client to turn queue
            if(!queueStarted){
                m_ClientTurnQueue.Add(MonkeNetworkManager.Singleton.ConnectedClients[clientId]);
                // if MonkeNetworkManager has at least 2 players connected, start turns
                if(m_ClientTurnQueue.Count >1){
                    queueStarted = true;
                    NetworkClient nextplayer = m_ClientTurnQueue[0];
                    StartPlayerTurn(nextplayer);
                    m_ClientTurnQueue.Remove(nextplayer);
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
