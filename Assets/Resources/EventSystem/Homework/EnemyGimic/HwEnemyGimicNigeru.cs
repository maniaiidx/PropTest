using UnityEngine;
using System.Collections;

public class HwEnemyGimicNigeru : MonoBehaviour
{
    public DataCounter DC;

    //ギミックリセット用のデータを読み取っておいて、HwEnemyObj再起時にリセット
    public GameObject HwEnemyObj;//オブジェクト取得
    public bool hwEnemyActiveCheckBool;//HwEnemy再起時フレームだけリセットを実行する用

    //リセットする変数
    public Vector3 defPos;
    public bool hwGimicStageMoveBarTouchBool;

    void Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        HwEnemyObj = transform.GetChild(1).gameObject;
        defPos = transform.position;
    }

    void Update()
    {
        #region//HwEnemyのアクティブ状態のON時フレームだけリセットを実行（手製OnEnable）
        //HwEnemyアクティブならtrue
        if (hwEnemyActiveCheckBool == false && HwEnemyObj.activeSelf == true)
        {
            hwEnemyActiveCheckBool = true;

            //リセット
            transform.position = defPos;
            hwGimicStageMoveBarTouchBool = false;
        }

        //非アクティブならfalse
        if (hwEnemyActiveCheckBool == true && HwEnemyObj.activeSelf == false)
        {
            hwEnemyActiveCheckBool = false;
        }
        #endregion        
    }

    void OnTriggerStay(Collider col)
    {
        //追いかけるスピードをマイナスで逃げる
        if (HwEnemyObj.activeSelf == true)
        { DC.HwEnemyGimicOikakeru(this.gameObject.transform, col, -0.05f, ref hwGimicStageMoveBarTouchBool); }
    }


}

