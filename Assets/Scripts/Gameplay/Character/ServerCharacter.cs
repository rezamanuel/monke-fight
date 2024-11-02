using UnityEngine;
using System;
using Monke.Gameplay.Actions;
using Unity.Netcode;
using Monke.Gameplay.ClientPlayer;
using Unity.VisualScripting;
using System.Text.RegularExpressions;
using Monke.Networking;
namespace Monke.Gameplay.Character
{
    [RequireComponent(typeof(NetworkHealthState))]
    [RequireComponent(typeof(DamageReceiver))]
    [RequireComponent(typeof(ServerCharacterAttributes))]
    [RequireComponent(typeof(CharacterCardInventory))]
    public class ServerCharacter : NetworkBehaviour
    {
        public ClientCharacter m_ClientCharacter;
        public CharacterCardInventory m_CharacterCardInventory;
        public ClientPlayerInput m_ClientPlayerInput;
        public Transform m_ArmTarget; // animation target for 'aiming' set on prefab

        ServerActionPlayer m_ServerActionPlayer;
        NetworkHealthState m_HealthState;
        DamageReceiver m_DamageReceiver;

        public ServerCharacterAttributes m_CharacterAttributes;

        public int HitPoints
        {
            get => m_HealthState.HitPoints.Value;
            private set => m_HealthState.HitPoints.Value = value;
        }
        public void Awake()
        {
            m_ClientCharacter = GetComponentInChildren<ClientCharacter>();
            m_ServerActionPlayer = new ServerActionPlayer(this);
            m_CharacterCardInventory = GetComponent<CharacterCardInventory>();
            m_HealthState = GetComponent<NetworkHealthState>();
            m_DamageReceiver = GetComponent<DamageReceiver>();
            m_CharacterAttributes = GetComponent<ServerCharacterAttributes>();
            m_ClientPlayerInput = GetComponentInChildren<ClientPlayerInput>();
        }
        public void InitializeCharacter()
        {
            Debug.Log("Initializing Character" + this.NetworkObject.OwnerClientId + " with prev HP" + HitPoints);
            InitializeHitPoints();
            Debug.Log("Initializing Character" + this.NetworkObject.OwnerClientId + " with cur HP" + HitPoints);
        }
        public void Update(){
            
            m_ServerActionPlayer.OnUpdate();
            
        }

        [ServerRpc]
        public void DoActionServerRpc(ActionRequestData actionRequestData) 
        {
            if(m_HealthState.HitPoints.Value > 0){
                //if we are alive, queue action. Server action player will decide if it is valid (ie. blocked by cooldown or etc.)
                m_ServerActionPlayer.QueueAction(actionRequestData);
            }
        }
        public override void OnNetworkSpawn(){
            if (IsClient && IsOwner)
            {
                
                //subscribe to health state events
                m_HealthState.HitPointsDepleted += () => m_ClientPlayerInput.SetEnabled(false);
                m_HealthState.HitPointsReplenished += () => m_ClientPlayerInput.SetEnabled(true);
            }
            if (IsServer) 
            {
                m_ArmTarget = m_ClientCharacter.transform.GetChild(0).Find("ShoulderAnchor").GetChild(0);
                m_HealthState.HitPointsDepleted += OnHitPointsDepleted;
                m_DamageReceiver.DamageReceived += ReceiveHP;
                InitializeHitPoints();
               
            }
            else{
                enabled = false;
            }
        }

        private void OnHitPointsDepleted()
        {
            PlayerLivesManager.Instance.OnPlayerDeath(this.NetworkObject.OwnerClientId);
        }

        public override void OnNetworkDespawn()
        {
            if(IsClient && IsOwner)
            {
                m_HealthState.HitPointsDepleted -= () => m_ClientPlayerInput.SetEnabled(false);
                m_HealthState.HitPointsReplenished -= () => m_ClientPlayerInput.SetEnabled(true);
            }
            if (!IsServer) return;
            
            m_HealthState.HitPointsDepleted -= OnHitPointsDepleted;
            m_DamageReceiver.DamageReceived -= ReceiveHP;
        }

        /// <summary>
        /// Receive an HP change from somewhere. Could be healing or damage.
        /// </summary>
        /// <param name="inflictor">Person dishing out this damage/healing. Can be null. </param>
        /// <param name="HP">The HP to receive. Positive value is healing. Negative is damage.  </param>
        void ReceiveHP(ServerCharacter inflictor, int HP)
        {
            HitPoints += HP;
            if (HP <= 0)
            {
                //if we are dead, clear all actions
                m_ServerActionPlayer.ClearActions();
            }
        }

        void InitializeHitPoints()
        {
            HitPoints = m_CharacterAttributes.m_MaxHealth.Value;
        }

    }
}