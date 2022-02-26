using UnityEngine;
using System.Collections;

public class visibleQuaternion : MonoBehaviour {

    public Quaternion thisQ;

    // Use this for initialization
    void Start () {
        thisQ = transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {

        thisQ = transform.rotation;

	}
}
