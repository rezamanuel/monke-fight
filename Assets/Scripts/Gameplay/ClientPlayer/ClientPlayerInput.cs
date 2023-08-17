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
    [RequireComponent(typeof(PlayerController))]
    public class ClientPlayerInput : NetworkBehaviour

    {
        public Vector3 m_Velocity; // should be readonly... havent figured out how to do it yet -- {get; private set} doesn't work
        public Vector3 m_MouseWorldPosition;
        public Transform m_ArmTarget; // animation target for 'aiming'
        [SerializeField] Vector2 m_MousePosition;
        [SerializeField] Vector2 m_MovementInput;
        public float jumpHeight = 4;
        public float timeToJumpApex = .4f;
        float m_AccelerationTimeAirborne = .2f;
        float m_AccelerationTimeGrounded = .1f;
        [SerializeField] float m_MoveSpeed = 6;

        float m_Gravity;
        float m_JumpVelocity;
        float m_VelocityXSmoothing;
        bool m_jumpFlag;

        [SerializeField] float movementLerpPercent;
        [SerializeField] Transform shootOrigin;
        ServerCharacter m_ServerCharacter;
        public PlayerController m_PlayerController;

        // Start is called before the first frame update
        void Start()
        {
            m_ServerCharacter = GetComponent<ServerCharacter>();
            m_PlayerController = GetComponent<PlayerController>();

            m_Gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            m_JumpVelocity = Mathf.Abs(m_Gravity) * timeToJumpApex;
            print("Gravity: " + m_Gravity + "  Jump Velocity: " + m_JumpVelocity);
        }

        void OnMove(InputValue value)
        {
            m_MovementInput = value.Get<Vector2>();
            //ignore if 0, we want to smooth down to 0.
            if (m_MovementInput.x == 0) return;
        }
        void OnLook()
        {
            m_MousePosition = Mouse.current.position.ReadValue();
            m_MouseWorldPosition.z = Camera.main.nearClipPlane + 1;
            m_MouseWorldPosition = Camera.main.ScreenToWorldPoint(m_MousePosition);
            m_MouseWorldPosition.z = 0;
            if (m_MouseWorldPosition.x - transform.position.x < 0) transform.GetChild(0).rotation = Quaternion.AngleAxis(90f, Vector3.up);
            else transform.GetChild(0).rotation = Quaternion.AngleAxis(270f, Vector3.up);
        }
        void OnFire()
        {
            // will trigger whatever action ID is occupying the 1st action slot.
            ActionRequestData data = new ActionRequestData
            {
                actionID = m_ServerCharacter.m_CharacterAttributes.m_ActionSlots[0],
                m_Position = shootOrigin.position,
                m_Direction = (m_MouseWorldPosition-shootOrigin.position).normalized,
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
        void OnJump(InputValue value)
        {
            m_jumpFlag = value.isPressed;
        }
        void FixedUpdate()
        {
            if (m_PlayerController.collisions.above || m_PlayerController.collisions.below)
            {
                m_Velocity.y = 0;
            }

           

            if (m_jumpFlag && m_PlayerController.collisions.below)
            {
                m_Velocity.y = m_JumpVelocity;
            }

            float targetVelocityX = m_MovementInput.x * m_MoveSpeed;
            if (m_MovementInput.x == 0)
                if (Mathf.Abs(m_Velocity.x) < .01f)
                {
                    m_Velocity.x = 0;
                }
                else
                {
                    m_Velocity.x = Mathf.Lerp(m_Velocity.x, 0, movementLerpPercent*.01f);
                }
            else m_Velocity.x = targetVelocityX;
            m_Velocity.y += m_Gravity * Time.deltaTime;
            m_PlayerController.Move(m_Velocity * Time.deltaTime);
        }
    }

}


