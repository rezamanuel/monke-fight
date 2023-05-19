using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;


namespace Monke.GameState 
{
    /// <summary>
    /// This State initializes in the Main Menu Scene, and is used to display lobby information
    /// 
    /// TODO: Populate Lobby, Join button, Hook into Connection Managers.
    /// </summary>
    [RequireComponent(typeof(NetcodeHooks))]
    public class MatchState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.Match; } }

        NetcodeHooks m_NetcodeHooks;
        [SerializeField]
        [Tooltip("A collection of locations for spawning players")]
        private Transform[] m_PlayerSpawnPoints;

        protected override void Awake()
        {
            base.Awake();
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkDespawn;
        }
        void OnNetworkSpawn()
        {
            foreach(var p in Networking.MonkeNetworkManager.Singleton.ConnectedClientsList){
                Debug.Log(p.ClientId);
            }
        }
        void OnNetworkDespawn()
        {

        }
    }
}
