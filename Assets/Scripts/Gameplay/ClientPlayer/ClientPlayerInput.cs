using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Monke.Gameplay.ClientPlayer
{
    /// <summary>
    ///  Sends input to the server
    /// </summary> 
    public class ClientPlayerInput:NetworkBehaviour
    {

        public override void OnNetworkSpawn(){
            
        }
        public override void OnNetworkDespawn(){

        }
        
    }
}