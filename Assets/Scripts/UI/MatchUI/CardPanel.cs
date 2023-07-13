using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Monke.GameState;
using Unity.Netcode;
using Monke.Cards;
namespace Monke.UI
{
    public class CardPanel : NetworkBehaviour
    {
        // Start is called before the first frame update

        [SerializeField] RectTransform leftAnchor;
        [SerializeField] RectTransform RightAnchor;
        [SerializeField] List<GameObject> m_DisplayedCards;
        [SerializeField] int m_SelectedCardIndex;
        [SerializeField] int m_HoveredCardIndex;

        public void RequestHover(int index){
            RequestHoverServerRpc(index);
        }
        public void RequestSelect(int index){
            RequestSelectCardServerRpc(index);
        }
        [ServerRpc(RequireOwnership = false)] void RequestHoverServerRpc(int index, ServerRpcParams serverRpcParams = default){
            if(serverRpcParams.Receive.SenderClientId.Equals(ClientMatchState.Instance.m_ClientInControl)){
                // if client is in control, set the hover
                SetHoveredCardIndexClientRpc(index);
            }
        }
        [ServerRpc(RequireOwnership = false)] void RequestSelectCardServerRpc(int index, ServerRpcParams serverRpcParams = default){
            if(serverRpcParams.Receive.SenderClientId.Equals(ClientMatchState.Instance.m_ClientInControl)){
                // if client is in control, set the selection
                SetSelectedCardIndexClientRpc(index);
            }
        }
        [ClientRpc] void SetHoveredCardIndexClientRpc(int index)
        {
            m_DisplayedCards[m_HoveredCardIndex].GetComponent<CardUI>().DisplayHoverGraphics(false);
            m_HoveredCardIndex = index;
            m_DisplayedCards[index].GetComponent<CardUI>().DisplayHoverGraphics(true);
        }
        [ClientRpc] void SetSelectedCardIndexClientRpc(int index)
        {
            m_SelectedCardIndex = index;
            m_DisplayedCards[index].GetComponent<CardUI>().DisplayClickGraphics();

            ClientMatchState.Instance.ClientCardSelected(m_DisplayedCards[index].GetComponent<CardUI>().GetCard().cardID);
        }

        public void ClearDisplayedCards(){
            foreach (GameObject card_go in m_DisplayedCards){
                card_go.GetComponent<CardUI>().OnHover -= RequestHover;
                card_go.GetComponent<CardUI>().OnClick -= RequestSelect;
                Destroy(card_go,.5f);
            }
        }
        public void SetDisplayedCards(List<GameObject> cardObjectList)
        {
            int count = cardObjectList.Count;
            float panel_length = leftAnchor.position.x - RightAnchor.position.x;
            float partition_length = panel_length / count; // length of bound per card.
            int i = 0;
            foreach (GameObject card_go in cardObjectList)
            {
                m_DisplayedCards.Add(card_go);
                card_go.transform.SetParent(this.transform, false);
                // center on cardpanel
                card_go.transform.position = Vector3.zero;
                // distribute horizontally
                card_go.transform.position = new Vector3(leftAnchor.position.x - (i * partition_length) - (partition_length / 2),
                    this.transform.position.y, this.transform.position.z);
                card_go.GetComponent<CardUI>().SetCardIndex(i);
                card_go.GetComponent<CardUI>().OnHover += RequestHover;
                card_go.GetComponent<CardUI>().OnClick += RequestSelect;
                
                i++;
            }
        }

        void Start()
        {
            m_DisplayedCards = new List<GameObject>();
        }

        void Update()
        {

        }
    }
}


