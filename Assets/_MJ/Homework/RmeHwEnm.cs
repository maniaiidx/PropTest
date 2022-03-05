using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;//DOTween

public class RmeHwEnm : MonoBehaviour
{

    DataCounter DC;
    void Awake()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();

        thisDefMat = GetComponent<Renderer>().material;
        thisDefColor = GetComponent<Renderer>().material.color;
    }



    public float
        hp = 3;
    [Header("ダメージのテンポ（何秒見続けたら1ダメージ与えるか）")]
    public float
        damageTempo = 1;

    [HideInInspector]
    public float //現在見られ続けている秒
        nowSeeCount;
    [HideInInspector]
    public Vector3 //見られている位置保持用（ヒット位置）
        nowSeePos;


    [Header("消滅時に別Objectに交換するなら設定")]
    public GameObject DestroyChangeObj;
    [Header("交換するObj名を指定するなら設定")]
    public string DestroyChangeObjName;


    [Header("ダメージ音")]
    public AudioClip
        damageAudClip;
    [Header("消滅音")]
    public AudioClip
        destroyAudClip;

    Material thisDefMat;
    Color thisDefColor;

    //ダメージカウント処理(DC側から送信される)
    public void damageCount()
    {
        //カウントアップ
        nowSeeCount += 1 * Time.deltaTime;
        //カウントがテンポ越えてたらHP削り処理
        if (nowSeeCount >= damageTempo)
        {
            StartCoroutine(SubHP());
            //カウントリセット
            nowSeeCount = 0;
        }
    }

    //HP削り処理
    IEnumerator SubHP()
    {
        hp--;
        //HP0以下になったら消滅処理へ
        if (hp <= 0)
        { delEnm(); }
        else
        {
            //ダメージ音
            if (damageAudClip != null)
            { DC.SEPlay(DC.OtherSEObj, damageAudClip, transform.position); }

            //ダメージ色
            thisDefMat.color = Color.white;
            thisDefMat.DOColor(thisDefColor, 0.3f);
        }

        yield break;
    }

    //消滅処理
    void delEnm()
    {
        //交換するObj指定あったら
        if (DestroyChangeObj != null)
        {
            //同じ場所に生成してから削除
            var tmpObj =
                Instantiate(DestroyChangeObj, transform.parent, true);
            tmpObj.transform.localPosition = transform.localPosition;

            //名前指定あったら設定
            if (!string.IsNullOrWhiteSpace(DestroyChangeObjName))
            { tmpObj.name = DestroyChangeObjName; }
        }

        //消滅音
        if (destroyAudClip != null)
        { DC.SEPlay(DC.OtherSEObj, destroyAudClip, transform.position); }

        //DC側のListから削除
        DC.RMEHW_enmObjList.Remove(gameObject);
        DC.RMEHW_nowLookEnmObjList.Remove(gameObject);
        //消滅
        Destroy(gameObject);

        Debug.Log("ヒエラルキーに存在する敵の数 " + DC.RMEHW_enmObjList.Count);

    }




}

