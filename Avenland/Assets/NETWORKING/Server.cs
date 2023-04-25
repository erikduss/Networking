using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Networking.Transport.Utilities;
using System.Linq;
using UnityEngine.Networking.Types;

namespace ChatClientExample {

    public delegate void ServerMessageHandler(Server server, NetworkConnection con, MessageHeader header);
    public delegate void ClientMessageHandler(Client client, MessageHeader header);

    public enum NetworkMessageType
    {
        HANDSHAKE,
        HANDSHAKE_RESPONSE,
        CHAT_MESSAGE,
        CHAT_QUIT,
        LOBBY_SPAWN,
        NETWORK_SPAWN,
        NETWORK_DESTROY,
        NETWORK_UPDATE_POSITION,
        INPUT_UPDATE,                        // uint networkId, InputUpdate (float, float, bool)
        PING,
        PONG,
        RPC,
        READY_STATUS_UPDATE,
        SPECIALIZATION_UPDATE,
        ASSIGN_SERVER_OPERATOR,
        CHANGE_SCENE
    }

    public enum MessageType
	{
        MESSAGE, 
        JOIN,
        QUIT
	}

    public class PingPong
	{
        public float lastSendTime = 0;
        public int status = -1;
        public string name = ""; // because of weird issues...
	}

    public static class NetworkMessageInfo
	{
        public static Dictionary<NetworkMessageType, System.Type> TypeMap = new Dictionary<NetworkMessageType, System.Type> {
            { NetworkMessageType.HANDSHAKE,                 typeof(HandshakeMessage) },
            { NetworkMessageType.HANDSHAKE_RESPONSE,        typeof(HandshakeResponseMessage) },
            { NetworkMessageType.CHAT_MESSAGE,              typeof(ChatMessage) },
            { NetworkMessageType.CHAT_QUIT,                 typeof(ChatQuitMessage) },
            { NetworkMessageType.LOBBY_SPAWN,               typeof(LobbySpawnMessage) },
            { NetworkMessageType.NETWORK_SPAWN,             typeof(SpawnMessage) },
            { NetworkMessageType.NETWORK_DESTROY,           typeof(DestroyMessage) },
            { NetworkMessageType.NETWORK_UPDATE_POSITION,   typeof(UpdatePositionMessage) },
            { NetworkMessageType.INPUT_UPDATE,              typeof(InputUpdateMessage) },
            { NetworkMessageType.PING,                      typeof(PingMessage) },
            { NetworkMessageType.PONG,                      typeof(PongMessage) },
            { NetworkMessageType.READY_STATUS_UPDATE,       typeof(ReadyStatusUpdateMessage) },
            { NetworkMessageType.SPECIALIZATION_UPDATE,     typeof(SpecializationUpdateMessage) },
            { NetworkMessageType.RPC,                       typeof(RPCMessage) },
            { NetworkMessageType.ASSIGN_SERVER_OPERATOR,    typeof(AssignServerOpertorMessage) },
            { NetworkMessageType.CHANGE_SCENE,              typeof(ChangeSceneMessage) }
        };
    }

    public class Server : MonoBehaviour
    {
        static Dictionary<NetworkMessageType, ServerMessageHandler> networkMessageHandlers = new Dictionary<NetworkMessageType, ServerMessageHandler> {
            { NetworkMessageType.HANDSHAKE,     HandleClientHandshake },
            { NetworkMessageType.CHAT_MESSAGE,  HandleClientMessage },
            { NetworkMessageType.CHAT_QUIT,     HandleClientExit },
            { NetworkMessageType.INPUT_UPDATE,  HandleClientInput },
            { NetworkMessageType.READY_STATUS_UPDATE, HandleClientReadyStatus },
            { NetworkMessageType.SPECIALIZATION_UPDATE, HandleClientSpecialization },
            { NetworkMessageType.PONG,          HandleClientPong },
            { NetworkMessageType.CHANGE_SCENE,  HandleClientSceneChange }
        };

        public NetworkDriver m_Driver;
        public NetworkPipeline m_Pipeline;
        private NativeList<NetworkConnection> m_Connections;

        private Dictionary<NetworkConnection, string> nameList = new Dictionary<NetworkConnection, string>();
        private Dictionary<NetworkConnection, NetworkedLobbyPlayer> lobbyPlayerInstances = new Dictionary<NetworkConnection, NetworkedLobbyPlayer>();
        private Dictionary<NetworkConnection, PingPong> pongDict = new Dictionary<NetworkConnection, PingPong>();

