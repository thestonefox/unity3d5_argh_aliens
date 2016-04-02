using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public Vector3 levelDimensions = new Vector3(75f, 1f, 75f);
    public GameObject playArea;
    public GameObject player;
    public GameObject building;
    public GameObject buildingTop;

    public float maxBuildingHeight = 20f;
    public int lives = 3;

    public static LevelManager instance = null;

    private Vector3 areaOffset;
    private GameObject[,,] buildingBlocks;
    private bool paused;

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

        buildingBlocks = new GameObject[(int)buildingLimits.x, (int)buildingLimits.y, (int)buildingLimits.z];

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

                for (int y = 0; y < buildingHeight; y++)
                {
                    Vector3 buildingPos = new Vector3(startPos.x + (x * (building.transform.localScale.x * 2)),
                                                      startPos.y + (y * building.transform.localScale.y),
                                                      startPos.z - (z * (building.transform.localScale.z * 2)));


                    GameObject buildingObject = new GameObject();

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
                    }

                    buildingObject.GetComponent<BuildingBlock>().coords = new Vector3(x, y, z);
                    buildingObject.GetComponent<BuildingBlock>().buildingHeight = buildingHeight;
                    buildingBlocks[x, y, z] = buildingObject;
                }
            }
        }
    }

    void Update()
    {
        CheckPause();
    }

    void CheckPause()
    {
        if (Input.GetButtonDown("Start"))
        {
            Debug.Log("Paused");
            paused = !paused;
        }
        if (paused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void BlowUpBuildingAbove(Vector3 coords)
    {
        GameObject buildingObject = buildingBlocks[(int)coords.x, (int)coords.y + 1, (int)coords.z];
        buildingObject.GetComponent<BuildingBlock>().Destroy(false);
    }

    public void SetFireBuildingBelow(Vector3 coords)
    {
        GameObject buildingObject = buildingBlocks[(int)coords.x, (int)coords.y - 1, (int)coords.z];
        buildingObject.GetComponent<BuildingBlock>().OnFire();
    }

    public void PlayerDie()
    {
        lives--;
        Invoke("ResetPlayer", 1f);
    }
}
