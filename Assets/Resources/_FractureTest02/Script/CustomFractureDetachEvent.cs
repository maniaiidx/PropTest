using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFractureDetachEvent : MonoBehaviour
{

    FracturedObject thisFracturedObject;

    //Generaterから付与される
    [HideInInspector]
    public CustomFractureActiveTriggerCollider customFractureActiveTriggerCollider;
    ////アニメコンポーネントあれば取得
    //CustomFractureAnimClipRecorder customFractureAnimClipRecorder;

    #region スポーン関係
    bool isOneceDetach = false;

    [System.Serializable]
    public struct SpawnObjs
    {
        public GameObject Obj;
        public float waitTime;
    }
    public SpawnObjs[] spawnObjs;

    #endregion

    #region コンポーネントオフ関係
    [Header("■接触後コンポーネント削除")]
    public float componentOffTime = 5;
    bool isComponentFullDelete = true;
    #endregion

    void Start()
    {
        Set();
    }
    public void Set()
    {
        thisFracturedObject = GetComponent<FracturedObject>();
        ////アニメコンポーネントあれば取得
        //if (GetComponent<CustomFractureAnimClipRecorder>())
        //{ customFractureAnimClipRecorder = GetComponent<CustomFractureAnimClipRecorder>(); }
    }

    void Update()
    {
        //シングルコリダーに触れた
        if (isOneceDetach == false && customFractureActiveTriggerCollider.isCrash)
        {
            isOneceDetach = true;
            //スポーンObj
            for (int i = 0; i < spawnObjs.Length; i++)
            { StartCoroutine(Spawn(spawnObjs[i].Obj, spawnObjs[i].waitTime)); }

            //■コンポーネントオフ
            ////アニメコンポーネントあってレコード状態なら。（アニメ状態なら元から消えてるのでなし）
            //if (customFractureAnimClipRecorder)
            //{
            //    if (customFractureAnimClipRecorder.isRecord)
            //    { StartCoroutine(ComponentOff()); }
            //}//（アニメコンポーネント自体なしでも実行）
            //else
            { StartCoroutine(ComponentOff()); }
        }
    }

    //スポーン命令
    IEnumerator Spawn(GameObject spawnObj, float waitTime)
    {
        if (spawnObj != null)
        {
            float time = 0;
            while (time < waitTime)
            {
                if (customFractureActiveTriggerCollider.isCrash)
                { time += 1 * Time.deltaTime; }
                //途中で戻ったらコルーチン終了
                else { yield break; }

                yield return null;
            }
            //オブジェ設置
            Instantiate(spawnObj, transform.parent, false);
        }
        yield break;
    }

    //コンポーネントオフ命令
    IEnumerator ComponentOff()
    {
        #region 待機
        float time = 0;
        while (time < componentOffTime)
        {
            if (customFractureActiveTriggerCollider.isCrash)
            { time += 1 * Time.deltaTime; }
            //途中で戻ったらコルーチン終了
            else { yield break; }

            yield return null;
        }
        #endregion

        #region■コンポーネントオフ
        ////デバッグメッセージ
        //if (isComponentFullDelete) { Debug.Log("ComponentFullDelete"); } else { Debug.Log("ComponentOff"); }

        for (int i = 0; i < thisFracturedObject.ListFracturedChunks.Count; i++)
        {
            //削除されてない時のみ
            if (thisFracturedObject.ListFracturedChunks[i])
            {
                //崩れず残留しているChunkのみ(isKinematicで判定（IsDetachedChunkはうまくいかない？）)
                if (thisFracturedObject.ListFracturedChunks[i].thisRigidBody.isKinematic)
                {
                    thisFracturedObject.ListFracturedChunks[i].enabled = false;
                    //thisFracturedObject.ListFracturedChunks[i].thisRigidBody.isKinematic = true;

                    //LocalGravity設定されていれば
                    if(thisFracturedObject.ListFracturedChunks[i].gameObject.GetComponent<LocalGravity>() != null)
                    {
                        thisFracturedObject.ListFracturedChunks[i].gameObject
                            .GetComponent<LocalGravity>().enabled = false;
                    }

                    if (isComponentFullDelete)
                    {
                        #region 完全オブジェクト化

                        Destroy(thisFracturedObject.ListFracturedChunks[i].gameObject
                            .GetComponent<Collider>());
                        Destroy(thisFracturedObject.ListFracturedChunks[i].gameObject
                            .GetComponent<LocalGravity>());
                        Destroy(thisFracturedObject.ListFracturedChunks[i].thisRigidBody);
                        Destroy(thisFracturedObject.ListFracturedChunks[i]);

                        //DieTimerはあったりなかったり多かったりなのでGetComponent"s"で削除
                        for (int k = 0; k < thisFracturedObject.ListFracturedChunks[i].gameObject.GetComponents<UltimateFracturing.DieTimer>().Length; k++)
                        {
                            Destroy(thisFracturedObject.ListFracturedChunks[i].gameObject.GetComponents<UltimateFracturing.DieTimer>()[k]);
                        }
                        #endregion
                    }
                }
            }
        }

        thisFracturedObject.enabled = false;

        if (isComponentFullDelete)
        {
            #region 完全オブジェクト化
            Destroy(thisFracturedObject.gameObject.GetComponent<CustomFracturedSetLocalGravity>());
            Destroy(thisFracturedObject.gameObject.GetComponent<CustomFractureActiveTriggerColliderGenerater>());
            Destroy(thisFracturedObject.gameObject.GetComponent<CustomFractureDetachEvent>());
            Destroy(thisFracturedObject);
            #endregion
        }
        #endregion

        yield break;
    }

}