        public ChatCanvas chat;
        public NetworkManager networkManager;

        public static ushort ServerPort = 9000;

        public static int operatorID = -1;

        public static int minimumAmountOfPlayersRequiredToStart = 2;

        void Start() {
            // Create Driver
            NetworkSettings settings = new NetworkSettings();
            settings.WithReliableStageParameters(windowSize: 32);
            m_Driver = NetworkDriver.Create(settings);
            m_Pipeline = m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

            // Open listener on server port
            NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;

            endpoint.Port = ServerPort; //1511 //9000 //12567
            if (m_Driver.Bind(endpoint) != 0)
                Debug.Log("Failed to bind to port 1511");
            else
                m_Driver.Listen();

            Debug.Log("Started server on port: " + endpoint.Port);

            m_Connections = new NativeList<NetworkConnection>(64, Allocator.Persistent);
        }

        // Write this immediately after creating the above Start calls, so you don't forget
        //  Or else you well get lingering thread sockets, and will have trouble starting new ones!
        void OnDestroy() {
            m_Driver.Dispose();
            m_Connections.Dispose();
        }

        void Update() {
            // This is a jobified system, so we need to tell it to handle all its outstanding tasks first
            m_Driver.ScheduleUpdate().Complete();

            CleanupDeadConnections();

            AcceptNewConnections();

            DataStreamReader stream;
            for (int i = 0; i < m_Connections.Length; i++) {
                if (!m_Connections[i].IsCreated)
                    continue;

                // Loop through available events
                NetworkEvent.Type cmd;
                while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty) {
                    if (cmd == NetworkEvent.Type.Data) {
                        // First UInt is always message type (this is our own first design choice)
                        NetworkMessageType msgType = (NetworkMessageType)stream.ReadUShort();

                        // Create instance and deserialize
                        MessageHeader header = (MessageHeader)System.Activator.CreateInstance(NetworkMessageInfo.TypeMap[msgType]);
                        header.DeserializeObject(ref stream);

                        if (networkMessageHandlers.ContainsKey(msgType)) {
                            try {
                                networkMessageHandlers[msgType].Invoke(this, m_Connections[i], header);
                            }
                            catch {
                                Debug.LogError($"Badly formatted message received: {msgType}");
                            }
                        }
                        else {
                            Debug.LogWarning($"Unsupported message type received: {msgType}", this);
                        }
                    }
                }
            }

            // Ping Pong stuff for timeout disconnects
            for (int i = 0; i < m_Connections.Length; i++) {
                if (!m_Connections[i].IsCreated)
                    continue;
                
                if ( pongDict.ContainsKey(m_Connections[i])) {
                    if ( Time.time - pongDict[m_Connections[i]].lastSendTime > 5f ) {
                        pongDict[m_Connections[i]].lastSendTime = Time.time;
                        if (pongDict[m_Connections[i]].status == 0 ) {
                            // Remove from all the dicts, save name / id for msg

                            // FIXME: for some reason, sometimes this isn't in the list?
                            if (nameList.ContainsKey(m_Connections[i])) {
                                nameList.Remove(m_Connections[i]);
                            }

                            uint destroyId = lobbyPlayerInstances[m_Connections[i]].networkId;
                            networkManager.DestroyWithId(destroyId);
                            lobbyPlayerInstances.Remove(m_Connections[i]);

                            string name = pongDict[m_Connections[i]].name;
                            pongDict.Remove(m_Connections[i]);

                            // Disconnect this player
                            m_Connections[i].Disconnect(m_Driver);

                            // Build messages
                            string msg = $"{name} has been Disconnected (connection timed out)";
                            chat.NewMessage(msg, ChatCanvas.leaveColor);
                        
                            ChatMessage quitMsg = new ChatMessage {
                                message = msg,
                                messageType = MessageType.QUIT
                            };
                            
                            DestroyMessage destroyMsg = new DestroyMessage {
                                networkId = destroyId
                            };

                            // Broadcast
                            SendBroadcast(quitMsg);
                            SendBroadcast(destroyMsg, m_Connections[i]);

                            // Clean up
                            m_Connections[i] = default;
                        }
                        else {
                            pongDict[m_Connections[i]].status -= 1;
                            PingMessage pingMsg = new PingMessage();
                            SendUnicast(m_Connections[i], pingMsg);
                        }
                    }
                }
                else if ( nameList.ContainsKey(m_Connections[i]) ) { //means they've succesfully handshaked
                    PingPong ping = new PingPong();
                    ping.lastSendTime = Time.time;
                    ping.status = 3;    // 3 retries
                    ping.name = nameList[m_Connections[i]];
                    pongDict.Add(m_Connections[i], ping);

                    PingMessage pingMsg = new PingMessage();
                    SendUnicast(m_Connections[i], pingMsg);
                }
            }

