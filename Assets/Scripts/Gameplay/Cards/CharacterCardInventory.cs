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
        NetworkList<CardID> m_ActiveCards;
        NetworkList<CardID> m_DrawnCards;
        void Start()
        {
            m_ServerCharacter = GetComponentInParent<ServerCharacter>();
        }

        void DrawCards(int num)
        {
            for (int i = 0; i < num; i++)
            {
                int random_num = Random.Range(0, 100); // roll d100
                if(random_num < 75){
                    m_DrawnCards.Add(GameDataSource.Instance.GetRandomCard(CardRarity.Common));
                }else if(random_num <90){
                    m_DrawnCards.Add(GameDataSource.Instance.GetRandomCard(CardRarity.Rare));
                }else{
                    m_DrawnCards.Add(GameDataSource.Instance.GetRandomCard(CardRarity.Legendary));
                }
            }
        }

        void PlayCard(CardID cardID)
        {
            m_DrawnCards.Add(cardID);
            Card card = GameDataSource.Instance.GetCardPrototypeByID(cardID);
            card.OnPlay(m_ServerCharacter);
        }
        void Update()
        {

        }
    }
}
