using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<GameObject> playerHUD = new List<GameObject>();
    [SerializeField] List<Image> playerHeroImages = new List<Image>();
    [SerializeField] List<Slider> playerHealthSliders = new List<Slider>();
    [SerializeField] List<TextMeshProUGUI> playerNameTexts = new List<TextMeshProUGUI>();

    [SerializeField] List<Sprite> secializationHeroImages = new List<Sprite>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerHUD(int amountOfPlayers, List<SpecializationType> specs, List<string> playerNames)
    {
        SetPlayerImage(amountOfPlayers, specs);
        SetPlayerName(amountOfPlayers, playerNames);
        DisableLeftOverPortraits(amountOfPlayers);
    }

    private void DisableLeftOverPortraits(int amountOfPlayers)
    {
        if(amountOfPlayers < playerHUD.Count)
        {
            for(int i= amountOfPlayers; i< playerHUD.Count; i++)
            {
                playerHUD[i].SetActive(false);
            }
        }
    }

    private void SetPlayerImage(int amountOfPlayers, List<SpecializationType> specs)
    {
        Sprite img;

        for (int i = 0; i < amountOfPlayers; i++)
        {
            switch (i)
            {
                case 0:
                        img = secializationHeroImages[((int)specs[i]) - 1];
                        playerHeroImages[i].sprite = img;
                    break;
                case 1:
                        img = secializationHeroImages[((int)specs[i]) - 1];
                        playerHeroImages[i].sprite = img;
                    break;
                case 2:
                        img = secializationHeroImages[((int)specs[i]) - 1];
                        playerHeroImages[i].sprite = img;
                    break;
                case 3:
                        img = secializationHeroImages[((int)specs[i]) - 1];
                        playerHeroImages[i].sprite = img;
                    break;
            }
        }
    }

    private void SetPlayerName(int amountOfPlayers, List<string> playerNames)
    {
        for (int i = 0; i < amountOfPlayers; i++)
        {
            playerNameTexts[i].text = playerNames[i];
        }
    }
}
