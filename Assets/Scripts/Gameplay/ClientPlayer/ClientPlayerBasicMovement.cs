using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class ClientPlayerBasicMovement : NetworkBehaviour 

{
    [SerializeField] private float MoveSpeed = 11f;
    Vector2 m_MousePosition;
    [SerializeField] Vector2 m_MovementInput;
    [SerializeField] Vector3 m_Velocity;
    [SerializeField] float jumpHeight = 11f;
    [SerializeField] float lerpPercent;
    [SerializeField] float jumpDecay;
    [SerializeField] bool isTouchingGround=true;
    [SerializeField] bool isJumping;
    [SerializeField] float gravity = 9.81f;

    [SerializeField] CapsuleCollider m_MovementCollider;
    // Start is called before the first frame update
    void Start()
    {
        if(!IsOwner) enabled = false;
    }

    void OnMove(InputValue value){
        m_MovementInput = value.Get<Vector2>();
        //ignore if 0, we want to smooth down to 0.
        if(m_MovementInput.x == 0)return;
        m_Velocity.x =  MoveSpeed * m_MovementInput.x * Time.fixedDeltaTime; //velocity per frame
    }
    void OnLook(){
        m_MousePosition += Mouse.current.position.ReadValue();
    }
    void OnCollisionEnter(Collision c){
        isTouchingGround = true;
        isJumping = false;
        m_Velocity.y = 0;
        Debug.Log("homola homoula");
    }void OnCollisionExit(Collision c){
        isTouchingGround = false;
    }
    void OnJump(InputValue value){
        if(!isTouchingGround && !isJumping) return;
        Debug.Log("FUCKEN EY");
        isJumping = value.isPressed;
    }
    void FixedUpdate()
    {
        if(!IsOwner) return;
        if(m_MovementInput.x == 0) 
            if(Mathf.Abs(m_Velocity.x) < .01f ){
                m_Velocity.x = 0;
            }
            else
            {
                m_Velocity.x = Mathf.Lerp(m_Velocity.x, 0, lerpPercent * .01f);
            }
        if(isJumping)
            m_Velocity.y = Mathf.Lerp(jumpHeight * Time.fixedDeltaTime,0, jumpDecay*.01f);
        else{
            m_Velocity.y = Mathf.Lerp(m_Velocity.y, -gravity*Time.fixedDeltaTime,jumpDecay*.01f);
        }
        transform.position += m_Velocity;
    }
}
