using System;
using System.Collections.Generic;
using UnityEngine;

namespace Monke.Gameplay.Actions
{
    public class ActionSource : MonoBehaviour // singleton single source of truth for server-side actions; populated at runtime, contains references to Action Prototypes that can be copied.
    // is also responsible for assigning ActionIds to all of the actions at the beginning of the match session
     {

        public static ActionSource Instance {  get; private set; }
        [SerializeField] private List<Action> m_ActionPrototypes; // List of ActionPrototypes; 
        // used to generate ActionIDs and as a reference point for creating Actions (which have been configured thnks to Scriptable Object)


        public Action GetActionPrototypeByID(ActionID actionId)
        {
            // will 
            return m_ActionPrototypes[actionId.ID];
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