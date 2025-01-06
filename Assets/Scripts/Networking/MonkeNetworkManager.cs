using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Monke.Utilities;
namespace Monke.Networking
{
    public class MonkeNetworkManager : NetworkManager
    {
        void OnLoadEventCompleted (){
            SceneLoaderWrapper.Instance.AddOnSceneEventCallback();
        }
        // called for local multiplayer 
        public void StartHostLocal(){
            StartHost();
        }
    }

}