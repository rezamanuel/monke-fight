using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;
using Monke.Cards;
using Monke.UI;
using Monke.Gameplay;
using Unity.Netcode;
using System.Linq;
using UnityEngine.SceneManagement;
using Monke.Gameplay.ClientPlayer;
using Monke.Networking;

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
        public static ClientMatchState Instance {get; private set;}
        public ulong m_ClientInControl { get; private set; } //set by NetworkMatchLogic

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
        override protected void OnDestroy()
        {
            m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkDespawn;
        }

        
        public void SetClientInControl(ulong clientId){
            m_ClientInControl = clientId;
        }
        public void DisplayCards(List<GameObject> cards){
            m_CardPanel.SetDisplayedCards(cards);
        }
        public void ClientCardSelected(CardID chosenCardID){
            m_MatchLogic.SelectCardServerRpc(chosenCardID);
        }
        //executing twice on host?
        public void ClearCards(){
            m_CardPanel.ClearDisplayedCards();
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
