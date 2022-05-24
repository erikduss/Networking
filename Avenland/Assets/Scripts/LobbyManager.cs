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

    private GameSettings settings;

    private bool readyState = false;
    private Client client;

    private NetworkedLobbyPlayer localPlayer;

    // Start is called before the first frame update
    void Start()
    {
        settings = GameObject.FindGameObjectWithTag("GameOptions").GetComponent<GameSettings>();
        client = gameObject.GetComponent<Client>();
        
        if (settings.isHost)
        {
            customSeedToggle.interactable = true;
            customSeedInput.interactable = true;
            dungeonSizeDropdown.interactable = true;
            SetStartGameButton(false); //button is only interactable if all players are ready
        }
        else
        {
            customSeedToggle.interactable = false;
            customSeedInput.interactable = false;
            dungeonSizeDropdown.interactable = false;
            SetStartGameButton(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void SetStartGameButton(bool state)
    {
        startGameButton.interactable = state;
    }
}
