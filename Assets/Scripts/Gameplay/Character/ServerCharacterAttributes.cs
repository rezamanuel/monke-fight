    using UnityEngine;
using System.Collections.Generic;
using Monke.Gameplay.Actions;

namespace Monke.Gameplay.Character
{
    public class ServerCharacterAttributes : ScriptableObject 

    // These attributes are baseline stats for each player, and get modified at runtime to reflect the bonuses from certain actions; IE: shootBullet offers no buffs, but Speedball offers +1 bulletSpeed, etc.
    {
        
        public Dictionary<ActionType, float> actionCooldowns;
        public float bulletSpeed;
        public float bulletForce;
        public float bulletDamage;
        public float bulletSize;

        public Action actionPrototype1; // default shoot
        public Action actionPrototype2; // default block
        public Action actionPrototype3;
        public int clipSize;
        public int maxHealth;
        public int moveSpeed;
    }
}