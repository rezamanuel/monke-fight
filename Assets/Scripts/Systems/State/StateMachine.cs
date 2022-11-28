using System.Collections;
using System.Collections.Generic;

// this Class is the parent class for all states;
namespace Systems.State
{

    public abstract class StateMachine
    {
        protected virtual void Enter(){

        }// defines behaviour when entering this state
        protected virtual void Exit(){
            throw new System.NotImplementedException();
        } // defines behaviour when exiting this state

    }


}