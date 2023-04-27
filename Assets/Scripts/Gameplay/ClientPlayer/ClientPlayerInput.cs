using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Monke.Gameplay.Character;
using Monke.Gameplay.Actions;

namespace Monke.Gameplay.ClientPlayer
{
    /// <summary>
    ///  Sends input to the server
    /// </summary> 
    public class ClientPlayerInput:NetworkBehaviour
    {
        // InputTriggerStyles are how actions are triggered (ie via key release, mouse click, etc.)
        ServerCharacter m_ServerCharacter;
        enum InputTriggerStyle{
            KeyRelease,
            KeyPress,
            MouseClick
        }
        struct ActionRequest{
            ActionID actionID;
            InputTriggerStyle inputTriggerStyle;
        }
        //static array of action Requests that can be inputted per frame. 
        //static = less memory overhead, we won't need more than 5
        ActionRequest[] m_ActionRequests = new ActionRequest[5];
        int m_ActionRequestCount;

        struct ActionSlot{
            ActionID slottedActionID;
            bool isEnabled;
            void SetSlottedActionId(ActionID id)
            {
                isEnabled = true;
                slottedActionID = id;
            }
        }
        ActionSlot m_ActionSlot1 = new ActionSlot();
        ActionSlot m_ActionSlot2 = new ActionSlot();
        ActionSlot m_ActionSlot3= new ActionSlot();
        public override void OnNetworkSpawn(){
            if (!IsClient || !IsOwner) enabled = false;
            m_ActionRequestCount = 0;
            
        }
        public override void OnNetworkDespawn(){

        }
        
    }
}