using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Monke.Gameplay.Character;

namespace Monke.Gameplay.Actions
{
[CreateAssetMenu(menuName = "Monke/Actions/ShootAction")]
    public class ShootAction : Action{

        [SerializeField] GameObject bulletPrefab;
        [SerializeField] string animationName;

        public override void End()
        {
            throw new System.NotImplementedException();
        }

        public override void OnStart(ServerCharacter serverCharacter){
            
        }
        public override void OnUpdate (ClientCharacter clientCharacter)
        {

        }
    }
} 