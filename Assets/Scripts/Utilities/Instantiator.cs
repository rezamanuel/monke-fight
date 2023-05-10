using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Instantiator : NetworkBehaviour
{
    [SerializeField] public GameObject m_GameObjectToSpawn;
    [SerializeField] public GameObject m_SpawnButton;
    // Start is called before the first frame update
    void Start()
    {
        if(!IsOwner){
            m_SpawnButton.SetActive(false);
            enabled = false;
        }
    }

    public void Instantiate(){
        Debug.Log("instantiated");
        ulong client_id = NetworkManager.LocalClientId;
        GameObject go = NetworkManager.Instantiate(m_GameObjectToSpawn) as GameObject;
        Rigidbody rb = go.GetComponent<Rigidbody>();
        go.GetComponent<NetworkObject>().SpawnAsPlayerObject(client_id);
        rb.isKinematic = false;
    }
    public void hideUI(){
        m_SpawnButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
