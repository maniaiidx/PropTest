using UnityEngine;
using System.Collections;

public class eye_lookat : MonoBehaviour {

    public bool lookAtOn = true;
    public Transform eyeL, eyeR, eyeTarget, testCube;


    public Quaternion _defaultRotationL;
    public Quaternion _defaultRotationR;

    // Use this for initialization
    void Start()
        {

        _defaultRotationL = eyeL.localRotation;
        _defaultRotationR = eyeR.localRotation;

        eyeTarget = GameObject.Find("eyeTarget").transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (lookAtOn) { 
            eyeL.LookAt(eyeTarget, -Vector3.right);
            eyeR.LookAt(eyeTarget, -Vector3.right);
            testCube.LookAt(eyeTarget);


            eyeL.LookAt(eyeTarget, -Vector3.right);
            eyeL.localRotation *= this._defaultRotationL;

            eyeR.LookAt(eyeTarget, -Vector3.right);
            eyeR.localRotation *= this._defaultRotationR;

        }
    }
}
