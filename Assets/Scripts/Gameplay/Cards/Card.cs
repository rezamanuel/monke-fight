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
        public Dictionary<CardBuffType,float> m_Buffs;
        public CardRarity m_CardRarity;
        public GameObject m_UICardPrefab;
        public void OnPlay(ServerCharacter serverCharacter){
            // Apply Buffs, replace Actions.
            ApplyBuffs(serverCharacter);
            Gameplay.Actions.Action action;
            if(GameDataSource.Instance.TryGetActionPrototypeByID(m_CardActionID, out action)){
                serverCharacter.m_CharacterAttributes.m_ActionSlots[action.m_ActionType] = action.actionID;
            }
        } 

        public void ApplyBuffs(ServerCharacter serverCharacter){
            foreach (CardBuffType buff in m_Buffs.Keys){
                switch(buff){
                    case CardBuffType.BulletSpeed:
                        serverCharacter.m_CharacterAttributes.m_BulletSpeed.Value += m_Buffs[buff]; break;
                    case CardBuffType.BulletForce:
                        serverCharacter.m_CharacterAttributes.m_BulletForce.Value += m_Buffs[buff]; break;
                    case CardBuffType.BulletDamage:
                        serverCharacter.m_CharacterAttributes.m_BulletDamage.Value += m_Buffs[buff]; break;
                    case CardBuffType.BulletSize:
                        serverCharacter.m_CharacterAttributes.m_BulletSize.Value += m_Buffs[buff]; break;
                    case CardBuffType.ClipSize:
                        serverCharacter.m_CharacterAttributes.m_ClipSize.Value += Mathf.RoundToInt(m_Buffs[buff]);
                        break;
                    case CardBuffType.MaxHealth:
                        serverCharacter.m_CharacterAttributes.m_MaxHealth.Value += Mathf.RoundToInt(m_Buffs[buff]);
                        break;
                    case CardBuffType.MoveSpeed:
                        serverCharacter.m_CharacterAttributes.m_MoveSpeed.Value += Mathf.RoundToInt(m_Buffs[buff]);
                        break;
                    case CardBuffType.ShootActionCooldown:
                        serverCharacter.m_CharacterAttributes.m_ActionCooldowns[ActionType.Shoot].Value += m_Buffs[buff];
                        break;
                    case CardBuffType.BlockActionCooldown:
                        serverCharacter.m_CharacterAttributes.m_ActionCooldowns[ActionType.Block].Value += m_Buffs[buff];
                        break;
                    default:
                        break;
                }
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
