using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monke.GameState 
{
    /// <summary>
    /// This State initializes in the Main Menu Scene, and is used to display lobby information
    /// 
    /// TODO: Populate Lobby, Join button, Hook into Connection Managers.
    /// </summary>
    public class MainMenuState : GameStateBehaviour
    {
        public override GameState ActiveState { get { return GameState.MainMenu; } }

        
    }
}
