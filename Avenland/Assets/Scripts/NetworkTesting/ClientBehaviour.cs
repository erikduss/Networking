using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System.Collections.Generic;

public class ClientBehaviour : MonoBehaviour
{
    //public NetworkedPlayer player;
    //public PlayActionPlan sequencer;
    //public MainMenu mainMenu;
    //public InGameUI ingameUI;

    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool m_Done;

    private bool creatingConnection;

    void Start()
    {
        //player = FindObjectOfType<NetworkedPlayer>();
        //sequencer = FindObjectOfType<PlayActionPlan>();
    }

    public void StartClient()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);

        //Get the IP and Port from the scene.
        //ushort port = (ushort)int.Parse(FindObjectOfType<MainMenu>().port.text);
        //var endpoint = NetworkEndPoint.Parse(mainMenu.ipAdress.text, port, NetworkFamily.Ipv4);

        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = 9000;
        m_Connection = m_Driver.Connect(endpoint);
    }

    public void SendServerRequest(string request)
    {
        string serverRequest = request;
        m_Driver.BeginSend(m_Connection, out var writer);
        writer.WriteFixedString128(serverRequest);
        m_Driver.EndSend(writer);
    }

    public void GetID() // Calls when game starts
    {
        SendServerRequest("0");
    }

    public void Disconnect()
    {
        try
        {
            //Send disconnect message to the server.
            //SendServerRequest("4 " + mainMenu.GetPlayerName());
        }
        catch (System.Exception)
        {

        }
        m_Connection.Disconnect(m_Driver);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }

    void Update()
    {
        if (!creatingConnection)
            return;

        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            if (!m_Done)
                Debug.Log("Something went wrong during connect");
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            switch (cmd)
            {
                case NetworkEvent.Type.Empty:
                    Debug.Log("Not sure? Connecting I guess?");
                    break;
                case NetworkEvent.Type.Data:
                    var dataReceived = stream.ReadFixedString128();
                    ConvertData(dataReceived);

                    //uint value = stream.ReadUInt();
                    //Debug.Log("Got the value = " + value + " back from the server");
                    //m_Done = true;

                    break;
                case NetworkEvent.Type.Connect:
                    //SendServerRequest("4 " + mainMenu.GetPlayerName());
                    Debug.Log("We are now connected to the server");

                    //uint value = 1;
                    //m_Driver.BeginSend(m_Connection, out var writer);
                    //writer.WriteUInt(value);
                    //m_Driver.EndSend(writer);
                    break;
                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Client got disconnected from server");
                    m_Connection = default(NetworkConnection);
                    break;

                default:
                    break;
            }
        }
    }

    public void ConvertData(FixedString128Bytes input)
    {
        List<float> parsedBytes = NetworkMessageHandler.GetDigits(input);
        // [0] What to do 
        // 0 [1]=ID Received
        // 2 [1]=UnitID [2]=ActionType [3]=PositionX [4]=PositionY [5]PositionZ
        // 3 [1]=ID [2] 1=Ready 2=not Ready
        // 4 Ask server for ID
        // 5 Update player count [1]=count
        // 6 Update player list
        // 7 Start Simulation
        // 8 Update ready number [1]=count

        switch (parsedBytes[0])
        {
            case 0: // Received ID
                break;
            case 1:
                break;
            case 2: // Update a Players Units
                break;
            case 3: // a Player is Ready
                break;
            case 4: // Ask server for ID
                break;
            case 5: // UpdatePlayerCount
                break;
            case 6: // UpdatePlayerList
                break;
            case 7: // Start Simulation
                break;
            case 8: // UpdateReadyNumber
                break;
            default:
                Debug.Log($"Client does not know what to do with {input}");
                break;
        }
    }
}
