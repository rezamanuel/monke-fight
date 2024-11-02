using UnityEngine;
using Unity.Netcode;
using Monke.Gameplay.Character;

namespace Monke.GameState
{
    public class FightLogic : NetworkBehaviour
    {
        [ServerRpc (RequireOwnership =false)] public void InitializePlayerServerRpc(ServerRpcParams clientRpcParams = default){
            var clientId = clientRpcParams.Receive.SenderClientId;
            Debug.Log("Initializing player " + clientId);
            if(NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId)){
                //initialize the character for the client
                {
                    var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
                    player.GetComponent<ServerCharacter>().InitializeCharacter();
                }
            }
        }
    }
}