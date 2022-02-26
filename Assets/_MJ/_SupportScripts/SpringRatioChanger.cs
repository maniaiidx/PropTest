using System.Collections;//IENumeratorを使う用
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;//DOTween

public class SpringRatioChanger : MonoBehaviour
{

    #region 変数

    DataCounter DC;

    UTJ.SpringManager //SpringManager取得（スカートなどの揺れモノのスクリプト）
        springManager;
    List<String> //スカートボーン一覧取得して引数で渡す用
        boneNamesList = new List<string>();

    //何らかの理由で強制的にONOFFする用
    public bool
        isForceSpring = false,
        isForceAnimation = false;

    [Range(0, 1)]
    public float
        springRatio = 1,
        animationRatio = 0;
    public float
        duration = 1;

    [HeaderAttribute("スプリング状態時にこのチェックがつく")]
    public bool //スプリングをONにするかOFFにするか
    isSpring = true;


    //■ボーンの回転
    //スカートのボーンの回転を取得して、アニメーション状態かデフォルト状態か監視する用
    public ReadTransforms[] readTransforms;
    [Serializable]
    public struct ReadTransforms
    {
        public Transform transform;
        public Vector3 defRot, nowAnimRot;
    }

    //■トリガー
    //スカートアニメ状態の読み取り トリガーボーン
    Transform
        SpringOFFTriggerTrs;


    //■ONOFFのTweener
    Tweener //DynamicRatio（揺れとモーションのブレンド具合）を変更するTween変数
        dynamicRatioTweener;
    bool //現在DynamicRatioが1に設定されているかどうか（Tweenerの切り替えなどに利用）
        isDynamicRatioOne = true;

    #endregion

    IEnumerator Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        springManager = GetComponent<UTJ.SpringManager>();
        //defRot取得
        for (int i = 0; i < readTransforms.Length; i++)
        { readTransforms[i].defRot = readTransforms[i].transform.localEulerAngles; }

        //取得できるようになるまで待ってから
        while(DC.GirlTrs == null) { yield return null; }
        //取得
        SpringOFFTriggerTrs = DC.GirlTrs.Find("SpringOFFTrigger");

        #region ■■■初期化

        #region SpringManagerにボーン名を渡し、DynamicRatio効く様にする
        //アニメーションするボーン名を入れる(Managerが一覧を持っていたのでForで取得)
        boneNamesList.Clear();
        for (int i = 0; i < springManager.springBones.Length; i++)
        { boneNamesList.Add(springManager.springBones[i].name); }

        //アニメーションと揺れのブレンドをするボーン名リストを入れる（これに引数で渡すとDynamicRatioが効く）
        springManager.UpdateBoneIsAnimatedStates(boneNamesList);

        #endregion
        //最初はスプリングの値に
        springManager.dynamicRatio = springRatio;

        #region ■Tweener初期設定
        dynamicRatioTweener =
        DOTween.To(() => springManager.dynamicRatio, (x) => springManager.dynamicRatio = x
        , 1
        , 1f)
        .Pause();
        #endregion

        #endregion

