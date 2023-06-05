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
                infoText.text = "Warriors are specialized in being able to block lots of incoming damage and taunting enemies (Health +++ & Enemy detection prevention -)";
                break;
            case SpecializationType.Mage:
                infoText.text = "Mages are not as well protected, but they are great at distracting enemies. (Health -- & Enemy detection prevention +++ & XP ++)";
                break;
            case SpecializationType.Rogue:
                infoText.text = "Rogues are specialized in revealing enemies & treasures. (Enemy detection prevention + & XP +++ & Luck +++)";
                break;
            case SpecializationType.Shaman:
                infoText.text = "Shamans are specialized in protecting their team at the cost of resources. (Health ++ & Enemy detection prevention + & XP - & Luck -)";
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
