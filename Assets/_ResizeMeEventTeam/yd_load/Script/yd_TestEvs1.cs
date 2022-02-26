//using追加も自由です（もし追加したら、後で教えてほしいかも）
using System.Collections;
using UnityEngine;
using DG.Tweening;//DOTween

//クラスは↓の通りにしてください。
public partial class DataCounter
{
    //■ここからコルーチンのメソッドを追加していきます

    //型はIEnumerator（コルーチンメソッド）　メソッド名は自由（RME_が付いてると、後で管理しやすいかも）
    public IEnumerator RME_YDIEnumTest()
    {

        //永遠にループし続ける
        while (true)
        {
            //キーボードのLが押されたら
            if (Input.GetKeyDown(KeyCode.L))
            {
                //フキダシを出す
                Hukidashi("・・・");
            }
            //1フレーム待つ（whileでループする際は、これをつけないとUnity固まるので注意（1フレーム内で永遠にループし続ける））
            yield return null;
        }

        //メソッドおわり（コルーチンはyield命令で値を返さないとエラーがでるので　何も返さないという値をダミーで入れてます。）
        yield break;
    }

    #region 皿変数取得
    //皿生成
    GameObject
        SaraObj,
    //取得(GameObject変数連続で指定している)
    PanObj,
    Pan_AObj,
    Pan_BObj,
    PankuzuObj,
    Pankuzu01Obj,
    Pankuzu02Obj;
    #endregion
    public IEnumerator RME_taberarerusetting()
    {
        //暗幕
        FadeBlack(1, 0);
        yield return null;

        //こもり虫に変え
        KankyouBGMChanger(semiSE.audioSource, "musi2_loop_EQKomori");

        //環境音フェード
        KankyouBGMVolumer(0.5f, 5);


        #region 設定
        //智恵理0位置ロック
        DB.isChieriPosLock = true;

        //アニメスピード
        girlAnim.speed = 1f;
        doorAnim.speed = 1f;
        chairAnim.speed = 1f;

        //椅子アニメオフ
        chairAnim.enabled = false;

        //スピーカー 位置に
        SpeakerTrs.gameObject.SetActive(true);
        GameObject SpeaKer_DeskPosObj = Resources.Load("_PosObj/D_Henai_Day1/SpeaKer_DeskPosObj") as GameObject;
        SpeakerTrs.localPosition = SpeaKer_DeskPosObj.transform.localPosition;
        SpeakerTrs.localEulerAngles = SpeaKer_DeskPosObj.transform.localEulerAngles;

        //宿題セット消し
        SharpenObj.SetActive(false);
        KeshigomuObj.SetActive(false);
        Drill_chieriTrs.gameObject.SetActive(false);


        #region 皿パン設定

        //皿生成
        SaraObj = Instantiate(Resources.Load("Main/KomonoObj/Sara/Sara") as GameObject, RoomTrs, false);
        //取得(GameObject変数連続で指定している)
        PanObj = SaraObj.transform.Find("Pan").gameObject;
        Pan_AObj = PanObj.transform.Find("Pan_A").gameObject;
        Pan_BObj = PanObj.transform.Find("Pan_B").gameObject;
        PankuzuObj = SaraObj.transform.Find("Pankuzu").gameObject;
        Pankuzu01Obj = PankuzuObj.transform.Find("Pankuzu01").gameObject;
        Pankuzu02Obj = PankuzuObj.transform.Find("Pankuzu02").gameObject;

        DB.evMoveDelObjList.Add(SaraObj);

        GameObject
            SaraHandObj = Instantiate(Resources.Load("Main/KomonoObj/Sara/SaraHand") as GameObject, GirlRHandTrs, false);
        DB.evMoveDelObjList.Add(SaraHandObj);

        saraAnim
            = SaraObj.GetComponent<Animator>();
        saraAnim.CrossFadeInFixedTime("デスク皿置かれてる", 0f, 0);
        yield return null;
        SaraObj.SetActive(false);


        //皿動かすためにアニメーター止め
        saraAnim.enabled = false;

        //皿 食事1位置に
        GameObject Sara_01_DeskSyokuzi01PosObj = Resources.Load("_PosObj/D_Henai_Day1/Sara_01_DeskSyokuzi01PosObj") as GameObject;
        SaraObj.transform.localPosition = Sara_01_DeskSyokuzi01PosObj.transform.localPosition;
        SaraObj.transform.localEulerAngles = Sara_01_DeskSyokuzi01PosObj.transform.localEulerAngles;

        //パンを片方消す
        Pan_AObj.SetActive(false);

        //手の皿消し
        SaraHandObj.SetActive(false);
        //机の皿ON
        SaraObj.SetActive(true);

        #endregion //皿パン設定


        //■プレイヤー パン前位置に
        GameObject Player020_PanMaePosObj
            = Resources.Load("_PosObj/0310Pan/Player020_PanMaePosObj") as GameObject;
        CameraObjectsTrs.localPosition = Player020_PanMaePosObj.transform.localPosition;
        CameraObjectsTrs.localEulerAngles = Player020_PanMaePosObj.transform.localEulerAngles;

        //カメラリセット値変更してリセット
        CameraReset(Player020_PanMaePosObj.transform.localEulerAngles
            , DB.cameraSitAnchorDefLocalPos//Anchorを座りに
            , false
            , Vector3.zero
            , false
            , true);

        //プレイヤー大きさ
        nowPlayerLocalScale = DB.playerScale_JougiDeHakariZero;

        //椅子位置
        GameObject Chair_01_SuwariPosObj = Resources.Load("_PosObj/D_Henai_Day1/Chair_01_SuwariPosObj") as GameObject;
        ChairTrs.localPosition = Chair_01_SuwariPosObj.transform.localPosition;
        ChairTrs.localEulerAngles = Chair_01_SuwariPosObj.transform.localEulerAngles;

        //IKプレイヤー見る
        FollowDOMove(IKLookAtEyeTargetTrs, PlayerEyeTargetTrs);
        DOTweenToLAIKSEyes(LAIKEyeS, LAIKSEyesDefWeight, 1);
        FollowDOMove(IKLookAtHeadTargetTrs, PlayerHeadTargetTrs, new Vector3(0, -0.2f, 0));
        DOTweenToLAIKSHead(LAIKHeadS, LAIKSHeadDefWeight, 1);




        #endregion □設定
        HukidashiNovelDistanceChange(1.6f, 0);
        kutipakuString = "口パクゆっくり";

        #region 設定2
        //ちえり
        ChieriMotion("デスク手のせパンくずポーズ", 0f, 0);
        ChieriMotion("笑顔01口眉_目閉じない", 0f, 2);
        //顔傾け用 事前
        ChieriMotion("h_noData", 0f, 7);

        //右手IK位置
        GameObject IKRHand020_NamePosObj
            = Resources.Load("_PosObj/0320Taberareru/IKRHand020_NamePosObj") as GameObject;
        IKRHandTargetTrs.localPosition = IKRHand020_NamePosObj.transform.localPosition;
        IKRHandTargetTrs.localEulerAngles = IKRHand020_NamePosObj.transform.localEulerAngles;

        ////右手IKON
        //DOTweenToIKEfPos(IKRHandEf, 1, 0);
        //DOTweenToIKEfRot(IKRHandEf, 1, 0);


        //■プレイヤー 乗ってのって位置に
        //CameraObjectsTrs.SetParent(GirlRHandTrs);
        GameObject Player005_NotteNottePosObj
            = Resources.Load("_PosObj/0320Taberareru/Player005_NotteNottePosObj") as GameObject;
        CameraObjectsTrs.localPosition = Player005_NotteNottePosObj.transform.localPosition;
        CameraObjectsTrs.localEulerAngles = Player005_NotteNottePosObj.transform.localEulerAngles;

        //カメラリセット値変更してリセット
        CameraReset(Player005_NotteNottePosObj.transform.localEulerAngles
            , DB.cameraStandAnchorDefLocalPos//Anchorを立ちに
            , false
            , Vector3.zero
            , false
            , true);


        //パンくず02 手乗せ位置に(ペアレント) ひとまず大きさも
        Pankuzu02Obj.transform.SetParent(GirlRHandTrs);
        GameObject Pankuzu02010_PareRHand_TenosePosObj
            = Resources.Load("_PosObj/0320Taberareru/Pankuzu02010_PareRHand_TenosePosObj") as GameObject;
        Pankuzu02Obj.transform.localPosition = Pankuzu02010_PareRHand_TenosePosObj.transform.localPosition;
        Pankuzu02Obj.transform.localEulerAngles = Pankuzu02010_PareRHand_TenosePosObj.transform.localEulerAngles;
        Pankuzu02Obj.transform.localScale = Pankuzu02010_PareRHand_TenosePosObj.transform.localScale;

        //皿 とりあえずの机位置に
        GameObject Sara010_DeskPosObj
            = Resources.Load("_PosObj/0320Taberareru/Sara010_DeskPosObj") as GameObject;
        SaraObj.transform.localPosition = Sara010_DeskPosObj.transform.localPosition;
        SaraObj.transform.localEulerAngles = Sara010_DeskPosObj.transform.localEulerAngles;


        #region 右手にWindnoiseSoundObj付与

        GameObject RHandSoundObj
            = Instantiate(Resources.Load("Main/Prefab/SoundObj/WindnoiseSoundObj") as GameObject
            , GirlRHandTrs);
        RHandSoundObj.name = nameof(RHandSoundObj);
        DB.evMoveDelObjList.Add(RHandSoundObj);

        //設定
        WindnoiseSound tmpRHandSound = RHandSoundObj.GetComponent<WindnoiseSound>();
        tmpRHandSound.aSource.maxDistance = 50;
        tmpRHandSound.pitchMax = 1f;
        tmpRHandSound.pitchUpRateAdjustFloat = 4;

        tmpRHandSound.aSource.volume = 0;

        //SoundObjのPos
        GameObject RHandSound010_MakotoIchiPosObj
            = Resources.Load("_PosObj/0320Taberareru/RHandSound010_MakotoIchiPosObj") as GameObject;
        RHandSoundObj.transform.localPosition = RHandSound010_MakotoIchiPosObj.transform.localPosition;

        #endregion

        #region HeadにWindnoiseSoundObj付与

        GameObject HeadSoundObj
            = Instantiate(Resources.Load("Main/Prefab/SoundObj/WindnoiseSoundObj") as GameObject
            , GirlHeadTrs);
        HeadSoundObj.name = nameof(HeadSoundObj);
        DB.evMoveDelObjList.Add(HeadSoundObj);

        //設定
        WindnoiseSound tmpHeadSound = HeadSoundObj.GetComponent<WindnoiseSound>();
        tmpHeadSound.aSource.maxDistance = 50;
        tmpHeadSound.pitchMax = 1f;
        tmpHeadSound.pitchUpRateAdjustFloat = 3;

        tmpHeadSound.aSource.volume = 0;

        //SoundObjのPos
        GameObject HeadSound010_MikenIchiPosObj
            = Resources.Load("_PosObj/0320Taberareru/HeadSound010_MikenIchiPosObj") as GameObject;
        HeadSoundObj.transform.localPosition
            = HeadSound010_MikenIchiPosObj.transform.localPosition;
        //= Vector3.zero;
        #endregion


        #region 鼻息設置
        GameObject BreathSound_Taberareru
            = Instantiate(Resources.Load("Main/KomonoObj/Kounai/BreathSound_Taberareru") as GameObject
            , Bip001HeadTrs, false);

        DB.evMoveDelObjList.Add(BreathSound_Taberareru);


        #endregion

        #endregion

        //開幕
        FadeBlack(0, 1);
        yield return new WaitForSeconds(3);

    }
    public IEnumerator RME_taberarealt()
    {
        #region パン舐め取り

        #region IK右手
        //右手IK位置
        GameObject IKRHand015_PanNamePosObj
            = Resources.Load("_PosObj/0320Taberareru/IKRHand015_PanNamePosObj") as GameObject;
        IKRHandTargetTrs.localPosition = IKRHand015_PanNamePosObj.transform.localPosition;
        IKRHandTargetTrs.localEulerAngles = IKRHand015_PanNamePosObj.transform.localEulerAngles;

        yield return null;
        //右手IKON
        DOTweenToIKEfPos(IKRHandEf, 1, 2, Ease.InOutSine);
        DOTweenToIKEfRot(IKRHandEf, 1, 4, Ease.InOutSine);

        #endregion

        //口元よせ
        ChieriMotion("デスク手のせ口元よせ", 2, 0);

        //パンくずに視線
        ChieriMotion("まばたき", 0f, 4);
        blinkTime = 0;
        FollowDOMove(IKLookAtEyeTargetTrs, Pankuzu02Obj.transform, 0f);

        //HeadIK切り
        DOTweenToLAIKSHead(LAIKHeadS, 0, 2);

        StartCoroutine(GirlAnimReadSystem()); yield return null;
        while (nowGirlAnimClipName != "デスク手のせ口元よせ") { yield return null; }
        while (girlAnimNomTime <= 0.7f) { yield return null; }


        #region パン 舐め取りとSE
        //舐め　同クリップ再生で舌出し同時（0レイヤーは口開けできないので）
        ChieriMotion("デスク手のせ舐め編集パン舐め取り", 0.7f, 0);
        ChieriMotion("デスク手のせ舐め編集パン舐め取り", 0.7f, 15);

        girlAnim.speed = 2f;

        //SE1舌だし
        SEPlay(Other3DSEObj, "舐め02短", GirlTang0001Trs.gameObject, 1.5f);

        //大体1秒
        //舐める瞬間（ペアレント瞬間）まで待機
        yield return new WaitForSeconds(0.5f);

        //パン 舌へペアレント処理
        FixedAction(() =>
        {
            #region ■パン 舌先位置に（ペアレント）
            Pankuzu02Obj.transform.SetParent(GirlTang0003Trs);

            //念のためTweener移動
            GameObject Pankuzu02020_PareTang3_NametorarePosObj
                = Resources.Load("_PosObj/0320Taberareru/Pankuzu02020_PareTang3_NametorarePosObj") as GameObject;
            Pankuzu02Obj.transform.DOLocalMove(Pankuzu02020_PareTang3_NametorarePosObj.transform.localPosition
                , 0.1f);
            Pankuzu02Obj.transform.DOLocalRotate(Pankuzu02020_PareTang3_NametorarePosObj.transform.localEulerAngles
                , 0.1f);

            #endregion
        });
        while (isFixedAction) { yield return null; }


        StartCoroutine(GirlAnimReadSystem(15)); yield return null;
        ////SE2舌入れ
        //while (nowGirlAnimClipName != "デスク手のせ舐め編集パン舐め取り") { yield return null; }
        //while (girlAnimOtherLayerNomTimeDict[15] <= 0.26f) { yield return null; }
        //SEPlay(Other3DSEObj, "舐め03短", GirlTang0001Trs.gameObject, 0.4f);


        //パン口に入るまで待機
        while (girlAnimNomTime <= 0.5f) { yield return null; }

        SEPlay(Other3DSEObj, "舐め03短", GirlTang0001Trs.gameObject, 1.5f);

        #region パンモーション

        //パンTweener移動（消え）
        GameObject Pankuzu02030_PareTang3_KounaiKieruPosObj
            = Resources.Load("_PosObj/0320Taberareru/Pankuzu02030_PareTang3_KounaiKieruPosObj") as GameObject;
        Pankuzu02Obj.transform.DOLocalMove(Pankuzu02030_PareTang3_KounaiKieruPosObj.transform.localPosition
            , 2f)
            .SetEase(Ease.InQuad);
        Pankuzu02Obj.transform.DOLocalRotate(Pankuzu02030_PareTang3_KounaiKieruPosObj.transform.localEulerAngles
            , 2f)
            .SetEase(Ease.InQuad);
        Pankuzu02Obj.transform.DOScale(Pankuzu02030_PareTang3_KounaiKieruPosObj.transform.localScale
            , 2f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                //消し
                Pankuzu02Obj.SetActive(false);
            });

