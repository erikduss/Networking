using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

namespace TransportExample
{
    public class TransportClient : MonoBehaviour
    {
        static Dictionary<GameEvent, GameEventHandler> gameEventDictionary = new Dictionary<GameEvent, GameEventHandler>() {
            // link game events to functions...
            { GameEvent.NUMBER_REPLY, NumberReplyHandler },
        };

        public NetworkDriver m_Driver;
        public NetworkConnection m_Connection;
        public bool Done;

        void Start() {
            m_Driver = NetworkDriver.Create();
            m_Connection = default(NetworkConnection);

            var endpoint = NetworkEndPoint.Parse("83.85.158.101", 1511, NetworkFamily.Ipv4);
            //endpoint.Port = 1511;
            m_Connection = m_Driver.Connect(endpoint);
        }

        public void OnDestroy() {
            m_Driver.Dispose();
        }

        void Update() {
            m_Driver.ScheduleUpdate().Complete();

            if (!m_Connection.IsCreated) {
                if (!Done)
                    Debug.Log("Something went wrong during connect");
                return;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log(m_Connection.IsCreated);
                DataStreamWriter writer;
                int result = m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out writer);
                if (result == 0)
                {
                    // Game Event
                    writer.WriteUInt((uint)NetworkMessageType.CHAT_MESSAGE);
                    writer.WriteFixedString128(new FixedString128("Test message"));
                    Debug.Log("Send message");

                    m_Driver.EndSend(writer);
                }
                else{
                    Debug.Log(result);
                }
                
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DataStreamWriter writer;
                int result = m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out writer);
                if (result == 0)
                {
                    // Game Event
                    writer.WriteUInt((uint)NetworkMessageType.CHAT_QUIT);
                    m_Connection.Disconnect(default);

                    m_Driver.EndSend(writer);
                }
            }

            DataStreamReader stream;
            NetworkEvent.Type cmd;
            while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty) {
                if (cmd == NetworkEvent.Type.Connect) {
                    Debug.Log("We are now connected to the server");

                    uint value = 1;
                    DataStreamWriter writer;
                    int result = m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out writer);
                    if (result == 0) {
                        // Game Event
                        writer.WriteUInt((uint)NetworkMessageType.HANDSHAKE);
                        writer.WriteFixedString128(new FixedString128("Erik"));

                        m_Driver.EndSend(writer);
                    }
                }
                else if (cmd == NetworkEvent.Type.Data) {
                    // Read GameEvent type from stream
                    GameEvent gameEventType = (GameEvent)stream.ReadUInt();
                    Debug.Log(gameEventType);

                    if (gameEventDictionary.ContainsKey(gameEventType)) {
                        gameEventDictionary[gameEventType].Invoke(stream, this, m_Connection);
                    }
                    else {
                        //Unsupported event received...
                    }                    
                }
                else if (cmd == NetworkEvent.Type.Disconnect) {
                    Debug.Log("Client got disconnected from server");
                    m_Connection = default(NetworkConnection);
                }
            }
        }

        // Event Functions
        static void NumberReplyHandler( DataStreamReader stream, object sender, NetworkConnection connection ) {
            uint value = stream.ReadUInt();
            Debug.Log("Got the value = " + value + " back from the server");

            TransportClient client = sender as TransportClient;
            
            //TODO: Remove when building more complex client...
            client.Done = true;
            connection.Disconnect(client.m_Driver);
            connection = default(NetworkConnection);
        }
    }
}