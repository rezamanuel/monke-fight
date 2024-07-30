using UnityEngine;
using Unity;
using Unity.VisualScripting;
using Unity.Netcode;
using System.Runtime.CompilerServices;
using System.Collections;
namespace Monke.Gameplay.Character
{
    // handles the visual representation of the character on the client-side
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
            m_NetworkHealthState.HitPointsDepleted -= VisualizeDeath;
        }
        void VisualizeDeath(){
            
            Instantiate(ragdollMonkeTemplate, this.transform.parent  );
            m_ClientMonkeTransform.gameObject.SetActive(false);
            StartCoroutine(Respawn());
        }

        IEnumerator Respawn(){
            yield return new WaitForSeconds(5);
            m_ClientMonkeTransform.gameObject.SetActive(true);
            // Destroy(this.transform.parent.Find("Ragdoll(Clone)").gameObject);
            
        }
    }
}