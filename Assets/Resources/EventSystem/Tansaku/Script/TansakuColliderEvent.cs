using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TansakuColliderEvent : MonoBehaviour
{

    DataCounter DC;
    IEnumerator Start()
    {
        while (GameObject.Find("Server") == null) { yield return new WaitForSeconds(0.5f); }
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        yield break;
    }


    IEnumerator OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            //もしイベント中なら終了まで待機
            while (DC.isTansakuEnter) { yield return null; }

            //メソッドにObj送信
            DC.TansakuEnter(this.gameObject, DataCounter.RayOrColl.Enter);
        }
        yield break;
    }
    IEnumerator OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            //もしイベント中なら終了まで待機
            while (DC.isTansakuEnter) { yield return null; }
            
            //メソッドにObj送信
            DC.TansakuEnter(this.gameObject, DataCounter.RayOrColl.Exit);
        }
        yield break;
    }

}
