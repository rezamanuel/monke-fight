using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monke.Gameplay.ClientPlayer;
public class FollowMouse : MonoBehaviour
{
    /// <summary>
    /// Script for having a gameobject match the mouse position according to PlayerInput;
    /// </summary>
    public ClientPlayerInput playerMovement;
    void Awake()
    {
        GetComponentInParent<ClientPlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.ClampMagnitude((playerMovement.m_MouseWorldPosition - transform.parent.position),1f) + transform.parent.position;

    }
}
