using System.Collections;
using UnityEngine;
using Monke.Networking;
using Unity.Netcode;
using System;
using System.Collections.Generic;

namespace Monke
{
    /// <summary>
    /// Match Manager is a singleton used to decide when a game ends.
    /// </summary>
    public class PlayerLivesManager : NetworkBehaviour
    {
        public static PlayerLivesManager Instance;
        public event Action OnMatchEnd;
        public event Action OnRoundEnd;

        private Dictionary<ulong,int> m_ConnectedClientLives = new Dictionary<ulong, int>();

        static int m_MaxLives = 5;
        
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        public void OnPlayerDeath(ulong clientId)
        {
            AddPlayerLives(clientId, -1);
        }
        public void AddPlayerLives(ulong clientId, int amount)
        {
            if (m_ConnectedClientLives.ContainsKey(clientId))
            {
                m_ConnectedClientLives[clientId] += amount;
            }
            else
            {
                m_ConnectedClientLives.Add(clientId, m_MaxLives + amount);
            }
            EndRound();
            foreach (var client in m_ConnectedClientLives)
            {
                if (client.Value <= 0)
                {
                    EndMatch();
                }
            }

        }
        public void EndRound()
        {
            OnRoundEnd?.Invoke();
        }
        public void EndMatch()
        {
            OnMatchEnd?.Invoke();
        }
        public override void  OnNetworkSpawn()
        {
            if (!MonkeNetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }
            
            
        }
        public override void OnNetworkDespawn()
        {
            if (!MonkeNetworkManager.Singleton.IsServer)
            {
                return;
            }
        }

       
    }
}
