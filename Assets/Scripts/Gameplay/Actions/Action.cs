using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

namespace Monke.Gameplay.Actions
{
    public class Action : ScriptableObject
    {
        public ActionID ActionID;

        protected ActionRequestData m_Data;

        public ref ActionRequestData Data => ref m_Data;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public struct ActionID : INetworkSerializable, IEquatable<ActionID>
    {
        public int ID;

        
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

    public class ActionRequestData
    {
    }
}
