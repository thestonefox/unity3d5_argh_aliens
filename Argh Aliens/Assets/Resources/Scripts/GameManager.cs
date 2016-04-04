using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public int playerLives;
    public int playerScore;
    public int levelPeeps;
    public Vector3 levelDimensions;

    public float fuelUsed;
    public int bombsDropped;
    public int peepsCaptured;
    public int peepsKilled;
    public int buildingsDamaged;
    public int buildingsDestroyed;

    public static GameManager instance = null;

    void Start()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }        
    }

    void ResetLevel()
    {
        fuelUsed = 0;
        bombsDropped = 0;
        peepsCaptured = 0;
        peepsKilled = 0;
        buildingsDestroyed = 0;
        buildingsDamaged = 0;
    }

    public void ResetGame()
    {
        playerLives = 3;
        playerScore = 0;
        levelPeeps = 3;
        levelDimensions = new Vector3(30f, 1f, 30f);
        ResetLevel();
    }

    public void NextLevel()
    {
        ResetLevel();
        levelPeeps++;
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
