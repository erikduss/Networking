using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System.Net.Sockets;

public class BaseServer : MonoBehaviour
{
    public int maxAmountOfPlayers = 4;

    public NetworkDriver driver;
    protected NativeList<NetworkConnection> connections;

#if UNITY_EDITOR
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        UpdateServer();
    }

    private void OnDestroy()
    {
        Shutdown();
    }
#endif

    public virtual void Init()
    {
        //Init the driver
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4; //WHO can connect to us
        endpoint.Port = 9000;

        if(driver.Bind(endpoint) != 0)
        {
            Debug.Log("There was an error binding to port " + endpoint.Port);
        }
        else
        {
            driver.Listen();
        }

        //init the connection list
        connections = new NativeList<NetworkConnection>(maxAmountOfPlayers, Allocator.Persistent);
    }

    public virtual void Shutdown()
    {
        driver.Dispose();
        connections.Dispose();
    }

    public virtual void UpdateServer()
    {
        driver.ScheduleUpdate().Complete();
        CleanupConnections();
        AcceptNewConnections();
        UpdateMessagePump();
    }

    private void CleanupConnections()
    {
        for (int i = 0; i< connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("Accepted a new connection");
        }
    }

    protected virtual void UpdateMessagePump()
    {
        DataStreamReader stream;
        for(int i = 0; i < connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if(cmd == NetworkEvent.Type.Data)
                {
                    OnData(stream);
                    byte opCode = stream.ReadByte();
                    //FixedString128 chatMessage = stream.ReadFixedString128();
                    Debug.Log("Got " + opCode + " as Operation Code");
                }
                else if(cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    public virtual void OnData(DataStreamReader stream)
    {
        NetMessage msg = null;

        var opCode = (OpCode)stream.ReadByte();

        switch (opCode)
        {
            case OpCode.CHAT_MESSAGE:
                msg = new Net_ChatMessage(stream);
                break;
            default:
                Debug.Log("Message received had no OpCode");
                break;
        }

        msg.ReceivedOnServer();
    }
}
