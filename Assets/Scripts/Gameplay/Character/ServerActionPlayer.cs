using Unity.Netcode;
using UnityEngine;
using Monke.Gameplay.Actions;
using System.Collections.Generic;

namespace Monke.Gameplay.Character
{

    public class ServerActionPlayer 
    {
        private ServerCharacter m_serverCharacter;
        private List<Action> m_actionQueue; // loaded at-will, unloaded each update()
        private List<Action> m_activeActionList; //list of active actions, check if still active every update(); if not, free from memory.
        private Dictionary<ActionType, Action> m_blockingActionList; //list of actions that will block new action queuing, sorted by ActionType
        ServerActionPlayer(ServerCharacter serverCharacter)
        {
            this.m_serverCharacter = serverCharacter;
            this.m_actionQueue = new List<Action>();
        }

        public void  QueueAction(ActionRequestData actionRequestData){
            ActionID actionID = actionRequestData.actionID;
            var action = ActionLibrary.CreateAction(actionRequestData);
            m_actionQueue.Add(action);
        }
        public void OnUpdate()
        {
            // expire blocking actions (they do not End(), but stop blocking new actions in their slot)
            foreach(var keyValuePair in m_blockingActionList)
            {
                var action = keyValuePair.Value;
                float slot_cooldown = m_serverCharacter.serverCharacterAttributes.actionCooldowns[action.m_ActionType];
                Action blocking_action = m_blockingActionList[action.m_ActionType];
                if (blocking_action.TimeRunning > slot_cooldown)
                {
                    m_blockingActionList.Remove(action.m_ActionType);
                }
            }
            // expire old actions (they have End()'d )
            foreach (var action in m_activeActionList){
                if(!action.isActive)
                {
                    ActionLibrary.ReturnAction(action);
                    m_activeActionList.Remove(action);
                }
            }
            // queue new actions
            foreach (var action in m_actionQueue)
            {
                //if cooldown has expired for that action's type, then queue; else discard.
                Action blocking_action = m_blockingActionList[action.m_ActionType];
                if ( blocking_action == null)
                {
                    m_blockingActionList[action.m_ActionType] = action;
                    action.OnStart(this.m_serverCharacter);
                }

                
            }
        }
    }
}