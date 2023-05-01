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
    public class ClientPlayerInput:NetworkBehaviour
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

        public class ActionSlot{
            public ActionID slottedActionID;
            public bool isEnabled = false;
        }
        
        public ActionSlot m_ActionSlot1;
        public ActionSlot m_ActionSlot2;
        public ActionSlot m_ActionSlot3;
        public override void OnNetworkSpawn(){
            if (!IsClient || !IsOwner) enabled = false;
            m_ActionRequestCount = 0;

            // initialize 'skill slots' from character attribute Scriptable Object
            if (ActionSource.Instance.TryGetActionPrototypeByID(m_ServerCharacter.m_ServerCharacterAttributes.actionPrototype1.ActionID, out Action action1)){
                m_ActionSlot1 = new ActionSlot() { slottedActionID = action1.ActionID, isEnabled = true };
            }
            if (ActionSource.Instance.TryGetActionPrototypeByID(m_ServerCharacter.m_ServerCharacterAttributes.actionPrototype2.ActionID, out Action action2)){
                m_ActionSlot2 = new ActionSlot() { slottedActionID = action2.ActionID, isEnabled = true };
            }
            if (ActionSource.Instance.TryGetActionPrototypeByID(m_ServerCharacter.m_ServerCharacterAttributes.actionPrototype1.ActionID, out Action action3)){
                m_ActionSlot3 = new ActionSlot() { slottedActionID = action3.ActionID, isEnabled = true };
            }
        }
        public override void OnNetworkDespawn(){
            // unsubscribe from events / delegates
        }

        public void SendActionRequest(ActionRequestData action){
            m_ServerCharacter.DoActionServerRpc(action);
        }

         /// <summary>
        /// Request an action be performed. This will occur on the next FixedUpdate.
        /// </summary>
        /// <param name="actionID"> The action you'd like to perform. </param>
        /// <param name="triggerStyle"> What input style triggered this action. </param>
        /// <param name="targetId"> NetworkObjectId of target. </param>
        public void RequestAction(ActionID actionID, InputTriggerStyle triggerStyle)
        {
            Assert.IsNotNull(ActionSource.Instance.GetActionPrototypeByID(actionID),
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
            //Process Key Inputs + request actions.
            input
            //if X key, Request Action

        }

        void FixedUpdate(){
            //dequeue action requests, pass onto ServerActionPlayer thru SendActionRequest

            //foreach ActionRequests[], verify prototype exists in ActionSource, Send Action Request.
        }
        
    }
}