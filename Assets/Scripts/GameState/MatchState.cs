using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;
using Monke.Networking;


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

        [SerializeField] NetcodeHooks m_NetcodeHooks;

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
        }
        void OnNetworkDespawn()
        {

        }
    }
}
