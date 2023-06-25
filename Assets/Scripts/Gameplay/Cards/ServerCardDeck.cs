using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Monke.Cards;
namespace Monke.Gameplay
{
    /// <summary>
    /// Class used for storing a Character's cardDeck during a Match. (DEPRECATED)
    /// </summary>
    public class ServerCardDeck : NetworkBehaviour
    {
        public List<CardID> m_CardDeck; //Set by GameDataSource 
        void Start()
        {
        }

        public CardID DrawFromDeck()
        {
            
            CardID card_id = new CardID(m_CardDeck[Random.Range(0, m_CardDeck.Count)].ID) ;
            m_CardDeck.Remove(card_id);
            return card_id;

        }
        public void InitializeDeck(){
                //TODO
        }
        public void ShuffleDeck(){
            int card_count = m_CardDeck.Count;
            List<CardID> shuffled_deck = new List<CardID>(card_count);
            for (int i = 0; i < card_count; i++){
                shuffled_deck[i] = m_CardDeck[Random.Range(0, m_CardDeck.Count)];
            }
            m_CardDeck = shuffled_deck;
        }

        override public void OnNetworkSpawn(){
             if (!IsServer) { enabled = false; }
             else{
                ShuffleDeck();
            }
        }
    }
}
