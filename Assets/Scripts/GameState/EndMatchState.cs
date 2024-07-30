
using UnityEngine;
using Monke.Utilities;
using Monke.Gameplay.Character;
using Unity.Netcode;
using Monke.Gameplay.ClientPlayer;
namespace Monke.GameState
{
    /// <summary>
    /// EndMatchState means the Players have finished fighting!
    /// </summary>
    [RequireComponent(typeof(NetcodeHooks))]
    public class EndMatchState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.Match; } }

        [SerializeField] NetcodeHooks m_NetcodeHooks;
        [SerializeField] TMPro.TextMeshProUGUI m_WinnerTextDisplay;

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
            
        }
        void OnClientSynchronized()
        {
            // tell all player network objects to initialize client character
            if(NetworkManager.Singleton.IsClient)
            {
                // disable input for local client.
                Debug.Log( "Client is synchronized, disabling input! (EndMatch)");
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClientPlayerInput>().SetEnabled(false);
            }
            if(NetworkManager.Singleton.IsServer)
            {
                // set the winner display
                setWinnerTextDisplayClientRpc();
            }
        }
        [ClientRpc] public void setWinnerTextDisplayClientRpc()
        {
            // set the winner display
            if (NetworkManager.Singleton.IsServer)
            {
                Debug.Log("Setting Winner Display");
                m_WinnerTextDisplay.text = "Player " + PlayerLivesManager.Instance.GetWinner().ToString() + " Wins!";
            }
        }

        void OnNetworkSpawn()
        {
            
        }

        void OnNetworkDespawn()
        {
        }
    }
    
}