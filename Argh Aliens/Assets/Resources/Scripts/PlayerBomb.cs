using UnityEngine;
using System.Collections;

public class PlayerBomb : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.parent != null && collision.collider.transform.parent.name.Contains("BuildingTop"))
        {
            BuildingBlock buildingTop = collision.collider.transform.parent.GetComponent<BuildingBlock>();
            buildingTop.Destroy(false);
            LevelManager.instance.SetFireBuildingBelow(buildingTop.coords);
        }
        gameObject.SetActive(false);
    }
}
