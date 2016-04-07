using UnityEngine;
using System.Collections;

public class PlayerCentralAnimation : MonoBehaviour {
    public float rotation = -180f;
    public bool pauseOnLanding;
    public bool animOnly;

    private float rotationAxisValue;

    void Update()
    {
        if (!animOnly)
        {
            rotationAxisValue = Input.GetAxis("Horizontal");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (animOnly || !pauseOnLanding || !gameObject.transform.parent.gameObject.transform.parent.GetComponent<Player>().landed)
        {
            float rotationSpeed = rotation - (rotationAxisValue * (rotation / 10));
            transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
        }
    }
}
