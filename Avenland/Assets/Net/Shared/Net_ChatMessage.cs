using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Net_ChatMessage : NetMessage
{
    // 0 - 8 OP CODE
    public FixedString128Bytes chatMessage { set; get; }

    public Net_ChatMessage()
    {
        code = OpCode.CHAT_MESSAGE;
    }

    public Net_ChatMessage(DataStreamReader reader)
    {
        code = OpCode.CHAT_MESSAGE;
        Deserialize(reader);
    }

    public Net_ChatMessage(string msg)
    {
        code = OpCode.CHAT_MESSAGE;
        chatMessage = msg;
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
        writer.WriteFixedString128(chatMessage);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        //The first byte is handled already
        chatMessage = reader.ReadFixedString128();
    }

    public override void ReceivedOnServer()
    {
        Debug.Log("SERVER:: " + chatMessage);
    }

    public override void ReceivedOnClient()
    {
        Debug.Log("CLIENT:: " + chatMessage);
    }
}
