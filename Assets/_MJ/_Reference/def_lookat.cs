using UnityEngine;
using System.Collections;

public class def_lookat : MonoBehaviour {

    public Transform eyeTarget;
    public Vector3 addEulerAngles;
    // Update is called once per frame

    void Start() {
      //  addEulerAngles.x = 81.31f;
    }

    void Update()
    {
        transform.LookAt(eyeTarget);


        /*
        //回転させない軸設定
        var newRotation = Quaternion.LookRotation(eyeTarget.transform.position - transform.position).eulerAngles;
        newRotation.x = 0;
        newRotation.z = 0;
        transform.rotation = Quaternion.Euler(newRotation);
        */
    }

    void LateUpdate(){
        transform.eulerAngles += addEulerAngles;


    }
}

