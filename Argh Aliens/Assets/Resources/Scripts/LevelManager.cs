using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour {
    public GameObject playArea;
    public GameObject player;
    public GameObject building;
    public GameObject buildingTop;
    public GameObject roadPiece;
    public GameObject roadCrossPiece;
    public GameObject peep;
    public GameObject peepRPG;
    public Text scoreText;
    public Text livesText;
    public Text peepsText;
    public Image fuelIndicator;
    public Image countdownImage;
    public Image hud;
    public GameObject pauseMenu;
    public Button defaultButton;
    public AudioSource levelMusic;

    public AudioClip selectSound;
    public AudioClip explosion1Sound;
    public AudioClip explosion2Sound;
    public AudioClip screamSound;
    public AudioClip scoreSound;
    public AudioClip rewardSound;
    public AudioClip readySound;
    public AudioClip steadySound;
    public AudioClip goSound;

    public static LevelManager instance = null;

    private Vector3 levelDimensions;
    private int lives;
    private int maxLevelPeeps;
    private int maxLevelPeepsRPG;
    private float maxBuildingHeight = 15f;
    private int score;

    private Vector3 areaOffset;
    private GameObject[,,] buildingBlocks;
    private bool paused;
    private bool hasPaused;
    private int alivePeeps;
    private float countdownTimer = 0.02f;
    private bool countdownOn = true;
    private string nextScene;
    private bool levelEnd = false;
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        Setup();
    }

    void Setup()
    {
        InitLevelVars();
        InitPlayArea();
        ResetPlayer();
        CountdownStart();
    }

    void LayoutHUD()
    {
        float width = Screen.width;
        float height = Screen.height;
        int fontSize = 28;
        if (width <= 1024)
        {
            fontSize = 18;
        }
        
        scoreText.transform.localPosition = new Vector3(-(width / 2.5f), (height / 2f) - 28f, 0f);
        livesText.transform.localPosition = new Vector3((width / 2.87f), (height / 2f) - 28f, 0f);
        peepsText.transform.localPosition = new Vector3((width / 2.24f), (height / 2f) - 28f, 0f);

        scoreText.fontSize = fontSize;
        livesText.fontSize = fontSize;
        peepsText.fontSize = fontSize;

        fuelIndicator.transform.localPosition = new Vector3(0f, (height / 2f) - 24f, 0f);
        fuelIndicator.rectTransform.sizeDelta = new Vector2((width / 1.75f), 45f);
    }

    void InitLevelVars()
    {
        levelDimensions = GameManager.instance.levelDimensions;
        lives = GameManager.instance.playerLives;
        maxLevelPeepsRPG = (GameManager.instance.levelPeepsRPG > 0 ? GameManager.instance.levelPeepsRPG : 0) ;
        maxLevelPeeps = GameManager.instance.levelPeeps + maxLevelPeepsRPG;
        score = GameManager.instance.playerScore;
        nextScene = "";
        levelEnd = false;
    }

    void InitPlayArea()
    {
        areaOffset = new Vector3(levelDimensions.x / 10, 0f, levelDimensions.z / 10);
        playArea.transform.localScale = levelDimensions;
        InitBuildings();
    }

    void ResetPlayer()
    {
        player.transform.position = new Vector3(0f, maxBuildingHeight + 5f, 0f);
        player.GetComponent<Player>().Revive();
    }

    void CountdownStart()
    {
        countdownOn = true;
        Time.timeScale = countdownTimer;
        countdownImage.sprite = Resources.Load("Materials/GUI/Countdown/Textures/Ready", typeof(Sprite)) as Sprite;
        countdownImage.enabled = true;
        Invoke("CountdownReady", countdownTimer);
    }

    void CountdownReady()
    {
        source.PlayOneShot(readySound, 0.2f);
        Invoke("CountdownSteady", countdownTimer * 1.5f);
    }

    void CountdownSteady()
    {
        source.PlayOneShot(steadySound, 0.2f);
        countdownImage.sprite = Resources.Load("Materials/GUI/Countdown/Textures/Steady", typeof(Sprite)) as Sprite;
        Invoke("CountdownGo", countdownTimer * 2f);
    }

    void CountdownGo()
    {
        source.PlayOneShot(goSound, 0.2f);
        levelMusic.Play();
        countdownImage.sprite = Resources.Load("Materials/GUI/Countdown/Textures/Go", typeof(Sprite)) as Sprite;
        Invoke("CountdownEnd", countdownTimer);
    }

    void CountdownEnd()
    {        
        countdownImage.enabled = false;
        countdownOn = false;
        Time.timeScale = 1f;
    }

    void InitBuildings()
    {
        float startX = (levelDimensions.x / 2) - (building.transform.localScale.x / 2) - areaOffset.x;
        float startY = building.transform.localScale.y / 2 + areaOffset.y;
        float startZ = (levelDimensions.z / 2) - (building.transform.localScale.z / 2) - areaOffset.z;
        Vector3 startPos = new Vector3(startX * -1, startY, startZ);

        Vector3 buildingLimits = new Vector3((levelDimensions.x - (areaOffset.x * 2)) / (building.transform.localScale.x * 2),
                                             maxBuildingHeight,
                                             (levelDimensions.z - (areaOffset.z * 2)) / (building.transform.localScale.z * 2));

        buildingBlocks = new GameObject[(int)buildingLimits.x, (int)buildingLimits.y, (int)buildingLimits.z];

        int[] peepLocations = CreatePeepLocations((int)buildingLimits.x * (int)buildingLimits.z, maxLevelPeeps);

        int currentLocation = 0;

        for (int x = 0; x < buildingLimits.x; x++)
        {
            for (int z = 0; z < buildingLimits.z; z++)
            {
                float buildingHeight = Random.Range(8f, buildingLimits.y);
                int buildingMaterialIndex = Random.Range(0, 8) + 1;
                int buildingRoofMaterialIndex = Random.Range(0, 8) + 1;
                Material buildingMaterial = Resources.Load("Materials/Buildings/tower" + buildingMaterialIndex, typeof(Material)) as Material;
                Material buildingTopMaterial = Resources.Load("Materials/Buildings/towertop" + buildingMaterialIndex, typeof(Material)) as Material;
                Material buildingRoofMaterial = Resources.Load("Materials/Buildings/roof" + buildingRoofMaterialIndex, typeof(Material)) as Material;

                float buildingPosX = startPos.x + (x * (building.transform.localScale.x * 2));
                float buildingPosZ = startPos.z - (z * (building.transform.localScale.z * 2));

                InitRoadPiece(startPos, x, z, building.transform.localScale, buildingLimits);

                for (int y = 0; y < buildingHeight; y++)
                {
                    float buildingPosY = startPos.y + (y * building.transform.localScale.y);

                    Vector3 buildingPos = new Vector3(buildingPosX, buildingPosY, buildingPosZ);


                    GameObject buildingObject = new GameObject();

                    Vector3 coords = new Vector3(x, y, z);

                    if (y < (int)buildingHeight)
                    {
                        buildingObject = Instantiate(building, buildingPos, Quaternion.identity) as GameObject;
                        buildingObject.GetComponent<Renderer>().material = buildingMaterial;
                    } else
                    {
                        buildingObject = Instantiate(buildingTop, buildingPos + new Vector3(0f, -0.6f, 0f), Quaternion.identity) as GameObject;

                        buildingObject.transform.Find("SafeZone").GetComponent<Renderer>().material = buildingRoofMaterial;
                        for (int wall = 1; wall <= 4; wall++)
                        {
                            buildingObject.transform.Find("Wall" + wall).GetComponent<Renderer>().material = buildingTopMaterial;
                        }

                        if (System.Array.IndexOf(peepLocations, currentLocation) >= 0)
                        {
                            string peepType = "peep";
                            GameObject peepClone;
                            if (maxLevelPeepsRPG > 0)
                            {
                                maxLevelPeepsRPG--;
                                peepType = "peepRPG";
                                peepClone = InitPeep(peepRPG, buildingObject.transform.localPosition);
                            } else
                            {
                                peepType = "peep";
                                peepClone = InitPeep(peep, buildingObject.transform.localPosition);
                            }

                            float peepRotation = Random.Range(0, 4);
                            peepClone.transform.Rotate(0f, 90 * peepRotation, 0f);

                            buildingObject.GetComponent<BuildingBlock>().peep = peepClone;
                            buildingObject.GetComponent<BuildingBlock>().peepType = peepType;
                        }
                    }

                    buildingObject.GetComponent<BuildingBlock>().coords = coords;
                    buildingObject.GetComponent<BuildingBlock>().buildingHeight = buildingHeight;
                    buildingBlocks[x, y, z] = buildingObject;
                }

                currentLocation++;
            }
        }
    }

    void InitRoadPiece(Vector3 startPos, float x, float z, Vector3 building, Vector3 limits)
    {
        float roadPosX = startPos.x + (x * (building.x * 2));
        float roadPosZ = startPos.z - (z * (building.z * 2));
        int extendedRoads = 15;

        if (x < limits.x-1 && z > 0)
        {
            Instantiate(roadPiece, new Vector3(roadPosX, 0f, roadPosZ + building.z), Quaternion.identity);
            Instantiate(roadPiece, new Vector3(roadPosX + building.x, 0f, roadPosZ), Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            Instantiate(roadCrossPiece, new Vector3(roadPosX + building.x, 0f, roadPosZ + building.z), Quaternion.identity);
        }

        if (x == limits.x -1  && z > 0)
        {
            Instantiate(roadPiece, new Vector3(roadPosX, 0f, roadPosZ + building.z), Quaternion.identity);
            for (int c = 1; c < extendedRoads; c++)
            {
                ChangeRoadColor(Instantiate(roadPiece, new Vector3(roadPosX + (building.x * c), 0f, roadPosZ + building.z), Quaternion.identity) as GameObject, c);
            }
        }

        if (z == 0 && x < limits.x-1)
        {
            Instantiate(roadPiece, new Vector3(roadPosX + building.x, 0f, roadPosZ), Quaternion.Euler(new Vector3(0f, 90f, 0f)));
            for (int c = 1; c < extendedRoads; c++)
            {
                ChangeRoadColor(Instantiate(roadPiece, new Vector3(roadPosX + building.x, 0f, roadPosZ + (building.z * c)), Quaternion.Euler(new Vector3(0f, 90f, 0f))) as GameObject, c);
            }
        }

        if (x == 0 && z > 0)
        {            
            for (int c = 1; c < extendedRoads; c++)
            {
                ChangeRoadColor(Instantiate(roadPiece, new Vector3(roadPosX - ((building.x) * c), 0f, roadPosZ + building.z), Quaternion.identity) as GameObject, c);
            }
        }

        if (z == limits.x-1 && x < limits.z-1)
        {
            for (int c = 1; c < extendedRoads; c++)
            {
                ChangeRoadColor(Instantiate(roadPiece, new Vector3(roadPosX + building.x, 0f, roadPosZ - (building.z * c)), Quaternion.Euler(new Vector3(0f, 90f, 0f))) as GameObject, c);
            }
        }
    }

    void ChangeRoadColor(GameObject road, int position)
    {
        Color[] fades = new Color[14];
        for(int c = 0; c < fades.Length; c++)
        {
            float shade = 0.9f - (0.1f * c);
            fades[c] = new Color(shade, shade, shade);
        }

        GameObject roadBit = road.transform.Find("Road").gameObject;
        GameObject sideWalk1 = road.transform.Find("Sidewalk1").gameObject;
        GameObject sideWalk2 = road.transform.Find("Sidewalk2").gameObject;

        roadBit.GetComponent<MeshRenderer>().material.color = fades[position-1];
        sideWalk1.GetComponent<MeshRenderer>().material.color = fades[position-1];
        sideWalk2.GetComponent<MeshRenderer>().material.color = fades[position-1];
    }

    int[] CreatePeepLocations(int playArea, int maxPeeps)
    {
        if (maxPeeps >= playArea)
        {
            maxPeeps = playArea;
        }
        alivePeeps = maxPeeps;

        int[] locations = new int[playArea];

        for(int i = 0; i < playArea; i++)
        {
            locations[i] = i;
        }

        for (int i = locations.Length - 1; i > 0; i--)
        {
            int swapPoint = Random.Range(0, i);
            int temp = locations[i];
            locations[i] = locations[swapPoint];
            locations[swapPoint] = temp;
        }

        int[] positions = new int[maxPeeps];
        for (int i = 0; i < maxPeeps; i++)
        {
            positions[i] = locations[i];
        }
        return positions;
    }

    GameObject InitPeep(GameObject peepType, Vector3 position)
    {
        return Instantiate(peepType, position + new Vector3(0f, 0.7f, 0f), Quaternion.identity) as GameObject;
    }

    void Update()
    {
        CheckPause();
        UpdateGui();
    }

    public void UnPause()
    {
        paused = false;
        hasPaused = false;
    }

    public void QuitLevel()
    {
        source.PlayOneShot(selectSound);
        paused = false;
        hasPaused = false;
        GameManager.instance.LoadScene("MainMenu");
    }

    void CheckPause()
    {
        if (Input.GetButtonDown("Start"))
        {
            source.PlayOneShot(selectSound);
            player.gameObject.GetComponent<Player>().StopSounds();
            paused = !paused;
        }
        if (!countdownOn)
        {
            if (paused)
            {
                pauseMenu.SetActive(true);
                if (!hasPaused)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
                    hasPaused = true;
                }
                Time.timeScale = 0f;                      
            }
            else
            {
                pauseMenu.SetActive(false);
                hasPaused = false;
                Time.timeScale = 1f;
            }
        }
    }

    public void BlowUpBuildingAbove(Vector3 coords)
    {
        source.PlayOneShot(explosion2Sound, 0.2f);
        GameObject buildingObject = buildingBlocks[(int)coords.x, (int)coords.y + 1, (int)coords.z];
        buildingObject.GetComponent<BuildingBlock>().Destroy(false);
    }

    public void SetFireBuildingBelow(Vector3 coords)
    {
        source.PlayOneShot(explosion2Sound, 0.2f);
        GameObject buildingObject = buildingBlocks[(int)coords.x, (int)coords.y - 1, (int)coords.z];
        buildingObject.GetComponent<BuildingBlock>().OnFire();
    }

    public void PlayerDie()
    {
        player.gameObject.GetComponent<Player>().StopSounds();
        source.PlayOneShot(explosion1Sound, 0.7f);
        UpdateLives();
        CheckLevelStatus();
        Invoke("ResetPlayer", 1f);
    }

    public void GotPeep(bool died)
    {
        source.PlayOneShot(screamSound, 0.7f);
        if (died)
        {
            UpdateScore(100);
        } else
        {
            source.PlayOneShot(scoreSound, 0.5f);
            UpdateScore(500);
            AddPlayerFuel(10);
        }
        alivePeeps--;
        CheckLevelStatus();
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    void CheckLevelStatus()
    {
        if (!levelEnd)
        {
            if (lives <= 0)
            {
                LevelEnd("GameOver");
            }

            if (lives > 0 && alivePeeps <= 0)
            {
                LevelEnd("NextLevel");
            }
        }
    }

    void LevelEnd(string scene)
    {
        levelEnd = true;
        player.GetComponent<Player>().isAlive = false;
        nextScene = scene;
        Invoke("LoadNextScene", 0.5f);
    }

    void LoadNextScene()
    {
        string scene = nextScene;
        nextScene = "";
        GameManager.instance.LoadScene(scene);
    }

    void UpdateLives()
    {
        lives--;
        GameManager.instance.playerLives = lives;
    }

    void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        GameManager.instance.playerScore = score;
    }

    void AddPlayerFuel(float fuelToAdd)
    {
        player.GetComponent<Player>().AddFuel(fuelToAdd);
    }

    void UpdateGui()
    {
        LayoutHUD();
        scoreText.text = score.ToString().PadLeft(11, '0');
        livesText.text = lives.ToString();
        peepsText.text = alivePeeps.ToString();

        float fuelLeft = player.GetComponent<Player>().fuel / player.GetComponent<Player>().defaultFuel;
        fuelIndicator.transform.localScale = new Vector3(fuelLeft, fuelIndicator.transform.localScale.y, fuelIndicator.transform.localScale.z);
    }
}
