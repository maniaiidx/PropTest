using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsinoboriRHandAreaColl : MonoBehaviour
{

    //handGrapタグとぶつかったらリストに追加 離れれば削除

    public DataCounter DC;

    void Start()
    { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }
    
    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "handGrap")
        { DC.RHandAreaContactGrapColliderList.Add(coll); }
    }
    void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "handGrap")
        { DC.RHandAreaContactGrapColliderList.Remove(coll); }
    }

}
