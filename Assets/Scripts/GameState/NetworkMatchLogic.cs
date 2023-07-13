
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

        public event Action<ulong> OnClientConnected;
        public event Action<CardID> OnCardSelected;
        [ServerRpc(RequireOwnership = false)] void InvokeOnClientConnectedServerRpc(ServerRpcParams serverRpcParams = default)
        {
            OnClientConnected?.Invoke(serverRpcParams.Receive.SenderClientId);
        }
        
        [ClientRpc] public void DisplayCardsClientRpc(CardID[] cardIDs){
            List<GameObject> spawned_card_gos = new List<GameObject>();
            foreach(CardID c_id in cardIDs){
                GameObject card_prefab = GameDataSource.Instance.GetCardPrototypeByID(c_id).m_UICardPrefab;
                GameObject card_go = Instantiate(card_prefab) as GameObject;
                spawned_card_gos.Add(card_go);
            }
            ClientMatchState.Instance.DisplayCards(spawned_card_gos);
        }
        [ClientRpc] public void ClearCardsClientRpc(){
            ClientMatchState.Instance.ClearCards();
        }

        /// <summary>
        /// Sets a particular NetworkClient in control of the GameState-changing actions. (ie selecting a card)
        /// </summary>
        /// <param name="clientId"></param>
        [ClientRpc] public void SetControlClientRpc(ulong clientId){
            ClientMatchState.Instance.SetClientInControl(clientId);
        }

        [ServerRpc(RequireOwnership = false)] public void SelectCardServerRpc(CardID selectedCardID){
            
            OnCardSelected?.Invoke(selectedCardID);
            
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
