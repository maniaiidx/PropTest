using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAreaTrigger : MonoBehaviour
{

    public DataCounter DC;
    public int listIndexInt;
    void Awake()
    {
        //List読み込むために取得
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
    }


    void OnTriggerEnter(Collider coll)
    {
        //判定取るコリダーだったら
        if (coll.tag == "HitReloadSkinMeshCollider")
        {
            //true
            DC.RSMC_isMeshReloadList[listIndexInt] = true;
        }
        //Debug.Log(listIndexInt + coll.tag + "Enter");
    }
    void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "HitReloadSkinMeshCollider")
        {
            //falseにして、メッシュコリダーのメッシュも外す（暫定的）
            DC.RSMC_isMeshReloadList[listIndexInt] = false;
            DC.RSMC_MeshColliderList[listIndexInt].sharedMesh = null;
        }
        //Debug.Log(listIndexInt + coll.tag + "Exit");
    }


    //void Start () {

    //}

    //void Update () {

    //}
}
