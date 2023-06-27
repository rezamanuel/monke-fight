using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
namespace Monke.UI
{
    public class CardPanel : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField] GameObject cardTemplate;
        [SerializeField] RectTransform leftAnchor;
        [SerializeField] RectTransform RightAnchor;
        [SerializeField] List<GameObject> displayedCards;
        public UnityEvent onCardSelected;

        public void InitiateSelectionAnimation(){
              //Enter Player Selection (play anims)
        }

        public void FinalizeSelectionAnimation(){
              //Exit Player Selection (play anims)
        }
        
        public void SetDisplayedCards(List<GameObject> cardObjectList){
            int count = cardObjectList.Count;
            float panel_length = leftAnchor.position.x - RightAnchor.position.x;
            float partition_length = panel_length / count; // length of bound per card.
            int i = 0;
            displayedCards.Clear();
            foreach(GameObject card_go in cardObjectList)
            {
                displayedCards.Add(card_go);
                card_go.transform.SetParent(this.transform, false);
                // center on cardpanel
                card_go.transform.position = Vector3.zero;
                // distribute horizontally
                card_go.transform.position = new Vector3(leftAnchor.position.x - (i * partition_length) - (partition_length / 2),
                    this.transform.position.y,this.transform.position.z);
                i++;
            }
        }

        void Start()
        {
            displayedCards = new List<GameObject>();
            
        }
        
        void Update()
        {
            
        }
}
}


