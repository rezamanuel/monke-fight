using UnityEngine;
using System;
using Monke.Gameplay.Actions;
using Unity.Netcode;

namespace Monke.Gameplay.Character
{
    [RequireComponent(typeof(NetworkHealthState))]
    public class ServerCharacter : NetworkBehaviour
    {
        //ServerActionPlayer
        
        NetworkHealthState healthState;

        public ServerCharacterAttributes serverCharacterAttributes;

        [ServerRpc]
        public void DoActionServerRpc(ActionID actionId, ActionRequestData actionRequestData) 
        {
            // call Server Action Player PlayAction( actionRequestData)
            // check for 
        }

    }
}