
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
        [SerializeField] ServerCharacterAttributes m_CharacterAttributes;

        // Start is called before the first frame update
        void Awake(){
            m_Slider = GetComponent<Slider>();
            m_NetworkHealthState = GetComponentInParent<NetworkHealthState>();
        }
        void OnEnable()
        {
            
            m_NetworkHealthState.HitPoints.OnValueChanged += UpdateHealthBar;
        }
         void OnDisable()
        {
            m_NetworkHealthState.HitPoints.OnValueChanged -= UpdateHealthBar;
        }

        private void UpdateHealthBar(int previousValue, int newValue)
        {
            Debug.Log("HealthBar: " + newValue);
            m_Slider.value = newValue;
            m_Slider.maxValue = m_CharacterAttributes.m_MaxHealth.Value;
        }


        // Update is called once per frame
        void Update()
        {
        }
    }
}