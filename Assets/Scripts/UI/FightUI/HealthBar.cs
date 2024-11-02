
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
        void Start()
        {
            m_Slider = GetComponent<Slider>();
            m_CharacterAttributes = GetComponentInParent<ServerCharacterAttributes>();
        }
        void OnEnable()
        {
            Debug.Log("HealthBar: OnEnable");
            m_NetworkHealthState.HitPoints.OnValueChanged += UpdateHealthBar;
        }
         void OnDisable()
        {
            m_NetworkHealthState.HitPoints.OnValueChanged -= UpdateHealthBar;
        }

        private void UpdateHealthBar(int previousValue, int newValue)
        {
            m_Slider.maxValue = m_CharacterAttributes.m_MaxHealth.Value;
            Debug.Log("HealthBar: newValue" + newValue + " previousValue: " + previousValue);
            m_Slider.value = newValue;
            Debug.Log("HealthBar: m_Slider.value" + m_Slider.value);
            
        }


        // Update is called once per frame
        void Update()
        {
        }
    }
}