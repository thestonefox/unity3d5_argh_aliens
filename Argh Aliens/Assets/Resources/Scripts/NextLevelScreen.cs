using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NextLevelScreen : MonoBehaviour {

    public Text fuelUsed;
    public Text bombsDropped;
    public Text buildingsDestroyed;
    public Text peepsKilled;
    public Text peepsCaptured;
    public Text totalScore;
    public Image continueImage;

    private int finalScore = 0;
    private int actualTotalScore = 0;
    private int scoreDisplayTimer;
    private int scoreDisplayInterval = 50;
    private int totalScoreIncrements = 0;
    private int totalScoreCap = 0;

	// Use this for initialization
	void Start () {
        scoreDisplayTimer = 0;
        actualTotalScore = 0;
        finalScore = GameManager.instance.playerScore;
        UpdateTotalScore(GameManager.instance.playerScore, true);
        fuelUsed.enabled = false;
        bombsDropped.enabled = false;
        buildingsDestroyed.enabled = false;
        peepsKilled.enabled = false;
        peepsCaptured.enabled = false;
        continueImage.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetButtonDown("Jump"))
        {
            if (scoreDisplayTimer >= 7 * scoreDisplayInterval)
            {
                GameManager.instance.NextLevel();
                GameManager.instance.LoadScene("Level");
            } else
            {
                scoreDisplayTimer = 7 * scoreDisplayInterval;                
            }
        }

        EnableScore(scoreDisplayTimer, scoreDisplayInterval, 0.75, fuelUsed, GameManager.instance.fuelUsed, -1, 1);
        EnableScore(scoreDisplayTimer, scoreDisplayInterval, 2, bombsDropped, GameManager.instance.bombsDropped, -1, 1);
        EnableScore(scoreDisplayTimer, scoreDisplayInterval, 3, buildingsDestroyed, GameManager.instance.buildingsDestroyed, -1, 1000);
        EnableScore(scoreDisplayTimer, scoreDisplayInterval, 4, peepsKilled, GameManager.instance.peepsKilled, 1, 100);
        EnableScore(scoreDisplayTimer, scoreDisplayInterval, 5, peepsCaptured, GameManager.instance.peepsCaptured, 1, 500);

        if(scoreDisplayTimer > scoreDisplayInterval * 6)
        {
            if (finalScore < 0)
            {
                finalScore = 0;
            }
            continueImage.enabled = true;
            GameManager.instance.playerScore = finalScore;
            totalScore.text = finalScore.ToString().PadLeft(11, '0');
            scoreDisplayTimer = 7 * scoreDisplayInterval;
        } else
        {
            scoreDisplayTimer++;
            UpdateTotalScore(totalScoreIncrements, false);
        }        
    }

    void UpdateTotalScore(int score, bool forceSet)
    {
        actualTotalScore += score;
        if(!forceSet && ((actualTotalScore > totalScoreCap && score > 0) || (actualTotalScore < totalScoreCap && score < 0)))
        {
            actualTotalScore = totalScoreCap;
        }
        totalScore.text = actualTotalScore.ToString().PadLeft(11, '0');
    }

    void EnableScore(int timer, int interval, double intervalStep, Text text, float score, int direction, int multiplier)
    {
        if (timer >= (interval * intervalStep) && !text.enabled)
        {
            float actualScore = (score * multiplier) * direction;
            string displayScore = (score * direction).ToString();
            totalScoreIncrements = CalcIncrements(actualScore, scoreDisplayInterval);
            totalScoreCap = (int) actualScore + actualTotalScore;
            finalScore += (int) actualScore;
            text.enabled = true;
            string suffix = "";
            if (multiplier > 1)
            {
                suffix = " * " + multiplier.ToString();
            }
            text.text = displayScore + suffix;
        }
    }

    int CalcIncrements(float score, int interval)
    {
        float result = score / interval;
        if (score < 0)
        {
            return Mathf.FloorToInt(result);
        } else
        {
            return Mathf.CeilToInt(result);
        }
    }
}
