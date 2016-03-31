using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public Vector3 levelDimensions = new Vector3(100f, 1f, 100f);
    public GameObject playArea;
    public GameObject player;
    public GameObject building;
    public GameObject buildingTop;

    public float maxBuildingHeight = 35f;
    public int lives = 3;

    public static LevelManager instance = null;

    private Vector3 areaOffset;

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
        InitPlayArea();
        ResetPlayer();
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

    void InitBuildings()
    {
        float startX = (levelDimensions.x / 2) - (building.transform.localScale.x / 2) - areaOffset.x;
        float startY = building.transform.localScale.y / 2 + areaOffset.y;
        float startZ = (levelDimensions.z / 2) - (building.transform.localScale.z / 2) - areaOffset.z;
        Vector3 startPos = new Vector3(startX * -1, startY, startZ);

        Vector3 buildingLimits = new Vector3((levelDimensions.x - (areaOffset.x * 2)) / (building.transform.localScale.x * 2),
                                             maxBuildingHeight,
                                             (levelDimensions.z - (areaOffset.z * 2)) / (building.transform.localScale.z * 2));

        for (int x = 0; x < buildingLimits.x; x++)
        {
            for (int z = 0; z < buildingLimits.z; z++)
            {
                float buildingHeight = Random.Range(5f, buildingLimits.y);

                for (int y = 0; y < buildingHeight; y++)
                {
                    Vector3 buildingPos = new Vector3(startPos.x + (x * (building.transform.localScale.x * 2)),
                                                      startPos.y + (y * building.transform.localScale.y),
                                                      startPos.z - (z * (building.transform.localScale.z * 2)));


                    Instantiate(building, buildingPos, Quaternion.identity);
                    if (y == (int)buildingHeight)
                    {
                        Instantiate(buildingTop, buildingPos + new Vector3(0f, 0.4f, 0f), Quaternion.identity);
                    }
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayerDie()
    {
        lives--;
        Invoke("ResetPlayer", 1f);
    }
}
