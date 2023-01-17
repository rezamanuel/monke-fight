using System;
using System.Collections.Generic;
using UnityEngine;

namespace Monke.Gameplay.Actions
{
    public class ActionSource : MonoBehaviour // singleton single source of truth for server-side actions; populated at runtime, contains references to Action Prototypes that can be copied.
    // is also responsible for assigning ActionIds to all of the actions at the beginning of the match session
     {

        public static ActionSource Instance {  get; private set; }
        [SerializeField] private List<Action> m_actionPrototypes; // List of ActionPrototypes; 
        // used to generate ActionIDs and as a reference point for creating Actions (which have been configured thnks to Scriptable Object)
        private List<Action> m_allActions;

        public Action GetActionPrototypeByID(ActionID actionId)
        {
            // will 
            return m_actionPrototypes[actionId.ID];
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

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
    }
}