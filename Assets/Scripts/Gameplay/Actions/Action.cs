using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using Monke.Gameplay.Character;

namespace Monke.Gameplay.Actions
{
    public enum ActionType
    {
        Shoot,
        Block
    }
    public abstract class Action : ScriptableObject
    {
        [NonSerialized] public ActionID ActionID;
        protected ActionType m_ActionType;
        public float TimeStarted { get; set; }
        public float TimeRunning { get { return (Time.time - TimeStarted); } }
        
        protected ActionRequestData m_Data;

        public ref ActionRequestData Data => ref m_Data;

        public void Initialize(ref ActionRequestData data)
        {
            m_Data = data;
            ActionID = m_Data.actionID;
            m_ActionType = m_Data.m_actionType;
        }


        public virtual void Reset()
        {
            m_Data = default;
            ActionID = default;
            TimeStarted = 0;
        }

        public abstract void OnStart(ServerCharacter serverCharacter);
        public abstract void OnUpdate(ClientCharacter clientCharacter);

        public abstract void End();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
         /// <summary>
        /// Called when the Action ends naturally. By default just calls Cancel()
        /// </summary>
        public virtual void End(ServerCharacter serverCharacter)
        {
            Cancel(serverCharacter);
        }

        /// <summary>
        /// This will get called when the Action gets canceled. The Action should clean up any ongoing effects at this point.
        /// (e.g. an Action that involves moving should cancel the current active move).
        /// </summary>
        public virtual void Cancel(ServerCharacter serverCharacter) { }
    }

    public struct ActionID : INetworkSerializable, IEquatable<ActionID>
    {
        public int ID;

        public ActionID(int id )
        {
            ID = id;
        }

        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ID);
        }

        public override bool Equals(object obj)
        {
            return obj is ActionID other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public static bool operator ==(ActionID x, ActionID y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(ActionID x, ActionID y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return $"ActionID({ID})";
        }
        public bool Equals(ActionID other)
        {
            return ID == other.ID;
        }

    }

   
}
