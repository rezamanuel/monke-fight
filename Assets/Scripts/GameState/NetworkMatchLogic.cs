
using System.Collections.Generic;
using UnityEngine;
using Monke.Networking;
using Monke.UI;
using Monke.Cards;
using Monke.Gameplay;
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
        
        [ClientRpc] public void DisplayCardsClientRpc(CardID[] cardIDs){
            List<GameObject> spawned_cards = new List<GameObject>();
            foreach(CardID c_id in cardIDs){
                GameObject card_prefab = GameDataSource.Instance.GetCardPrototypeByID(c_id).m_UICardPrefab;
                GameObject card_go = Instantiate(card_prefab) as GameObject;
                spawned_cards.Add(card_go);
            }
             ClientMatchState.Instance.OnDisplayCards(spawned_cards);
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
