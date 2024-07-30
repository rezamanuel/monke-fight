using System.Collections;
using UnityEngine;
using Monke.Networking;
using Unity.Netcode;
using System;
using System.Collections.Generic;

namespace Monke
{
    /// <summary>
    /// Match Manager is a singleton used to decide when a game ends. Server Side only
    /// </summary>
    public class PlayerLivesManager : NetworkBehaviour
    {
        public static PlayerLivesManager Instance;
        public event Action OnMatchEnd;
        public event Action OnRoundEnd;
        private ulong WinnerClientId;
        private Dictionary<ulong, int> m_ConnectedClientLives = new Dictionary<ulong, int>();

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
        void Start(){
            if (!MonkeNetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }
            m_ConnectedClientLives.Clear();
            foreach (var client in MonkeNetworkManager.Singleton.ConnectedClients)
            {
                m_ConnectedClientLives.Add(client.Key, m_MaxLives);
            }
        }
        public void OnPlayerDeath(ulong clientId)
        {
            Debug.Log("Player " + clientId + " has died!");
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
            Debug.Log("Player " + clientId + " now has " + m_ConnectedClientLives[clientId] + " lives.");
            EndRound();


        }
        /// <summary>
        /// Ends the round. If a player has 0 lives, they lose.
        /// </summary>
        /// <param name="winnerClientId"></param>
        private void EndRound()
        {

            ulong winnerClientId = ulong.MinValue;
            foreach (var client in m_ConnectedClientLives)
            {
                if (winnerClientId == ulong.MinValue)
                {
                    winnerClientId = client.Key;
                }
                else if (client.Value > m_ConnectedClientLives[winnerClientId])
                {
                    winnerClientId = client.Key;
                }
                if (client.Value <= 0)
                {
                    WinnerClientId = winnerClientId;
                    EndMatch();
                }
            }
            Debug.Log("Round has ended!");
            WinnerClientId = winnerClientId;
            OnRoundEnd?.Invoke();
        }
        private void EndMatch()
        {
            Debug.Log("Match has ended! Winner is " + WinnerClientId + "!");
            OnMatchEnd?.Invoke();
        }
        public ulong GetWinner()
        {
            return WinnerClientId;
        }
        public override void OnNetworkSpawn()
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
