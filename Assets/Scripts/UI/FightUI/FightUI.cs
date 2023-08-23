using System.Collections;
using UnityEngine;
using Monke.GameState;
using Unity.Netcode;
using Unity.VisualScripting;
using Monke.Networking;

namespace Monke.UI
{
    public class FightUI : NetworkBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] GameObject m_HealthBarTemplate;

        void Start()
        {
            
        }

        void Update()
        {

        }
    }
}


