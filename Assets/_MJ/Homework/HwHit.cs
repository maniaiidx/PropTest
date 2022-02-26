using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;//正規表現を使うRegexを使うのに必要

public class HwHit : MonoBehaviour
{
    public DataCounter DC;

    void Awake()
    {
        //衝突フラグを送る為に取得
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
    }

    // ぶつかったら
    void OnTriggerEnter(Collider col)
    { DC.HwPointOnTriggerEnter(col); }

    void OnTriggerExit(Collider col)
    { DC.HwPointOnTriggerExit(col); }
}