            CheckIfNewOperatorNeedsToBeAssigned();
        }

        private void CleanupDeadConnections()
        {
            // Clean up connections, remove stale ones
            for (int i = 0; i < m_Connections.Length; i++)
            {
                if (!m_Connections[i].IsCreated)
                {
                    m_Connections.RemoveAtSwapBack(i);
                    // This little trick means we can alter the contents of the list without breaking/skipping instances
                    --i;
                }
            }
        }

        private void AcceptNewConnections()
        {
            // Accept new connections
            NetworkConnection c;
            while ((c = m_Driver.Accept()) != default(NetworkConnection))
            {
                m_Connections.Add(c);
                Debug.Log("Accepted a connection");
            }
        }

        public void SendUnicast( NetworkConnection connection, MessageHeader header, bool realiable = true ) {
            DataStreamWriter writer;
            int result = m_Driver.BeginSend(realiable ? m_Pipeline : NetworkPipeline.Null, connection, out writer);
            if (result == 0) {
                header.SerializeObject(ref writer);
                m_Driver.EndSend(writer);
            }
        }

        public void SendBroadcast(MessageHeader header, NetworkConnection toExclude = default, bool realiable = true) {
            for (int i = 0; i < m_Connections.Length; i++) {
                if (!m_Connections[i].IsCreated || m_Connections[i] == toExclude)
                    continue;

                DataStreamWriter writer;
                int result = m_Driver.BeginSend(realiable ? m_Pipeline : NetworkPipeline.Null, m_Connections[i], out writer);
                if (result == 0) {
                    header.SerializeObject(ref writer);
                    m_Driver.EndSend(writer);
                }
            }
        }

        // Static handler functions
        //  - Client handshake                  (DONE)
        //  - Client chat message               (DONE)
        //  - Client chat exit                  (DONE)
        //  - Input update

        static void HandleRPC(Server serv, MessageHeader header)
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

