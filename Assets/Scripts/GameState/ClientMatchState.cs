using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;
using Monke.Cards;
using Monke.UI;
using Monke.Gameplay;
using Unity.Netcode;

namespace Monke.GameState
{
    /// <summary>
    /// Match State reflects a Connected State where Players choose cards for their Characters.
    /// Each Player will choose a card from a pool
    /// </summary>
    [RequireComponent(typeof(NetcodeHooks), typeof(NetworkMatchLogic))]
    public class ClientMatchState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.Match; } }
        /// <summary>
        /// Instance variable so UI can access the GameState obj
        /// </summary>
        /// <value></value>
        public ClientMatchState Instance {get; private set;}

        [SerializeField] NetcodeHooks m_NetcodeHooks;
        [SerializeField] NetworkMatchLogic m_MatchLogic;
        
        [SerializeField] CardPanel m_CardPanel; // needs to be set in inspector
        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkDespawn;
        }
        [ClientRpc] public void DisplayCardsClientRpc(List<CardID> cardIDs){
            foreach(CardID c_id in cardIDs){
                GameObject card_prefab = GameDataSource.Instance.GetCardPrototypeByID(c_id).m_UICardPrefab;
                GameObject card_go = Instantiate(card_prefab) as GameObject;
            }
        }
        public void OnDrawCards(){
            GameObject[] all_cards = GameObject.FindGameObjectsWithTag("Card");
            List<GameObject> spawned_cards = new List<GameObject>();
            foreach(GameObject c in all_cards){
                spawned_cards.Add(c);
            }
            m_CardPanel.SetDisplayedCards(spawned_cards);

        }
        
        void OnNetworkSpawn()
        {

            if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
                return;
            }

        }
        void OnNetworkDespawn()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
            }
        }
    }
}
