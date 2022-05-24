using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChatClientExample
{
    public class ClientServerSelection : MonoBehaviour
    {
        public string serverScene, clientScene;
        public TextMeshProUGUI serverIPInput, nameInput, nameHostInput;
        private GameSettings settings;

		private void Start() {
            Application.targetFrameRate = 60;
            settings = GameObject.FindGameObjectWithTag("GameOptions").GetComponent<GameSettings>();
		}

		public void GoServer() {
            settings.isHost = true;
            settings.SetUpServer();

            Client.serverIP = "127.0.0.1"; //Only this works
            //Client.serverIP = "192.168.2.28";

            string name = nameHostInput.text;
            if (string.IsNullOrEmpty(nameHostInput.text))
            {
                name = "";
                for (int i = 0; i < 16; ++i)
                {
                    name += (char)Random.Range(97, 97 + 26);
                }
            }
            Client.clientName = name;
            Client.isServer = true;
            settings.playerNames.Add(name);

            SceneManager.LoadScene(serverScene);
        }

        public void GoClient() {
            NetworkEndPoint endPoint;
            if (NetworkEndPoint.TryParse(serverIPInput.text, 9000, out endPoint, NetworkFamily.Ipv4)) {
                Client.serverIP = serverIPInput.text;
            }
            else {
                Client.serverIP = "127.0.0.1";
            }

            string name = nameInput.text;
            if (string.IsNullOrEmpty(nameInput.text)) {
                name = "";
                for (int i = 0; i < 16; ++i) {
                    name += (char)Random.Range(97, 97 + 26);
                }
            }
            Client.clientName = name;
            settings.playerNames.Add(name);

            settings.isHost = false;

            SceneManager.LoadScene(clientScene);
        }
    }
}