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
        
        [SerializeField] CardPanel m_CardPanel;
        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkDespawn;
        }

        public void OnDrawCards(List<GameObject> cardObjectList){
            m_CardPanel.SetDisplayedCards(cardObjectList);

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
