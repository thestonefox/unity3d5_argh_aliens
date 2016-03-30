using UnityEngine;
using System.Collections;

public class PlayerWingAnimation : MonoBehaviour {
    public float rotation = 180f;

    private float rotationAxisValue;

    void Update()
    {
        rotationAxisValue = Input.GetAxis("Horizontal");
    }
	
	void FixedUpdate () {
        float rotationSpeed = rotation - (rotationAxisValue * (rotation/10));
        transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
    }
}
