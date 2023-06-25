using System;
using System.Collections.Generic;
using UnityEngine;
using Monke.Gameplay.Actions;
using Action = Monke.Gameplay.Actions.Action;
using Monke.Cards;

namespace Monke.Gameplay
{
    /// <summary>
    /// 
    /// </summary>
    public class GameDataSource : MonoBehaviour // singleton single source of truth for ALL gamedata; populated at runtime, contains references to Action Prototypes that can be copied.
                                                // is also responsible for assigning ActionIds to all of the actions at the beginning of the match session
    {
        public List<CardID> m_CommonCardIDs; // List of Common Card IDs
        public List<CardID> m_RareCardIDs; // List of Rare Card IDs
        public List<CardID> m_LegendaryCardIDs; // List of Legendary Card IDs
        public static GameDataSource Instance { get; private set; }
        [SerializeField] private List<Action> m_ActionPrototypes; // List of ActionPrototypes; 
        // used to generate ActionIDs and as a reference point for creating Actions (which have been configured thnks to Scriptable Object)
        [SerializeField] private List<Action> m_AllActions;

        [SerializeField] private List<Card> m_CardPrototypes; // List of CardPrototypes; 
        // used to generate CardIDs and as a reference point for creating Cards (which have been configured thnks to Scriptable Object)
        [SerializeField] private List<Card> m_AllCards;
        [SerializeField] private Dictionary<Card, Action> m_CardActionMap; // Links specific Card Prototypes to specific Action Prototypes.

        public Card GetCardPrototypeByID(CardID CardId)
        {
            // returns card
            return m_CardPrototypes[CardId.ID];
        }
        public CardID GetRandomCard(CardRarity rarity)
        {
            if (rarity == CardRarity.Common)
            {
                return m_CommonCardIDs[UnityEngine.Random.Range(0, m_CommonCardIDs.Count)];
            }
            else if (rarity == CardRarity.Rare)
            {
                return m_RareCardIDs[UnityEngine.Random.Range(0, m_CommonCardIDs.Count)];
            }
            else if (rarity == CardRarity.Legendary)
            {
                return m_LegendaryCardIDs[UnityEngine.Random.Range(0, m_CommonCardIDs.Count)];
            }
            else
            {
                Debug.LogError("GameDataSource.GetRandomCard() called without specifying rarity.");
            }
            return new CardID{ID=1}; // Default Return CardID 1
        }
        public bool TryGetCardPrototypeByID(CardID CardId, out Card Card)
        {
            // will return true if m_CardPrototypes contains the provided ID
            try
            {
                Card = m_CardPrototypes[CardId.ID];
                return true;
            }
            catch
            {
                Card = null;
                return false;
            }
        }

        private void PopulateAllCards()
        {
            // todo: initialize ActionIDs for cards as well, ensure this happens after ActionSource
            var Cards = new HashSet<Card>(m_CardPrototypes);
            int i = 0;
            foreach (Card c in Cards)
            {
                CardID card_id = new CardID { ID = i };
                m_AllCards.Add(c);
                m_AllCards[i].cardID = card_id;
                if (m_CardActionMap.ContainsKey(c))
                {
                    m_AllCards[i].m_CardActionID = new ActionID
                    {
                        ID = m_AllActions.IndexOf(m_CardActionMap.GetValueOrDefault(c))
                    };
                }
                if (c.m_CardRarity == CardRarity.Common)
                {
                    m_CommonCardIDs.Add(card_id);
                }
                else if (c.m_CardRarity == CardRarity.Rare)
                {
                    m_RareCardIDs.Add(card_id);
                }
                else if (c.m_CardRarity == CardRarity.Legendary)
                {
                    m_LegendaryCardIDs.Add(card_id);
                }
                i++;
            }
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                m_CardActionMap = new Dictionary<Card, Actions.Action>();
                m_CommonCardIDs = new List<CardID>();
                m_RareCardIDs = new List<CardID>();
                m_LegendaryCardIDs = new List<CardID>();
                PopulateAllActions();
                PopulateAllCards();
            }
        }

        public Action GetActionPrototypeByID(ActionID actionId)
        {
            // will 
            return m_ActionPrototypes[actionId.ID];
        }
        public bool TryGetActionPrototypeByID(ActionID actionId, out Action action)
        {
            // will return true if m_actionPrototypes contains the provided ID
            try
            {
                action = m_ActionPrototypes[actionId.ID];
                return true;
            }
            catch
            {
                action = null;
                return false;
            }
        }

        private void PopulateAllActions()
        {
            var actions = new HashSet<Action>(m_ActionPrototypes);
            int i = 0;
            foreach (Action a in actions)
            {
                m_AllActions.Add(a);
                m_AllActions[i].actionID = new ActionID { ID = i };
                i++;
            }
        }
    }
}