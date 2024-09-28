
using UnityEngine;
using Monke.Utilities;
using Monke.Gameplay.Character;
using Unity.Netcode;
using Monke.Gameplay.ClientPlayer;
using Monke.Networking;
using System.Collections;
namespace Monke.GameState 
{
    /// <summary>
    /// FightState means the Players are currently actively fighting!
    /// </summary>
    [RequireComponent(typeof(NetcodeHooks))]
    public class FightState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.Match; } }

        [SerializeField] NetcodeHooks m_NetcodeHooks;
        [SerializeField]
        [Tooltip("A collection of locations for spawning players")]
        private Transform[] m_PlayerSpawnPoints;
        
        // has the first player spawned?
        // need to figure out how to make this consistent even when the first client to connect is not the server.
   
        protected override void Awake()
        {
            base.Awake();
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;
            SceneLoaderWrapper.Instance.OnClientSynchronized += OnClientSynchronized;
        }
        protected override void Start(){
            base.Start();
           
            if (MonkeNetworkManager.Singleton.IsServer)
            {
                foreach (var client in NetworkManager.Singleton.ConnectedClients)
                {
                    var player = NetworkManager.Singleton.ConnectedClients[client.Key].PlayerObject;
                    player.GetComponent<ServerCharacter>().InitializeCharacter();
                }
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }
        protected override void OnDestroy()
        {
            m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            SceneLoaderWrapper.Instance.OnClientSynchronized -= OnClientSynchronized;
            
        }
        void OnClientConnected(ulong clientId){
            var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            player.GetComponent<ServerCharacter>().InitializeCharacter();
        }

        void OnRoundEnd(){
                StartCoroutine(OnRoundEndCoroutine());
        }

        IEnumerator OnRoundEndCoroutine(){
            yield return new WaitForSeconds(2);
            SceneLoaderWrapper.Instance.QueueNextScene("CardSelect");
            SceneLoaderWrapper.Instance.UnloadScene();
        }
        void OnMatchEnd(){
            SceneLoaderWrapper.Instance.QueueNextScene("MatchEnd");
            SceneLoaderWrapper.Instance.UnloadScene();
        }
        void OnClientSynchronized()
        {
            // tell all player network objects to initialize client character
            
             if(NetworkManager.Singleton.IsClient)
            {
                Debug.Log( "Client is synchronized and fighting, enabling input!");
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClientPlayerInput>().SetEnabled(true);
                if (NetworkManager.Singleton.IsServer)
                {
                    // has the first player spawned?
                    // need to figure out how to make this consistent even when the first client to connect is not the server.
                    NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = m_PlayerSpawnPoints[0].position;
                }
                else
                {
                    NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = m_PlayerSpawnPoints[1].position;
                }
            }
        }

        
        void OnNetworkSpawn()
        {
             PlayerLivesManager.Instance.OnRoundEnd += OnRoundEnd;
            PlayerLivesManager.Instance.OnMatchEnd += OnMatchEnd;
        }

        void OnNetworkDespawn()
        {
            PlayerLivesManager.Instance.OnRoundEnd -= OnRoundEnd;
            PlayerLivesManager.Instance.OnMatchEnd -= OnMatchEnd;
             if(NetworkManager.Singleton.IsServer){
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
    }
}
