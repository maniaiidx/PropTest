using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenCloseSE : MonoBehaviour
{
    public DataCounter DC;
    IEnumerator Start()
    {
        while (GameObject.Find("Server") == null) { yield return new WaitForSeconds(0.5f); }
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        yield break;
    }

    //ドアレバー音
    void SE_DoorOpen()
    {
        DC.SEPlay(DC.DoorSEObj,"DoorOpen_p-10", this.gameObject);
    }
    //ドア閉まる音
    void SE_DoorClose()
    {
        DC.SEPlay(DC.DoorSEObj,"DoorClose_p-10", this.gameObject);
    }
}
