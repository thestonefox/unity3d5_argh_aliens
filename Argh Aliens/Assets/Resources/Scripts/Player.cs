using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public float movementMaxSpeed = 8f;
    public float rotationSpeed = 180f;
    public float movementAcceleration = 0.5f;
    public float thrustPower = 50f;
    public float terminalVelocity = 5f;
    public int bombCount = 20;
    public bool isAlive;
    public float defaultFuel = 300f;
    public bool landed;
    public float fuel;

    public GameObject playerBody;
    public GameObject engineParticles;
    public GameObject deathParticles;
    public GameObject bombBody;

    private Rigidbody rb;
    private float movementSpeed = 0f;
    private float moveInputValue;
    private float strafeSpeed = 0f;
    private float strafeInputValue;
    private float rotationInputValue;
    private ParticleSystem engineParticleSystem;
    private bool hasEngineFired;
    private Vector3 previousVelocity;
    private Vector3[] cameraPositions;
    private Vector3[] cameraRotations;

    private Vector3[] cameraRayPositions;
    private Vector3[] cameraRayRotations;
    private Vector3[] cameraRayScales;

    private int currentCameraView = 0;
    private GameObject[] bombs;
    private int currentBomb = 0;
    private int bombDelay = 0;
    private int bombDelayTimer = 10;    
    private float speedDeadzone = 1f;    

    void Awake()
    {
        CreateCameras();
        UpdateCameraView();
        CreateBombs();
        rb = GetComponent<Rigidbody>();
        engineParticleSystem = engineParticles.GetComponent<ParticleSystem>();        
        Revive();
    }

    void CreateBombs()
    {
        bombs = new GameObject[bombCount];
        for(int i = 0; i < bombCount; i++)
        {
            bombs[i] = Instantiate(bombBody, transform.position, Quaternion.identity) as GameObject;
        }
    }

    void CreateCameras()
    {
        cameraPositions = new Vector3[3];
        cameraRotations = new Vector3[3];
        cameraRayPositions = new Vector3[3];
        cameraRayRotations = new Vector3[3];
        cameraRayScales = new Vector3[3];

        cameraPositions[0] = new Vector3(0f, 1.5f, -4f);
        cameraRotations[0] = new Vector3(25f, 0f, 0f);
        cameraRayPositions[0] = new Vector3(0f, 1.7f, -2.5f);
        cameraRayRotations[0] = new Vector3(5f, 0f, 0f);
        cameraRayScales[0] = new Vector3(2f, 2.5f, 4f);

        cameraPositions[1] = new Vector3(0f, 7f, 0f);
        cameraRotations[1] = new Vector3(90f, 0f, 0f);
        cameraRayPositions[1] = new Vector3(0f, 3.5f, 0f);
        cameraRayRotations[1] = new Vector3(90f, 0f, 0f);
        cameraRayScales[1] = new Vector3(0.7f, 0.7f, 7f);

        cameraPositions[2] = new Vector3(0f, 0.35f, -0.13f);
        cameraRotations[2] = new Vector3(0f, 0f, 0f);
        cameraRayPositions[2] = new Vector3(0f, 3.5f, 0f);
        cameraRayRotations[2] = new Vector3(90f, 0f, 0f);
        cameraRayScales[2] = new Vector3(0.7f, 0.7f, 7f);
    }

    void UpdateCameraView()
    {
        gameObject.transform.Find("PlayerCamera").gameObject.transform.localPosition = cameraPositions[currentCameraView];
        gameObject.transform.Find("PlayerCamera").gameObject.transform.localRotation = Quaternion.Euler(cameraRotations[currentCameraView].x, cameraRotations[currentCameraView].y, cameraRotations[currentCameraView].z);

        gameObject.transform.Find("CameraRay").gameObject.transform.localPosition = cameraRayPositions[currentCameraView];
        gameObject.transform.Find("CameraRay").gameObject.transform.localRotation = Quaternion.Euler(cameraRayRotations[currentCameraView].x, cameraRayRotations[currentCameraView].y, cameraRayRotations[currentCameraView].z);
        gameObject.transform.Find("CameraRay").gameObject.transform.localScale = cameraRayScales[currentCameraView];
    }

    void hasLanded(Collision collision)
    {        
        if(collision.collider.tag == "SafeCollision" && previousVelocity.y > -terminalVelocity)
        {
            landed = true;
        } else
        {
            landed = false;
        }        
    }

    void OnCollisionEnter(Collision collision)
    {
        hasLanded(collision);
        if (collision.collider.tag != "IgnoreCollision" && !landed)
        {
            Die();
        }

        if (collision.collider.transform.parent != null && collision.collider.transform.parent.name.Contains("BuildingTop") && !landed)
        {
            collision.collider.transform.parent.GetComponent<BuildingBlock>().Destroy(true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "SafeCollision" && landed && rb.velocity.y > 0)
        {
            landed = false;
        }
    }

    void ToggleBuildingRenderer(Collider collider, bool toggle)
    {
        collider.GetComponent<MeshRenderer>().enabled = toggle;
        collider.transform.Find("Roof").GetComponent<MeshRenderer>().enabled = toggle;
    }

    void ToggleBuildingTopRenderer(Collider collider, bool toggle)
    {
        Transform parent = collider.transform.parent;
        parent.transform.Find("SafeZone").GetComponent<MeshRenderer>().enabled = toggle;
        for (int wall = 1; wall <= 4; wall++)
        {
            parent.transform.Find("Wall" + wall).GetComponent<MeshRenderer>().enabled = toggle;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.name.Contains("BuildingBlock"))
        {
            ToggleBuildingRenderer(collider, false);
        }

        if(collider.transform.parent != null && collider.transform.parent.name.Contains("BuildingTop"))
        {
            ToggleBuildingTopRenderer(collider, false);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.name.Contains("BuildingBlock"))
        {
            ToggleBuildingRenderer(collider, true);
        }

        if(collider.transform.parent != null && collider.transform.parent.name.Contains("BuildingTop"))
        {
            ToggleBuildingTopRenderer(collider, true);
        }
    }

    void Update()
    {
        GetInput();        
        SwitchCamera();
        previousVelocity = rb.velocity;
        bombDelay--;
        if(bombDelay < 0)
        {
            bombDelay = 0;
        }
    }
	
	void FixedUpdate () {
        if (isAlive)
        {
            Move();
            Turn();
            Thrust();
            DropBomb();
        } else
        {
            rb.isKinematic = true;
        }
    }

    void GetInput()
    {
        moveInputValue = Input.GetAxis("Vertical");
        rotationInputValue = Input.GetAxis("Horizontal");
        strafeInputValue = Input.GetAxis("Rudder");

        movementSpeed = CalculatetSpeed(moveInputValue, movementSpeed, movementMaxSpeed, movementAcceleration);
        strafeSpeed = CalculatetSpeed(strafeInputValue, strafeSpeed, movementMaxSpeed, movementAcceleration);
    }

    float CalculatetSpeed(float inputValue, float speed, float maxSpeed, float acceleration)
    {
        if ((inputValue < 0 && speed < maxSpeed) || 
            (inputValue > 0 && speed > -maxSpeed))
        {
            speed = speed + (acceleration * inputValue);
        }
        else
        {
            speed = CalculateDeceleration(speed, acceleration);
        }
        return Mathf.Clamp(speed, -maxSpeed, maxSpeed);
    }

    float CalculateDeceleration(float speed, float acceleration)
    {
        if (speed < speedDeadzone && speed > -speedDeadzone)
        {
            return 0;
        }

        if (speed > 0)
        {
            return speed - acceleration;
        }
        else if (speed < 0)
        {
            return speed + acceleration;
        }

        return 0;
    }

    void Move()
    {
        Vector3 move = transform.forward * movementSpeed * Time.deltaTime;
        Vector3 strafe = transform.right * strafeSpeed * Time.deltaTime;
        if (landed)
        {
            move = Vector3.zero;
            strafe = Vector3.zero;
        }
        rb.MovePosition(rb.position + (move + strafe));
    }

    void Turn()
    {
        float turn = rotationInputValue * rotationSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    void Thrust()
    {        
        if (Input.GetButton("Jump") && fuel > 0)
        {
            engineParticles.SetActive(true);
            engineParticleSystem.loop = true;
            if (!engineParticleSystem.IsAlive())
            {
                engineParticleSystem.startLifetime = 0.8f;
                engineParticleSystem.Play();
            }

            rb.AddForce(Vector3.up * thrustPower);
            fuel--;
            GameManager.instance.fuelUsed++;
            hasEngineFired = true;
        } else
        {
            if (hasEngineFired)
            {
                engineParticleSystem.startLifetime = 0.5f;
                engineParticleSystem.loop = false;
            }
        }
    }

    void DropBomb()
    {
        if (!landed && Input.GetButtonDown("Fire2") && bombDelay == 0)
        {
            bombDelay = bombDelayTimer;
            bombs[currentBomb].SetActive(true);
            bombs[currentBomb].transform.localPosition = gameObject.transform.localPosition + new Vector3(0f, -0.5f, 0f);
            bombs[currentBomb].GetComponent<Rigidbody>().rotation = Quaternion.identity;
            bombs[currentBomb].GetComponent<Rigidbody>().velocity = Vector3.zero;
            bombs[currentBomb].GetComponent<Rigidbody>().AddForce(Vector3.up * -500f);
            bombs[currentBomb].GetComponent<Rigidbody>().AddForce(Vector3.forward + (rb.transform.forward * 50));

            GameManager.instance.bombsDropped++;
            currentBomb++;
            if (currentBomb >= bombCount)
            {
                currentBomb = 0;
            }
        }
    }

    void SwitchCamera()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            currentCameraView++;
            if (currentCameraView >= cameraPositions.Length)
            {
                currentCameraView = 0;
            }
            UpdateCameraView();
        }
    }

    void Die()
    {
        rb.isKinematic = true;
        playerBody.SetActive(false);
        deathParticles.SetActive(true);
        if (isAlive)
        {
            LevelManager.instance.PlayerDie();
        }
        isAlive = false;        
    }

    public void Revive()
    {
        rb.isKinematic = false;
        playerBody.SetActive(true);
        deathParticles.SetActive(false);
        movementSpeed = 0;
        bombDelay = 0;
        isAlive = true;
        landed = false;
        fuel = defaultFuel;
    }

    public void AddFuel(float fuelToAdd)
    {
        float newFuel = fuelToAdd + fuel;
        if(newFuel > defaultFuel)
        {
            newFuel = defaultFuel;
        }
        fuel = newFuel;
    }
}
