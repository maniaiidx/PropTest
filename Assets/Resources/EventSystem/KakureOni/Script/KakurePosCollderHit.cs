using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KakurePosCollderHit : MonoBehaviour
{
    //触れると消える・一度でも触れたかどうかでObj変更スクリプト

    DataCounter DC;

    Renderer thisRenderer;

    GameObject //パーティクルの親Obj群
        RootNaviEffect_ExObj,
        RootNaviEffect_ExGreenObj,
        RootNaviEffect_ExStopObj,
        RootNaviEffect_SquareObj,
        RootNaviEffect_SquareGreenObj,
        RootNaviEffect_SquareStopObj;

    List<Transform> //触れたら消えるパーティクル子Obj群（触れたらExとSquareを入れ替える）
        particleTrsList = new List<Transform>();

    public bool tocuh = false;

    void Awake()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        thisRenderer = gameObject.GetComponent<Renderer>();
        
        #region パーティクルObj(変換されたもの)が既にあれば削除
        if (transform.Find("RootNaviEffect") != null)
        { Destroy(transform.Find("RootNaviEffect").gameObject); }
        if (transform.Find("RootNaviEffectGreen") != null)
        { Destroy(transform.Find("RootNaviEffectGreen").gameObject); }
        if (transform.Find("RootNaviEffectStop") != null)
        { Destroy(transform.Find("RootNaviEffectStop").gameObject); }
        #endregion

        #region パーティクルExObjがあれば取得
        if (transform.Find("RootNaviEffect_Ex") != null)
        { RootNaviEffect_ExObj = transform.Find("RootNaviEffect_Ex").gameObject; }
        if (transform.Find("RootNaviEffect_ExGreen") != null)
        { RootNaviEffect_ExGreenObj = transform.Find("RootNaviEffect_ExGreen").gameObject; }
        if (transform.Find("RootNaviEffect_ExStop") != null)
        { RootNaviEffect_ExStopObj = transform.Find("RootNaviEffect_ExStop").gameObject; }
        #endregion
        #region パーティクルSquareObjがあれば取得
        if (transform.Find("RootNaviEffect_Square") != null)
        { RootNaviEffect_SquareObj = transform.Find("RootNaviEffect_Square").gameObject; }
        if (transform.Find("RootNaviEffect_SquareGreen") != null)
        { RootNaviEffect_SquareGreenObj = transform.Find("RootNaviEffect_SquareGreen").gameObject; }
        if (transform.Find("RootNaviEffect_SquareStop") != null)
        { RootNaviEffect_SquareStopObj = transform.Find("RootNaviEffect_SquareStop").gameObject; }
        #endregion

        //tocuh状況で動作するパーティクルObj切り替えメソッド実行
        if (DC.KO_isParticleObjMode)
        { ParticleObjChange(tocuh); }
    }

    //void Update()
    //{
    //    DC.nearSizeAjust(transform, 800);
    //}
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == DC.CameraObjectsTrs.gameObject)
        {
            //初回触れならbool切り替え
            if (tocuh == false)
            {
                tocuh = true;
                //パーティクルモードならObj切り替えメソッド実行
                if (DC.KO_isParticleObjMode)
                { ParticleObjChange(tocuh); }
            }

            #region パーティクルモードなら子TrsでactiveOff
            //アクティブオフ
            if (DC.KO_isParticleObjMode)
            {
                for (int i = 0; i < particleTrsList.Count; i++)
                { particleTrsList[i].gameObject.SetActive(false); }
            }
            #endregion
            #region 従来のスフィア(RendererのTrueFalse)
            else
            { thisRenderer.enabled = false; }
            #endregion
            ////※レイヤー変更なんであったか思い出せない
            //gameObject.layer = LayerMask.NameToLayer("seeRayKakurePos");
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == DC.CameraObjectsTrs.gameObject)
        {
            #region パーティクルモードなら子TrsでactiveON
            if (DC.KO_isParticleObjMode)
            {
                for (int i = 0; i < particleTrsList.Count; i++)
                { particleTrsList[i].gameObject.SetActive(true); }
            }
            #endregion
            #region 従来のスフィア(RendererのTrueFalse)
            else
            { thisRenderer.enabled = true; }
            #endregion
            ////※レイヤー変更なんであったか思い出せない
            //gameObject.layer = LayerMask.NameToLayer("seeRayBlock");
        }
    }

    void ParticleObjChange(bool SquareMain = false)
    {
        if (SquareMain)
        {
            #region 名前変更
            RootNaviEffect_ExObj.name = "RootNaviEffect_Ex";
            RootNaviEffect_ExGreenObj.name = "RootNaviEffect_ExGreen";
            RootNaviEffect_ExStopObj.name = "RootNaviEffect_ExStop";

            RootNaviEffect_SquareObj.name = "RootNaviEffect";
            RootNaviEffect_SquareGreenObj.name = "RootNaviEffectGreen";
            RootNaviEffect_SquareStopObj.name = "RootNaviEffectStop";
            #endregion
            #region アクティブ状況を交換
            RootNaviEffect_SquareObj.SetActive(RootNaviEffect_ExObj.activeSelf);
            RootNaviEffect_SquareGreenObj.SetActive(RootNaviEffect_ExGreenObj.activeSelf);
            RootNaviEffect_SquareStopObj.SetActive(RootNaviEffect_ExStopObj.activeSelf);

            RootNaviEffect_ExObj.SetActive(false);
            RootNaviEffect_ExGreenObj.SetActive(false);
            RootNaviEffect_ExStopObj.SetActive(false);
            #endregion
            #region 子のTrsListをSquareに
            particleTrsList.Clear();
            foreach (Transform i in RootNaviEffect_SquareObj.transform)
            { particleTrsList.Add(i); }
            foreach (Transform i in RootNaviEffect_SquareGreenObj.transform)
            { particleTrsList.Add(i); }
            foreach (Transform i in RootNaviEffect_SquareStopObj.transform)
            { particleTrsList.Add(i); }
            #endregion
        }
        else
        {
            #region 名前変更
            RootNaviEffect_ExObj.name = "RootNaviEffect";
            RootNaviEffect_ExGreenObj.name = "RootNaviEffectGreen";
            RootNaviEffect_ExStopObj.name = "RootNaviEffectStop";

            RootNaviEffect_SquareObj.name = "RootNaviEffect_Square";
            RootNaviEffect_SquareGreenObj.name = "RootNaviEffect_SquareGreen";
            RootNaviEffect_SquareStopObj.name = "RootNaviEffect_SquareStop";
            #endregion
            #region アクティブ状況を交換
            RootNaviEffect_ExObj.SetActive(RootNaviEffect_SquareObj.activeSelf);
            RootNaviEffect_ExGreenObj.SetActive(RootNaviEffect_SquareGreenObj.activeSelf);
            RootNaviEffect_ExStopObj.SetActive(RootNaviEffect_SquareStopObj.activeSelf);

            RootNaviEffect_SquareObj.SetActive(false);
            RootNaviEffect_SquareGreenObj.SetActive(false);
            RootNaviEffect_SquareStopObj.SetActive(false);
            #endregion
            #region 子のTrsListをExに
            particleTrsList.Clear();
            foreach (Transform i in RootNaviEffect_ExObj.transform)
            { particleTrsList.Add(i); }
            foreach (Transform i in RootNaviEffect_ExGreenObj.transform)
            { particleTrsList.Add(i); }
            foreach (Transform i in RootNaviEffect_ExStopObj.transform)
            { particleTrsList.Add(i); }
            #endregion
        }
    }
}
