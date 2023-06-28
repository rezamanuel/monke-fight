using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
namespace Monke.UI
{
    public class CardPanel : NetworkBehaviour
    {
        // Start is called before the first frame update

        [SerializeField] RectTransform leftAnchor;
        [SerializeField] RectTransform RightAnchor;
        [SerializeField] List<GameObject> m_DisplayedCards;
        [SerializeField] NetworkVariable<int> m_SelectedCardIndex = new NetworkVariable<int>();
        [SerializeField] NetworkVariable<int> m_HoveredCardIndex = new NetworkVariable<int>();
        public void SetSelectedCardIndex(int index)
        {
            m_SelectedCardIndex.Value = index;
            Debug.Log("Selected Index: "+ index);

        }
         public void SetHoveredCardIndex(int index)
        {
            m_HoveredCardIndex.Value = index;

        }
        public void InitiateSelectionAnimation()
        {
            //Enter Player Selection (play anims)
        }

        public void FinalizeSelectionAnimation()
        {
            //Exit Player Selection (play anims)
        }

        public void SetDisplayedCards(List<GameObject> cardObjectList)
        {
            int count = cardObjectList.Count;
            float panel_length = leftAnchor.position.x - RightAnchor.position.x;
            float partition_length = panel_length / count; // length of bound per card.
            int i = 0;
            //Cleanup Displayed Cards.
            m_DisplayedCards.Clear();
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
                card_go.GetComponent<CardUI>().OnHover += SetHoveredCardIndex;
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


