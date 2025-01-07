using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Monke.Gameplay.Actions;
using Monke.Gameplay.Character;

using UnityEngine.Assertions;
using Unity.VisualScripting;

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
        float m_JumpAssistAirborne;
        bool m_jumpFlag;
        bool m_jumpFlagIsHeld = false;
        [SerializeField] bool isMouseInput = true;

        [SerializeField] float movementLerpPercent;
        [SerializeField] Transform shootOrigin;
        ServerCharacter m_ServerCharacter;
        public PlayerController m_PlayerController;

        public void SetEnabled(bool enabled)
        {
            if (!IsClient && !IsOwner) return;
            base.enabled = enabled;
            m_PlayerController.enabled = enabled;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_ServerCharacter = GetComponent<ServerCharacter>();
            m_PlayerController = GetComponent<PlayerController>();
            m_Gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            m_JumpVelocity = Mathf.Abs(m_Gravity) * timeToJumpApex;
            m_JumpAssistAirborne = m_JumpVelocity / 6;
            print("Gravity: " + m_Gravity + "  Jump Velocity: " + m_JumpVelocity + "  Jump Assist: " + m_JumpAssistAirborne);
        }
        void OnMove(InputValue value)
        {
            if (enabled == false) return;
            m_MovementInput = value.Get<Vector2>();
            //ignore if 0, we want to smooth down to 0.
            if (m_MovementInput.x == 0) return;
        }
        void OnLook(InputValue value)
        {
            if (enabled == false) return;
            if(value.Get<Vector2>().magnitude == 0) return;
            if(value.Get<Vector2>() == Mouse.current.position.ReadValue())
            {
                m_MousePosition = Mouse.current.position.ReadValue();
                m_MouseWorldPosition.z = Camera.main.nearClipPlane + 1;
                m_MouseWorldPosition = Camera.main.ScreenToWorldPoint(m_MousePosition);
                m_MouseWorldPosition.z = 0;
                if (m_MouseWorldPosition.x - transform.position.x < 0) transform.GetChild(0).rotation = Quaternion.AngleAxis(90f, Vector3.up);
                else transform.GetChild(0).rotation = Quaternion.AngleAxis(270f, Vector3.up);
                isMouseInput = true;
            }
            else
            {
                m_MousePosition = value.Get<Vector2>().normalized;
                m_MouseWorldPosition.z = this.transform.position.z;
                isMouseInput = false;
                if (m_MouseWorldPosition.x - transform.position.x < 0) transform.GetChild(0).rotation = Quaternion.AngleAxis(90f, Vector3.up);
                else transform.GetChild(0).rotation = Quaternion.AngleAxis(270f, Vector3.up);
            }

        }
        void OnFire()
        {
            if (enabled == false) return;
            // will trigger whatever action ID is occupying the 1st action slot.
            ActionRequestData data = new ActionRequestData
            {
                actionID = m_ServerCharacter.m_CharacterAttributes.m_ActionSlots[0],
                m_Position = shootOrigin.position,
                m_Direction = (m_MouseWorldPosition - shootOrigin.position).normalized,
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
                enabled = false;
                //disable input for non-owning clients
                GetComponent<PlayerInput>().enabled = false;
                return;
            }


        }

        void OnJump(InputValue value)
        {
            if (!enabled) return;
            m_jumpFlag = value.isPressed;
        }
        void OnGUI()
        {
            Debug.DrawLine(transform.position, m_MouseWorldPosition, Color.red);    // Draw a red line in the Scene view
        }
        void Update()
        {
            if (m_PlayerController.collisions.above || m_PlayerController.collisions.below)
            {
                m_Velocity.y = 0;
            }

            float targetVelocityX = m_MovementInput.x * m_MoveSpeed;
            if (m_MovementInput.x == 0)
                if (Mathf.Abs(m_Velocity.x) < .01f)
                {
                    m_Velocity.x = 0;
                }
                else
                {
                    m_Velocity.x = Mathf.Lerp(m_Velocity.x, 0, movementLerpPercent * .01f);
                }
            else m_Velocity.x = targetVelocityX;
            if (m_jumpFlag)
            {
                if ((m_PlayerController.collisions.below || m_PlayerController.collisions.left || m_PlayerController.collisions.right) && m_jumpFlagIsHeld == false)
                { 
                    m_Velocity.y = m_JumpVelocity;
                }
                else{
                    m_Velocity.y += m_JumpAssistAirborne * Time.fixedDeltaTime;
                    m_jumpFlagIsHeld = true;
                }
            }
            else{
                m_jumpFlagIsHeld = false;
            }
            m_Velocity.y += m_Gravity * Time.deltaTime;
            m_PlayerController.inputVelocity = m_Velocity;
            if (!isMouseInput)
            {
                m_MouseWorldPosition.x = shootOrigin.position.x + m_MousePosition.x * 5f;
                m_MouseWorldPosition.y = shootOrigin.position.y + m_MousePosition.y * 5f;
            }
        }

    }
}


