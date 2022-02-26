using UnityEngine;
using System.Collections;

public class quaternionCopy : MonoBehaviour {

    public GameObject chieriEyeCube;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = chieriEyeCube.transform.rotation;
	}
}
