using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using unityEvent = UnityEngine.Events.UnityEvent;

public class DelayCall : NetworkBehaviour
{
    [SerializeField] public unityEvent m_Event;
    [SerializeField] public float m_Delay = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayedCall());
    }

    private IEnumerator DelayedCall()
    {
        yield return new WaitForSeconds(m_Delay);
        m_Event.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
