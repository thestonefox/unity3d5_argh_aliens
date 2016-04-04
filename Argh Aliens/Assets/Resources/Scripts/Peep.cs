using UnityEngine;
using System.Collections;

public class Peep : MonoBehaviour {

    bool isAlive;

    Transform leftArm;
    Transform rightArm;
    Transform leftLeg;
    Transform rightLeg;

    Vector2 armRotationLimits = new Vector2(45f, 175f);
    float armRotation = 0;
    bool armUp = true;

    Vector2 legRotationLimits = new Vector2(0f, 25f);
    float legRotation = 0;
    bool legUp = true;

    float originalYPos;

    Vector2 jumpLimits = new Vector2(0f, 0.07f);
    bool jumpUp = true;
    float jumpAmount = 0;

    void Awake()
    {
        leftArm = gameObject.transform.Find("BodyParts").gameObject.transform.Find("LeftArm");
        rightArm = gameObject.transform.Find("BodyParts").gameObject.transform.Find("RightArm");

        leftLeg = gameObject.transform.Find("BodyParts").gameObject.transform.Find("LeftLeg");
        rightLeg = gameObject.transform.Find("BodyParts").gameObject.transform.Find("RightLeg");

        originalYPos = gameObject.transform.localPosition.y;
        isAlive = true;
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.name == "ShipBody" && collider.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.GetComponent<Player>().landed)
        {
            Capture();
        }
    }

    public void Die()
    {
        GameManager.instance.peepsKilled++;
        Death(true);
    }

    public void Capture()
    {
        GameManager.instance.peepsCaptured++;
        Death(false);
    }

    void Death(bool died)
    {
        if (isAlive)
        {
            LevelManager.instance.GotPeep(died);
        }
        isAlive = false;
        gameObject.transform.Find("Blood").gameObject.SetActive(true);
        gameObject.transform.Find("BodyParts").gameObject.SetActive(false);        
        Invoke("HideDead", 1f);
    }

    void HideDead()
    {
        gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        MoveBody();
        Jump();
    }

    void MoveBody()
    {
        RotateBodyPart(ref leftArm, ref rightArm, 3, ref armUp, armRotationLimits, ref armRotation);
        RotateBodyPart(ref leftLeg, ref rightLeg, 2, ref legUp, legRotationLimits, ref legRotation);
    }

    void Jump()
    {
        jumpAmount = TweenMotion(0.01f, ref jumpUp, jumpLimits, jumpAmount);
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, originalYPos + jumpAmount, gameObject.transform.localPosition.z);
    }

    void RotateBodyPart(ref Transform left, ref Transform right, float speed, ref bool direction, Vector2 limits, ref float rotation)
    {
        rotation = TweenMotion(speed, ref direction, limits, rotation);

        left.localRotation = Quaternion.Euler(rotation, 0f, 0f);
        right.localRotation = Quaternion.Euler(-rotation, 0f, 0f);
    }

    float TweenMotion(float speed, ref bool direction, Vector2 limits, float amount)
    {
        if (direction)
        {
            amount += speed;
            if (amount + limits.x >= limits.y)
            {
                direction = false;
            }
        }
        else
        {
            amount -= speed;
            if (amount + limits.x <= limits.x)
            {
                direction = true;
            }
        }

        if (amount < limits.x)
        {
            amount = limits.x;
            direction = true;
        }

        if (amount > limits.y)
        {
            amount = limits.y;
            direction = false;
        }
        return amount;
    }
}
