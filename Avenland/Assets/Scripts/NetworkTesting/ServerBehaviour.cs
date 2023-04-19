using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Collections.Generic;

public class ServerBehaviour : MonoBehaviour
{
    public NetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;

    public int idToGive = 0;
    public int playersReady = 0;
    public int maxAllowedPlayers = 3;
    [Space]
    public string thisPlayerName;
    //public MainMenu mainMenu;
    //public List<string> players = new List<string>();

    private bool creatingConnection;
    private bool gameStarted;

    void Start()
    {
       
    }

    public void StartServer()
    {
        m_Driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4; // The local address to which the client will connect to is 127.0.0.1
        endpoint.Port = 9000;
        //endpoint.Port = (ushort)int.Parse(FindObjectOfType<MainMenu>().port.text);

        if (m_Driver.Bind(endpoint) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
            m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        creatingConnection = true;
    }

    public void OnDestroy()
    {
        DisconnectServer();
        m_Connections.Dispose();
    }

    public void DisconnectServer()
    {
        creatingConnection = false;
        m_Driver.Dispose();
    }

    void Update()
    {
        if (!creatingConnection)
            return;

        m_Driver.ScheduleUpdate().Complete();
        CleanUpConnections();
        AcceptNewConnections();
        RetrieveData();
    }

    void CleanUpConnections()
    {
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
                SendToAll("5 " + m_Connections.Length);
            }
        }
        //mainMenu.EnoughPlayersToStart(m_Connections.Length);
    }

    void AcceptNewConnections()
    {
        if (gameStarted)
        {
            return;
        }
        if (m_Connections.Length == maxAllowedPlayers)
        {
            return;
        }
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            Debug.Log("Accepted a connection");
            SendToAll("5 " + m_Connections.Length);
        }
    }

    void RetrieveData()
    {
        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    var number = stream.ReadFixedString128();
                    ConvertData(number, m_Connections[i]);

                    //uint number = stream.ReadUInt();

                    //Debug.Log("Got " + number + " from the Client adding + 2 to it.");
                    //number += 2;

                    //m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
                    //writer.WriteUInt(number);
                    //m_Driver.EndSend(writer);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        SendToAll("4");
    }

    void SendToAll(FixedString128Bytes number)
    {
        for (int i = 0; i < m_Connections.Length; i++)
        {
            m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
            writer.WriteFixedString128(number);
            m_Driver.EndSend(writer);
        }
    }

    public void ConvertData(FixedString128Bytes input, NetworkConnection sender)
    {
        List<float> parsedBytes = NetworkMessageHandler.GetDigits(input);
        // [0] What to do 
        // 1 [1]=ID
        // 2 [1]=UnitID [2]=ActionType [3]=PositionX [4]=PositionY [5]PositionZ
        // 3 Player is ready
        // 4 Send Updated Playerlist

        switch (parsedBytes[0])
        {
            case 0: // Request ID
                SendID(sender);
                break;
            case 1: // Disconnect
                break;
            case 2: // Update Units and send to all except player who send
                Debug.Log("Server received unit Update");
                SendToAll
                    (
                    "2 "
                    + parsedBytes[1] + " "
                    + parsedBytes[2] + " "
                    + parsedBytes[3] + " "
                    + parsedBytes[4] + " "
                    + parsedBytes[5]
                    );
                break;
            case 3: // a Player is Ready
                playersReady++;
                break;
            case 4:
                CreatePlayerList();
                break;
            default:
                Debug.Log($"Server does not know what to do with {input}");
                break;
        }
    }

    void CreatePlayerList()
    {
        string playerNamesList = "6 ";
        
        SendToAll(playerNamesList);
    }

    void SendID(NetworkConnection sender)
    {
        idToGive++;
        FixedString128Bytes idByte = "0 " + idToGive.ToString();
        m_Driver.BeginSend(NetworkPipeline.Null, sender, out var writer);
        writer.WriteFixedString128(idByte);
        m_Driver.EndSend(writer);
    }
}
