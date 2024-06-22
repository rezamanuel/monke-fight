using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Utilities;
using Monke.Networking;
using Monke.Gameplay.Character;
using Unity.Netcode;
using Monke.Infrastructure;
using Monke.Gameplay.ClientPlayer;
using UnityEngine.iOS;
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
        [SerializeField] private CharacterSpawner m_CharacterSpawner;
        
        

        protected override void Awake()
        {
            base.Awake();
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkDespawn;
            SceneLoaderWrapper.Instance.OnClientSynchronized += OnClientSynchronized;
        }
        protected override void OnDestroy()
        {
            m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkDespawn;
            SceneLoaderWrapper.Instance.OnClientSynchronized -= OnClientSynchronized;

            // cleanup all client characters
            if(MonkeNetworkManager.Singleton.IsServer)
            {
                foreach (var player in MonkeNetworkManager.Singleton.ConnectedClientsList)
                {
                    var serverCharacter = player.PlayerObject.GetComponent<ServerCharacter>();
                    DespawnClientCharacterRpc();
                }
            }
        }
        void OnClientSynchronized()
        {
            // tell all player network objects to initialize client character
            if(NetworkManager.Singleton.IsServer)
            {
                SpawnClientCharacters();
            }
            
        }

        void SpawnClientCharacters()
        {
            // tell all player network objects to initialize client character
            Debug.Log("Spawning client character");
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var p in players)
            {
                m_CharacterSpawner.SpawnClientCharacter(p.GetComponent<ServerCharacter>(), p.GetComponent<NetworkObject>().OwnerClientId);
            }
        }
        [ClientRpc]void DespawnClientCharacterRpc()
        {
            // tell all player network objects to despawn client character
           ServerCharacter[] serverCharacters = GameObject.FindObjectsOfType<ServerCharacter>();
            foreach (var serverCharacter in serverCharacters)
            {
                m_CharacterSpawner.DespawnClientCharacter(serverCharacter);
            }
        }
        void OnNetworkSpawn()
        {
            Debug.Log("OnNetworkSpawn");
            if(NetworkManager.Singleton.IsClient)
            {
                Debug.Log( "Client is spawning");
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClientPlayerInput>().enabled = true;
                Debug.Log(NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClientPlayerInput>().enabled);
            }
        }

        void OnNetworkDespawn()
        {
           if(NetworkManager.Singleton.IsClient)
            {
                
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClientPlayerInput>().SetActive(false);
            }
        }
    }
}