        #endregion

        //SE3飲み込み
        while (girlAnimOtherLayerNomTimeDict[15] <= 0.7f) { yield return null; }
        SEPlay(SocksSEObj, "口ペチャペチャ02_飲み込みピッチ-10", GirlTang0000Trs.gameObject, 1.5f);

        while (girlAnimNomTime <= 0.5f) { yield return null; }

        girlAnim.speed = 1f;

        #endregion

        #endregion

        #region パンくず持ち上げ位置に戻し
        //右手IKOFF
        DOTweenToIKEfPos(IKRHandEf, 0, 2, Ease.InOutSine);
        DOTweenToIKEfRot(IKRHandEf, 0, 4, Ease.InOutSine);
        //HeadIK戻し
        DOTweenToLAIKSHead(LAIKHeadS, LAIKSHeadDefWeight, 1);

        ChieriMotion("デスク手のせパンくずポーズ", 2f, 0);

        //プレイヤーに視線戻し
        ChieriMotion("まばたき", 0f, 4);
        blinkTime = 0;
        FollowDOMove(IKLookAtEyeTargetTrs, PlayerEyeTargetTrs, 0f);

        yield return new WaitForSeconds(1);

        #endregion
        yield break;
    }

  
}

//メソッド名をタイムラインで指定すれば、そこで実行されます。

