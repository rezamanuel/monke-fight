using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Monke.Cards
{
    /// <summary>
    /// Class used for storing a Character's cards during a Match. 
    /// </summary>
    public class CharacterCardInventory : NetworkBehaviour
    {
        List<Card> CardList;
        
        void Start()
        {
            
        }

        void DrawCard(int n){
            for (int i = 0; i < n;i++){

            }
        }
        
        void Update()
        {
            
        }
}
}
