using UnityEngine;
using Unity;
using Unity.VisualScripting;
using Unity.Netcode;

namespace Monke.Gameplay.Character
{
    
    public class ClientCharacter : MonoBehaviour
    {
        [SerializeField] NetworkHealthState m_NetworkHealthState; 
        [SerializeField] Transform m_ClientMonkeTransform;
        [SerializeField] Object ragdollMonkeTemplate;
        void Awake(){
            m_NetworkHealthState = GetComponentInParent<NetworkHealthState>();
        }
        void OnEnable(){
            m_NetworkHealthState.HitPointsDepleted += VisualizeDeath;
        }
        void OnDisable(){
            m_NetworkHealthState.HitPointsDepleted += VisualizeDeath;
        }
        void VisualizeDeath(){
            
            Instantiate(ragdollMonkeTemplate, this.transform.parent);
            m_ClientMonkeTransform.gameObject.SetActive(false);
        }
    }
}