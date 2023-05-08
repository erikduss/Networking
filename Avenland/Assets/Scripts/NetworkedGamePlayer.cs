using ChatClientExample;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkedGamePlayer : NetworkedBehaviour
{
    public bool isLocal = false;
    public bool isServerOperator = false;

    public string playerName = "Player Name";

    Client client;

    public SpecializationType selectedSpecialization = SpecializationType.Warrior;

    private void Awake()
    {
        if(networkId == GameSettings.instance.localPlayerID)
        {
            isLocal = true;
        }

        if (isLocal)
        {
            client = FindObjectOfType<Client>();
            GameSettings.instance.isServerOperator = isServerOperator;
        }

        DontDestroyOnLoad(this.gameObject); //make sure this survives the scene switch. Destroy it elsewere.
    }

    private void Start()
    {
        if (isLocal)
        {
            if (client == null)
            {
                client = FindObjectOfType<Client>();
            }
        }
    }

    public void SpawnLobbyPlayer()
    {

    }
}
