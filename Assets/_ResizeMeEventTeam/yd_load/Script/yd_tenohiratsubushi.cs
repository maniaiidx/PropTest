//using追加も自由です（もし追加したら、後で教えてほしいかも）
using System.Collections;
using UnityEngine;
using DG.Tweening;//DOTween

//クラスは↓の通りにしてください。
public partial class DataCounter
{
    //■ここからコルーチンのメソッドを追加していきます

    //型はIEnumerator（コルーチンメソッド）　メソッド名は自由（RME_が付いてると、後で管理しやすいかも）
#region ■手潰しシチュ用

    #region 振り上げ位置取得用Objを取得（予めTLでプレイヤーの子に設置しておく）
    GameObject
        TetsubushiHandTargetAnchorObj = null,
        TetsubushiHandTargetObj = null;
    public IEnumerator RME_ReadTetsubushiHandTaget()
    {
        TetsubushiHandTargetAnchorObj =
        CameraObjectsTrs.Find("TetsubushiHandTargetAnchorObj").gameObject;
        TetsubushiHandTargetObj =
        TetsubushiHandTargetAnchorObj.transform.Find("TetsubushiHandTargetObj").gameObject;
     
        if (TetsubushiHandTargetAnchorObj != null)
        {
            Debug.Log("事前取得TetsubushiHandTargetAnchorObj");
            }
        else
        {
            Debug.Log("TetsubushiHandTargetAnchorObjがない");
        }

        if (TetsubushiHandTargetObj != null)
        {
            Debug.Log("事前取得TetsubushiHandTargetObj");
        }
        else
        {
            Debug.Log("TetsubushiHandTargetObjがない");
        }
        yield break;
    }
    #endregion

    //手スピード（振り上げ 振り下ろし兼用）
    float handSpeed = 3;

    //動きキャンセルできるようにTweener変数
    Tweener
        tetsubushiPosTweener = null,
        tetsubushiEulTweener = null;

    //当たり判定Obj取得用
    GameObject KO_02tetubure_preObj = null;
    //コライダー判定Obj取得用
    GameObject Rhand_Collider_tesubushi = null; 

    public IEnumerator RME_GetKO_02tetubure_pre()
    {
        Rhand_Collider_tesubushi = FixedMoveChieriColliderFollderObj.transform.Find("BoneBip001 R Hand")
            .Find("Rhand_Collider_tesubushi").gameObject;
        if (GirlRHandTrs.Find("KO_02tetubure_pre") != null)
        {
            KO_02tetubure_preObj = GirlRHandTrs.Find("KO_02tetubure_pre").gameObject;
            Debug.Log("事前取得KO_02tetubure_preObj");
            yield break;
        }
        else
        {
            Debug.Log("KO_02tetubure_preがない");
        }
    }

