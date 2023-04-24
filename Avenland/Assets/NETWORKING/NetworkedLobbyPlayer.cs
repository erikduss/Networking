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
		public bool isServerOperator = false;

		public string playerName = "Player Name";
		[SerializeField] private TextMeshProUGUI playerNameText;
		[SerializeField] private Image readyImage;

		[SerializeField] private Image playerPortrait;
		[SerializeField] private List<Sprite> playerPortraitSprites = new List<Sprite>();

		[SerializeField] private GameObject operatorIndicator;

		Client client;

		public bool isReady = false;
		public SpecializationType selectedSpecialization = SpecializationType.Warrior;

        private void Awake()
        {
            if (isLocal)
            {
                client = FindObjectOfType<Client>();
            }

            SetOperatorStatus(false);
        }

        private void Start() {
            if (isLocal)
            {
				if(client == null)
				{
                    client = FindObjectOfType<Client>();
                }
            }
            //if ( isServer ) 
            //{
            //	server = FindObjectOfType<Server>();
            //	lobbyManager = FindObjectOfType<LobbyManager>();
            //}

            playerNameText.text = playerName;
            if (isLocal)
            {
				transform.parent.SetAsFirstSibling();
			}

            SetOperatorStatus(isServerOperator);
        }

		private void Update() {
			
		}

		public void SetOperatorStatus(bool status)
		{
            operatorIndicator.SetActive(status);
			isServerOperator = status;

			if (isLocal)
			{
				if (status)
				{
					GameSettings.instance.GrandOperatorPower();
				}
				else
				{
                    GameSettings.instance.TakeOperatorPower();
				}
			}
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

			//if (isServer)
			//{
			//	//Send the ready status to all clients
			//	ReadyStatusUpdateMessage readyMsg = new ReadyStatusUpdateMessage
			//	{
			//		networkId = this.networkId,
			//		status = stat
			//	};

			//	server.SendBroadcast(readyMsg);
			//}
		}

		public void SendSpecializationUpdate(uint spec)
		{
			if (isLocal)
			{
				SpecializationUpdateMessage specMsg = new SpecializationUpdateMessage
				{
					networkId = this.networkId,
					specialization = spec
				};
				client.SendPackedMessage(specMsg);
			}

			//if (isServer)
			//{
			//	SpecializationUpdateMessage specMsg = new SpecializationUpdateMessage
			//	{
			//		networkId = this.networkId,
			//		specialization = spec
			//	};

			//	server.SendBroadcast(specMsg);
			//}
		}

		public void SetPlayerReadyStatus(bool status)
        {
			isReady = status;

            /*if (isServer)
            {
				lobbyManager.CheckReadyValidState(); //THIS DOESNT PREVENT THE HOST FROM CLICKING IT IF THE OTHER PLAYER IS NOT READY AFTER BEING READY BEFORE
			}*/

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

		public void UpdateSpecialization(uint response)
		{
			if (response == (uint)SpecializationType.Warrior)
			{
				playerPortrait.sprite = playerPortraitSprites[0];
				selectedSpecialization = SpecializationType.Warrior;
			}
			else if(response == (uint)SpecializationType.Mage)
			{
				playerPortrait.sprite = playerPortraitSprites[1];
				selectedSpecialization = SpecializationType.Mage;
			}
			else if (response == (uint)SpecializationType.Rogue)
			{
				playerPortrait.sprite = playerPortraitSprites[2];
				selectedSpecialization = SpecializationType.Rogue;
			}
			else if (response == (uint)SpecializationType.Shaman)
			{
				playerPortrait.sprite = playerPortraitSprites[3];
				selectedSpecialization = SpecializationType.Shaman;
			}
            else
            {
				Debug.Log("Invalid specialization");
            }
		}
	}
}