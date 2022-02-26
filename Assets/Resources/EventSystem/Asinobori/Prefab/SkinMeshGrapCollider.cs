using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinMeshGrapCollider : MonoBehaviour {

    //public DataCounter DC;
    //void Awake()
    //{
    //    //List読み込むために取得
    //    DC = GameObject.Find("Server").GetComponent<DataCounter>();
    //}

    public List<Collider>
        stayColliderList = new List<Collider>();

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "ChieriCollider")
        {
            if (stayColliderList.Contains(collider) == false)
            {
                stayColliderList.Add(collider);
            }
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "ChieriCollider")
        {
            if (stayColliderList.Contains(collider))
            {
                stayColliderList.Remove(collider);
            }
        }

    }


    //void Start () {

    //}

    //void Update () {

    //}
}