    #region 右手
    #region ■プレイヤーの上の位置に手を振り上げる
    public IEnumerator RME_IKRHandToPlayerUpMove()
    {
        //辺り判定オフ
        KO_02tetubure_preObj.SetActive(false);

        //TetsubushiHandTargetAnchorObjを、ちえりの右肩に向かせる
        TetsubushiHandTargetAnchorObj.transform.LookAt(Bip001_R_ClavicleTrs.position);

        //かつ、手首が前傾しないように、x角度だけ0にする
        var handTargetEul = new Vector3(0, TetsubushiHandTargetAnchorObj.transform.localEulerAngles.y, TetsubushiHandTargetAnchorObj.transform.localEulerAngles.z);
        TetsubushiHandTargetAnchorObj.transform.localEulerAngles = handTargetEul;


        //高さも設定
        TetsubushiHandTargetAnchorObj.transform.position =
            new Vector3
            (TetsubushiHandTargetAnchorObj.transform.position.x
            , 50
            , TetsubushiHandTargetAnchorObj.transform.position.z);

        //前のTweenerキャンセル
        if (tetsubushiPosTweener != null) { tetsubushiPosTweener.Kill(); }
        if (tetsubushiEulTweener != null) { tetsubushiEulTweener.Kill(); }

        //手潰しターゲットの場所へspeed秒かけて移動する（アンカーの子にある）
        tetsubushiPosTweener =
            IKRHandTargetTrs.DOMove(TetsubushiHandTargetObj.transform.position, handSpeed)
            .SetEase(Ease.InOutSine);

        //speed秒かけて回転する（子のTargetで回転を指定している）
        tetsubushiEulTweener = 
            IKRHandTargetTrs.DORotate
            (TetsubushiHandTargetObj.transform.eulerAngles
            , handSpeed)
            .SetEase(Ease.InOutSine);


        //speed秒待つ
        yield return new WaitForSeconds(handSpeed);

        //手乗り越えコライダーオフ
        Rhand_Collider_tesubushi.SetActive(false);

        yield break;
    }
    #endregion
    #region ■↑で取得した位置から振り下ろす。
    public IEnumerator RME_IKRHandToTable()
    {
        //前のTweenerキャンセル
        if (tetsubushiPosTweener != null) { tetsubushiPosTweener.Kill(); }
        if (tetsubushiEulTweener != null) { tetsubushiEulTweener.Kill(); }

        //位置取得
        var IKRHandNowLocalPos = IKRHandTargetTrs.localPosition;
        IKRHandNowLocalPos.y = 0.3351f;

        //当たり判定ON
        KO_02tetubure_preObj.SetActive(true);
        
        //振り下ろし開始
        tetsubushiPosTweener =
            IKRHandTargetTrs.DOLocalMove(IKRHandNowLocalPos, handSpeed)
            .SetEase(Ease.InSine)
            .OnComplete(()=>
            {
                //動きが終わったら、当たり判定OFF
                KO_02tetubure_preObj.SetActive(false);
                //動きが終わったら、手乗り越え防止コライダーON
                Rhand_Collider_tesubushi.SetActive(true);

                if (handSpeed == 1) {

                    //揺れ
                    CameraAnchorTrs.DOShakePosition(2f, 0.8f);
                    //SEObj直接設置（右手）
                    GameObject Audio_tenohira2_dan_PitchDown = Instantiate(Resources.Load("_TimelineUse/SEObj/yd_loadTimelineTest02_tenohira/Audio_tenohira2_dan_PitchDown") as GameObject
                         , GirlRHandTrs);
                }
                else if(handSpeed == 2)
                {
                    //揺れ
                    CameraAnchorTrs.DOShakePosition(1f, 0.2f);
                    //SEObj直接設置（右手）
                    //Instantiate(Resources.Load("_TimelineUse/SEObj/yd_loadTimelineTest02_tenohira/Audio_tenohira2_dan_PitchDown"));
                    GameObject Audio_tenohira2_dan_PitchDown = Instantiate(Resources.Load("_TimelineUse/SEObj/yd_loadTimelineTest02_tenohira/Audio_tenohira2_dan_PitchDown_speed2") as GameObject
                        , GirlRHandTrs);
                }
                else if (handSpeed >= 3)
                {
                    //揺れ
                    CameraAnchorTrs.DOShakePosition(1f, 0.2f);
                    //SEObj直接設置（右手）
                    GameObject Audio_tenohira2_dan_PitchDown = Instantiate(Resources.Load("_TimelineUse/SEObj/yd_loadTimelineTest02_tenohira/Audio_tenohira2_dan_PitchDown_speed3") as GameObject
                         , GirlRHandTrs);
                }
            });





        yield break;
    }
    #endregion
    #endregion

