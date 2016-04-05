using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public int playerLives;
    public int playerScore;
    public int levelPeeps;
    public Vector3 levelDimensions;

    public Texture2D transitionTexture;
    public float transitionSpeed = 0.8f;

    public float fuelUsed;
    public int bombsDropped;
    public int peepsCaptured;
    public int peepsKilled;
    public int buildingsDamaged;
    public int buildingsDestroyed;

    public static GameManager instance = null;

    int transitionDrawDepth = -1000;
    float transitionAlpha = 1f;
    int transitionDirection = -1;
    string sceneToLoad = "";

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

    void OnGUI()
    {
        transitionAlpha += transitionDirection * transitionSpeed * 0.02f;
        transitionAlpha = Mathf.Clamp01(transitionAlpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, transitionAlpha);
        GUI.depth = transitionDrawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), transitionTexture);
    }

    void OnLevelWasLoaded()
    {
        transitionAlpha = 1f;
        BeginTransition(-1);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(sceneToLoad);
        sceneToLoad = "";
    }

    void ResetLevel()
    {
        sceneToLoad = "";
        fuelUsed = 0;
        bombsDropped = 0;
        peepsCaptured = 0;
        peepsKilled = 0;
        buildingsDestroyed = 0;
        buildingsDamaged = 0;
    }

    public void ResetGame()
    {
        sceneToLoad = "";
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

    public float BeginTransition(int direction)
    {
        transitionDirection = direction;
        return transitionSpeed;
    }

    public void LoadScene(string name)
    {
        sceneToLoad = name;
        float wait = BeginTransition(1);
        Invoke("ChangeScene", wait);
    }
}
