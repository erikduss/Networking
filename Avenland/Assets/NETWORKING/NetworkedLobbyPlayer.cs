using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChatClientExample
{
	public class NetworkedLobbyPlayer : NetworkedBehaviour
	{
		public bool isLocal = false;
		public bool isServer = false;

		public string playerName = "Player Name";
		[SerializeField] private TextMeshProUGUI playerNameText;
		[SerializeField] private Image readyImage;

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
			
		}

		public void SendStatusUpdate(uint stat)
        {
			if (isLocal)
			{
				//Send to the server if player is ready
				ReadyStatusUpdateMessage readyMsg = new ReadyStatusUpdateMessage
				{
					networkId = this.networkId,
					status = stat
				};
				client.SendPackedMessage(readyMsg);
			}

			if (isServer)
			{
				//Send the ready status to all clients
				if (Time.frameCount % 3 == 0) { // assuming 60fps, so 20fps motion updates
												// We could consider sending this over a non-reliable pipeline
					ReadyStatusUpdateMessage readyMsg = new ReadyStatusUpdateMessage
					{
						networkId = this.networkId,
						status = stat
					};

					server.SendBroadcast(readyMsg);
				}
			}
		}

		public void SetPlayerReadyStatus(bool status)
        {
			isReady = status;

            if (isReady)
            {
				readyImage.color = Color.green;
            }
            else
            {
				readyImage.color = Color.white;
            }
        }

		public void UpdateReadyStatus(uint response)
        {
			if (response == 1)
			{
				SetPlayerReadyStatus(true);
			}
            else
            {
				SetPlayerReadyStatus(false);
			}
        }
	}
}