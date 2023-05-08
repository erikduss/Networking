using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager instance { get { return _instance; } }

    [SerializeField] List<GameObject> playerHUD = new List<GameObject>();
    [SerializeField] List<Image> playerHeroImages = new List<Image>();
    [SerializeField] List<Slider> playerHealthSliders = new List<Slider>();
    [SerializeField] List<TextMeshProUGUI> playerNameTexts = new List<TextMeshProUGUI>();

    [SerializeField] List<Sprite> secializationHeroImages = new List<Sprite>();

    private void Awake()
    {
        //THERE CAN ONLY BE ONE INSTANCE OF THIS SCRIPT AT ONE TIME
        if (instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DisablePortraits();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerHUD(int playerID, SpecializationType spec, string playerName)
    {
        EnablePlayerHUD(playerID);
        SetPlayerImage(playerID, spec);
        SetPlayerName(playerID, playerName);
        
    }

    private void DisablePortraits()
    {
        for (int i = 0; i < playerHUD.Count; i++)
        {
            playerHUD[i].SetActive(false);
        }
    }

    private void EnablePlayerHUD(int playerID)
    {
        playerHUD[playerID].SetActive(true);
    }

    private void SetPlayerImage(int playerID, SpecializationType spec)
    {
        Sprite img;

        img = secializationHeroImages[(int)spec-1];
        playerHeroImages[playerID].sprite = img;
    }

    private void SetPlayerName(int playerID, string playerName)
    {
        playerNameTexts[playerID].text = playerName;
    }
}
