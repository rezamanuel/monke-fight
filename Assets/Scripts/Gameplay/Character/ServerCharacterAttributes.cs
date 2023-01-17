    using UnityEngine;

namespace Monke.Gameplay.Character
{
    public class ServerCharacterAttributes : ScriptableObject 

    // These attributes are baseline stats for each player, and get modified at runtime to reflect the bonuses from certain actions; IE: shootBullet offers no buffs, but Speedball offers +1 bulletSpeed, etc.
    {
        public float bulletSpeed;
        public float bulletForce;
        public float bulletDamage;
        public float bulletSize;
        public float fireRate;

        public float reloadSpeed;
        public int clipSize;
        public int maxHealth;
        public int moveSpeed;
    }
}