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

    //型はIEnumerator（コルーチンメソッド）　メソッド名は自由（RME_が付いてると、後で管理しやすいかも）
    public IEnumerator RME_MJIEnumTest()
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
    public IEnumerator RME_MJKeyCheck()
    {
        while (Input.GetKeyDown(KeyCode.L) == false)
        {
            yield return null;
        }
        yield break;
    }
    public IEnumerator RME_StomchSceneLoad()
    {
        #region 暗幕胃準備
        isF3030KounaiEffect = false;
        StartCoroutine(F3030StomachEffect());

        //ちえりオフ
        GirlMeshTrs.gameObject.SetActive(false);
        //部屋オフ
        RoomTrs.gameObject.SetActive(false);
        //外オフ
        SotoTrs.gameObject.SetActive(false);
        //ホコリオフ
        Particle_DustTrs.gameObject.SetActive(false);


        //胃プレファブ
        GameObject StomachObj
            = Instantiate(Resources.Load("Main/KomonoObj/Stomach/StomachRoot") as GameObject
            , GameObjectsTrs, false);

        //胃アニメ取得
        var stomachAnim
            = StomachObj.GetComponent<Animator>();

        //胃液取得
        Transform IekiTrs
            = StomachObj.transform.Find("Ieki");

        #region //胃ライト VR時は変更 やっぱやめ

        ////VRだったら交換
        //if (XRSettings.enabled)
        //{
        //    StomachObj.transform.Find("Spot Light").gameObject.SetActive(false);
        //    StomachObj.transform.Find("Spot LightVR").gameObject.SetActive(true);
        //}
        //元々VRライトのみオフなので、elseいらない

        #endregion


        #region プレイヤー設置
        //■プレイヤー 胃の位置に（ペアレントしない）
        CameraObjectsTrs.SetParent(GameObjectsTrs);

        GameObject Player200_StomachPosObj
            = Resources.Load("_PosObj/0330Kounai/Player200_StomachPosObj") as GameObject;
        CameraObjectsTrs.localPosition = Player200_StomachPosObj.transform.localPosition;
        CameraObjectsTrs.localEulerAngles = Player200_StomachPosObj.transform.localEulerAngles;

        //カメラリセット値変更してリセット
        CameraReset(Player200_StomachPosObj.transform.localEulerAngles
            , DB.cameraSitAnchorDefLocalPos//座りに
            , false
            , Vector3.zero
            , false
            , true);



        //ダミー
        PlayerMotion("Player020舌中央安定", 0f, 0);
        //Player_DummyTrs.localPosition = defDummyPos;
        //Player_DummyTrs.localEulerAngles = defDummyRot;
        Player_DummyTrs.localPosition = Vector3.zero;
        Player_DummyTrs.localEulerAngles = Vector3.zero;


        #endregion

        //カメラ設定
        VRCamera.clearFlags
            = TPSCamera.clearFlags
            = CameraClearFlags.SolidColor;

        VRCamera.backgroundColor
            = TPSCamera.backgroundColor
            = Color.black;

        #endregion


        yield break;
    }
    public IEnumerator RME_KounaiEffect()
    {

        OriBreathController.enabled = false;

        //↓ ここから 咥内用ポストプロセスライト設定コルーチン の内容をコピペして改変

        if (isF3030KounaiEffect) { yield break; }
        isF3030KounaiEffect = true;


        #region まず特殊ポストエフェクトとライトを設置（子がライト）
        GameObject PostProcessVolumeKounaiObj =
            Instantiate(Resources.Load("PostProcessing/Prefab/PostProcessVolume_HurosameKounaiRoomOn") as GameObject
            , GirlHeadTrs, false);

        Light lightPoint01 = PostProcessVolumeKounaiObj.transform.Find("LightPoint01").GetComponent<Light>();
        Light lightPoint02 = PostProcessVolumeKounaiObj.transform.Find("LightPoint02").GetComponent<Light>();

        Light lightPoint03 = PostProcessVolumeKounaiObj.transform.Find("LightPoint03右頬").GetComponent<Light>();
        Light lightPoint04 = PostProcessVolumeKounaiObj.transform.Find("LightPoint04左頬").GetComponent<Light>();
        Light lightPoint05 = PostProcessVolumeKounaiObj.transform.Find("LightPoint05口").GetComponent<Light>();

        Light lightPoint06 = PostProcessVolumeKounaiObj.transform.Find("LightPoint06右頬_NotImportant").GetComponent<Light>();
        Light lightPoint07 = PostProcessVolumeKounaiObj.transform.Find("LightPoint07左頬_NotImportant").GetComponent<Light>();
        Light lightPoint08 = PostProcessVolumeKounaiObj.transform.Find("LightPoint08口_NotImportant").GetComponent<Light>();

        Light lightPoint03VR = PostProcessVolumeKounaiObj.transform.Find("LightPoint03右頬VR").GetComponent<Light>();
        Light lightPoint04VR = PostProcessVolumeKounaiObj.transform.Find("LightPoint04左頬VR").GetComponent<Light>();
        Light lightPoint05VR = PostProcessVolumeKounaiObj.transform.Find("LightPoint05口VR").GetComponent<Light>();

        #endregion

        #region 既存のポストプロセスとライトをオフ
        //既存のポストプロセスオフ
        PostProcessVolume00DirectDataObj.SetActive(false);

        ////既存のライトオフ（使う方向で）
        //var defLightObj = GameObject.Find("GameObjects/Light");
        //defLightObj.SetActive(false);

        //既存のライトからEventLightTargetレイヤーオフ
        var defLight = GameObject.Find("Bath_Light/Sun_Realtime").GetComponent<Light>();
        defLight.cullingMask &= ~(1 << LayerMask.NameToLayer("EventLightTarget"));


        //既存のフェイスライトオフ
        var faceLightObj = GirlHeadTrs.Find("Face Light").gameObject;
        faceLightObj.SetActive(false);

        //環境色Def取得
        var defAmbientSkyColor = RenderSettings.ambientSkyColor;

        //環境色　VRだったら少し違う
        if (XRSettings.enabled)
        { RenderSettings.ambientSkyColor = new Color(0.004f, 0.0003f, 0); }
        else
        { RenderSettings.ambientSkyColor = new Color(0.04f, 0.003f, 0); }

        #endregion

        #region ■各メッシュのレンダラ設定（LightProbeやupdateOffScreen レイヤーなど）
        //■舌
        var tangSMR =
            GirlMeshTrs.Find("Tang_def").GetComponent<SkinnedMeshRenderer>();

        //LightProbe　RefrectionProbeオフ（それだけでも光が当たっているので）
        tangSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        tangSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        //ちえりを置きなおさないと正常なBounds位置が取得できないので、Bounds手動調整してオフ
        tangSMR.updateWhenOffscreen = false;

        //マテリアルを置き換え
        tangSMR.material = tangSMR.GetComponent<MatChangeResources>().otherMat;

        //レイヤー変更
        tangSMR.gameObject.layer = LayerMask.NameToLayer("EventLightTarget");


        //■歯
        var toothSMR =
            GirlMeshTrs.Find("Tooth_def").GetComponent<SkinnedMeshRenderer>();

        toothSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        toothSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        toothSMR.updateWhenOffscreen = false;

        toothSMR.material = toothSMR.GetComponent<MatChangeResources>().otherMat;

        toothSMR.gameObject.layer = LayerMask.NameToLayer("EventLightTarget");


        //■咥内
        var kounaiSMR =
            GirlMeshTrs.Find("Kounai_def").GetComponent<SkinnedMeshRenderer>();

        kounaiSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        kounaiSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        kounaiSMR.updateWhenOffscreen = false;

        kounaiSMR.material = kounaiSMR.GetComponent<MatChangeResources>().otherMat;

        kounaiSMR.gameObject.layer = LayerMask.NameToLayer("EventLightTarget");


        //■TPSプレイヤーDummyマテリアル
        var playerDummyBodySMR =
            Player_DummyTrs.Find("mesh/Body").GetComponent<SkinnedMeshRenderer>();

        //forでの入れ替えは出来なかった　直接toArrayしたらできた（Instanceではなくなる）
        playerDummyBodySMR.materials = playerDummyBodySMR.GetComponent<MatChangeResources>().otherMatList.ToArray();
        //for (int i = 0; i < playerDummyBodySMR.materials.Length; i++)
        //{
        //    playerDummyBodySMR.materials[i] = playerDummyBodySMR.GetComponent<MatChangeResources>().otherMatList[i];
        //}

        #endregion

        ////唾液設置
        //GameObject DaekiObj = Instantiate(Resources.Load("Main/KomonoObj/Daeki/DaekiObj") as GameObject
        //    , GirlTooth00Trs, false);


        #region プレイヤーを舌アニメで操作するために、ガイドCubeを設置
        //マスクに直接書き込んだりしないとできない荒業

        CameraObjectsGuideCubeTrs
            = Instantiate(Resources.Load("_PosObj/0330Kounai/CameraObjectsGuideCube") as GameObject
            , GirlTang0003Trs
            , false).transform;
        CameraObjectsGuideCubeTrs.name = "CameraObjectsGuideCube";

        yield return null;
        #endregion

        #region GirlTrsをONOFFすることで、アニメーターONOFFによりアニメーターがライトなどを取得（スクリプトではアニメーターONOFFだとダメだった）
        //yield return null;
        //GirlTrs.gameObject.SetActive(false);
        //yield return null;
        //GirlTrs.gameObject.SetActive(true);
        //yield return null;
        #endregion

        #region 口開けとライト同期よう変数（アニメーターでライトをやると、後で調整効かないのでやっぱりスクリプトで同期する）

        //ライトのデフォルト（最大値）取得
        float defLightPoint01Intensity = lightPoint01.intensity;
        float defLightPoint02Intensity = lightPoint02.intensity;

        //咥内
        float defLightPoint03Intensity = lightPoint03.intensity;
        float defLightPoint04Intensity = lightPoint04.intensity;
        float defLightPoint05Intensity = lightPoint05.intensity;

        //咥内
        float defLightPoint03VRIntensity = lightPoint03VR.intensity;
        float defLightPoint04VRIntensity = lightPoint04VR.intensity;
        float defLightPoint05VRIntensity = lightPoint05VR.intensity;

        //VRだったら
        if (XRSettings.enabled)
        {
            //02（口奥明るさ）下げる。
            defLightPoint02Intensity = defLightPoint02Intensity / 5;

            //345 678消し
            lightPoint03.gameObject.SetActive(false);
            lightPoint04.gameObject.SetActive(false);
            lightPoint05.gameObject.SetActive(false);
            lightPoint06.gameObject.SetActive(false);
            lightPoint07.gameObject.SetActive(false);
            lightPoint08.gameObject.SetActive(false);

            //345VR ON
            lightPoint03VR.gameObject.SetActive(true);
            lightPoint04VR.gameObject.SetActive(true);
            lightPoint05VR.gameObject.SetActive(true);

        }

        //口開き具合取得用
        float //ひとまず適当な値
            nowMouthWaWait = 0,
            prevMouthWaWait = 0;

        #endregion



        //初期化終了
        isF3030KounaiEffectInitialize = true;

        while (isF3030KounaiEffect)
        {
            #region CameraObjectsの位置をガイドcubeの位置と同期

            if (isF3030CameraObjectsGuideCubeDouki)
            {
                CameraObjectsTrs.position = CameraObjectsGuideCubeTrs.position;
                CameraObjectsTrs.eulerAngles = CameraObjectsGuideCubeTrs.eulerAngles;
            }

            #endregion

            #region 口あけとライト同期
            //now今フレ口あき値取得
            nowMouthWaWait = Kounai_def.GetBlendShapeWeight(5);

            //前フレと口あき具合ちがったら
            if (nowMouthWaWait != prevMouthWaWait)
            {
                //ライト調整（口全開100を1として デフォルト明るさ（最大値）と掛け算）
                lightPoint01.intensity = defLightPoint01Intensity * (nowMouthWaWait / 100);
                lightPoint02.intensity = defLightPoint02Intensity * (nowMouthWaWait / 100);


                //345はVRだったらVRのに
                if (XRSettings.enabled)
                {
                    //咥内ライト（口全開100を0とする、デフォ - (デフォ * (口あき/100)　(これで閉じるほどデフォ値に近づく)
                    lightPoint03VR.intensity = defLightPoint03VRIntensity - (defLightPoint03VRIntensity * (nowMouthWaWait / 100));
                    lightPoint04VR.intensity = defLightPoint04VRIntensity - (defLightPoint04VRIntensity * (nowMouthWaWait / 100));
                    lightPoint05VR.intensity = defLightPoint05VRIntensity - (defLightPoint05VRIntensity * (nowMouthWaWait / 100));
                }
                else
                {
                    //咥内ライト（口全開100を0とする、デフォ - (デフォ * (口あき/100)　(これで閉じるほどデフォ値に近づく)
                    lightPoint03.intensity = defLightPoint03Intensity - (defLightPoint03Intensity * (nowMouthWaWait / 100));
                    lightPoint04.intensity = defLightPoint04Intensity - (defLightPoint04Intensity * (nowMouthWaWait / 100));
                    lightPoint05.intensity = defLightPoint05Intensity - (defLightPoint05Intensity * (nowMouthWaWait / 100));
                }

            }

            //prev今フレ口あき値取得
            prevMouthWaWait = nowMouthWaWait;
            #endregion

            if (tPSModeInt >= 2)
            {
                #region オフ
                if (PostProcessVolumeKounaiObj.activeSelf)
                {
                    //■イベントポストプロセス ライトオフ
                    PostProcessVolumeKounaiObj.SetActive(false);
                    #region 既存のポストプロセスとライトをオン
                    //既存のポストプロセスオン
                    PostProcessVolume00DirectDataObj.SetActive(true);

                    //既存のライトからEventLightTargetレイヤーオン
                    defLight.cullingMask |= (1 << LayerMask.NameToLayer("EventLightTarget"));

                    //既存のフェイスライトオン
                    faceLightObj.SetActive(true);

                    //環境色戻し
                    RenderSettings.ambientSkyColor = defAmbientSkyColor;
                    #endregion
                    #region ■各メッシュのレンダラ設定（LightProbeやupdateOffScreen レイヤーなど）
                    //■舌
                    //LightProbe　RefrectionProbeオフ（それだけでも光が当たっているので）
                    tangSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                    tangSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;

                    //Bounds手動調整オン
                    tangSMR.updateWhenOffscreen = true;

                    //マテリアルを置き換え
                    tangSMR.material = tangSMR.GetComponent<MatChangeResources>().defaultMat;

                    //レイヤー変更
                    tangSMR.gameObject.layer = LayerMask.NameToLayer("chieri");


                    //■歯
                    toothSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                    toothSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;

                    toothSMR.updateWhenOffscreen = true;

                    toothSMR.material = toothSMR.GetComponent<MatChangeResources>().defaultMat;

                    toothSMR.gameObject.layer = LayerMask.NameToLayer("chieri");


                    //■咥内
                    kounaiSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                    kounaiSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;

                    kounaiSMR.updateWhenOffscreen = true;

                    kounaiSMR.material = kounaiSMR.GetComponent<MatChangeResources>().defaultMat;

                    kounaiSMR.gameObject.layer = LayerMask.NameToLayer("chieri");


                    //■TPSプレイヤーDummyマテリアル
                    playerDummyBodySMR.materials = playerDummyBodySMR.GetComponent<MatChangeResources>().defaultMatList.ToArray();



                    #endregion

                    ////唾液OFF
                    //DaekiObj.SetActive(false);
                }
                #endregion
            }
            else
            {
                #region オン
                if (PostProcessVolumeKounaiObj.activeSelf == false)
                {
                    //イベントポストプロセス ライトオン
                    PostProcessVolumeKounaiObj.SetActive(true);
                    #region 既存のポストプロセスとライトをオフ
                    //既存のポストプロセスオフ
                    PostProcessVolume00DirectDataObj.SetActive(false);

                    //既存のライトからEventLightTargetレイヤーオフ
                    defLight.cullingMask &= ~(1 << LayerMask.NameToLayer("EventLightTarget"));

                    //既存のフェイスライトオフ
                    faceLightObj.SetActive(false);

                    //環境色　VRだったら少し違う
                    if (XRSettings.enabled)
                    { RenderSettings.ambientSkyColor = new Color(0.004f, 0.0003f, 0); }
                    else
                    { RenderSettings.ambientSkyColor = new Color(0.04f, 0.003f, 0); }

                    #endregion
                    #region ■各メッシュのレンダラ設定（LightProbeやupdateOffScreen レイヤーなど）
                    //■舌
                    //LightProbe　RefrectionProbeオフ（それだけでも光が当たっているので）
                    tangSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    tangSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                    //ちえりを置きなおさないと正常なBounds位置が取得できないので、Bounds手動調整してオフ
                    tangSMR.updateWhenOffscreen = false;

                    //マテリアルを置き換え
                    tangSMR.material = tangSMR.GetComponent<MatChangeResources>().otherMat;

                    //レイヤー変更
                    tangSMR.gameObject.layer = LayerMask.NameToLayer("EventLightTarget");


                    //■歯
                    toothSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    toothSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                    toothSMR.updateWhenOffscreen = false;

                    toothSMR.material = toothSMR.GetComponent<MatChangeResources>().otherMat;

                    toothSMR.gameObject.layer = LayerMask.NameToLayer("EventLightTarget");


                    //■咥内
                    kounaiSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    kounaiSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                    kounaiSMR.updateWhenOffscreen = false;

                    kounaiSMR.material = kounaiSMR.GetComponent<MatChangeResources>().otherMat;

                    kounaiSMR.gameObject.layer = LayerMask.NameToLayer("EventLightTarget");


                    //■TPSプレイヤーDummyマテリアル
                    playerDummyBodySMR.materials = playerDummyBodySMR.GetComponent<MatChangeResources>().otherMatList.ToArray();


                    #endregion

                    ////唾液ON
                    //DaekiObj.SetActive(true);
                }

                #endregion
            }

            yield return null;
        }

        //■終了処理
        //ポストプロセス削除（子ライトも削除される）
        Destroy(PostProcessVolumeKounaiObj);

        #region 既存のポストプロセスとライトをオン
        //既存のポストプロセスオン
        PostProcessVolume00DirectDataObj.SetActive(true);

        //既存のライトからEventLightTargetレイヤーオン
        defLight.cullingMask |= (1 << LayerMask.NameToLayer("EventLightTarget"));

        //既存のフェイスライトオン
        faceLightObj.SetActive(true);

        //環境色
        RenderSettings.ambientSkyColor = defAmbientSkyColor;
        #endregion

        #region ■各メッシュのレンダラ設定（LightProbeやupdateOffScreen レイヤーなど）
        //■舌
        //LightProbe　RefrectionProbeオフ（それだけでも光が当たっているので）
        tangSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
        tangSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;

        //Bounds手動調整オン
        tangSMR.updateWhenOffscreen = true;

        //マテリアルを置き換え
        tangSMR.material = tangSMR.GetComponent<MatChangeResources>().defaultMat;

        //レイヤー変更
        tangSMR.gameObject.layer = LayerMask.NameToLayer("chieri");


        //■歯
        toothSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
        toothSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;

        toothSMR.updateWhenOffscreen = true;

        toothSMR.material = toothSMR.GetComponent<MatChangeResources>().defaultMat;

        toothSMR.gameObject.layer = LayerMask.NameToLayer("chieri");


        //■咥内
        kounaiSMR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
        kounaiSMR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;

        kounaiSMR.updateWhenOffscreen = true;

        kounaiSMR.material = kounaiSMR.GetComponent<MatChangeResources>().defaultMat;

        kounaiSMR.gameObject.layer = LayerMask.NameToLayer("chieri");


        //■TPSプレイヤーDummyマテリアル
        playerDummyBodySMR.materials = playerDummyBodySMR.GetComponent<MatChangeResources>().defaultMatList.ToArray();
        #endregion

        ////唾液削除
        //Destroy(DaekiObj);

        isF3030KounaiEffectInitialize = false;//念のため初期化完了BoolをFalseに


        yield break;
    }
    public IEnumerator RME_FadeinGameover()
    {
        FadeBlack(0, 3);
        //メニュー強制開き
        StartCoroutine(MenuSystemIEnum(true, false, true));
        yield break;
    }
    public IEnumerator RME_Gameover()
    {
        //メニュー強制開き
        StartCoroutine(MenuSystemIEnum(true, false, true));
        yield break;
    }
    public IEnumerator RME_NovelOff()
    {
        isNovelFadeTween = isNovelSetVisIng = false;
        yield break;
    }
    public IEnumerator RME_DOTWeenKillAll()
    {
        DOTween.KillAll();
        yield break;
    }
    #region F3080 （家つまみイベント）
    bool isF3080Reset = false; //whileとかの強制終了をこれで制御
    public IEnumerator RME_F3080Reset()
    {
        isF3080Reset = true;
        yield return null;
        isF3080Reset = false;
        yield break;
    }

    public IEnumerator RME_KankyouBGM_Asa()
    {
        //朝環境音に変え
        KankyouBGMChanger(semiSE.audioSource, "residential-area-in-early-morning-1_loop");
        yield break;
    }

    public IEnumerator RME_TsukueUeKaiwaLoad()
    {
        if (debugEventMoveFlag == true)
        {

        }

        #region 宿題ペン消しゴムなくし スピーカーを所定位置に
        //宿題 位置に
        Drill_chieriTrs.gameObject.SetActive(false);
        //プレイヤーの
        DrillTrs.gameObject.SetActive(false);
        //ペンなくし
        SharpenObj.SetActive(false);
        //消しゴム無くし
        KeshigomuObj.SetActive(false);

        //スピーカー 位置に
        SpeakerTrs.gameObject.SetActive(true);
        GameObject SpeaKer_DeskPosObj = Resources.Load("_PosObj/D_Henai_Day1/SpeaKer_DeskPosObj") as GameObject;
        SpeakerTrs.localPosition = SpeaKer_DeskPosObj.transform.localPosition;
        SpeakerTrs.localEulerAngles = SpeaKer_DeskPosObj.transform.localEulerAngles;

        #endregion //宿題置き


        #region 設定
        #region ■智恵理 SoundObj

        //頭にWindnoiseSoundObj付与
        GameObject HeadSoundObj = Instantiate(Resources.Load("Main/Prefab/SoundObj/WindnoiseSoundObj") as GameObject
            , HeadSphereTrs);//（頭）
        DB.evMoveDelObjList.Add(HeadSoundObj);

        AudioSource tmpHeadAS = HeadSoundObj.GetComponent<AudioSource>();
        //ミュート
        tmpHeadAS.mute = true;

        //SoundObjのPos
        GameObject SoundObj_01_Ningyou_PareHeadPosObj = Resources.Load("_PosObj/D_Henai_Day1/SoundObj_01_Ningyou_PareHeadPosObj") as GameObject;
        HeadSoundObj.transform.localPosition = SoundObj_01_Ningyou_PareHeadPosObj.transform.localPosition;

        //右人差し指にWindnoiseSoundObj付与
        GameObject RHitosasiSoundObj = Instantiate(Resources.Load("Main/Prefab/SoundObj/WindnoiseSoundObj") as GameObject
            , GirlRhitosashi02Trs);
        DB.evMoveDelObjList.Add(RHitosasiSoundObj);

        AudioSource tmpRhitosasiAS = RHitosasiSoundObj.GetComponent<AudioSource>();
        //ミュート
        tmpRhitosasiAS.mute = true;
        //SoundObjのPos
        GameObject SoundObj_04_TsumamiYubi_RHitosasiPosObj = Resources.Load("_PosObj/D_Henai_Day1_Tansaku1-2/SoundObj_04_TsumamiYubi_RHitosasiPosObj") as GameObject;
        RHitosasiSoundObj.transform.localPosition = SoundObj_04_TsumamiYubi_RHitosasiPosObj.transform.localPosition;

        ////IKプレイヤー見る
        //FollowDOMove(IKLookAtEyeTargetTrs, PlayerEyeTargetTrs);
        //DOTweenToLAIKSEyes(LAIKEyeS, LAIKSEyesDefWeight, 1);
        //FollowDOMove(IKLookAtHeadTargetTrs, PlayerHeadTargetTrs, new Vector3(0, -0.045f, 0));
        //DOTweenToLAIKSHead(LAIKHeadS, LAIKSHeadDefWeight, 1);
        #endregion






        #endregion 設定

        #region //ペットボトル取り出すまで

        //yield return new WaitForSeconds(2);
        //FadeBlack(0, 2);
        //yield return new WaitForSeconds(2);


        ////ミュート解除
        //tmpHeadAS.mute = false;
        //tmpRhitosasiAS.mute = false;

        //NovelSetVis("E4100010");//見つめられている-やんわりと伝えよう・・
        //while (isNovelSetVisIng) { yield return null; }
        //yield return new WaitForSeconds(1);

        ////同時発声でノベル消し
        //NovelSetVis("E4100020");//あのさ
        //yield return new WaitForSeconds(0.2f);
        //ChieriMotion("f_あはは・・2", 0.2f, 2);
        //ChieriMotionDelay(3, "f_あはは・・2目開き", 0.2f, 2);
        //Hukidashi("E4100010");//お腹すいたね
        //yield return new WaitForSeconds(0.5f);
        //isNovelFadeTween = isNovelSetVisIng = false;
        //yield return new WaitForSeconds(1f); yield return KeyOrWait(3);

        //NovelSetVis("E4100030");//ほぼ同時だった。
        //while (isNovelSetVisIng) { yield return null; }

        ////まばたき
        //ChieriMotion("b_まばたきSlow用", 0f, 4);
        //blinkTime = 0;
        ////上向き
        //ChieriMotion("f_本当に小さいね～", 0.3f, 2);
        ////IKカレンダー見
        //FollowDOMove(IKLookAtEyeTargetTrs, CalenderTrs, 0f);
        //yield return new WaitForSeconds(1);
        //Hukidashi("E4100020");//朝ごはんも食べたいけど
        //yield return new WaitForSeconds(1f); yield return KeyOrWait(3);

        ////■"デスク肘つき→起き上がり"モーションだけ速度変更（animatorのParametersでスピード変更（ステート毎に設定できるので、表情が遅くならない））
        //girlAnim.SetFloat("デスク肘つき→起き上がりSpeed", 0.4f); yield return null;
        //ChieriMotion("デスク肘つき→起き上がり", 0, 0);

        ////IKプレイヤー見る
        //FollowDOMove(IKLookAtEyeTargetTrs, PlayerEyeTargetTrs, 0f);
        ////まばたき
        //ChieriMotion("b_まばたきSlow用", 0f, 4);
        //blinkTime = 0;

        //yield return new WaitForSeconds(1);

        //Hukidashi("E4100030");//のど渇いてるんじゃない？

        //ChieriMotion("f_眉高い笑顔少しジト目", 0.3f, 2);

        //yield return new WaitForSeconds(1f); yield return KeyOrWait(3);
        //yield return new WaitForSeconds(1f);

        //NovelSetVis("E4100040");//あ、うん
        //while (novelCurrentPageInt < 2) { yield return null; }

        ////ミュート
        //tmpHeadAS.mute = true;
        //tmpRhitosasiAS.mute = true;

        ////ノベル中暗幕でペットボトル取り出しポーズに
        //FadeBlack(1, 0.5f);
        //yield return new WaitForSeconds(0.5f);

        //DB.isChieriPosLock = true;
        //ChieriMotion("デスクペットボトル取り出し前ポーズ", 0f, 0);

        //while (novelCurrentPageInt < 6) { yield return null; }

        ////開幕
        //FadeBlack(0, 0.5f);
        //yield return new WaitForSeconds(0.5f);
        //while (isNovelSetVisIng) { yield return null; }
        ////ミュート解除
        //tmpHeadAS.mute = false;
        //tmpRhitosasiAS.mute = false;


        //yield return new WaitForSeconds(1);
        //ChieriMotion("笑顔01口眉", 0.05f, 2);
        //ChieriMotionDelay(1, "笑顔01口眉_目閉じない", 0.1f, 2);//時間差表情戻し
        //Hukidashi("E4100040");//いいものがあるから
        //yield return new WaitForSeconds(1f); yield return KeyOrWait(3);


        ////■ペットボトル取り出し開始
        //ChieriMotion("デスクペットボトル取り出し", 0f, 0);
        //NovelSetVis("E4100050");//あ、うん

        ////鞄へ手を伸ばすあたりまで待ち
        //StartCoroutine(GirlAnimReadSystem());
        //yield return null;
        //while (girlAnimNomTime <= 0.5f)
        //{ yield return null; }


        ////IK鞄に
        //GameObject tmpSchoolBagObj = GameObject.Find("SchoolBag");
        //FollowDOMove(IKLookAtEyeTargetTrs, tmpSchoolBagObj.transform, 0.1f);
        ////顔はオフ
        //DOTweenToLAIKSHead(LAIKHeadS, 0, 1.5f);

        ////まばたき
        //ChieriMotion("b_まばたきSlow用", 0f, 4);
        //blinkTime = 0;

        //while (novelCurrentPageInt < 3) { yield return null; }
        ////暗幕
        //FadeBlack(1, 1f);
        //yield return new WaitForSeconds(1f);

        ////プレイヤー 机の上位置に
        //CameraObjectsTrs.localPosition = Player_010_DeskUeChieriTsumamiPosObj.transform.localPosition;
        //CameraObjectsTrs.localEulerAngles = Player_010_DeskUeChieriTsumamiPosObj.transform.localEulerAngles;

        ////まさぐり音仮
        //SEPlay("laundry-spread1", tmpSchoolBagObj);
        //yield return new WaitForSeconds(2);

        ////風切り音出す用に逆再生
        //ChieriMotion("デスクペットボトル取り出し逆再生", 0f, 0);
        //yield return null;
        //while (girlAnimNomTime <= 0.8f)
        //{ yield return null; }
        //isGirlAnimReadSystem = false;
        //NovelSetVis("E4100060");//そして出てきたのは

        ////IKプレイヤー見る
        //FollowDOMove(IKLookAtEyeTargetTrs, PlayerEyeTargetTrs);
        //DOTweenToLAIKSEyes(LAIKEyeS, LAIKSEyesDefWeight, 1);
        //FollowDOMove(IKLookAtHeadTargetTrs, PlayerHeadTargetTrs, new Vector3(0, -0.045f, 0));
        //DOTweenToLAIKSHead(LAIKHeadS, LAIKSHeadDefWeight, 1);

        ////ミュート
        //tmpHeadAS.mute = true;
        //tmpRhitosasiAS.mute = true;
        #endregion

        yield break;
    }
    public IEnumerator RME_LHandSumahoToridashi()
    {
        #region ■左手スマホ取り出し　マスク IK版

        GirlPosToIKTargetPosRot();

        string
            tmpBaseAnimStateName = "片膝立ちミラー手伸ばしループ原点";
        #region スマホ片手いじりPosへ
        GameObject ChieriSumaho_LHand_SekurabeKatatePosObj
            = Resources.Load("_PosObj/_ParentPoseObjs/ChieriSumaho_LHand_SekurabeKatatePosObj") as GameObject;
        ChieriSumahoObj.transform.DOLocalMove(ChieriSumaho_LHand_SekurabeKatatePosObj.transform.localPosition, 3f);
        ChieriSumahoObj.transform.DOLocalRotate(ChieriSumaho_LHand_SekurabeKatatePosObj.transform.localEulerAngles, 3f);
        #endregion

        StartCoroutine(GirlAnimReadSystem(11));

        //マスクのため、レイヤーのアニメを1フレでベースと同じモーション状態にする（ParameterでNormlizedTimeが指定できる（ただし指定し続けないと静止））
        ChieriMotion(tmpBaseAnimStateName, 1f, 11);//現在のベースアニメと同じアニメ指定
        girlAnim.SetFloat("現在ベースアニメのNormalizedTime", girlAnimNomTime);//同じNomTimeに

        yield return null;//↑適用に1フレ必要

        ChieriMotion("背比べ左手スマホ取り出し", 0.6f, 11);

        while (nowGirlAnimOtherLayerClipNameDict[11] != "背比べスマホ取り出し") { yield return null; }
        while (girlAnimOtherLayerNomTimeDict[11] <= 0.375f) { yield return null; }
        isGirlAnimReadSystem = false;
        //スマホObjオン（取り出し）
        ChieriSumahoObj.SetActive(true);
        //画面つける（画面更新も一緒に起動される）
        ChieriSumahoPower(true);

        #region スマホ見
        ChieriMotion("まばたき", 0f, 4); blinkTime = 0;
        FollowDOMove(IKLookAtEyeTargetTrs, ChieriSumahoObj.transform, 0f);
        DOTweenToLAIKSEyes(LAIKEyeS, LAIKSEyesDefWeight, 0f);
        FollowDOMove(IKLookAtHeadTargetTrs, ChieriSumahoObj.transform, new Vector3(0, -0.5f, 0), 2f);
        DOTweenToLAIKSHead(LAIKHeadS, 0.8f, 1);
        #endregion


        #region 左手IKいじり位置へ（力技
        //まずSpineにPosObj設置
        GameObject IKLHand_PareSpine_SumahoToridashiPosObj
            = Instantiate(Resources.Load("_PosObj/IKLHand_PareSpine_SumahoToridashiPosObj") as GameObject
            , GirlSpineTrs, false);


        //ターゲットTween移動
        //GameObject IKLHand010_SumahoPosObj
        //    = Resources.Load("_PosObj/0370YukaueKisyou/IKLHand010_SumahoPosObj") as GameObject;
        IKLHandTargetTrs.DOMove(IKLHand_PareSpine_SumahoToridashiPosObj.transform.position, 1f);
        IKLHandTargetTrs.DORotate(IKLHand_PareSpine_SumahoToridashiPosObj.transform.eulerAngles, 1f);

        //少しウェイト下げて
        DOTweenToIKEfPos(IKLHandEf, 0.6f, 1f);
        DOTweenToIKEfRot(IKLHandEf, 0.6f, 1f);

        yield return new WaitForSeconds(1f);
        //ウェイト戻す
        DOTweenToIKEfPos(IKLHandEf, 1, 1f);
        DOTweenToIKEfRot(IKLHandEf, 1, 1f);
        #endregion

        #endregion

    }
    public IEnumerator RME_LHandSumahoSimau()
    {
        //スマホしまい
        ChieriMotion("背比べ左手スマホしまう", 0f, 11);

        //ウェイトオフ
        DOTweenToIKEfPos(IKLHandEf, 0, 1f);
        DOTweenToIKEfRot(IKLHandEf, 0, 1f);

        //スマホしまう位置まで待機
        StartCoroutine(GirlAnimReadSystem(11));
        while (nowGirlAnimOtherLayerClipNameDict[11] != "背比べスマホしまう") { Debug.Log(nowGirlAnimOtherLayerClipNameDict[11]); yield return null; }
        while (girlAnimOtherLayerNomTimeDict[11] <= 0.65f) { Debug.Log("bb"); yield return null; }
        //スマホObjオフ（しまい）//画面勝手に消える
        ChieriSumahoObj.SetActive(false);

        while (girlAnimOtherLayerNomTimeDict[11] <= 1) { yield return null; }
        yield return null;
        //マスクオフ
        ChieriMotion("_noData", 1f, 11);

        Debug.Log("cc");

        yield break;
    }

    //家に入るとき こうすると登れる
    public IEnumerator RME_PlayerSkinWidthSet()
    {
        yield return null;
        KO_CharacterController.skinWidth = 0.0001f;
        yield break;
    }
    //家の縮小待機
    public IEnumerator RME_HouseShrinkWait()
    {
        //家取得して
        GameObject tmpObj =
            GameObject.Find("House00_TsukueUe_AddCollider");

        //家の大きさが0.0016くらいになるまで待機
        while (tmpObj.transform.localScale.x > 0.001601f)
        {
            yield return null;
        }

        yield break;
    }

    //家の中のコリダーObj用
    public List<GameObject> houseColObjList = new List<GameObject>();
    public IEnumerator RME_houseColObjListLoad()
    {
        houseColObjList.Clear();

        //foreachで取得
        foreach (Transform trs in GameObjectsTrs.Find("House00_TsukueUe_AddCollider").Find("ColloderObjs").transform)
        {
            houseColObjList.Add(trs.gameObject);
        }

        yield break;
    }

    public IEnumerator RME_houseColObjRigidOn()
    {
        for (int i = 0; i < houseColObjList.Count; i++)
        {
            houseColObjList[i].GetComponent<Rigidbody>().isKinematic = false;
        }

        yield break;
    }

    //マコト家飛び出しジャンプ killできるように
    Tweener
        makotoF3090JumpTweener,
        makotoDummyF3090JumpRotTweener;
    public IEnumerator RME_F3090MakotoJumpTweener()
    {
        //マコトジャンプ移動位置取得
        GameObject Player093_IeJumpPosObj
            = Resources.Load("_PosObj/3090_OwarihetoChikadukuHibi/Player093_IeJumpPosObj") as GameObject;

        //キャンセルできるようにTweener
        makotoF3090JumpTweener =
            CameraObjectsTrs.DOLocalMove
            (Player093_IeJumpPosObj.transform.localPosition
            , 10)
            .SetEase(Ease.OutExpo);

        //マコトDummyジャンプ時回転位置取得
        GameObject Player_Dummy030_JumpKaiten
            = Resources.Load("_PosObj/3090_OwarihetoChikadukuHibi/Player_Dummy030_JumpKaiten") as GameObject;

        //キャンセルできるようにTweener
        makotoDummyF3090JumpRotTweener =
            Player_DummyTrs.DOLocalRotate
            (Player_Dummy030_JumpKaiten.transform.localEulerAngles
            , 10)
            .SetEase(Ease.OutExpo);
        yield break;
    }
    public IEnumerator RME_F3090MakotoJumpTweenerKill()
    {
        makotoF3090JumpTweener.Kill();
        makotoDummyF3090JumpRotTweener.Kill();
        yield break;
    }


    //マコトゆっくり落ちと白フェード制御
    Tweener makotoSlowFallTweener;
    public IEnumerator RME_SlowFall()
    {
        //マコト落ちていく位置取得
        GameObject Player110_TobideOti
            = Resources.Load("_PosObj/3090_OwarihetoChikadukuHibi/Player110_TobideOti") as GameObject;

        //キャンセルできるようにTweener
        makotoSlowFallTweener =
            CameraObjectsTrs.DOLocalMove
            (Player110_TobideOti.transform.localPosition
            , 100)
            .SetEase(Ease.InOutSine);

        //ガレキアニメ取得
        Animator garekiAnim =
            GameObjectsTrs.Find("House00_Fumi_Fractured_Pos").Find("House00_Fumi_Fractured").GetComponent<Animator>();

        //待機
        while (isF3080Reset == false &&
            garekiAnim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.95f)
        { yield return null; }


        //リセット対応
        if (isF3080Reset) { yield break; }


        //白フェード
        FadeWhite(1, 4f);

        ////マコト落ちストップ
        //makotoSlowFallTweener.Kill();

        yield break;
    }
    public IEnumerator RME_MakotFallTweenerKill()
    {
        makotoSlowFallTweener.Kill();
        yield break;
    }

    //IK頭調整
    public IEnumerator RME_ChieriHeadIKFull()
    {
        DOTweenToLAIKSHead(LAIKHeadS, 1, 0);
        yield break;
    }
    public IEnumerator RME_ChieriHeadIKFullFade()
    {
        DOTweenToLAIKSHead(LAIKHeadS, 1, 2);
        yield break;
    }

    //照れほほ
    public IEnumerator RME_F3080ChieriTerehoho()
    {
        TereHoho();
        yield break;
    }
    public IEnumerator RME_F3080ChieriTerehohoOff()
    {
        TereHoho(false);
        yield break;
    }

    //足音強制鳴らし
    public IEnumerator RME_isAnimTriggerRForcePlayTrue()
    {

        isAnimTriggerRForcePlay = true;
        //任意位置で再生用Objに位置代入
        GameObject tmpPosObj = new GameObject();
        animTriggerForcePlayObj = tmpPosObj;

        //位置ちゃんと取るようにMoreLate
        MoreLateAction(() =>
        {
            tmpPosObj.transform.position = GirlRoya02Trs.position;
            tmpPosObj.transform.localScale = new Vector3(0.3090f, 0.3090f, 0.3090f);
        });




        yield return null;

        //ちゃんと消し（nullなら普通の挙動になる仕様）
        Destroy(animTriggerForcePlayObj);
        animTriggerForcePlayObj = null;

        yield break;
    }
    #endregion


    public IEnumerator RME_TestCor()
    {
        for (int i = 0; i < houseColObjList.Count; i++)
        {
            Debug.Log(i + houseColObjList[i].name);
        }

        yield break;
    }

    public IEnumerator RME_KankyouBGM_Yuugata()
    {
        //ヒグラシに変え
        KankyouBGMChanger(semiSE.audioSource, "Higurashi_nc104892_間追加");

        yield break;
    }
    public IEnumerator RME_KankyouBGM_Hiru()
    {
        //セミに変え
        KankyouBGMChanger(semiSE.audioSource, "minminzemi-cry1");

        yield break;
    }

    public IEnumerator RME_PPv2MemaiOn()
    {
        PPv2Memai();
        yield break;
    }

    public IEnumerator RME_EventNo104Change()
    {
        DB.nowEventNum = 104;
        //コンティニュー時に開くイベントは、セーブデータからイベントナンバーを読み込むのでセーブ

        //移動先イベントのコマデータにPlayerVisを
        flowChartKomaDataObjList[DB.nowEventNum].GetComponent<FlowChartKoma>().isPlayerVisFlag = true;

        //時計合わせ
        if (isClockSystem) { SetClockHourMinute(); }

        Save();
        yield break;
    }


    //スパッツUpdateOffScreen表示
    public IEnumerator RME_EnableSpatsUpdateOffScreen()
    {
        //あれば
        if (GameObject.Find("Spats") != null)
        {
            GameObject.Find("Spats").GetComponent<SkinnedMeshRenderer>()
                .updateWhenOffscreen = true;
        }
        yield break;
    }

    //ちえりのアニメーター再設定（TL再生でリアルタイム切り替えすると、膝とかずれたので）
    public IEnumerator RME_chieriAnimatorReload()
    {
        Debug.Log("scripton");
        girlAnim.enabled = false;
        yield return new WaitForSeconds(1);
        girlAnim.enabled = true;
        Debug.Log("Reload");

        yield break;
    }

}





//メソッド名をタイムラインで指定すれば、そこで実行されます。

