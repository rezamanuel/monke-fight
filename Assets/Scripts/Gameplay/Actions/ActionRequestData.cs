using UnityEngine;
using Unity.Netcode;
using System;

namespace Monke.Gameplay.Actions
{
    public class ActionRequestData : INetworkSerializable
    {
        public ActionID actionID;
        public Vector3 m_Position;
        public Vector3 m_Direction; // Direction of action, usually normalized

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            PackFlags flags = PackFlags.None;
            if (!serializer.IsReader)
            {
                flags = GetPackFlags();
            }

            serializer.SerializeValue(ref actionID);
            serializer.SerializeValue(ref flags);

            if ((flags & PackFlags.HasPosition) != 0)
            {
                serializer.SerializeValue(ref m_Position);
            }
            if ((flags & PackFlags.HasDirection) != 0)
            {
                serializer.SerializeValue(ref m_Direction);
            }
        }

        [Flags]
        private enum PackFlags
        {
            None = 0,
            HasPosition = 1,
            HasDirection = 1 << 1
        }

        private PackFlags GetPackFlags()
        {
            PackFlags flags = PackFlags.None;
            if (m_Position != Vector3.zero) { flags |= PackFlags.HasPosition; }
            if (m_Direction != Vector3.zero) { flags |= PackFlags.HasDirection; }
            return flags;
        }



    }
}