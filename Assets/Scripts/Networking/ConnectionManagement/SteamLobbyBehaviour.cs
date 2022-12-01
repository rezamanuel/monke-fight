using System.Collections;
using UnityEngine;
using Netcode.Transports;
using Steamworks;
namespace Monke.Networking
{

    public class SteamLobbyBehaviour : MonoBehaviour 
    {
        // Callbacks
        protected Callback<LobbyCreated_t> LobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> JoinRequest;
        protected Callback<LobbyEnter_t> LobbyEntered;

        // Variables
        public ulong CurrentLobbyID;
        private const string HostAddressKey = "HostAddress";
        private Monke.Networking.MonkeNetworkManager netmanager;

        public void Start()
        {
            LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
            LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        public void HostLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 2); // hardcoded to 2 players max for now.

        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK) { Debug.LogError("Could not successfully create Lobby"); return; }

            Debug.Log("Lobby created Successfully");

            netmanager.StartHost();

            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());

            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
        }

        private void OnJoinRequest(GameLobbyJoinRequested_t callback)
        {
            Debug.Log("Request to Join Lobby");
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            //Everyone

            CurrentLobbyID = callback.m_ulSteamIDLobby;
            Debug.Log("Lobby Name: " + SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name"));

            //Clients

            netmanager.GetComponent<SteamNetworkingTransport>().ConnectToSteamID = callback.m_ulSteamIDLobby;


        }
    }
}