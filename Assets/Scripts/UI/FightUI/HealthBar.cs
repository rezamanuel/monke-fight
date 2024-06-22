
using UnityEngine;
using Unity.Netcode;
using Monke.Gameplay.Character;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;

namespace Monke.UI
{
    public class HealthBar : NetworkBehaviour
    {
        [SerializeField] NetworkHealthState m_NetworkHealthState;
        [SerializeField] Slider m_Slider;
        [SerializeField] ServerCharacter m_ServerCharacter;

        // Start is called before the first frame update
        void Awake(){
            m_Slider = GetComponent<Slider>();
            m_NetworkHealthState = GetComponentInParent<NetworkHealthState>();
            m_ServerCharacter = GetComponentInParent<ServerCharacter>();
            m_NetworkHealthState.HitPoints.OnValueChanged += UpdateHealthBar;
        }
        void OnEnable()
        {
            
        }
         void OnDisable()
        {
            m_NetworkHealthState.HitPoints.OnValueChanged -= UpdateHealthBar;
        }

        private void UpdateHealthBar(int previousValue, int newValue)
        {
            m_Slider.value = newValue;
        }

        [ClientRpc] void InitializeHealthBarClientRpc(int maxHealth){
            m_Slider.maxValue = maxHealth;
            m_Slider.value = maxHealth;
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}