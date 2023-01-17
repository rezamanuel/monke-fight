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
    }
}