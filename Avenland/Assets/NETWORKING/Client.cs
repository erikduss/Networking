using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine.UI;
using Unity.Networking.Transport.Utilities;
using UnityEngine.SceneManagement;
using TMPro;

namespace ChatClientExample
{
    public class Client : MonoBehaviour
    {
        static Dictionary<NetworkMessageType, ClientMessageHandler> networkMessageHandlers = new Dictionary<NetworkMessageType, ClientMessageHandler> {
            { NetworkMessageType.HANDSHAKE_RESPONSE,        HandleServerHandshakeResponse },
            { NetworkMessageType.NETWORK_SPAWN,             HandleNetworkSpawn },             // uint networkId, uint objectType
            { NetworkMessageType.NETWORK_DESTROY,           HandleNetworkDestroy },           // uint networkId
            { NetworkMessageType.NETWORK_UPDATE_POSITION,   HandleNetworkUpdate },            // uint networkId, vector3 position, vector3 rotation
            { NetworkMessageType.CHAT_MESSAGE,              HandleChatMessage },
            { NetworkMessageType.PING,                      HandlePing },
            { NetworkMessageType.READY_STATUS_UPDATE,       HandleReadyStatusUpdate },
            { NetworkMessageType.SPECIALIZATION_UPDATE,     HandleSpecializationUpdate },
            { NetworkMessageType.RPC,                       HandleRPC }
        };

        public NetworkDriver m_Driver;
        public NetworkPipeline m_Pipeline;
        public NetworkConnection m_Connection;
        public bool Done;

        private NetworkManager networkManager;

        public ChatCanvas chatCanvas;

        public static string serverIP;
        public static string clientName = "";

        bool connected = false;
        float startTime = 0;

        public static bool isServer = false;

        // Start is called before the first frame update
        void Start() {
            startTime = Time.time;
            // Create connection to server IP
            m_Driver = NetworkDriver.Create(new ReliableUtility.Parameters { WindowSize = 32 });
            m_Pipeline = m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

            m_Connection = default(NetworkConnection);

            var endpoint = NetworkEndPoint.Parse(serverIP, 9000, NetworkFamily.Ipv4);
            endpoint.Port = 1511;
            m_Connection = m_Driver.Connect(endpoint);

            networkManager = GameObject.FindGameObjectWithTag("GameOptions").GetComponent<NetworkManager>();
        }

        // No collections list this time...
        void OnApplicationQuit() {
            // Disconnecting on application exit currently (to keep it simple)
            if (m_Connection.IsCreated) {
                m_Connection.Disconnect(m_Driver);
                m_Connection = default(NetworkConnection);
            }
        }

        void OnDestroy() {
            m_Driver.Dispose();
        }

        void Update() {
            m_Driver.ScheduleUpdate().Complete();

            if (!connected && Time.time - startTime > 5f) {
                SceneManager.LoadScene(0);
            }

            if (!m_Connection.IsCreated) {
                if (!Done)
                    Debug.Log("Something went wrong during connect");
                return;
            }

            DataStreamReader stream;
            NetworkEvent.Type cmd;
            while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty) {
                if (cmd == NetworkEvent.Type.Connect) {
                    connected = true;
                    Debug.Log("We are now connected to the server");

                    // TODO: Create handshake message
                    var header = new HandshakeMessage {
                        name = clientName
                    };
                    SendPackedMessage(header);
                }
                else if (cmd == NetworkEvent.Type.Data) {
                    Done = true;

                    // First UInt is always message type (this is our own first design choice)
                    NetworkMessageType msgType = (NetworkMessageType)stream.ReadUShort();

                    // TODO: Create message instance, and parse data...
                    MessageHeader header = (MessageHeader)System.Activator.CreateInstance(NetworkMessageInfo.TypeMap[msgType]);
                    header.DeserializeObject(ref stream);

                    if (networkMessageHandlers.ContainsKey(msgType)) {
                        networkMessageHandlers[msgType].Invoke(this, header);
                    }
                    else {
                        Debug.LogWarning($"Unsupported message type received: {msgType}", this);
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect) {
                    Debug.Log("Client got disconnected from server");
                    m_Connection = default(NetworkConnection);
                }
            }
        }

        public string GetName()
        {
            return clientName;
        }

        public TMP_InputField input;

        // UI FUNCTIONS (0 refs)
        public void SendMessage() {
            ChatMessage chatMsg = new ChatMessage {
                message = input.text
            };
            if ( connected ) SendPackedMessage(chatMsg);
            input.text = "";
        }

        public void ExitChat() {
            ChatQuitMessage chatQuitMsg = new ChatQuitMessage();
            if (connected) SendPackedMessage(chatQuitMsg);
            SceneManager.LoadScene(0);
        }
        // END UI FUNCTIONS

        public void SendPackedMessage( MessageHeader header ) {
            DataStreamWriter writer;
            int result = m_Driver.BeginSend(m_Pipeline, m_Connection, out writer);

            // non-0 is an error code
            if (result == 0) {
                header.SerializeObject(ref writer);
                m_Driver.EndSend(writer);
            }
            else {
                Debug.LogError($"Could not write message to driver: {result}", this);
            }
        }

