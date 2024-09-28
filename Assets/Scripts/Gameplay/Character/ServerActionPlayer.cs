using Unity.Netcode;
using UnityEngine;
using Monke.Gameplay.Actions;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine.InputSystem.Interactions;
using System;
using Action = Monke.Gameplay.Actions.Action;
using System.Runtime.CompilerServices;

namespace Monke.Gameplay.Character
{

    public class ServerActionPlayer 
    {
        private ServerCharacter m_ServerCharacter;
        private List<Action> m_ActionQueue; // loaded at-will, unloaded each update()
        private List<Action> m_ActiveActionList; //list of active actions, check if still active every update(); if not, free from memory.
        private Dictionary<ActionType, Action> m_BlockingActionList; //list of actions that will block new action queuing, sorted by ActionType

        private List<ActionType> expiredActionTypes; // used as a memo array to hold actiontypes we are removing from the blocking action list.
        private List<Action> expiredActions; // used as a memo array to hold actions we are removing from iether active actions or the action queue.
        public ServerActionPlayer(ServerCharacter serverCharacter)
        {
            this.m_ServerCharacter = serverCharacter;
            this.m_ActionQueue = new List<Action>();
            this.m_ActiveActionList = new List<Action>();
            this.m_BlockingActionList = new Dictionary<ActionType, Action>();
            this.expiredActionTypes = new List<ActionType>();
            this.expiredActions = new List<Action>();
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
            foreach(var actionType in m_BlockingActionList.Keys)
            {
                var action = m_BlockingActionList[actionType];
                float slot_cooldown = m_ServerCharacter.m_CharacterAttributes.GetCooldown(actionType);

                Action blocking_action;
                m_BlockingActionList.TryGetValue(action.m_ActionType, out blocking_action);
                if (blocking_action.TimeRunning > slot_cooldown)
                {
                     Debug.Log("ADDING ACTION TO EXPIRED ACTIONS");
                    expiredActionTypes.Add(action.m_ActionType);
                }
            }
            foreach(var actionType in expiredActionTypes){
                m_BlockingActionList[actionType].Reset();
                m_BlockingActionList.Remove(actionType);
            }
            expiredActionTypes.Clear(); 
            // expire old actions (they have End()'d )
            foreach (var action in m_ActiveActionList){
                if(!action.isActive)
                {
                    ActionLibrary.ReturnAction(action);
                    expiredActions.Add(action);
                }
                // else update old actions
                action.OnUpdate(m_ServerCharacter);
            }
            foreach(var action in expiredActions){
                m_ActiveActionList.Remove(action);
                 
            }
            expiredActions.Clear();
            // queue new actions
            foreach (var action in m_ActionQueue)
            {
                //if cooldown has expired for that action's type, then queue; else discard.
                Action blocking_action;
                m_BlockingActionList.TryGetValue(action.m_ActionType, out blocking_action);
                if ( blocking_action == null)
                {
                    m_BlockingActionList.Add(action.m_ActionType, action);
                    m_ActiveActionList.Add(action); 
                    action.OnStart(this.m_ServerCharacter);
                    
                }
                else{
                    Debug.Log("Blocking Action:" +blocking_action.name);
                }              
                
                expiredActions.Add(action);  
            }
            foreach(var action in expiredActions){
                TryReturnAction(action);
                m_ActionQueue.Remove(action);
            }
            expiredActions.Clear();
        }
    }
}