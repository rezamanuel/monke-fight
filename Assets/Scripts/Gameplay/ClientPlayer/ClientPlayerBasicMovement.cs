using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]
public class ClientPlayerBasicMovement : NetworkBehaviour 

{
    [SerializeField] private float MoveSpeed = 11f;
    [SerializeField] Vector2 m_MousePosition;
    [SerializeField] Vector3 m_MouseWorldPosition;
    [SerializeField] Vector2 m_MovementInput;
    [SerializeField] Vector3 m_Velocity;
    [SerializeField] float jumpVelocity = 11f;
    [SerializeField] float jumpPower;
    [SerializeField] float movementLerpPercent;
    [SerializeField] float gravityLerpPercent;
    [SerializeField] float jumpDecayTime;
    [SerializeField] bool isTouchingGround=true;
    [SerializeField] bool isJumping;
    [SerializeField] float gravity = 9.81f;

    [SerializeField] CapsuleCollider m_MovementCollider;
    // Start is called before the first frame update
    void Start()
    {
        jumpPower = jumpVelocity;
    }

    void OnMove(InputValue value){
        m_MovementInput = value.Get<Vector2>();
        //ignore if 0, we want to smooth down to 0.
        if(m_MovementInput.x == 0)return;
        m_Velocity.x =  MoveSpeed * m_MovementInput.x * Time.fixedDeltaTime; //velocity per frame
    }
    void OnLook(){
        m_MousePosition= Mouse.current.position.ReadValue();
    }
    void OnFire(){
        m_MouseWorldPosition = m_MousePosition;
        m_MouseWorldPosition.z = Camera.main.nearClipPlane + 1;
        m_MouseWorldPosition = Camera.main.ScreenToWorldPoint(m_MouseWorldPosition);
        m_MouseWorldPosition.z = 0;
    }
    override public void OnNetworkSpawn(){
        if (!IsClient || !IsOwner)
            {
                GetComponent<PlayerInput>().enabled = false;
                enabled = false;
                // dont need to do anything else if not the owner
                return;
            }
    }
    void OnCollisionEnter(Collision c){
        isTouchingGround = true;
        isJumping = false;
        m_Velocity.y = 0;
        jumpPower = jumpVelocity;
    }void OnCollisionExit(Collision c){
        isTouchingGround = false;
    }
    void OnJump(InputValue value)
    {
        if (!isTouchingGround && !isJumping) return;
        else
        {
            isJumping = value.isPressed;
        }

    }
    void FixedUpdate()
    {
        if (m_MovementInput.x == 0)
            if (Mathf.Abs(m_Velocity.x) < .01f)
            {
                m_Velocity.x = 0;
            }
            else
            {
                m_Velocity.x = Mathf.Lerp(m_Velocity.x, 0, movementLerpPercent * .01f);
            }
        if (isJumping && jumpPower > 0){
            
            m_Velocity.y = Mathf.Lerp(jumpPower, 0, jumpDecayTime * .01f);
            jumpPower -= jumpDecayTime * Time.fixedDeltaTime;
            
        }
        else if(!isTouchingGround){
            m_Velocity.y = Mathf.Lerp(m_Velocity.y, -gravity*Time.fixedDeltaTime,Time.fixedDeltaTime*gravityLerpPercent*.01f);
        }
        transform.position += m_Velocity;
    }
}
