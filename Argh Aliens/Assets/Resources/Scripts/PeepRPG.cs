using UnityEngine;
using System.Collections;

public class PeepRPG : MonoBehaviour {
    bool isAlive;
    Vector3 playerPosition;
    Vector3 look;
    int fireTime;
    GameObject rocket;
    float shootHeightDifference = 10f;
    float initialFireDelay = 250f;

    void Awake()
    {
        isAlive = true;
    }

    void Start()
    {
        GameObject baseRocket = gameObject.transform.Find("Rocket").gameObject;
        rocket = Instantiate(baseRocket, baseRocket.transform.position, Quaternion.identity) as GameObject;
        rocket.transform.parent = null;
        rocket.GetComponent<PeepRocket>().SetPosition(baseRocket.transform.position);
        rocket.GetComponent<PeepRocket>().Reset();
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.name == "ShipBody" && collider.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.GetComponent<Player>().landed && isAlive)
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

    void Update()
    {
        if (initialFireDelay > 0)
        {
            initialFireDelay--;
        }

        fireTime = Random.Range(0, 35);
        if (initialFireDelay <= 0 && fireTime == 1 && RightHeightToShoot())
        {
            rocket.GetComponent<PeepRocket>().Fire();
        }
    }

    void FixedUpdate()
    {
        playerPosition = LevelManager.instance.GetPlayerPosition();
        look = playerPosition - transform.position;
        look.y = 0f;
        transform.rotation = Quaternion.LookRotation(look);
        rocket.GetComponent<PeepRocket>().SetRotation(transform.rotation);
        transform.Rotate(0f, -90f, 0f);
    }


    bool RightHeightToShoot()
    {
        Vector3 playerPos = LevelManager.instance.GetPlayerPosition();

        return ((playerPos.y > transform.position.y) && (playerPos.y < transform.position.y + shootHeightDifference));
    }
}
