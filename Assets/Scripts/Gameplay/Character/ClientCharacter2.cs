// using UnityEngine;
// using Unity;
// using Unity.VisualScripting;
// using Unity.Netcode;
// namespace Monke.Gameplay.Character
// {
//     // handles the visual representation of the character on the client-side
//     public class ClientCharacter2 : MonoBehaviour
//     {
//         public void InitializeCharacterAvatar(CharacterAvatar character)
//         {
//             if(m_ClientCharacter != null) return;

//             m_ClientCharacter = character;
//             m_ClientCharacter.gameObject.SetActive(true);
//             m_ClientPlayerInput = m_ClientCharacter.GetComponentInChildren<ClientPlayerInput>();
//             m_ArmTarget = m_ClientCharacter.transform.Find("ShoulderAnchor").GetChild(0);
//             //subscribe to health state events
//             m_HealthState.HitPointsDepleted += () => m_ClientPlayerInput.SetActive(false);
//             m_HealthState.HitPointsReplenished += () => m_ClientPlayerInput.SetActive(true);
//         }
//         public void CleanUpCharacterAvatar()
//         {
//             if(m_ClientCharacter == null) return;

//             m_HealthState.HitPointsDepleted -= () => m_ClientPlayerInput.SetActive(false);
//             m_HealthState.HitPointsReplenished -= () => m_ClientPlayerInput.SetActive(true);
//         }
//     }
// }