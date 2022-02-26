using UnityEngine;
using System.Collections;

public class HwEnemyGimicRhythmUp : MonoBehaviour
{
    public DataCounter DC;


    //■※消滅時の処理はHwEnemyに

    //ギミックリセット用のデータを読み取っておいて、HwEnemyObj再起時にリセット
    public GameObject HwEnemyObj;//オブジェクト取得

    //ブロック崩しムーブ用変数
    float
        moveX = 6f, moveY = 6f;
    bool
        isHwPointStay;

    void Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        HwEnemyObj = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        //HwEnemyアクティブなら //当たっているときは動かない
        if (HwEnemyObj.activeSelf == true &&
            isHwPointStay == false)
        {
            transform.Translate(moveX * Time.deltaTime, moveY * Time.deltaTime, 0);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "ShopStgBar" || col.name == "MainStgBar")
        { moveY = -moveY; }
        if (col.name == "NextStgBar" || col.name == "PreStgBar")
        { moveX = -moveX; }

        Debug.Log(col.name);

        if (col.name.IndexOf("HwPoint") >= 0)
        { isHwPointStay = true; }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.name.IndexOf("HwPoint") >= 0)
        { isHwPointStay = false; }

    }

}

