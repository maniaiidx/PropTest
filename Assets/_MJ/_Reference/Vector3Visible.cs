using UnityEngine;
using System.Collections;

public class Vector3Visible : MonoBehaviour {

    public Vector3
        position, localPosition, eulerAngles, localEulerAngles;

    public MeshFilter meshFilter;
    public Vector3[] meshs, worldMeshs;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        position = gameObject.transform.position;
        localPosition = gameObject.transform.localPosition;
        eulerAngles = gameObject.transform.eulerAngles;
        localEulerAngles = gameObject.transform.localEulerAngles;
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshs = meshFilter.mesh.vertices;
    }
}
