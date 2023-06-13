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
            // will 
            return m_CardPrototypes[CardId.ID];
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
                m_AllCards.Add(c);
                m_AllCards[i].cardID = new CardID { ID = i };
                if (m_CardActionMap.ContainsKey(c))
                {
                    m_AllCards[i].m_CardActionID = new ActionID 
                    { 
                        ID = m_AllActions.IndexOf(m_CardActionMap.GetValueOrDefault(c)) 
                    };
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