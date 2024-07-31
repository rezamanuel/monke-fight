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
        [SerializeField] Rigidbody[] m_Rigidbodies;

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
            m_ClientMonkeTransform.GetComponent<Animator>().enabled = false;
            foreach(Rigidbody rb in m_Rigidbodies){
                rb.isKinematic = false;
            }
            StartCoroutine(Respawn());
        }

        IEnumerator Respawn(){
            yield return new WaitForSeconds(2);
            foreach(Rigidbody rb in m_Rigidbodies){
                rb.isKinematic = true;
            }
            m_ClientMonkeTransform.GetComponent<Animator>().enabled = true;
        }
    }
}