        yield break;
    }


    void LateUpdate()
    {
        //取得するまで待機
        if (SpringOFFTriggerTrs == null) { return; }

        #region スカートのBone、アニメで動いているか判定
        bool isTmpAllSame = true;//全部同じだったかどうか判定用

        for (int i = 0; i < readTransforms.Length; i++)
        {
            //nowRot取得
            readTransforms[i].nowAnimRot = readTransforms[i].transform.localEulerAngles;

            //defと違えばfalseにして抜け
            if (readTransforms[i].nowAnimRot != readTransforms[i].defRot)
            {
                isTmpAllSame = false;
                //break; //一応値確認のため抜けずに次のボーン数値出しておく
            }
        }

        //forの結果、trueのまま（全スカートボーン アニメで動いてない、デフォルト）だったら
        if (isTmpAllSame)
        { if (isSpring == false) { isSpring = true; } }
        //スカートボーンひとつでも動いていたら
        else
        { if (isSpring) { isSpring = false; } }

        #endregion

        #region SkirtBoneTriggerで判定
        if (SpringOFFTriggerTrs.localPosition.y < -0.01f)
        {
            if (isSpring) { isSpring = false; }
        }
        else if (SpringOFFTriggerTrs.localPosition.y > 0)
        {
            if (isSpring == false) { isSpring = true; }
        }
        //値が0（何もキーが打たれてないと仮定）の場合、何もしない（スカートのボーンの動きでの判定でいく））
        else if (SpringOFFTriggerTrs.localPosition.y == 0)
        { }
        #endregion

        #region 胴・足IK時はスプリングON
        if (DC.IKLFootEf.positionWeight != 0 ||
            DC.IKRFootEf.positionWeight != 0)
        {
            if (isSpring == false) { isSpring = true; }
        }

        #endregion

        #region 強制（isForce）がONの場合
        if (isForceAnimation)
        { if (isSpring) { isSpring = false; } }
        else if (isForceSpring)
        { if (isSpring == false) { isSpring = true; } }
        #endregion

        //＝＝ここまでが判定

        #region スカート揺れとモーション切り替えTweenerオンオフ
        //スカートアニメで動いていない時
        if (isSpring)
        {
            if (isDynamicRatioOne == false)//1フレだけ処理用
            {
                //trueにして
                isDynamicRatioOne = true;
                //Tweener更新 揺れON
                dynamicRatioTweener.Kill();
                dynamicRatioTweener =
                    DOTween.To(() => springManager.dynamicRatio, (x) => springManager.dynamicRatio = x
                    , springRatio
                    , duration)
                    .SetAutoKill(false);
            }
        }
        //スカートアニメで動いている時
        else if (isSpring == false)
        {
            if (isDynamicRatioOne)//1フレだけ処理用
            {
                //falseにして
                isDynamicRatioOne = false;
                //Tweener更新 揺れOFF
                dynamicRatioTweener.Kill();
                dynamicRatioTweener =
                    DOTween.To(() => springManager.dynamicRatio, (x) => springManager.dynamicRatio = x
                    , animationRatio
                    , duration)
                    .SetAutoKill(false);
            }
        }
        #endregion


#if UNITY_EDITOR
        #region 設定を即反映する用

        #region isForce切り替え

        if (isForceAnimation && isForceSpring)
        {
            if (isPreForceAnimation)
            {
                isForceAnimation = false;
                isForceSpring = true;
            }
            if (isPreForceSpring)
            {
                isForceAnimation = true;
                isForceSpring = false;
            }
        }
        //前フレの状態取得
        isPreForceAnimation = isForceAnimation;
        isPreForceSpring = isForceSpring;
        #endregion

        #region Ratio書き換え
        if (preSpringRatio != springRatio
            && isSpring)
        {
            //Tweener更新
            dynamicRatioTweener.Kill();
            dynamicRatioTweener =
                DOTween.To(() => springManager.dynamicRatio, (x) => springManager.dynamicRatio = x
                , springRatio
                , 0)
                .SetAutoKill(false);
        }
        else if (preAnimationRatio != animationRatio
            && isSpring == false)
        {
            //Tweener更新
            dynamicRatioTweener.Kill();
            dynamicRatioTweener =
                DOTween.To(() => springManager.dynamicRatio, (x) => springManager.dynamicRatio = x
                , animationRatio
                , 0)
                .SetAutoKill(false);
        }

        //前フレRatio取得
        preSpringRatio = springRatio;
        preAnimationRatio = animationRatio;
        #endregion

        #endregion
#endif
    }

#if UNITY_EDITOR
    #region 設定を即反映する用 変数
    bool
        isPreForceAnimation,
        isPreForceSpring;

    float
        preSpringRatio = 1,
        preAnimationRatio = 0;
    #endregion
#endif

}
