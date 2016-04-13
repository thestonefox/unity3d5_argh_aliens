using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {

    public Text finalLevel;
    public Text finalScore;
    public Image continueImage;
    public AudioClip selectSound;
    public AudioClip scoreSound;

    private int scoreDisplayTimer;
    private int scoreDisplayInterval = 50;
    private string levelReached;
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {
        finalLevel.enabled = false;
        finalScore.enabled = false;
        continueImage.enabled = false;        
	}

    void LayoutGUI()
    {
        levelReached = ((GameManager.instance.levelPeeps - GameManager.instance.startLevelPeeps)+1).ToString();
        float width = Screen.width;
        float height = Screen.height;
        int scoreFontSize = 50;
        if (width <= 1280)
        {
            scoreFontSize = 28;
        }

        finalLevel.transform.localPosition = new Vector3(0f, (height / 10.7f), 0f);
        finalScore.transform.localPosition = new Vector3(0f, -(height / 15.7f), 0f);

        finalLevel.fontSize = scoreFontSize;
        finalScore.fontSize = scoreFontSize;
    }

    // Update is called once per frame
    void Update () {
        LayoutGUI();
        if (Input.GetButtonDown("Jump"))
        {
            source.PlayOneShot(selectSound);
            if (scoreDisplayTimer >= 4 * scoreDisplayInterval)
            {                
                GameManager.instance.LoadScene("MainMenu");
            }
            else
            {
                scoreDisplayTimer = 4 * scoreDisplayInterval;
            }
        }

        DisplayText(scoreDisplayTimer, scoreDisplayInterval, 0.75, finalLevel, levelReached);
        DisplayText(scoreDisplayTimer, scoreDisplayInterval, 2, finalScore, GameManager.instance.playerScore.ToString());

        if (scoreDisplayTimer > scoreDisplayInterval * 3)
        {
            continueImage.enabled = true;
            scoreDisplayTimer = 4 * scoreDisplayInterval;
        }
        else
        {
            scoreDisplayTimer++;
        }
    }

    void DisplayText(int timer, int interval, double intervalStep, Text text, string value)
    {
        if (timer >= (interval * intervalStep) && !text.enabled)
        {
            source.PlayOneShot(scoreSound, 0.2f);
            text.enabled = true;
            text.text = value;
        }
    }
}
