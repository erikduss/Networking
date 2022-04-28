using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;

public class ClientBehaviour : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool Done;

    private float timeLeft = 15;

    // Start is called before the first frame update
    void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);

        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = 9000;
        m_Connection = m_Driver.Connect(endpoint);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            if (!Done)
            {
                Debug.Log("Something went wrong during connect");
            }
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if(cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");

                uint value = 1;

                DataStreamWriter writer;
                int result = m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out writer);

                writer.WriteUInt(value);
                m_Driver.EndSend(writer);
            }
            else if(cmd == NetworkEvent.Type.Data)
            {
                GameEventType varType = (GameEventType)stream.ReadUInt();

                if(varType == GameEventType.INT)
                {
                    int value = stream.ReadInt();
                    Debug.Log("Got the value = " + value + " back from the server");
                }
                else if(varType == GameEventType.STRING)
                {
                    FixedString64 value = stream.ReadFixedString64();
                    Debug.Log("Got the string = " + value + " back from the server");
                }
                
                //Done = true;
                //m_Connection.Disconnect(m_Driver);
                //m_Connection = default(NetworkConnection);
            }
            else if(cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                m_Connection = default(NetworkConnection);
            }
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            SendEmpty();
        }
    }

    public void SendEmpty()
    {
        DataStreamWriter writer;
        int result = m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out writer);

        writer.WriteUInt(0);
        m_Driver.EndSend(writer);

        timeLeft = 15;
    }

    public void SendInt(uint val)
    {
        DataStreamWriter writer;
        int result = m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out writer);

        writer.WriteUInt(2);
        writer.WriteUInt(val);
        m_Driver.EndSend(writer);
    }

    public void SendString(FixedString64 val)
    {
        DataStreamWriter writer;
        int result = m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out writer);

        writer.WriteUInt(1);
        writer.WriteFixedString64(val);
        m_Driver.EndSend(writer);
    }
}
