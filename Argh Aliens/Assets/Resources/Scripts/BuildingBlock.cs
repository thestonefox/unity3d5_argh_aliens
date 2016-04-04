using UnityEngine;
using System.Collections;

public class BuildingBlock : MonoBehaviour {
    public Vector3 coords;
    public GameObject explosionParticles;
    public float buildingHeight;
    public GameObject peep;

    void OnCollisionEnter(Collision collision)
    {
        Destroy(true);
    }

    public void Destroy(bool setFire)
    {
        if (gameObject.activeSelf)
        {
            Instantiate(explosionParticles, gameObject.transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            if (peep != null)
            {
                peep.GetComponent<Peep>().Die();
            }

            if (setFire && coords.y > 0)
            {
                LevelManager.instance.SetFireBuildingBelow(coords);
            }

            if (coords.y < buildingHeight - 1)
            {
                LevelManager.instance.BlowUpBuildingAbove(coords);
            }

            GameManager.instance.buildingsDamaged++;
            if (coords.y == 0)
            {
                GameManager.instance.buildingsDestroyed++;
            }
        }
    }

    public void OnFire()
    {
        gameObject.transform.Find("Roof").GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f);
        gameObject.transform.Find("BuildingFire").gameObject.SetActive(true);
    }
}
