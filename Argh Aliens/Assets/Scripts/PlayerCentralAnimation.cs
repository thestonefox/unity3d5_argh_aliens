using UnityEngine;
using System.Collections;

public class PlayerCentralAnimation : MonoBehaviour {
    public float rotation = -180f;

    private float rotationAxisValue;

    void Update()
    {
        rotationAxisValue = Input.GetAxis("Horizontal");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float rotationSpeed = rotation - (rotationAxisValue * (rotation / 10));
        transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
    }
}
