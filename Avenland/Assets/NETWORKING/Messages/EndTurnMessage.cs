using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace ChatClientExample
{
    public class EndTurnMessage : MessageHeader
    {
        public override NetworkMessageType Type
        {
            get
            {
                return NetworkMessageType.CLIENT_END_TURN;
            }
        }

        public uint networkId;
        public uint moveDirection;

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            // very important to call this first
            base.SerializeObject(ref writer);

            writer.WriteUInt(networkId);
            writer.WriteUInt(moveDirection);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            // very important to call this first
            base.DeserializeObject(ref reader);

            networkId = reader.ReadUInt();
            moveDirection = reader.ReadUInt();

        }
    }
}

