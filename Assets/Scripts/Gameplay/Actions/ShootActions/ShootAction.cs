using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Monke.Gameplay.Character;
using Monke.Projectiles;
using Monke.Infrastructure;
using Monke.Gameplay.ClientPlayer;
using Unity.VisualScripting;

namespace Monke.Gameplay.Actions
{
[CreateAssetMenu(menuName = "Monke/Actions/ShootAction")]
    public class ShootAction : Action{

        [SerializeField] GameObject bulletPrefab;
        [SerializeField] string animationName;
        float bulletSize;

        public override void End()
        {
            isActive = false;
        }

        public override void OnStart(ServerCharacter serverCharacter){
           
            NetworkObject basicBullet_no =  NetworkObjectPool.Singleton.GetNetworkObject(bulletPrefab);
            basicBullet_no.transform.SetPositionAndRotation(serverCharacter.m_ArmTarget.position, Quaternion.identity);
            basicBullet_no.Spawn(true);
            BasicBullet basicBullet = basicBullet_no.GetComponent<BasicBullet>();
            var characterAttributes = serverCharacter.m_CharacterAttributes;
            basicBullet.Initialize(serverCharacter.OwnerClientId,
             characterAttributes.m_BulletSpeed, 
             characterAttributes.m_BulletForce,
              characterAttributes.m_BulletDamage,
               characterAttributes.m_BulletSize,
               m_Data.m_Direction);
            End();
        }

        public override void OnUpdate(ServerCharacter clientCharacter)
        {
            
            return;
        }
    }
} 