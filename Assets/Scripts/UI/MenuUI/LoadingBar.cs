using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monke.UI
{
    public class LoadingBar : MonoBehaviour
    {
        private Slider slider;
        // Start is called before the first frame update
        void Start()
        {
            slider = GetComponentInChildren<Slider>();
        }

        // Update is called once per frame
        public void SetValue(int value)
        {
            slider.value = value;
        }
    }

}
