using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChieriFootstepDecalAreaColl : MonoBehaviour
{

    public DataCounter DC;
    void Awake()
    { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }

    //ちえりの足コリダー左右が入ってるか判定を取っているだけ

    public bool
        isRFootStay,
        isLFootStay;

    void OnTriggerEnter(Collider collider)
    {
        if (isRFootStay == false)
        {
            if (collider.name == "Tsumasaki_R_coll_Convex")
            {
                isRFootStay = true;
            }
        }
        if (isLFootStay == false)
        {
            if (collider.name == "Tsumasaki_L_coll_Convex")
            {
                isLFootStay = true;
            }
        }

    }
    //void OnTriggerStay(Collider collider)
    //{
    //}
    void OnTriggerExit(Collider collider)
    {
        if (isRFootStay)
        {
            if (collider.name == "Tsumasaki_R_coll_Convex")
            {
                isRFootStay = false;
            }
        }
        if (isLFootStay)
        {
            if (collider.name == "Tsumasaki_L_coll_Convex")
            {
                isLFootStay = false;
            }
        }
    }

}
