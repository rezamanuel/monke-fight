using System.Collections;
using System.Collections.Generic;

// this Class is the parent class for all states;
namespace Systems.State
{
        public enum GameState{
            Init,
            Load,
            MainMenu,
            Connecting,
            Lobby,
            Match, // match has sub-states: PauseMenu, Fight, RoundEnd
            PauseMenu,
            Fight, 
            RoundEnd

        }
    public class GameStateMachine :StateMachine
    {
        
        protected GameState _gameState;
        protected void Enter(GameState gs)
        {
            OnExit(); // exit last state
            _gameState = gs;
            switch(gs){
                case GameState.Init:
                    // Init
                case GameState.Load:
                    //Load
                case GameState.MainMenu:
                    //Main Menu
                case GameState.Connecting:
                    //Connecting
                case GameState.Lobby:
                    //Lobby
                case GameState.Match:
                    //Match
                case GameState.Fight:
                    //Fight
                case GameState.PauseMenu:
                    //PauseMenu
                case GameState.RoundEnd:
                    //RoundEnd
                throw new System.ArgumentException();    
            }
        }
          protected void OnExit()
        {
            switch(_gameState){
                case GameState.Init:
                    // Init
                case GameState.Load:
                    //Load
                case GameState.MainMenu:
                    //Main Menu
                case GameState.Connecting:
                    //Connecting
                case GameState.Lobby:
                    //Lobby
                case GameState.Match:
                    //Match
                case GameState.Fight:
                    //Fight
                case GameState.PauseMenu:
                    //PauseMenu
                case GameState.RoundEnd:
                    //RoundEnd           
                throw new System.ArgumentException();     
            }
        }
    }


}