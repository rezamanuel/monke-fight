using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Monke.Gameplay.Actions;
using System;

namespace Monke.Cards
{
    /// <summary>
    /// Cards are items a Player collects that offers buffs to Character Attributes or unlocks / replaces actions. 
    /// </summary>
    public class Card : ScriptableObject
    {
        public CardID CardID;
        public ActionID ActionID; //if action with same ActionType is added to inventory, new action replaces old action.
        public Dictionary<CardBuffType,float> Buffs;
        public GameObject UICardPrefab;
    }

    /// <summary>
    /// CardBuffType will be used to discern which Character Attributes a Card may affect.
    /// </summary>
    public enum CardBuffType
    {
        BulletSpeed,
        BulletForce,
        BulletDamage,
        BulletSize,
        ClipSize,
        MaxHealth,
        MoveSpeed,
        ShootActionCooldown,
        BlockActionCooldown
    }

    /// <summary>
    /// CardID wraps an int, used for Network Messaging about Cards.
    /// </summary>
     public struct CardID : INetworkSerializable, IEquatable<CardID>
    {
        public int ID;

        public CardID(int id )
        {
            ID = id;
        }

        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ID);
        }

        public override bool Equals(object obj)
        {
            return obj is CardID other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public static bool operator ==(CardID x, CardID y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(CardID x, CardID y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return $"CardID({ID})";
        }
        public bool Equals(CardID other)
        {
            return ID == other.ID;
        }

    }
}
