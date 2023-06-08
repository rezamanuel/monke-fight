using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;
using Monke.Networking;


namespace Monke.GameState 
{
    /// <summary>
    /// FightState means the Players are currently actively fighting!
    /// </summary>
    [RequireComponent(typeof(NetcodeHooks))]
    public class FightState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.Match; } }

        [SerializeField] NetcodeHooks m_NetcodeHooks;
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
             if (!MonkeNetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }
            foreach(var p in Networking.MonkeNetworkManager.Singleton.ConnectedClientsList){
                Debug.Log(p.ClientId);
            }
        }
        void OnNetworkDespawn()
        {

        }
    }
}
