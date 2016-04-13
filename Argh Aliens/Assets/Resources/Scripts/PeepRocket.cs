using UnityEngine;
using System.Collections;

public class PeepRocket : MonoBehaviour {
    bool firing;
    bool isAlive;
    Rigidbody rb;
    Vector3 defaultPosition;
    private AudioSource rocketSound;

    // Use this for initialization
    void Awake () {
        rocketSound = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter()
    {
        Reset();
    }

    public void SetPosition(Vector3 position)
    {
        defaultPosition = position;
    }

    public void SetRotation(Quaternion rotation)
    {
        if (!firing)
        {
            transform.rotation = rotation;
        }
    }

    public void Reset()
    {
        firing = false;
        isAlive = false;
        rocketSound.Stop();
        gameObject.SetActive(false);
        transform.position = defaultPosition;        
    }

    public void Fire()
    {
        if (firing == false && isAlive == false)
        {            
            gameObject.SetActive(true);
            rocketSound.Play();
            firing = true;
            isAlive = true;
            rb.AddRelativeForce(new Vector3(0, 0.9f, 1f) * 500f);
        }
    }
}
