using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Monke.Gameplay.Character;
using Monke.Gameplay.Actions;
using UnityEngine.Assertions;

namespace Monke.Gameplay.ClientPlayer
{
    /// <summary>
    ///  Sends input to the server
    /// </summary>
    [RequireComponent(typeof(ServerCharacter))]
    public class ClientPlayerActionInput:NetworkBehaviour
    {
        // InputTriggerStyles are how actions are triggered (ie via key release, mouse click, etc.)
        ServerCharacter m_ServerCharacter;
        public enum InputTriggerStyle{
            KeyRelease,
            KeyPress,
            MouseClick
        }
        struct ActionRequest{
            public ActionID actionID;
            public InputTriggerStyle inputTriggerStyle;
        }
        //static array of action Requests that can be inputted per frame. 
        //static = less memory overhead, we won't need more than 5
        ActionRequest[] m_ActionRequests = new ActionRequest[5];
        int m_ActionRequestCount;

        public void Awake(){
            m_ServerCharacter = this.GetComponent<ServerCharacter>();
            
        }

        public override void OnNetworkSpawn(){
            if (!IsClient || !IsOwner) enabled = false;
            m_ActionRequestCount = 0;
        }
        public override void OnNetworkDespawn(){
            // unsubscribe from events / delegates
        }

        public void SendActionRequest(ActionRequestData action){
            m_ServerCharacter.DoActionServerRpc(action);
        }

         /// <summary>
        /// Request an action be performed. This will occur on the next Update.
        /// </summary>
        /// <param name="actionID"> The action you'd like to perform. </param>
        /// <param name="triggerStyle"> What input style triggered this action. </param>
        public void RequestAction(ActionID actionID, InputTriggerStyle triggerStyle)
        {
            Assert.IsNotNull(GameDataSource.Instance.GetActionPrototypeByID(actionID),
                $"Action with actionID {actionID} must be contained in the Action prototypes of ActionSource!");

            if (m_ActionRequestCount < m_ActionRequests.Length)
            {
                m_ActionRequests[m_ActionRequestCount].actionID = actionID;
                m_ActionRequests[m_ActionRequestCount].inputTriggerStyle = triggerStyle;
                m_ActionRequestCount++;
            }
        }

        void Update()
        {
            
            //dequeue action requests, pass onto ServerActionPlayer thru SendActionRequest

            foreach(var actionRequest in m_ActionRequests){
                
            }
            //foreach ActionRequests[], verify prototype exists in ActionSource, Send Action Request.

        }

        
    }
}