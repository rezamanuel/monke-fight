using UnityEngine;
using Unity.Netcode;
using Monke.Networking;
using Monke.Gameplay.ClientPlayer;
namespace Monke.Utilities
{
    /// <summary>
    /// InputDisabler is a utility that disables input for a client when the server has synchronized for the first time.
    /// </summary>
    public class InputDisabler : MonoBehaviour
    {
        void Awake()
        {
            if (MonkeNetworkManager.Singleton.IsServer)
            {
                SceneLoaderWrapper.Instance.OnClientSynchronized += OnClientSynchronized;
            }
        }
        void OnDestroy()
        {
            if (MonkeNetworkManager.Singleton.IsServer)
            {
                SceneLoaderWrapper.Instance.OnClientSynchronized -= OnClientSynchronized;
            }
        }
        // Start is called before the first frame update
         void OnClientSynchronized()
        {
            if(NetworkManager.Singleton.IsClient)
            {
                // disable input for local client.
                Debug.Log( "Client is synchronized, disabling input! (MatchLogic)");
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClientPlayerInput>().SetEnabled(false);
            }
            Destroy(this);
        }
    }
}