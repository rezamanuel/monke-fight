using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Monke.Gameplay.ClientPlayer;
public class ClientPlayerAnimationBehaviour : NetworkBehaviour
{
    Animator animator;
    PlayerController playerMovement;
    [SerializeField] float RunSpeedModifierRatio = 1f;
    // Start is called before the first frame update
    void Start () {
        
        animator = GetComponent<Animator>();
        playerMovement = GetComponentInParent<PlayerController>();
    }
    public override void OnNetworkSpawn(){
        if(!IsOwner){
            this.enabled = false;
        }
    }
    // Update is called once per frame
    void Update () {
        animator.SetFloat("RunSpeedModifier",Mathf.Abs(playerMovement.actualVelocity.x)*RunSpeedModifierRatio*Mathf.Sin(transform.parent.rotation.eulerAngles.y));
    }

}
