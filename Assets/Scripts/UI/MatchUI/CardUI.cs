using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Monke.Cards;
using System;
namespace Monke.UI
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] public int m_CardIndex { get; private set; }
        [SerializeField] Transform m_HoverOutline;
        [SerializeField] Card m_Card;
        public event Action<int> OnHover;
        public event Action<int> OnClick; //Informs CardPanelGraphics a card has been clicked.z

        public void SetCardIndex(int i){
            m_CardIndex = i;
        }
        public void InvokeOnHover(){
            OnHover?.Invoke(m_CardIndex);
        }
        public void InvokeOnClick(){
        
            OnClick?.Invoke(m_CardIndex);

        }
        public Card GetCard(){
            return m_Card;
        }
        public void DisplayHoverGraphics(bool isOutlined){
            m_HoverOutline.gameObject.SetActive(isOutlined);
        }
        public void DisplayClickGraphics(){
            m_HoverOutline.gameObject.SetActive(true);
            m_HoverOutline.gameObject.GetComponent<Image>().color = Color.green;
        }

    }

}