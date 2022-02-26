using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairMoveSE : MonoBehaviour
{
    public DataCounter DC;
    public DataBridging DB;
    IEnumerator Start()
    {
        while(GameObject.Find("Server") == null) { yield return new WaitForSeconds(0.5f); }
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        while (GameObject.Find("DataBridging") == null) { yield return new WaitForSeconds(0.5f); }
        DB = GameObject.Find("DataBridging").GetComponent<DataBridging>();
        yield break;
    }


    //イベント名で鳴らし分け

    //長め
    void SE_ChairMove01()
    {
        if (
            DC.evs[DB.nowEventNum].Key == "椅子へ飛び降り"
            || DC.evs[DB.nowEventNum].Key == "これからの事"
            )
        { DC.SEPlay("ChairMove01_ピッチ-20_time", this.gameObject); }
        else { DC.SEPlay("ChairMove01", this.gameObject); }
    }
    //短い
    void SE_ChairMove02()
    {
        if (
            DC.evs[DB.nowEventNum].Key == "椅子へ飛び降り"
            || DC.evs[DB.nowEventNum].Key == "これからの事"
            )
        { DC.SEPlay("ChairMove02_ピッチ-20_time", this.gameObject); }
        else { DC.SEPlay("ChairMove02", this.gameObject); }
    }
    //ちょっと短い
    void SE_ChairMove03()
    {
        if (
            DC.evs[DB.nowEventNum].Key == "椅子へ飛び降り"
            || DC.evs[DB.nowEventNum].Key == "これからの事"
            )
        { DC.SEPlay("ChairMove03_ピッチ-20_time", this.gameObject); }
        else { DC.SEPlay("ChairMove03", this.gameObject); }
    }

    //座り音
    void SE_ChairSitDown01() { DC.SEPlay("futon-dive1", this.gameObject); }

}
