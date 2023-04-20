using ChatClientExample;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class AssignServerOpertorMessage : MessageHeader
{
    public override NetworkMessageType Type
    {
        get
        {
            return NetworkMessageType.ASSIGN_SERVER_OPERATOR;
        }
    }

    public uint isServerOperator; //0 is not operator, 1 is operator
    public uint networkId;

    public override void SerializeObject(ref DataStreamWriter writer)
    {
        // very important to call this first
        base.SerializeObject(ref writer);

        writer.WriteUInt(networkId);
        writer.WriteUInt(isServerOperator);
    }

    public override void DeserializeObject(ref DataStreamReader reader)
    {
        // very important to call this first
        base.DeserializeObject(ref reader);

        networkId = reader.ReadUInt();
        isServerOperator = reader.ReadUInt();
    }
}
