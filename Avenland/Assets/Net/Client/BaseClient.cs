using ChatClientExample;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class BaseClient : MonoBehaviour
{
    public NetworkDriver driver;
    protected NetworkConnection connection;

    public static string serverIP;
    public static string clientName = "";

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
        connection = default(NetworkConnection);

        NetworkEndPoint endpoint = NetworkEndPoint.Parse("86.84.11.223", 9000, NetworkFamily.Ipv4);
        //NetworkEndPoint endpoint = NetworkEndPoint.Parse(serverIP, 9000, NetworkFamily.Ipv4);
        endpoint.Port = 9000;

        connection = driver.Connect(endpoint);
    }

    public virtual void Shutdown()
    {
        driver.Dispose();
    }

    public virtual void UpdateServer()
    {
        driver.ScheduleUpdate().Complete();
        CheckAlive();
        UpdateMessagePump();
    }

    private void CheckAlive()
    {
        if (!connection.IsCreated)
        {
            Debug.Log("Something went wrong, lost connection to server");
        }
    }

    protected virtual void UpdateMessagePump()
    {
        DataStreamReader stream;

        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if(cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                uint value = stream.ReadByte();
                Debug.Log("Got the value = " + value + " from the server");
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnected from server");
                connection = default(NetworkConnection);
            }
        }
    }

    public virtual void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);

        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }
}
