    using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using Monke.Gameplay.Actions;
using Monke.Cards;

namespace Monke.Gameplay.Character
{
    /// <summary>
    /// getter / setter for Serialized Character Attributes. 
    /// </summary>
    public class ServerCharacterAttributes : NetworkBehaviour
    {
        public float m_BulletSpeed { get; private set; }
        public float m_BulletForce { get; private set; }
        public int m_BulletDamage { get; private set; }
        public float m_BulletSize { get; private set; }   
        public int m_ClipSize { get; private set; } 
        public int m_MaxHealth { get; private set; } 
        public float m_MoveSpeed { get; private set; }
        public Dictionary<ActionType, ActionID> m_ActionSlots;
        public Dictionary<ActionType, float> m_ActionCooldowns;
        public int m_CharacterScore { get; private set; }
        private void InitializeActionSlots(){
            var action_defaults = GameDataSource.Instance.m_DefaultActionIDs;
            m_ActionSlots = new Dictionary<ActionType, ActionID>();
            foreach( ActionID action_id in action_defaults){
                Action action = GameDataSource.Instance.GetActionPrototypeByID(action_id);
                m_ActionSlots[action.m_ActionType] = action_id;
            }
        }
        private void InitializeActionCooldowns(){

            m_ActionCooldowns = new Dictionary<ActionType, float>();
            foreach (var action in m_ActionSlots){
                Action action_prototype = GameDataSource.Instance.GetActionPrototypeByID(action.Value);
                m_ActionCooldowns[action_prototype.m_ActionType] = action_prototype.defaultCooldown;
            }
        }
        private void InitializeAttributes(){
            m_BulletSpeed = 1.0f;
            m_BulletForce = .5f;
            m_BulletDamage = 1;
            m_BulletSize = 1.0f;
            m_ClipSize = 5;
            m_MaxHealth = 30;
            m_MoveSpeed = 15.4f;
            m_CharacterScore = 0;
        }
        public void AddScore(int score){
            m_CharacterScore += score;
        }
        public void ApplyCardBuff(CardBuffType buff, float amount){
            switch(buff){
                    case CardBuffType.BulletSpeed:
                        m_BulletSpeed += amount; break;
                    case CardBuffType.BulletForce:
                        m_BulletForce += amount; break;
                    case CardBuffType.BulletDamage:
                        m_BulletDamage += Mathf.RoundToInt(amount); break;                
                    case CardBuffType.BulletSize:
                        m_BulletSize += amount; break;
                    case CardBuffType.ClipSize:
                        m_ClipSize += Mathf.RoundToInt(amount);
                        break;
                    case CardBuffType.MaxHealth:
                        m_MaxHealth += Mathf.RoundToInt(amount);
                        break;
                    case CardBuffType.MoveSpeed:
                        m_MoveSpeed += amount;
                        break;
                    case CardBuffType.ShootActionCooldown:
                        m_ActionCooldowns[ActionType.Shoot] += amount;
                        break;
                    case CardBuffType.BlockActionCooldown:
                        m_ActionCooldowns[ActionType.Block] += amount;
                        break;
                    default:
                        break;
                }
        }
        public void Awake(){
            InitializeAttributes();
            InitializeActionSlots();
            InitializeActionCooldowns();

        }

        override public void OnNetworkSpawn(){
            if (!this.IsServer){
                enabled = false;
                return;
            }
            
            // set Serialized Attributes to defaults, sync them.
            

        }
        
        override public void OnNetworkDespawn(){

        }
    }

}
   