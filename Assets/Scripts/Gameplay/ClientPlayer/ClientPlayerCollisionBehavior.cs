using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayerCollisionBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] CapsuleCollider playerCollider;
    [SerializeField] CapsuleCollider playerCollisionBlocker;
    void Start()
    {
        Physics.IgnoreCollision(playerCollider, playerCollisionBlocker, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
