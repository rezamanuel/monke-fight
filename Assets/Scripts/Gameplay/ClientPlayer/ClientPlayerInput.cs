using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Monke.Gameplay.Actions;
using Monke.Gameplay.Character;

using UnityEngine.Assertions;

namespace Monke.Gameplay.ClientPlayer
{
    [RequireComponent(typeof(PlayerInput))]
    public class ClientPlayerInput : NetworkBehaviour

    {
        public Vector3 m_Velocity; // should be readonly... havent figured out how to do it yet -- {get; private set} doesn't work
        public Vector3 m_MouseWorldPosition;
        [SerializeField] private float MoveSpeed = 11f;
        [SerializeField] Vector2 m_MousePosition;
        [SerializeField] Vector2 m_MovementInput;
        [SerializeField] float jumpVelocity = 11f;
        [SerializeField] float jumpPower;
        [SerializeField] float movementLerpPercent;
        [SerializeField] float gravityLerpPercent;
        [SerializeField] float jumpDecayTime;
        [SerializeField] bool isTouchingGround = true;
        [SerializeField] bool isJumping;
        [SerializeField] float gravity = 9.81f;
        [SerializeField] Transform ArmTarget; // animation target for 'aiming'
        //[SerializeField] Rigidbody rb;
        ServerCharacter m_ServerCharacter;

        [SerializeField] CapsuleCollider m_MovementCollider;
    // Start is called before the first frame update
    void Start()
        {
            jumpPower = jumpVelocity;
            m_ServerCharacter = GetComponent<ServerCharacter>();
           // rb = GetComponent<Rigidbody>();
        }

        void OnMove(InputValue value)
        {
            m_MovementInput = value.Get<Vector2>();
            //ignore if 0, we want to smooth down to 0.
            if (m_MovementInput.x == 0) return;
            m_Velocity.x = MoveSpeed * m_MovementInput.x * Time.fixedDeltaTime; //velocity per frame

        }
        void OnLook()
        {
            m_MousePosition = Mouse.current.position.ReadValue();
            m_MouseWorldPosition.z = Camera.main.nearClipPlane + 1;
            m_MouseWorldPosition = Camera.main.ScreenToWorldPoint(m_MousePosition);
            m_MouseWorldPosition.z = 0;
            if (m_MouseWorldPosition.x - transform.position.x < 0) transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);
            else transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
        void OnFire()
        {
            // will trigger whatever action ID is occupying the 1st action slot.
            ActionRequestData data = new ActionRequestData
            {
                actionID = m_ServerCharacter.m_CharacterAttributes.m_ActionSlots[0],
                m_Position = ArmTarget.position,
                m_Direction = m_MouseWorldPosition.normalized,
                m_actionType = ActionType.Shoot,
            };
            Assert.IsNotNull(GameDataSource.Instance.GetActionPrototypeByID(data.actionID),
                $"Action with actionID {data.actionID} must be contained in the Action prototypes of ActionSource!");

            m_ServerCharacter.DoActionServerRpc(data);
        }
        override public void OnNetworkSpawn()
        {
            if (!IsClient || !IsOwner)
            {
                GetComponent<PlayerInput>().enabled = false;
                enabled = false;
                // dont need to do anything else if not the owner
                return;
            }
        }
        void OnCollisionEnter(Collision c)
        {
            isTouchingGround = true;
            isJumping = false;
            m_Velocity.y = 0;
            jumpPower = jumpVelocity;
        }
        void OnCollisionExit(Collision c)
        {
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
            if (isJumping && jumpPower > 0)
            {

                m_Velocity.y = Mathf.Lerp(jumpPower, 0, jumpDecayTime * .01f);
                jumpPower -= jumpDecayTime * Time.fixedDeltaTime;

            }
            else if (!isTouchingGround)
            {
                m_Velocity.y = Mathf.Lerp(m_Velocity.y, -gravity * Time.fixedDeltaTime, Time.fixedDeltaTime * gravityLerpPercent * .01f);
            }
            transform.position += m_Velocity;
            //rb.position += m_Velocity;
        }
    }

}