        static void HandleClientHandshake(Server serv, NetworkConnection connection, MessageHeader header) {
            HandshakeMessage message = header as HandshakeMessage;

            // Add to list
            serv.nameList.Add(connection, message.name);
            string msg = $"{message.name.ToString()} has joined the chat.";
            serv.chat.NewMessage(msg, ChatCanvas.joinColor);

            ChatMessage chatMsg = new ChatMessage {
                messageType = MessageType.JOIN,
                message = msg
            };

            // Send all clients the chat message
            serv.SendBroadcast(chatMsg);

            // spawn a non-local, server player
            GameObject player;
            uint networkId = 0;
            if (serv.networkManager.SpawnWithId(NetworkSpawnObject.PLAYERLOBBY, NetworkManager.NextNetworkID, out player)) {
                // Get and setup player instance
                NetworkedLobbyPlayer playerInstance = player.GetComponent<NetworkedLobbyPlayer>();

                //otherwise the host client will have multiple players with the isLocal and isServer true
                if (playerInstance.networkId == 1)
                {
                    playerInstance.isServerOperator = true; //the first player in the lobby will be the server operator
                    playerInstance.isLocal = true;
                }
                else
                {
                    playerInstance.isServerOperator = false;
                    playerInstance.isLocal = false;
                }

                playerInstance.transform.parent = GameObject.FindGameObjectWithTag("LobbyPlayerPanel").transform;
                playerInstance.playerName = message.name.ToString();
                networkId = playerInstance.networkId;

                serv.lobbyPlayerInstances.Add(connection, playerInstance);

                // Send spawn local player back to sender
                HandshakeResponseMessage responseMsg = new HandshakeResponseMessage {
                    message = $"Welcome {message.name.ToString()}!",
                    networkId = playerInstance.networkId
                };

                serv.SendUnicast(connection, responseMsg);
            }
            else {
                Debug.LogError("Could not spawn player instance");
            }

            // Send all existing players to this player
            foreach (KeyValuePair<NetworkConnection, NetworkedLobbyPlayer> pair in serv.lobbyPlayerInstances) {
                if (pair.Key == connection) continue;

                LobbySpawnMessage spawnMsg = new LobbySpawnMessage {
                    networkId = pair.Value.networkId,
                    objectType = NetworkSpawnObject.PLAYERLOBBY,
                    playerName = pair.Value.playerName,
                    selectedSpecialization = pair.Value.selectedSpecialization
                };

                if (pair.Value.isReady)
                    spawnMsg.isReady = 1;
                else
                    spawnMsg.isReady = 0;

                serv.SendUnicast(connection, spawnMsg);
            }

            // Send creation of this player to all existing players
            if (networkId != 0) {
                NetworkedLobbyPlayer tmpPlayer = player.GetComponent<NetworkedLobbyPlayer>();

                LobbySpawnMessage spawnMsg = new LobbySpawnMessage {
                    networkId = networkId,
                    objectType = NetworkSpawnObject.PLAYERLOBBY,
                    playerName = tmpPlayer.playerName
                };

                if (tmpPlayer.isReady)
                    spawnMsg.isReady = 1;
                else
                    spawnMsg.isReady = 0;

                serv.SendBroadcast(spawnMsg, connection);

                AssignServerOpertorMessage operatorMessage;

                if(operatorID != -1)
                {
                    operatorMessage = new AssignServerOpertorMessage
                    {
                        networkId = (uint)operatorID,
                        isServerOperator = 1
                    };

                    serv.SendUnicast(connection, operatorMessage);
                }
                else
                {
                    operatorID = (int)networkId;

                    operatorMessage = new AssignServerOpertorMessage
                    {
                        networkId = (uint)operatorID,
                        isServerOperator = 1
                    };

                    serv.SendBroadcast(operatorMessage);
                }
            }
            else {
                Debug.LogError("Invalid network id for broadcasting creation");
            }
        }

        private void CheckIfNewOperatorNeedsToBeAssigned()
        {
            if(lobbyPlayerInstances.Values.Count > 0)
            {
                int lowestID = 1000;
                bool operatorAvailable = false;

                foreach (NetworkedLobbyPlayer player in lobbyPlayerInstances.Values)
                {
                    if (player.isServerOperator)
                    {
                        operatorAvailable = true;
                    }

                    if (player.networkId < lowestID)
                    {
                        lowestID = ((int)player.networkId);
                    }
                }

                if (!operatorAvailable)
                {
                    NetworkedLobbyPlayer newOperator;
                    newOperator = lobbyPlayerInstances.Values.Where(x => x.networkId == lowestID).First();

                    if (newOperator != null)
                    {
                        AssignServerOpertorMessage operatorMessage = new AssignServerOpertorMessage
                        {
                            networkId = ((uint)lowestID),
                            isServerOperator = 1
                        };

                        operatorID = lowestID;
                        newOperator.SetOperatorStatus(true);

                        this.SendBroadcast(operatorMessage);
                    }
                    else
                    {
                        Debug.Log("NEW OPERATOR NOT FOUND");
                    }
                }
            }
        }

        static void HandleClientMessage(Server serv, NetworkConnection connection, MessageHeader header) {
            ChatMessage receivedMsg = header as ChatMessage;

            if (serv.nameList.ContainsKey(connection)) {
                string msg = $"{serv.nameList[connection]}: {receivedMsg.message}";
                serv.chat.NewMessage(msg, ChatCanvas.chatColor);

                receivedMsg.message = msg;

                // forward message to all clients
                serv.SendBroadcast(receivedMsg);
            }
            else {
                Debug.LogError($"Received message from unlisted connection: {receivedMsg.message}");
            }
        }

