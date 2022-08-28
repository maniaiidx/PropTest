using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_Spherecast : MonoBehaviour
{

    [Button(nameof(StartSphereCast), "StartSphereCast")]
    public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない
    void StartSphereCast()
    {
        Transform trans = GameObject.Find("Sphere_E").transform;
        RaycastHit[] hits = Physics.SphereCastAll(trans.position, trans.localScale.z, -transform.right, 100);

        List<RaycastHit> hitList = new List<RaycastHit>();

        foreach(RaycastHit hit in hits)
        {
            hitList.Add(hit);
        }
        hitList.Sort(CompareDistance);

        foreach (RaycastHit list in hitList)
        {
            //Debug.Log(list.collider.name);
            Debug.Log(list.textureCoord + "," + list.collider.name);
            //Debug.Log(list.distance);
        }


    }

    private static int CompareDistance(RaycastHit x , RaycastHit y)
    {
        if (x.distance > y.distance)
            return 1;
        else if (x.distance < y.distance)
            return -1;
        else // if (x.distance == y.distance)
            return 0;
    }

    //memo : Z軸（青色の矢印）を正面、x軸（赤色の矢印）を右、y軸（緑色の矢印）を上として表現します。

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
