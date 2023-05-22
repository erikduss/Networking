using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace ChatClientExample
{
    public class EndGameMessage : MessageHeader
    {
        public override NetworkMessageType Type
        {
            get
            {
                return NetworkMessageType.END_GAME;
            }
        }

        public uint networkId;
        public uint teamEscaped; //0 = false, 1 = true

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            // very important to call this first
            base.SerializeObject(ref writer);

            writer.WriteUInt(networkId);
            writer.WriteUInt(teamEscaped);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            // very important to call this first
            base.DeserializeObject(ref reader);

            networkId = reader.ReadUInt();
            teamEscaped = reader.ReadUInt();

        }
    }
}

