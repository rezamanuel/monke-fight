using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    /// <summary>
    /// Script for having a gameobject match the mouse position according to PlayerInput;
    /// </summary>
    public ClientPlayerBasicMovement playerMovement;
    void Awake()
    {
        GetComponentInParent<ClientPlayerBasicMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.ClampMagnitude((playerMovement.m_MouseWorldPosition - transform.parent.position),1f) + transform.parent.position;

    }
}
