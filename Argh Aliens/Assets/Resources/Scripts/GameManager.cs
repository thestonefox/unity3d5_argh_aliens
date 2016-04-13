using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public int playerLives;
    public int playerScore;
    public int levelPeeps;
    public int levelPeepsRPG;
    public Vector3 levelDimensions;

    public Texture2D transitionTexture;
    public float transitionSpeed = 0.8f;

    public float fuelUsed;
    public int bombsDropped;
    public int peepsCaptured;
    public int peepsKilled;
    public int buildingsDamaged;
    public int buildingsDestroyed;

    public int startLevelPeeps = 3;
    public int startLevelPeepsRPG = -2;

    public static GameManager instance = null;

    int transitionDrawDepth = -1000;
    public float transitionAlpha = 1f;
    int transitionDirection = -1;
    string sceneToLoad = "";
    int cursorHideSet = 100;
    int cursorHideTimer = 0;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        ToggleCursor(false);
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

    void Update()
    {
        if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            ToggleCursor(true);
            cursorHideTimer = cursorHideSet;
        }

        if (cursorHideTimer > 0)
        {
            cursorHideTimer--;
        } else
        {
            cursorHideTimer = 0;
            ToggleCursor(false);
        }
    }

    void ToggleCursor(bool show)
    {
        if (show)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ResetGame()
    {
        sceneToLoad = "";
        playerLives = 3;
        playerScore = 0;
        levelPeeps = startLevelPeeps;
        levelPeepsRPG = startLevelPeepsRPG;
        levelDimensions = new Vector3(35f, 1f, 35f);
        ResetLevel();
    }

    public void NextLevel()
    {
        ResetLevel();
        levelPeeps++;
        levelPeepsRPG++;
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
