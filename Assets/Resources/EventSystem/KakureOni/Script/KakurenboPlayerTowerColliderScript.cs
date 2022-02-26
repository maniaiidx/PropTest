using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KakurenboPlayerTowerColliderScript : MonoBehaviour
{

    public DataCounter DC;
    public string colObjNameString;
    void Start()
    {
        //衝突フラグを送る為に取得
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
    }


    void OnTriggerEnter(Collider col)
    {
        if(LayerMask.LayerToName(col.gameObject.layer) == "seeRayBlock")
        {
            if (colObjNameString != col.transform.parent.parent.name)
            {
                colObjNameString = col.transform.parent.parent.name;
            }
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col == null)
        {
            colObjNameString = null;
        }
    }

}