//using追加も自由です（もし追加したら、後で教えてほしいかも）
using System.Collections;
using UnityEngine;
using DG.Tweening;//DOTween
using UnityEngine.XR;

//クラスは↓の通りにしてください。
public partial class DataCounter
{
    //■ここからコルーチンのメソッドを追加していきます

    //型はIEnumerator（コルーチンメソッド）　メソッド名は自由（RME_が付いてると、後で管理しやすいかも）
    
    public IEnumerator RME_samecaptured()
    {

        yield return new WaitForSeconds(2f);
        SEPlay("嚥下音00", 0.65f);
        yield return new WaitForSeconds(2f);

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


        yield return null;

        //ダミー
        PlayerMotion("Player020舌中央安定", 0f, 0);
        //Player_DummyTrs.localPosition = defDummyPos;
        //Player_DummyTrs.localEulerAngles = defDummyRot;
        Player_DummyTrs.localPosition = new Vector3(0, 0f, 0);
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

        #region 胃液上がり
        GameObject Ieki010_SuiiLastPosObj
            = Resources.Load("_PosObj/D_Henai_Day1/_ResizeMeEventteam/yd_load/yd_Iekiyd_Suii001PosObj") as GameObject;

        IekiTrs.DOLocalMove(Ieki010_SuiiLastPosObj.transform.localPosition
            , 150f)
            .SetEase(Ease.Linear);

        stomachAnim.Play("RotRepair");

        #endregion
        yield break;
    }

    public IEnumerator RME_samegameover()
    {
        FadeBlack(0, 3);
        //メニュー強制開き
        StartCoroutine(MenuSystemIEnum(true, false, true));
        yield break;
    }

    public IEnumerator RME_sameKounaiEffect()
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

    Tweener RME_SameChaseTweener;
    public IEnumerator RME_SameChaseTweenerOn()
    {
        //■PosObj読み込み
        GameObject yd_chierirPosObj_jaws1
            = Resources.Load("_PosObj/D_Henai_Day1/_ResizeMeEventteam/yd_load/yd_chierirPosObj_jaws1") as GameObject;

        //追いかける動きを専用変数に付与
        RME_SameChaseTweener =
            GirlTrs.DOLocalMove
            (yd_chierirPosObj_jaws1.transform.localPosition
            , 30)
            .SetEase(Ease.InOutQuad);

        yield break;
    }
    public IEnumerator RME_SameChaseTweenerKill()
    {
        RME_SameChaseTweener.Kill();
        yield break;
    }

        Tweener RME_tonderuTweener;
    public IEnumerator RME_SameTonderuPlayerHakidashi()
    {
        //Tweenで動かしつつ、セリフが終わったら次の動きへ上書き更新

        //■PosObj読み込み
        //吐き出すPosObj
        GameObject yd_playerPosObj_jaws5_sora
            = Resources.Load("_PosObj/D_Henai_Day1/_ResizeMeEventteam/yd_load/yd_playerPosObj_jaws5_sora") as GameObject;

        //吐き出す動きを専用変数に付与
        RME_tonderuTweener =
            CameraObjectsTrs.DOLocalMove
            (yd_playerPosObj_jaws5_sora.transform.localPosition
            , 3)
            .SetEase(Ease.OutQuart);

        //吐き出し時は回転も同時に
        CameraObjectsTrs.DOLocalRotate
        (yd_playerPosObj_jaws5_sora.transform.localEulerAngles
        , 2)
        .SetEase(Ease.OutQuart);


        yield break;
    }
    public IEnumerator RME_SameTonderuPlayerSlowMove()
    {
        //Tweenで動かしつつ、セリフが終わったら次の動きへ上書き更新

        //■PosObj読み込み
        //ゆっくり動き
        GameObject yd_playerPosObj_jaws5_5_Tonderu
            = Resources.Load("_PosObj/D_Henai_Day1/_ResizeMeEventteam/yd_load/yd_playerPosObj_jaws5_5_Tonderu") as GameObject;


        //吐き出す動きをキャンセル
        RME_tonderuTweener.Kill();

        //ゆっくり動きを専用変数に付与
        RME_tonderuTweener =
            CameraObjectsTrs.DOLocalMove
            (yd_playerPosObj_jaws5_5_Tonderu.transform.localPosition
            , 60)
            .SetEase(Ease.OutSine);

        yield break;
    }
    public IEnumerator RME_SameTonderuPlayerOtiru()
    {
        //Tweenで動かしつつ、セリフが終わったら次の動きへ上書き更新

        //■PosObj読み込み
        //落ちる
        GameObject yd_playerPosObj_jaws6_otiru
            = Resources.Load("_PosObj/D_Henai_Day1/_ResizeMeEventteam/yd_load/yd_playerPosObj_jaws6_otiru") as GameObject;

        //浮き上がる
        GameObject yd_playerPosObj_jaws7_Ukiagari
            = Resources.Load("_PosObj/D_Henai_Day1/_ResizeMeEventteam/yd_load/yd_playerPosObj_jaws7_Ukiagari") as GameObject;


        //ゆっくり動きをキャンセル
        RME_tonderuTweener.Kill();

        //落ちる動きを専用変数に付与
        RME_tonderuTweener =
            CameraObjectsTrs.DOLocalMove
            (yd_playerPosObj_jaws6_otiru.transform.localPosition
            , 5)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => {

                RME_tonderuTweener =
            CameraObjectsTrs.DOLocalMove
            (yd_playerPosObj_jaws7_Ukiagari.transform.localPosition
            , 5)
            .SetEase(Ease.OutQuad);

            });

        yield break;
    }

    public IEnumerator RME_sameKounaiEffectOff()
    {
        isF3030KounaiEffect = false;
        yield break;
    }
}

//メソッド名をタイムラインで指定すれば、そこで実行されます。

