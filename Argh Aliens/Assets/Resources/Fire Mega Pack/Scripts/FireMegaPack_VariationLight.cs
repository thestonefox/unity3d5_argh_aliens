using UnityEngine;
using System.Collections;

public class UltraShader_VariationLight : MonoBehaviour {

	private Vector3 OriginPosition;
	private float OriginIntensity;
	public float VarianteIntensity=0.2f;
	public float VariantePosition=0.8f;
	public float VariationLightSpeed=0.05f;
	public float VariationPositionSpeed=0.05f;

	// Use this for initialization
	void Start () 
	{
		OriginIntensity = this.GetComponent<Light>().intensity;
		OriginPosition = this.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 oldv = this.transform.localPosition;
		float oldl = this.GetComponent<Light>().intensity;
		this.GetComponent<Light>().intensity = Mathf.MoveTowards (oldl, Random.Range (OriginIntensity - VarianteIntensity, OriginIntensity + VarianteIntensity),VariationLightSpeed);
		float vx=Random.Range (OriginPosition.x - VariantePosition, OriginPosition.x + VariantePosition);
		float vy=Random.Range (OriginPosition.y - VariantePosition, OriginPosition.y + VariantePosition);
		float vz=Random.Range (OriginPosition.z - VariantePosition, OriginPosition.z + VariantePosition);
		vx = Mathf.MoveTowards (oldv.x, vx,VariationPositionSpeed);
		vy = Mathf.MoveTowards (oldv.y, vy,VariationPositionSpeed);
		vz = Mathf.MoveTowards (oldv.z, vz,VariationPositionSpeed);
		this.transform.localPosition = new Vector3 (vx, vy, vz);
	}
}
