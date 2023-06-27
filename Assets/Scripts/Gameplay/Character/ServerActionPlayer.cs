using Unity.Netcode;
using UnityEngine;
using Monke.Gameplay.Actions;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Monke.Gameplay.Character
{

    public class ServerActionPlayer 
    {
        private ServerCharacter m_ServerCharacter;
        private List<Action> m_ActionQueue; // loaded at-will, unloaded each update()
        private List<Action> m_ActiveActionList; //list of active actions, check if still active every update(); if not, free from memory.
        private Dictionary<ActionType, Action> m_BlockingActionList; //list of actions that will block new action queuing, sorted by ActionType
        public ServerActionPlayer(ServerCharacter serverCharacter)
        {
            this.m_ServerCharacter = serverCharacter;
            this.m_ActionQueue = new List<Action>();
        }
        private void TryReturnAction(Action action)
        {
            if (m_ActionQueue.Contains(action))
            {
                return;
            }
            ActionLibrary.ReturnAction(action);
        }

        public void  QueueAction(ActionRequestData actionRequestData){
            ActionID actionID = actionRequestData.actionID;
            var action = ActionLibrary.CreateAction(actionRequestData);
            m_ActionQueue.Add(action);
        }
         public void ClearActions()
        {
            if (m_ActionQueue.Count > 0)
            {
                m_ActionQueue[0].Cancel(m_ServerCharacter);
            }

            //clear the action queue
            {
                var removedActions = ListPool<Action>.Get();

                foreach (var action in m_ActionQueue)
                {
                    removedActions.Add(action);
                }

                m_ActionQueue.Clear();

                foreach (var action in removedActions)
                {
                    TryReturnAction(action);
                }

                ListPool<Action>.Release(removedActions);
            }
        }
        public void OnUpdate()
        {
            // expire blocking actions (they do not End(), but stop blocking new actions in their slot)
            foreach(var keyValuePair in m_BlockingActionList)
            {
                var action = keyValuePair.Value;
                float slot_cooldown = m_ServerCharacter.m_CharacterAttributes.m_ActionCooldowns[action.m_ActionType];
                Action blocking_action = m_BlockingActionList[action.m_ActionType];
                if (blocking_action.TimeRunning > slot_cooldown)
                {
                    m_BlockingActionList.Remove(action.m_ActionType);
                }
            }
            // expire old actions (they have End()'d )
            foreach (var action in m_ActiveActionList){
                if(!action.isActive)
                {
                    ActionLibrary.ReturnAction(action);
                    m_ActiveActionList.Remove(action);
                }
            }
            // queue new actions
            foreach (var action in m_ActionQueue)
            {
                //if cooldown has expired for that action's type, then queue; else discard.
                Action blocking_action = m_BlockingActionList[action.m_ActionType];
                if ( blocking_action == null)
                {
                    m_BlockingActionList[action.m_ActionType] = action;
                    action.OnStart(this.m_ServerCharacter);
                }

                
            }
        }
    }
}