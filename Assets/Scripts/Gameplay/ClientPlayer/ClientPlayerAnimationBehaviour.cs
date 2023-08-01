using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Monke.Gameplay.ClientPlayer;

public class ClientPlayerAnimationBehaviour : NetworkBehaviour
{
    Animator animator;
    ClientPlayerInput playerMovement;
    // Start is called before the first frame update
    void Start () {
        
        animator = GetComponent<Animator>();
        playerMovement = GetComponentInParent<ClientPlayerInput>();
    }
    public override void OnNetworkSpawn(){
        if(!IsOwner){
            this.enabled = false;
        }
    }
    // Update is called once per frame
    void Update () {

        animator.SetFloat("RunSpeedModifier",playerMovement.m_Velocity.magnitude);
    }

}
