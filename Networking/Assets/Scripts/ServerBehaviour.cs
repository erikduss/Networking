using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;

public enum GameEventType
{
    EMPTY,
    STRING,
    INT
}

public class ServerBehaviour : MonoBehaviour
{
    public NetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;

    // Start is called before the first frame update
    void Start()
    {
        m_Driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = 9000;
        if(m_Driver.Bind(endpoint) != 0)
        {
            Debug.Log("Faled to bind to port 9000");
        }
        else
        {
            m_Driver.Listen();
        }

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    private void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        for(int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        NetworkConnection c;

        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            Debug.Log("Accepted a connection");
        }

        DataStreamReader stream;
        for(int i=0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                continue;
            }
            NetworkEvent.Type cmd;

            while((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if(cmd == NetworkEvent.Type.Data)
                {
                    GameEventType varType = (GameEventType)stream.ReadUInt();

                    if(varType == GameEventType.EMPTY)
                    {
                        Debug.Log("Received empty, ping ping.");

                        DataStreamWriter writer;
                        int result = m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out writer);

                        writer.WriteUInt(0);
                        writer.WriteUInt(0);
                        m_Driver.EndSend(writer);
                    }

                    else if(varType == GameEventType.INT)
                    {
                        int number = stream.ReadInt();
                        Debug.Log("Got " + number + " from the Client adding + 2 to it.");

                        number += 2;

                        DataStreamWriter writer;
                        int result = m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out writer);

                        writer.WriteUInt(2);
                        writer.WriteInt(number);
                        m_Driver.EndSend(writer);
                    }
                    else if (varType == GameEventType.STRING)
                    {
                        FixedString64 str;

                        try
                        {
                            str = stream.ReadFixedString64();
                        }
                        catch
                        {
                            str = "Empty test";
                        }

                        
                        Debug.Log("Got string " + str + " from the Client");

                        str = "This is your return string";

                        DataStreamWriter writer;
                        int result = m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out writer);

                        writer.WriteUInt(1);
                        writer.WriteFixedString64(str);
                        m_Driver.EndSend(writer);
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }
    }
}