        // Receive message function
        // TODO: rewrite as MessageHeader handlers
        //      - Server response handshake (DONE)
        //      - network spawn             (WIP)
        //      - network destroy           (WIP)
        //      - network update            (WIP)

        static void HandleRPC(Client client, MessageHeader header)
        {
            RPCMessage msg = header as RPCMessage;

            // try to call the function
            try
            {
                msg.mInfo.Invoke(msg.target, msg.data);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
        }
        static void HandleReadyStatusUpdate(Client client, MessageHeader header)
        {
            ReadyStatusUpdateMessage posMsg = header as ReadyStatusUpdateMessage;

            GameObject obj;
            if (client.networkManager.GetReference(posMsg.networkId, out obj))
            {
                NetworkedLobbyPlayer playerStat = obj.GetComponent<NetworkedLobbyPlayer>();
                playerStat.UpdateReadyStatus(posMsg.status);
                if (isServer)
                {
                    GameObject.FindObjectOfType<LobbyManager>().CheckReadyValidState();
                }
            }
            else
            {
                Debug.LogError($"Could not find object with id {posMsg.networkId}!");
            }
        }

        static void HandleSpecializationUpdate(Client client, MessageHeader header)
        {
            SpecializationUpdateMessage specMsg = header as SpecializationUpdateMessage;

            GameObject obj;
            if (client.networkManager.GetReference(specMsg.networkId, out obj))
            {
                NetworkedLobbyPlayer playerStat = obj.GetComponent<NetworkedLobbyPlayer>();
                playerStat.UpdateSpecialization(specMsg.specialization);
            }
            else
            {
                Debug.LogError($"Could not find object with id {specMsg.networkId}!");
            }
        }

        static void HandleServerHandshakeResponse(Client client, MessageHeader header) {
            HandshakeResponseMessage response = header as HandshakeResponseMessage;

            GameObject obj;

            if (!isServer)
            {
                if (client.networkManager.SpawnWithId(NetworkSpawnObject.PLAYERLOBBY, response.networkId, out obj))
                {
                    NetworkedLobbyPlayer player = obj.GetComponent<NetworkedLobbyPlayer>();
                    player.isLocal = true;
                    player.isServer = false;
                    player.playerName = client.GetName();
                    GameObject parentObj = GameObject.FindGameObjectWithTag("LobbyPlayerPanel");
                    player.transform.SetParent(parentObj.transform);
                }
                else
                {
                    Debug.LogError("Could not spawn player!");
                }
            }
            else
            {
                if (isServer)
                {
                    GameObject.FindObjectOfType<LobbyManager>().CheckReadyValidState();
                }
            }
        }

        static void HandleNetworkSpawn(Client client, MessageHeader header) {
            SpawnMessage spawnMsg = header as SpawnMessage;

            GameObject obj;

            if (!isServer)
            {
                if (client.networkManager.SpawnWithId(spawnMsg.objectType, spawnMsg.networkId, out obj))
                {
                    //This is required to set the parent and the name of the non local player correctly in the client scene
                    NetworkedLobbyPlayer nonLocalPlayer = obj.GetComponent<NetworkedLobbyPlayer>();
                    if (nonLocalPlayer != null)
                    {
                        //Debug.Log(spawnMsg.playerName.ToString());
                        nonLocalPlayer.playerName = spawnMsg.playerName.ToString();
                        GameObject parentObj = GameObject.FindGameObjectWithTag("LobbyPlayerPanel");
                        nonLocalPlayer.transform.SetParent(parentObj.transform);
                    }
                }
                else
                {
                    Debug.LogError($"Could not spawn {spawnMsg.objectType} for id {spawnMsg.networkId}!");
                }
            }
            else
            {
                if (isServer)
                {
                    GameObject.FindObjectOfType<LobbyManager>().CheckReadyValidState();
                }
            }
        }

        static void HandleNetworkDestroy(Client client, MessageHeader header) {
            DestroyMessage destroyMsg = header as DestroyMessage;
            if (!client.networkManager.DestroyWithId(destroyMsg.networkId)) {
                Debug.LogError($"Could not destroy object with id {destroyMsg.networkId}!");
            }

            if (isServer)
            {
                GameObject.FindObjectOfType<LobbyManager>().CheckReadyValidState();
            }
        }

        static void HandleNetworkUpdate(Client client, MessageHeader header) {
            UpdatePositionMessage posMsg = header as UpdatePositionMessage;

            GameObject obj;
            if (client.networkManager.GetReference(posMsg.networkId, out obj)) {
                obj.transform.position = posMsg.position;
                obj.transform.eulerAngles = posMsg.rotation;
            }
            else {
                Debug.LogError($"Could not find object with id {posMsg.networkId}!");
            }
        }

        static void HandleChatMessage(Client client, MessageHeader header) {
            ChatMessage chatMsg = header as ChatMessage;

            Color c = ChatCanvas.chatColor;
            if (chatMsg.messageType == MessageType.JOIN) c = ChatCanvas.joinColor;
            if (chatMsg.messageType == MessageType.QUIT) c = ChatCanvas.leaveColor;

            client.chatCanvas.NewMessage(chatMsg.message, c);
        }

        static void HandlePing(Client client, MessageHeader header) {
            Debug.Log("PING");

            PongMessage pongMsg = new PongMessage();
            client.SendPackedMessage(pongMsg);
        }
    }
}