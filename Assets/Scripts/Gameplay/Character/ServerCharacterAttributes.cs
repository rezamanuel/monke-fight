    using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using Monke.Gameplay.Actions;
using Monke.Cards;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Monke.Gameplay.Character
{
    /// <summary>
    /// getter / setter for Serialized Character Attributes. 
    /// </summary>
    public class ServerCharacterAttributes : NetworkBehaviour
    {
        public NetworkVariable<float> m_BulletSpeed { get; private set; }
        public NetworkVariable<float> m_BulletForce { get; private set; }
        public NetworkVariable<int> m_BulletDamage { get; private set; }
        public NetworkVariable<float> m_BulletSize { get; private set; }   
        public NetworkVariable<int> m_ClipSize { get; private set; } 
        public NetworkVariable<int> m_MaxHealth { get; private set; } 
        public NetworkVariable<float> m_MoveSpeed { get; private set; }
        public NetworkVariable<int> m_CharacterScore { get; private set; }

        // These 2 synced lists share the index of the action referenced. actionslots[0] has cooldowns[0]
        public NetworkList<ActionID> m_ActionSlots;
        public NetworkList<float> m_ActionCooldowns;
        public DefaultAttributeSet m_DefaultAttributeSet;

        public float GetCooldown(ActionType actionType)
        {
            int index = actionType == ActionType.Shoot ? 0 : 1;
            return m_ActionCooldowns[index];
        }
        private void InitializeActionSlots()
        {
            var actionDefaults = GameDataSource.Instance.m_DefaultActionIDs;
            if (actionDefaults == null)
            {
                Debug.LogError("DefaultActionIDs is null");
                return;
            }
            
            foreach (ActionID actionId in actionDefaults)
            {
                Action action = GameDataSource.Instance.GetActionPrototypeByID(actionId);
                if (action != null)
                {
                    m_ActionSlots.Add(actionId);
                }
                else
                {
                    Debug.LogError($"Action prototype for ID {actionId.ID} is null");
                }
            }
        }
        private void InitializeActionCooldowns(){
            foreach (var action_id in m_ActionSlots){
                Action action_prototype = GameDataSource.Instance.GetActionPrototypeByID(action_id);
                m_ActionCooldowns.Add(action_prototype.defaultCooldown);
            }
        }
        private void InitializeAttributes(){
            m_BulletSpeed = new NetworkVariable<float>(m_DefaultAttributeSet.m_BulletSpeed);
            m_BulletForce = new NetworkVariable<float>(m_DefaultAttributeSet.m_BulletForce);
            m_BulletDamage = new NetworkVariable<int>(m_DefaultAttributeSet.m_BulletDamage);
            m_BulletSize = new NetworkVariable<float>(m_DefaultAttributeSet.m_BulletSize);
            m_ClipSize = new NetworkVariable<int>(m_DefaultAttributeSet.m_ClipSize);
            m_MaxHealth = new NetworkVariable<int>(m_DefaultAttributeSet.m_MaxHealth);
            m_MoveSpeed = new NetworkVariable<float>(m_DefaultAttributeSet.m_MoveSpeed);
            m_CharacterScore = new NetworkVariable<int>(m_DefaultAttributeSet.m_CharacterScore);
            m_ActionCooldowns = new NetworkList<float>();
            m_ActionSlots = new NetworkList<ActionID>();
        }
        public void AddScore(int score){
            m_CharacterScore.Value += score;
        }
        public void ApplyCardBuff(CardBuffType buff, float amount){
            switch(buff){
                case CardBuffType.BulletSpeed:
                    m_BulletSpeed.Value += amount; 
                    break;
                case CardBuffType.BulletForce:
                    m_BulletForce.Value += amount; 
                    break;
                case CardBuffType.BulletDamage:
                    m_BulletDamage.Value += Mathf.RoundToInt(amount); 
                    break;                
                case CardBuffType.BulletSize:
                    m_BulletSize.Value += amount; 
                    break;
                case CardBuffType.ClipSize:
                    m_ClipSize.Value += Mathf.RoundToInt(amount);
                    break;
                case CardBuffType.MaxHealth:
                    m_MaxHealth.Value += Mathf.RoundToInt(amount);
                    break;
                case CardBuffType.MoveSpeed:
                    m_MoveSpeed.Value += amount;
                    break;
                case CardBuffType.ShootActionCooldown:
                    m_ActionCooldowns[0] += amount; // hardcoded to 0 for now   
                    break;
                case CardBuffType.BlockActionCooldown:
                    m_ActionCooldowns[1] += amount; // hardcoded to 1 for now
                    break;
                default:
                    break;
            }
        }
        private void Awake(){
            InitializeAttributes();
            

        }
        void Start(){
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
   