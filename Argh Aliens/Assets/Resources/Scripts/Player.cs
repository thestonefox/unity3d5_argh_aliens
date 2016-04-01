using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public float movementMaxSpeed = 20f;
    public float rotationSpeed = 180f;
    public float movementAcceleration = 0.25f;
    public float thrustPower = 50f;
    public float terminalVelocity = 5f;
    public bool isAlive;

    public GameObject playerBody;
    public GameObject engineParticles;
    public GameObject deathParticles;

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
    private int currentCameraView = 0;

    void Awake()
    {
        CreateCameras();
        UpdateCameraView();
        rb = GetComponent<Rigidbody>();
        engineParticleSystem = engineParticles.GetComponent<ParticleSystem>();
        Revive();
    }

    void CreateCameras()
    {
        cameraPositions = new Vector3[3];
        cameraRotations = new Vector3[3];

        cameraPositions[0] = new Vector3(0f, 1.5f, -4f);
        cameraRotations[0] = new Vector3(25f, 0f, 0f);

        cameraPositions[1] = new Vector3(0f, 7f, 0f);
        cameraRotations[1] = new Vector3(90f, 0f, 0f);

        cameraPositions[2] = new Vector3(0f, 0.35f, -0.2f);
        cameraRotations[2] = new Vector3(0f, 0f, 0f);
    }

    void UpdateCameraView()
    {
        gameObject.transform.Find("PlayerCamera").gameObject.transform.localPosition = cameraPositions[currentCameraView];
        gameObject.transform.Find("PlayerCamera").gameObject.transform.localRotation = Quaternion.Euler(cameraRotations[currentCameraView].x, cameraRotations[currentCameraView].y, cameraRotations[currentCameraView].z);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "IgnoreCollision" ||
            (collision.collider.tag == "SafeCollision" && previousVelocity.y > -terminalVelocity))
        {
            //Collision is ok so do other checking here
        } else
        {
            Die();
        }
    }

    void Update()
    {
        GetInput();
        SwitchCamera();
        previousVelocity = rb.velocity;
    }
	
	void FixedUpdate () {
        if (isAlive)
        {
            Move();
            Turn();
            Thrust();
        }
    }

    void GetInput()
    {
        Vector2 leftStick = StickInput(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 rudder = StickInput(Input.GetAxis("TriggerAxis"), 0f);
        moveInputValue = leftStick.y;
        rotationInputValue = leftStick.x;
        strafeInputValue = rudder.x;

        movementSpeed = CalculatetSpeed(moveInputValue, movementSpeed, movementMaxSpeed, movementAcceleration);
        strafeSpeed = CalculatetSpeed(strafeInputValue, strafeSpeed, movementMaxSpeed, movementAcceleration);
    }

    Vector2 StickInput(float xAxis, float yAxis)
    {
        float deadzone = 0.25f;
        Vector2 stickInput = new Vector2(xAxis, yAxis);
        if (stickInput.magnitude < deadzone)
            stickInput = Vector2.zero;
        else
            stickInput = stickInput.normalized * ((stickInput.magnitude - deadzone) / (1 - deadzone));

        return stickInput;
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
        if (Input.GetButton("Fire1"))
        {
            engineParticles.SetActive(true);
            engineParticleSystem.loop = true;
            if (!engineParticleSystem.IsAlive())
            {
                engineParticleSystem.startLifetime = 0.8f;
                engineParticleSystem.Play();
            }

            rb.AddForce(Vector3.up * thrustPower);
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

    void SwitchCamera()
    {
        if (Input.GetButtonDown("Fire2"))
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
        isAlive = false;
        LevelManager.instance.PlayerDie();
    }

    public void Revive()
    {
        rb.isKinematic = false;
        playerBody.SetActive(true);
        deathParticles.SetActive(false);
        isAlive = true;
    }
}
