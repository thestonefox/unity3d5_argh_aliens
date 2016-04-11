using UnityEngine;
using System.Collections;

public class PlayerBomb : MonoBehaviour {

    public AudioClip explosionSound;
    public AudioClip dropSound;

    private AudioSource source;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        source.PlayOneShot(dropSound);
        gameObject.transform.Find("Explosion").gameObject.transform.position = gameObject.transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        source.Stop();
        source.PlayOneShot(explosionSound);
        if (collision.collider.transform.parent != null && collision.collider.transform.parent.name.Contains("BuildingTop"))
        {
            BuildingBlock buildingTop = collision.collider.transform.parent.GetComponent<BuildingBlock>();
            buildingTop.Destroy(false);
            LevelManager.instance.SetFireBuildingBelow(buildingTop.coords);
        }

        rb.isKinematic = true;
        ToggleMesh(false);
        gameObject.transform.Find("Explosion").gameObject.SetActive(true);
        Invoke("Disable", 0.5f);
    }

    void Disable()
    {
        gameObject.transform.Find("Explosion").gameObject.SetActive(false);
        gameObject.SetActive(false);
        rb.isKinematic = false;
        ToggleMesh(true);
    }

    void ToggleMesh(bool on)
    {
        foreach (MeshRenderer mesh in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = on;
        }
    }
}
