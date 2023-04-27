using UnityEngine;
using System;
using Monke.Gameplay.Actions;
using Unity.Netcode;

namespace Monke.Gameplay.Character
{
    [RequireComponent(typeof(NetworkHealthState))]
    public class ServerCharacter : NetworkBehaviour
    {
        ClientCharacter m_ClientCharacter;
        ServerActionPlayer m_ServerActionPlayer;
        NetworkHealthState m_HealthState;
        DamageReceiver m_DamageReceiver;

        public ServerCharacterAttributes m_ServerCharacterAttributes;

        public int HitPoints
        {
            get => m_HealthState.HitPoints.Value;
            private set => m_HealthState.HitPoints.Value = value;
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
            if (!IsServer) { enabled = false; }
             else
            {
                m_DamageReceiver.DamageReceived += ReceiveHP;
                InitializeHitPoints();
            }
        }
        public override void OnNetworkDespawn()
        {
             m_DamageReceiver.DamageReceived -= ReceiveHP;
        }

        /// <summary>
        /// Receive an HP change from somewhere. Could be healing or damage.
        /// </summary>
        /// <param name="inflictor">Person dishing out this damage/healing. Can be null. </param>
        /// <param name="HP">The HP to receive. Positive value is healing. Negative is damage.  </param>
        void ReceiveHP(ServerCharacter inflictor, int HP){
            HitPoints += HP;
            if(HP<=0) m_ServerActionPlayer.ClearActions();
        }

        void InitializeHitPoints()
        {
            HitPoints = m_ServerCharacterAttributes.maxHealth;
        }

    }
}