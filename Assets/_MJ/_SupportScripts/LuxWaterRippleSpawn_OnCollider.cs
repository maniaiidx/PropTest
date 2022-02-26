using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuxWaterRippleSpawn_OnCollider : MonoBehaviour
{
    public Collider targetCollider;
    public GameObject spawnObj;
    public float spawnHeight = 30;
    public float spawnTempo = 2;
    [SerializeField, HeaderAttribute("↑の秒数毎、↓の分だけ動いてたら生成")]
    public float spawnDistance;

    //Stay中一定以上動いてたらSpawnするよう変数
    float stayTimeCountFlt;
    Vector3 prePos = new Vector3();

    //当たったら生成
    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("EnterCol");
        if (col == targetCollider)
        {
            Debug.Log("EnterColTarget");

            //ぶつかった場所取得（Triggerの場合は大体の場所しか取れないらしい）（そして結局Obj位置な感じ）
            Vector3 tmpPos = col.ClosestPoint(this.transform.position);

            //ぶつかった場所で、高さは指定した高さで生成
            GameObject.Instantiate(spawnObj
                , new Vector3(tmpPos.x, spawnHeight, tmpPos.z)
                , spawnObj.transform.rotation);
            //前回の位置更新
            prePos = transform.position;
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col == targetCollider)
        {
            //カウント
            stayTimeCountFlt += Time.deltaTime;

            //カウントが指定値越えたら
            if (stayTimeCountFlt >= spawnTempo)
            {
                Debug.Log("ColStayCount");
                //前回の距離より指定値動いていたら
                if (Vector3.Distance(transform.position, prePos) >= spawnDistance)
                {
                    Debug.Log("ColStayPos" + Vector3.Distance(transform.position, prePos));

                    //場所取得（Triggerの場合は大体の場所しか取れないらしい）（そして結局Obj位置な感じ）
                    Vector3 tmpPos = col.ClosestPointOnBounds(this.transform.position);

                    //その場所で、高さは指定した高さで生成
                    GameObject.Instantiate(spawnObj
                        , new Vector3(tmpPos.x, spawnHeight, tmpPos.z)
                        , spawnObj.transform.rotation);
                }

                //カウントリセット
                stayTimeCountFlt = 0;
                //前回の位置更新
                prePos = transform.position;
            }

        }

    }

    private void OnTriggerExit(Collider col)
    {

        Debug.Log("ExitCol");
        if (col == targetCollider)
        {
            Debug.Log("ExitColTarget");

            //ぶつかった場所取得（Triggerの場合は大体の場所しか取れないらしい）（そして結局Obj位置な感じ）
            Vector3 tmpPos = col.ClosestPoint(this.transform.position);

            //ぶつかった場所で、高さは指定した高さで生成
            GameObject.Instantiate(spawnObj
                , new Vector3(tmpPos.x, spawnHeight, tmpPos.z)
                , spawnObj.transform.rotation);

            //カウントリセット
            stayTimeCountFlt = 0;
        }

    }

}
