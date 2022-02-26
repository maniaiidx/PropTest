//using追加も自由です（もし追加したら、後で教えてほしいかも）
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using DG.Tweening;
using System.Collections.Generic; //Listに必要

//クラスは↓の通りにしてください。
public partial class DataCounter
{
    //■ここからコルーチンのメソッドを追加していきます

    #region ゴミ箱持ち関係
    //ゴミ箱持ちコルーチン開始コルーチン
    Coroutine RME_ChieriGomibakoMotionParentCor;
    public IEnumerator RME_ChieriGomibakoMotionParent()
    {
        //変数に格納しながらスタート（途中でやめられるように）
        RME_ChieriGomibakoMotionParentCor
            = StartCoroutine(RME_ChieriGomibakoMotionParentIEnum());
        yield break;
    }

    //ゴミ箱持ちコルーチン本体
    public IEnumerator RME_ChieriGomibakoMotionParentIEnum()
    {
        StartCoroutine(GirlAnimReadSystem());//アニメ情報読み込みコルーチン開始

        //ゴミ箱掴むアニメになるまで待ち
        float timeCountFlt = 0;
        while (nowGirlAnimClipName != "デスク横探しゴミ箱持ち上げ")
        {
            ////アニメ待ち中、1分過ぎたらおそらくイベント通り過ぎているので抜け
            //timeCountFlt += 1 * Time.deltaTime;
            //if (timeCountFlt > 60)
            //{ goto 抜け; }
            yield return null;
        }
        timeCountFlt = 0;

        //ゴミ箱掴むフレームまで待ち
        while (girlAnimNomTime <= 0.0f)
        {
            //フレーム待ち中、1分過ぎたらおそらくイベント通り過ぎているので抜け
            timeCountFlt += 1 * Time.deltaTime;
            if (timeCountFlt > 60)
            { goto 抜け; }
            yield return null;
        }

        //左手にペアレント
        GomibakoTrs.SetParent(Bip001_L_HandTrs);

        //位置合わせ
        GameObject GomibakoLHandParePosObj
            = Resources.Load("_PosObj/GomibakoLHandParePosObj") as GameObject;
        GomibakoTrs.localPosition = GomibakoLHandParePosObj.transform.localPosition;
        GomibakoTrs.localEulerAngles = GomibakoLHandParePosObj.transform.localEulerAngles;

        //微振動
        StartCoroutine(UTLDOShakePosition
            (3f
            , 0.05f
            , 300
            , 90 //Randomness
            , false //Snaping
            , true)
            );


        抜け:
        yield break;
    }

    //強制ゴミ箱持ち
    public IEnumerator RME_ChieriGomibakoMotiKyousei()
    {
        //持ち待ちコルーチンあったら停止
        if (RME_ChieriGomibakoMotionParentCor != null)
        { StopCoroutine(RME_ChieriGomibakoMotionParentCor); }

        //左手にペアレント
        GomibakoTrs.SetParent(Bip001_L_HandTrs);

        //位置合わせ
        GameObject GomibakoLHandParePosObj
            = Resources.Load("_PosObj/GomibakoLHandParePosObj") as GameObject;
        GomibakoTrs.localPosition = GomibakoLHandParePosObj.transform.localPosition;
        GomibakoTrs.localEulerAngles = GomibakoLHandParePosObj.transform.localEulerAngles;

        yield break;
    }
    #endregion

    //型はIEnumerator（コルーチンメソッド）　メソッド名は自由（RME_が付いてると、後で管理しやすいかも）

}





//メソッド名をタイムラインで指定すれば、そこで実行されます。