        static void HandleClientExit(Server serv, NetworkConnection connection, MessageHeader header) {
            ChatQuitMessage quitMsg = header as ChatQuitMessage;

            if (serv.nameList.ContainsKey(connection)) {
                string msg = $"{serv.nameList[connection]} has left the chat.";
                serv.chat.NewMessage(msg, ChatCanvas.leaveColor);

                // Clean up
                serv.nameList.Remove(connection);
                // if you join and quit quickly, might not be in this dict yet
                if (serv.pongDict.ContainsKey(connection)) {
                    serv.pongDict.Remove(connection);
                }

                connection.Disconnect(serv.m_Driver);

                if (serv.lobbyPlayerInstances[connection].networkId == operatorID)
                {
                    operatorID = -1;
                }

                uint destroyId = serv.lobbyPlayerInstances[connection].networkId;
                Destroy(serv.lobbyPlayerInstances[connection].gameObject);
                serv.lobbyPlayerInstances.Remove(connection);

                // Build messages
                ChatMessage chatMsg = new ChatMessage {
                    message = msg,
                    messageType = MessageType.QUIT
                };

                DestroyMessage destroyMsg = new DestroyMessage {
                    networkId = destroyId
                };

                // Send Messages to all other clients
                serv.SendBroadcast(chatMsg, connection);
                serv.SendBroadcast(destroyMsg, connection);
            }
            else {
                Debug.LogError("Received exit from unlisted connection");
            }
        }

        static void HandleClientInput(Server serv, NetworkConnection connection, MessageHeader header) {
            InputUpdateMessage inputMsg = header as InputUpdateMessage;

            if (serv.lobbyPlayerInstances.ContainsKey(connection)) {
                if (serv.lobbyPlayerInstances[connection].networkId == inputMsg.networkId) {
                    //does not yet contain input
                    //serv.lobbyPlayerInstances[connection].UpdateInput(inputMsg.input);
                }
                else {
                    Debug.LogError("NetworkID Mismatch for Player Input");
                }
            }
            else {
                Debug.LogError("Received player input from unlisted connection");
            }
        }

        static void HandleClientReadyStatus(Server serv, NetworkConnection connection, MessageHeader header)
        {
            ReadyStatusUpdateMessage inputMsg = header as ReadyStatusUpdateMessage;

            if (serv.lobbyPlayerInstances.ContainsKey(connection))
            {
                if (serv.lobbyPlayerInstances[connection].networkId == inputMsg.networkId)
                {
                    serv.lobbyPlayerInstances[connection].UpdateReadyStatus(inputMsg.status);
                    serv.SendBroadcast(inputMsg);
                }
                else
                {
                    Debug.LogError("NetworkID Mismatch for Player Input");
                }
            }
            else
            {
                Debug.LogError("Received player ready status from unlisted connection");
            }
        }

        static void HandleClientSceneChange(Server serv, NetworkConnection connection, MessageHeader header)
        {
            ChangeSceneMessage inputMsg = header as ChangeSceneMessage;

            if (serv.lobbyPlayerInstances.ContainsKey(connection))
            {
                if (serv.lobbyPlayerInstances[connection].networkId == inputMsg.networkId)
                {
                    if (serv.lobbyPlayerInstances[connection].isServerOperator)
                    {
                        bool canStart = true;
                        foreach(NetworkedLobbyPlayer player in serv.lobbyPlayerInstances.Values)
                            if (!player.isReady) canStart = false;

                        if(serv.lobbyPlayerInstances.Values.Count < Server.minimumAmountOfPlayersRequiredToStart)
                            canStart = false;

                        if (canStart)
                            serv.SendBroadcast(inputMsg);

                        //change the server to game view mode too.
                    }
                    else
                    {
                        Debug.LogError("Received scene change command from a non server operator!");
                    }
                }
                else
                {
                    Debug.LogError("NetworkID Mismatch for Player Input");
                }
            }
            else
            {
                Debug.LogError("Received player ready status from unlisted connection");
            }
        }

        static void HandleClientSpecialization(Server serv, NetworkConnection connection, MessageHeader header)
        {
            SpecializationUpdateMessage specMsg = header as SpecializationUpdateMessage;

            if (serv.lobbyPlayerInstances.ContainsKey(connection))
            {
                if (serv.lobbyPlayerInstances[connection].networkId == specMsg.networkId)
                {
                    serv.lobbyPlayerInstances[connection].UpdateSpecialization(specMsg.specialization);
                    serv.SendBroadcast(specMsg);
                }
                else
                {
                    Debug.LogError("NetworkID Mismatch for Player Input");
                }
            }
            else
            {
                Debug.LogError("Received player ready status from unlisted connection");
            }
        }

        static void HandleClientPong(Server serv, NetworkConnection connection, MessageHeader header) {
            // Debug.Log("PONG");
            serv.pongDict[connection].status = 3;   //reset retry count
        }
    }
}