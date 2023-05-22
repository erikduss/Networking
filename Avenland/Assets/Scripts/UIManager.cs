using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager instance { get { return _instance; } }

    [SerializeField] List<GameObject> playerHUD = new List<GameObject>();
    [SerializeField] List<Image> playerHeroImages = new List<Image>();
    [SerializeField] List<Image> playerTurnStatus = new List<Image>();
    [SerializeField] List<Slider> playerHealthSliders = new List<Slider>();
    [SerializeField] List<TextMeshProUGUI> playerNameTexts = new List<TextMeshProUGUI>();

    [SerializeField] List<Sprite> secializationHeroImages = new List<Sprite>();

    [Header("End Game Panel")]
    [SerializeField] GameObject endGamePanel;
    [SerializeField] Button leaveButton;
    [SerializeField] TextMeshProUGUI endGameStatusText;
    [SerializeField] TextMeshProUGUI endGameScoreText;

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
        leaveButton.interactable = false;
        endGamePanel.SetActive(false);
        DisablePortraits();
    }

    // Update is called once per frame
    void Update()
    {
        if(UploadNewScore.instance.uploadedScore)
        {
            leaveButton.interactable = true;
        }
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

    public void SetPlayerTurnStatus(int playerID, bool status)
    {
        if (status)
        {
            playerTurnStatus[playerID].color = Color.green;
        }
        else
        {
            playerTurnStatus[playerID].color = Color.red;
        }
    }

    public void ReturnToMenu()
    {
        //This SHOULD break connection to the server due to dontdestroy exception, double check if this is the case.
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowHighScores()
    {

    }

    public void EndGameAndShowScore(bool hasEscaped, int score)
    {
        UploadNewScore.instance.AddPoints(score);
        endGamePanel.SetActive(true);
        endGameScoreText.text = score.ToString();
        if (hasEscaped)
        {
            endGameStatusText.text = "Escaped!";
            endGameStatusText.color = Color.green;
        }
        else
        {
            endGameStatusText.text = "Died!";
            endGameStatusText.color = Color.red;
        }

        UploadNewScore.instance.UploadScore(score);

        //TODO NEXT TIME!!!!!!!
        //Update query from cyberduck to this!
        //UPDATE `UsersLogin` SET `lastplayed`= CURRENT_DATE WHERE `username` = 'Erikduss'
        //otherwise it will give an error and not update the score.
        //PLUS, the score uploaded is not being checked to if its a highscore or not, it always overrites the last score.
    }
}
