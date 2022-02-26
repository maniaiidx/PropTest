using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChieriMatagiColScript : MonoBehaviour
{

    public DataCounter DC;

    void Start()
    {
        //衝突フラグを送る為に取得
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
    }


    void OnTriggerEnter(Collider col)
    {
        //Playerタグのコリダーが入ったらtrue
        if (col.tag == "kakurenboMatagiObj")
        {
            if (DC.KO_ChieriMatagiColOnTriggerStayBool == false) { DC.KO_ChieriMatagiColOnTriggerStayBool = true; }
        }
    }
    void OnTriggerExit(Collider col)
    {
        //Playerタグのコリダーが出たらfalse
        if (col.tag == "kakurenboMatagiObj")
        {
            if (DC.KO_ChieriMatagiColOnTriggerStayBool == true) { DC.KO_ChieriMatagiColOnTriggerStayBool = false; }
        }
    }

}