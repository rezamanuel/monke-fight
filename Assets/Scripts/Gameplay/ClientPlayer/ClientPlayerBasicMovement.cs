using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientPlayerBasicMovement : NetworkBehaviour
{
    [SerializeField] private float MoveSpeed = 6f;
    
    private Vector3 moveDir;
    // Start is called before the first frame update
    void Start()
    {
        if(!IsOwner) enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = new Vector3 (0,0,0);
        moveDir.x = Input.GetAxisRaw("Horizontal");
        transform.position += MoveSpeed* moveDir * Time.deltaTime;
    }
}
