    using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using Monke.Gameplay.Actions;

namespace Monke.Gameplay.Character
{
    /// <summary>
    /// getter / setter for Serialized Character Attributes. 
    /// </summary>
    public class ServerCharacterAttributes : NetworkBehaviour
    {
        public NetworkVariable<float> m_BulletSpeed { get; private set; }
        public NetworkVariable<float> m_BulletForce { get; private set; }
        public NetworkVariable<float> m_BulletDamage { get; private set; }
        public NetworkVariable<float> m_BulletSize { get; private set; }   
        public NetworkVariable<int> m_ClipSize { get; private set; } 
        public NetworkVariable<int> m_MaxHealth { get; private set; } 
        public NetworkVariable<int> m_MoveSpeed { get; private set; }
        public Dictionary<ActionType, ActionID> m_ActionSlots;
        public Dictionary<ActionType, NetworkVariable<float>> m_ActionCooldowns;

        private void AssignActionSlots(){
            var action_defaults = GameDataSource.Instance.m_DefaultActionIDs;
            foreach( ActionID action_id in action_defaults){
                Action action = GameDataSource.Instance.GetActionPrototypeByID(action_id);
                m_ActionSlots[action.Data.m_actionType] = action_id;
            }
        }
        private void InitializeActionCooldowns(){
            foreach (var action in m_ActionSlots){
                Action action_prototype = GameDataSource.Instance.GetActionPrototypeByID(action.Value);
                m_ActionCooldowns[action_prototype.m_ActionType] = new NetworkVariable<float>(action_prototype.defaultCooldown);
            }
        }

        override public void OnNetworkSpawn(){
            if (!this.IsServer){
                enabled = false;
                return;
            }
            AssignActionSlots();
            InitializeActionCooldowns();
            // set Serialized Attributes to defaults, sync them.
            

        }
        
        override public void OnNetworkDespawn(){

        }
    }

}
   