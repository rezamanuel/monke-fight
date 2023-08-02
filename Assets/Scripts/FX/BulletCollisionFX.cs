using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollisionFX : MonoBehaviour
{
    Collider collider;
    [SerializeField]ParticleSystem particleSystem;
    // Start is called before the first frame update
    void Awake(){
        collider = this.GetComponent<Collider>();
        particleSystem = this.GetComponent<ParticleSystem>();
    }
    void OnCollisionEnter(){
//particleSystem.Play();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
