using UnityEngine;
using Unity;
using Unity.Netcode;
using Monke.Gameplay.Interfaces;
using Monke.Gameplay.Character;
using System;
public class DamageReceiver : NetworkBehaviour, IDamageable
    {
        public event Action<ServerCharacter, int> DamageReceived;

        public event Action<Collision> CollisionEntered;


    [SerializeField] NetworkHealthState healthState;

    public void ReceiveHP(ServerCharacter inflicter, int HP)
        {
            if (IsDamageable())
            {
                DamageReceived?.Invoke(inflicter, HP);
            }
        }

        public bool IsDamageable()
        {
            return healthState.HitPoints.Value > 0;
        }

        void OnCollisionEnter(Collision other)
        {
            CollisionEntered?.Invoke(other);
        }
    }