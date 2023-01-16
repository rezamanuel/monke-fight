using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using System;
using Unity.Netcode;

namespace Monke.Gameplay.Actions
{
    public static class ActionLibrary
    {

        private static  Dictionary<ActionID, ObjectPool<Action>> s_ActionPools = new Dictionary<ActionID, ObjectPool<Action>>(); //holds a cached copy of action prototypes. (prototypes are like prefabs, but for Actions) each dict value is a separate Action Pool, w/ multiple copies

        private static ObjectPool<Action> GetActionPool(ActionID actionId)
        {
            if(!s_ActionPools.TryGetValue(actionId, out var actionPool))
            {
                actionPool = new ObjectPool<Action>(
                    createFunc: () => Object.Instantiate(ActionSource.Instance.GetActionPrototypeByID(actionId)),
                    actionOnRelease: action => action.Reset(),
                    actionOnDestroy: Object.Destroy);

                s_ActionPools.Add(actionId, actionPool);
            }

            return actionPool;
            
        }

        public static Action CreateAction(ActionRequestData actionData)
        {
            //check if action exists in ActionPool

            GetActionPool(actionData.actionID).Get( out Action action);
            action.Initialize(ref actionData);
            return action;
        }

    }
}