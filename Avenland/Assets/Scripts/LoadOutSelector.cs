using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadOutSelector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoTitleText;
    [SerializeField] private TextMeshProUGUI infoText;
   
    private SpecializationType selectedSpec = SpecializationType.NONE;

    [SerializeField] private LobbyManager lobbyManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SpecializationSelected(int specID)
    {
        selectedSpec = (SpecializationType)specID;
        infoTitleText.text = "Specialization: " + selectedSpec.ToString();

        switch (selectedSpec)
        {
            case SpecializationType.Warrior:
                infoText.text = "Warriors come equiped with heavy armor and deal medium damage. They're specialized in being able to block lots of incoming damage and taunting enemies to divert damage away from allies.";
                break;
            case SpecializationType.Mage:
                infoText.text = "Mages are strong against groups of enemies, they're able to hit multiple enemies at once and slow down enemies. They deal a lot of damage but are only able to wear light armor";
                break;
            case SpecializationType.Rogue:
                infoText.text = "Rogues are specialized in dealing high damage while using medium weight armor and revealing enemies & treasures.";
                break;
            case SpecializationType.Shaman:
                infoText.text = "Shamans are specialized in healing and buffing group members while being able to deal medium damage and medium armor.";
                break;
            default:
                infoText.text = "Select a specialization for more information about the them.";
                break;
        }
    }

    public void ConfirmSpecialization()
    {
        if(selectedSpec == SpecializationType.NONE)
        {
            return;
        }

        lobbyManager.SpecializationConfirmButtonClicked(((int)selectedSpec));
    }
}
