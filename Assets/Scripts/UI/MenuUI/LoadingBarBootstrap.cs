using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarBootstrap : MonoBehaviour
{
    private Slider slider;
    [SerializeField] Systems.ApplicationController bootstrap;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = bootstrap.getLoadProgress();
        if(slider.value == 1){
            Destroy(this.gameObject);
        }
    }
}
