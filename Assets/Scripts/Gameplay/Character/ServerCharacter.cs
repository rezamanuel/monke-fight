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

        [ServerRpc]
        public void DoActionServerRpc(ActionID actionId, ActionRequestData actionRequestData) 
        {
            // call Server Action Player PlayAction( actionRequestData) // only 1 server action player in lobby; will add action to its queue
            // check for 
        }

    }
}