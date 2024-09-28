using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Monke.Gameplay.Actions;
using System;
using Monke.Gameplay.Character;
using Monke.Gameplay;

namespace Monke.Cards
{
    /// <summary>
    /// Cards are items a Player collects that offers buffs to Character Attributes or unlocks / replaces actions. 
    /// </summary>
    
    [CreateAssetMenu(menuName = "Monke/Cards/Card")]
    public class Card : ScriptableObject
    {
        [NonSerialized] public CardID cardID;
        [NonSerialized] public ActionID m_CardActionID;
        // Action that gets unlocked for the character after picking up this card.
        // if Action with same ActionType is added to inventory, new Action replaces old Action.
        public List<CardBuffType> m_BuffTypes;//m_BuffTypes and m_Buffs must be one-to-one
        public List<float> m_Buffs;
        public CardRarity m_CardRarity;
        public GameObject m_UICardPrefab;
        public void OnPlay(ServerCharacter serverCharacter){
            // Apply Buffs, replace Actions.
            ApplyBuffs(serverCharacter);
            Gameplay.Actions.Action action;
            if(GameDataSource.Instance.TryGetActionPrototypeByID(m_CardActionID, out action)){
                //serverCharacter.m_CharacterAttributes.m_ActionSlots[action.m_ActionType] = action.actionID;
            }
        } 

        public void ApplyBuffs(ServerCharacter serverCharacter){
            int i = 0;
            foreach (CardBuffType buff in m_BuffTypes){
                serverCharacter.m_CharacterAttributes.ApplyCardBuff(buff, m_Buffs[i]);
                i++;
            }
        }
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


    public enum CardRarity
    {
        Common, // 70% chance to draw
        Rare, // 20%
        Legendary // 10%
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
