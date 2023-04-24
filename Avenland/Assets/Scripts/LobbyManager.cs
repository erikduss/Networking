using ChatClientExample;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Toggle customSeedToggle;
    [SerializeField] private TMP_InputField customSeedInput;
    [SerializeField] private TMP_Dropdown dungeonSizeDropdown;

    [SerializeField] private Image readyButtonImage;
    [SerializeField] private Text readyButtonText;
    [SerializeField] private Button startGameButton;

    [SerializeField] private Text specializationButtonText;

    private GameSettings settings;

    private bool readyState = false;
    private Client client;

    private NetworkedLobbyPlayer localPlayer;

    private void Awake()
    {
        SetOperatorStatus(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        settings = GameObject.FindGameObjectWithTag("GameOptions").GetComponent<GameSettings>();
        //client = gameObject.GetComponent<Client>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOperatorStatus(bool status)
    {
        if (status)
        {
            customSeedToggle.interactable = true;
            customSeedInput.interactable = true;
            dungeonSizeDropdown.interactable = true;
            SetStartGameButton(false); //button is only interactable if all players are ready
            CheckReadyValidState();
        }
        else
        {
            customSeedToggle.interactable = false;
            customSeedInput.interactable = false;
            dungeonSizeDropdown.interactable = false;
            SetStartGameButton(false);
        }
    }

    public void StartGame()
    {
        if (settings.isServerOperator)
        {
            //RPC wont work, RPC is meant for calling functions on networked entities like player objects. Will be useful for moving the player to a specific location in game scene

            //Instead of an RPC message, try to send a regular message(s) to ->
            // - Disable user input on their client?
            // - update players their option
            // - load into the game scene

            //Good to seperate these into individual messages because these features are useful for other game elements
            //(Disable input -> turn based, only the player that has the turn can interact)
            //(update player options -> maybe a future mechanic to change spec)
            //(loading into different scenes)


            //RPCMessage rpcMsg = new RPCMessage
            //{
            //    networkId = localPlayer.networkId
            //};
            //client.SendPackedMessage(rpcMsg);
        }
    }

    public void CheckReadyValidState()
    {
        if (settings.isServerOperator)
        {
            List<GameObject> lobbyPlayers = new List<GameObject>();
            lobbyPlayers.AddRange(GameObject.FindGameObjectsWithTag("LobbyPlayer"));

            bool canStart = true;

            foreach(GameObject ob in lobbyPlayers)
            {
                if (!ob.GetComponent<NetworkedLobbyPlayer>().isReady) canStart = false;
            }

            SetStartGameButton(canStart);
        }
    }

    public void ReadyButtonClicked()
    {
        readyState = !readyState;

        if(localPlayer == null)
        {
            localPlayer = GameObject.FindGameObjectsWithTag("LobbyPlayer").Where(x => x.GetComponent<NetworkedLobbyPlayer>().isLocal = true).First().GetComponent<NetworkedLobbyPlayer>();
        }

        if (readyState)
        {
            readyButtonImage.color = Color.green;
            readyButtonText.text = "Ready";
            localPlayer.SendStatusUpdate(1);
        }
        else
        {
            readyButtonImage.color = Color.red;
            readyButtonText.text = "Not Ready";
            localPlayer.SendStatusUpdate(0);
        }
    }

    public void SpecializationConfirmButtonClicked(int spec)
    {
        if (localPlayer == null)
        {
            localPlayer = GameObject.FindGameObjectsWithTag("LobbyPlayer").Where(x => x.GetComponent<NetworkedLobbyPlayer>().isLocal = true).First().GetComponent<NetworkedLobbyPlayer>();
        }

        localPlayer.SendSpecializationUpdate((uint)spec);

        if(spec == (int)SpecializationType.Warrior)
        {
            specializationButtonText.text = "Warrior (click to change)";
        }
        else if (spec == (int)SpecializationType.Mage)
        {
            specializationButtonText.text = "Mage (click to change)";
        }
        else if(spec == (int)SpecializationType.Rogue)
        {
            specializationButtonText.text = "Rogue (click to change)";
        }
        else if(spec == (int)SpecializationType.Shaman)
        {
            specializationButtonText.text = "Shaman (click to change)";
        }
    }

    public void SetStartGameButton(bool state)
    {
        startGameButton.interactable = state;
    }
}
