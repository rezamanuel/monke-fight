using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientPlayerAnimationBehaviour : NetworkBehaviour
{
    Animator animator;
    ClientPlayerBasicMovement playerMovement;
    // Start is called before the first frame update
    void Start () {
        
        animator = GetComponent<Animator>();
        playerMovement = GetComponentInParent<ClientPlayerBasicMovement>();
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
