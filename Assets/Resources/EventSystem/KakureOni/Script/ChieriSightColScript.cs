using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChieriSightColScript : MonoBehaviour
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
        if (col.tag == "Player")
        {
            if (DC.KO_isChieriSightColOnTriggerStay == false) { DC.KO_isChieriSightColOnTriggerStay = true; }
        }
    }
    void OnTriggerExit(Collider col)
    {
        //Playerタグのコリダーが出たらfalse
        if (col.tag == "Player")
        {
            if (DC.KO_isChieriSightColOnTriggerStay == true) { DC.KO_isChieriSightColOnTriggerStay = false; }
        }
    }

}
