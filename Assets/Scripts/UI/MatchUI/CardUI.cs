using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
namespace Monke.UI
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] public int CardIndex{ get; private set; }

        public event Action<int> OnHover;
        public event Action<int> OnClick;
        public void SetCardIndex(int i){
            CardIndex = i;
        }

        public void InvokeOnHover(){
            OnHover?.Invoke(CardIndex);
            Debug.Log("invokeonhover");
        }
        public void InvokeOnClick(){
            OnClick?.Invoke(CardIndex);
        }

    }

}