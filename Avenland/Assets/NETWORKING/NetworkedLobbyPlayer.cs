using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ChatClientExample
{
	public class NetworkedLobbyPlayer : NetworkedBehaviour
	{
		public bool isLocal = false;
		public bool isServer = false;

		public string playerName = "Player Name";
		[SerializeField] private TextMeshProUGUI playerNameText;

		Client client;
		Server server;

		public bool isReady = false;

		private void Start() {
			if (isLocal) 
			{
				client = FindObjectOfType<Client>();
			}
			if ( isServer ) 
			{
				server = FindObjectOfType<Server>();
			}

			playerNameText.text = playerName;
            if (isLocal)
            {
				transform.parent.SetAsFirstSibling();
			}
		}

		private void Update() {
			if (isLocal) 
			{
				//Send to the server if player is ready
				/*
				InputUpdateMessage inputMsg = new InputUpdateMessage {
					input = update,
					networkId = this.networkId
				};
				client.SendPackedMessage(inputMsg);*/
			}

			if (isServer) 
			{
				//Send the ready status to all clients
				/*
				// TODO: Send position update to all clients (maybe not every frame!)
				if (Time.frameCount % 3 == 0) { // assuming 60fps, so 20fps motion updates
												// We could consider sending this over a non-reliable pipeline
					UpdatePositionMessage posMsg = new UpdatePositionMessage {
						networkId = this.networkId,
						position = transform.position,
						rotation = transform.eulerAngles
					};

					server.SendBroadcast(posMsg);
				}*/
			}
		}
	}
}