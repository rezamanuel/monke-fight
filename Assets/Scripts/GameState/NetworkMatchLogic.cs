
using System.Collections.Generic;
using UnityEngine;
using Monke.Networking;
using Monke.UI;
using Monke.Gameplay.Character;
using Unity.Netcode;
using System;

namespace Monke.GameState 
{
    /// <summary>
    /// RPC logic for MatchState
    /// </summary>
    public class NetworkMatchLogic : NetworkBehaviour
    {   

        [SerializeField] List<NetworkClient> m_ClientTurnQueue; //Server-side updates.
        public event Action<ulong> OnClientConnected;
        
       
        [ServerRpc(RequireOwnership = false)] void InvokeOnClientConnectedServerRpc(ServerRpcParams serverRpcParams = default)
        {
            OnClientConnected?.Invoke(serverRpcParams.Receive.SenderClientId);
            
        }
        

        override public void OnNetworkSpawn()
        {
             if (!MonkeNetworkManager.Singleton.IsClient)
            {
                enabled = false;
                return;
            } 
            
            InvokeOnClientConnectedServerRpc();
        }
        override public void OnNetworkDespawn()
        {
             if (!MonkeNetworkManager.Singleton.IsClient)
            {
                return;
            } 
           
        }
    }
}
