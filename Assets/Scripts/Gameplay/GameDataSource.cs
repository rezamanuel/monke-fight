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
        [SerializeField] private List<Action> m_actionPrototypes; // List of ActionPrototypes; 
        // used to generate ActionIDs and as a reference point for creating Actions (which have been configured thnks to Scriptable Object)
        [SerializeField] private List<Action> m_allActions;

        [SerializeField] private List<Card> m_CardPrototypes; // List of CardPrototypes; 
        // used to generate CardIDs and as a reference point for creating Cards (which have been configured thnks to Scriptable Object)
        [SerializeField] private List<Card> m_allCards;

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
            foreach (Card a in Cards)
            {
                m_allCards.Add(a);
                m_allCards[i].CardID = new CardID { ID = i };
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
                PopulateAllActions();
                PopulateAllCards();
            }
        }

        public Action GetActionPrototypeByID(ActionID actionId)
        {
            // will 
            return m_actionPrototypes[actionId.ID];
        }
        public bool TryGetActionPrototypeByID(ActionID actionId, out Action action)
        {
            // will return true if m_actionPrototypes contains the provided ID
            try
            {
                action = m_actionPrototypes[actionId.ID];
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
            var actions = new HashSet<Action>(m_actionPrototypes);
            int i = 0;
            foreach (Action a in actions)
            {
                m_allActions.Add(a);
                m_allActions[i].ActionID = new ActionID { ID = i };
                i++;
            }
        }
    }
}