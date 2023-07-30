using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayerAnimationBehaviour : MonoBehaviour
{
    Animator animator;
    ClientPlayerBasicMovement playerMovement;
    // Start is called before the first frame update
    void Start () {
        animator = GetComponent<Animator>();
        playerMovement = GetComponentInParent<ClientPlayerBasicMovement>();
    }
    
    // Update is called once per frame
    void Update () {

        animator.SetFloat("RunSpeedModifier",playerMovement.m_Velocity.magnitude);
    }

}
