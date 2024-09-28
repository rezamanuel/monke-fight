using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Monke.Gameplay.ClientPlayer;
using Monke.Gameplay.Character;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
public class ClientPlayerAnimationBehaviour : NetworkBehaviour
{
    Animator animator;
    [SerializeField] Transform playerTransform;
    UnityEngine.Vector3 clientLastFramePosition;
    [SerializeField] float xspeed;
    [SerializeField] float RunSpeedModifierRatio = 1f;
    // Start is called before the first frame update
    void Start () {
         
        animator = GetComponent<Animator>();
        clientLastFramePosition = playerTransform.position;
        
    }
    public override void OnNetworkSpawn(){
        if(!IsOwner){
            enabled = false;
        }
    }
    // Update is called once per frame
    void FixedUpdate () {
        xspeed = Mathf.Abs(playerTransform.position.x - clientLastFramePosition.x)/Time.fixedDeltaTime;
        animator.SetFloat("RunSpeedModifier",xspeed*RunSpeedModifierRatio);
        clientLastFramePosition = playerTransform.position;
    }

}