    #region 左手
    #region ■プレイヤーの上の位置に手を振り上げる
    public IEnumerator RME_IKLHandToPlayerUpMove()
    {
        //TetsubushiHandTargetAnchorObjを、ちえりの左肩に向かせる
        TetsubushiHandTargetAnchorObj.transform.LookAt(Bip001_L_ClavicleTrs.position);
        //高さも設定
        TetsubushiHandTargetAnchorObj.transform.position =
            new Vector3
            (TetsubushiHandTargetAnchorObj.transform.position.x
            , 50
            , TetsubushiHandTargetAnchorObj.transform.position.z);

        //前のTweenerキャンセル
        if (tetsubushiPosTweener != null) { tetsubushiPosTweener.Kill(); }
        if (tetsubushiEulTweener != null) { tetsubushiEulTweener.Kill(); }

        //手潰しターゲットの場所へspeed秒かけて移動する（アンカーの子にある）
        tetsubushiPosTweener =
            IKLHandTargetTrs.DOMove(TetsubushiHandTargetObj.transform.position, handSpeed)
            .SetEase(Ease.InOutSine);

        //speed秒かけて回転する（子のTargetで回転を指定している）
        tetsubushiEulTweener =
            IKLHandTargetTrs.DORotate
            (TetsubushiHandTargetObj.transform.eulerAngles
            , handSpeed)
            .SetEase(Ease.InOutSine);


        //speed秒待つ
        yield return new WaitForSeconds(handSpeed);

        yield break;
    }
    #endregion
    #region ■↑で取得した位置から振り下ろす。
    public IEnumerator RME_IKLHandToTable()
    {
        //前のTweenerキャンセル
        if (tetsubushiPosTweener != null) { tetsubushiPosTweener.Kill(); }
        if (tetsubushiEulTweener != null) { tetsubushiEulTweener.Kill(); }

        //位置取得
        var IKLHandNowLocalPos = IKLHandTargetTrs.localPosition;
        IKLHandNowLocalPos.y = 0.3351f;

        tetsubushiPosTweener =
            IKLHandTargetTrs.DOLocalMove(IKLHandNowLocalPos, handSpeed)
            .SetEase(Ease.InSine);
        yield break;
    }
    #endregion
    #endregion

    //振り上げと降ろし速度変更命令サンプル
    public IEnumerator RME_IKRHandSpeed1sSet()
    {
        handSpeed = 1;
        yield break;
    }
    public IEnumerator RME_IKRHandSpeed2sSet()
    {
        handSpeed = 2;
        yield break;
    }

    public IEnumerator RME_TetsubushiLocalShrink()
    {
        bool isShrinkComplete = false;
        //段階別で目眩具合と縮小度変更して縮小
        PPv2MemaiLittle(1f);

        DOTween.To(() => nowPlayerLocalScale, (x) => nowPlayerLocalScale = x
        , DB.playerScale_Vore01, 15f)
        .OnComplete(() => { isShrinkComplete = true; });
        SEPlay("magic-attack-darkness1_long(-12)", 0.5f);

        while (isShrinkComplete == false) { yield return null; }
        //yield return new WaitForSeconds(1);
        yield break;
    }

    //MJ スクリプトデバッグ用
    public IEnumerator RME_TetsubushiDebug()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("L振り上げ開始");
                StartCoroutine(RME_IKLHandToPlayerUpMove());
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("L振り降ろし");
                StartCoroutine(RME_IKLHandToTable());
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log("R振り上げ開始");
                StartCoroutine(RME_IKRHandToPlayerUpMove());
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("R振り降ろし");
                StartCoroutine(RME_IKRHandToTable());
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                Debug.Log("handspeedを3");
                handSpeed = 3;
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("handspeedを2");
                handSpeed = 2;
            }

            yield return null;
        }
        yield break;
    }
    public IEnumerator RME_tenohiragameover()
    {
        //メニュー強制開き
        StartCoroutine(MenuSystemIEnum(true, false, true));
        yield break;
    }
    #endregion
    public IEnumerator RME_PPv2MemaiNoiseYowameOn()
    {
        PPv2Memai(true, 5, 0.3f);
        SEPlay("heart_def", 0.5f);
        yield break;
    }



    public IEnumerator RME_PPv2MemaiOff()
    {
        PPv2Memai(false, 0);
        yield break;
    }

}

//メソッド名をタイムラインで指定すれば、そこで実行されます。

