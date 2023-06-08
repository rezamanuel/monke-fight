using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;
using Monke.Networking;


namespace Monke.GameState 
{
    /// <summary>
    /// Match State reflects a Connected State where Players choose cards for their Characters.
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
