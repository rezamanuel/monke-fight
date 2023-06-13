using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Monke.Cards;
namespace Monke.Gameplay.Character
{
    /// <summary>
    /// Class used for storing a Character's cards during a Match. 
    /// </summary>
    public class CharacterCardInventory : NetworkBehaviour
    {
        ServerCharacter m_ServerCharacter;
        NetworkList<CardID> m_CardsInPlay;
        NetworkList<CardID> m_CardDrawn;
        ServerCardDeck m_ServerCardDeck;
        void Start()
        {
            m_ServerCharacter = GetComponentInParent<ServerCharacter>();
        }

        void DrawCards(int num)
        {
            for (int i = 0; i < num; i++)
            {

            }
        }

        void PlayCard(CardID cardID)
        {
            m_CardDrawn.Add(cardID);
            Card card = GameDataSource.Instance.GetCardPrototypeByID(cardID);
            card.OnPlay(m_ServerCharacter);
        }
        void Update()
        {

        }
    }
}
