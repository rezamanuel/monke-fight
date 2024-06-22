using UnityEngine;

namespace Monke.Gameplay
{
    [CreateAssetMenu(menuName = "Monke/DefaultAttributeSet")]
    public class DefaultAttributeSet : ScriptableObject
    {
        public float m_BulletSpeed = 1.0f;
        public float m_BulletForce = .5f;
        public int m_BulletDamage = 1;
        public float m_BulletSize = 1.0f;
        public int m_ClipSize = 5;
        public int m_MaxHealth = 30;
        public float m_MoveSpeed = 15.4f;
        public int m_CharacterScore = 0;
    }
}