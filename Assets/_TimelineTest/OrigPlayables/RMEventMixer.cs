using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;
using DG.Tweening;//DOTween
using System;//array

public class RMEventMixer : PlayableBehaviour
{

    //この変数はトラックから割り当てられる（作られた際（プレイ時でも？））
    public PlayableDirector m_PlayableDirector;
    public RMEventTrack m_RMEventTrack;
    public List<TimelineClip> m_clipsList;
    public DataCounter DC;
    public DataBridging DB;
    public PlayableGraph graph;

    //ミキサー（これ）で、Clipの位置から処理を行う

    bool isPreValueRead = false;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (Application.isPlaying)//■再生中のみ
        {
            #region セリフとノベルのプレビューや、ReadObj（Gameobject名読み込み）　フラグ初期化
            if (isPreValueRead == false)
            {
                for (int i = 0; i < m_clipsList.Count; i++)
                {
                    var bh = (m_clipsList[i].asset as RMEventClip).m_behaviour; // クリップが持つBehaviourを参照（パラメータ）
                    var cp = (m_clipsList[i].asset as RMEventClip); // クリップを参照

                    //プレビューテキスト読み込み
                    if (!string.IsNullOrWhiteSpace(bh.serihuKey))
                    { bh.previewSerihuValue = DC.serihuDict[bh.serihuKey]; }
                    if (!string.IsNullOrWhiteSpace(bh.novelKey))
                    { bh.previewNovelValue = DC.novelDict[bh.novelKey]; }
                    if (!string.IsNullOrWhiteSpace(cp.AKeySentakushi))
                    { cp.previewAValueSentakushi = DC.serihuDict[cp.AKeySentakushi]; }
                    if (!string.IsNullOrWhiteSpace(cp.BKeySentakushi))
                    { cp.previewBValueSentakushi = DC.serihuDict[cp.BKeySentakushi]; }


                    #region ヒエラルキーのオブジェクト指定があればオブジェクト名を読み込み（ペアレントオブジェなど）
                    // PlayableBehaviour に, 入力値を引き渡す.
                    if (bh.ReadParentObj.Resolve(graph.GetResolver()) != null)
                    {
                        bh.ParentObjName = bh.ReadParentObj.Resolve(graph.GetResolver()).name;
                    }
                    if (bh.ReadChildObj.Resolve(graph.GetResolver()) != null)
                    {
                        bh.ChildObjName = bh.ReadChildObj.Resolve(graph.GetResolver()).name;
                    }
                    if (bh.readMoveObj.Resolve(graph.GetResolver()) != null)
                    {
                        bh.moveObjName = bh.readMoveObj.Resolve(graph.GetResolver()).name;
                    }

                    //■一括の方
                    //一つ以上指定あれば（構造体）
                    if (1 <= cp.moveObjs.Length)
                    {
                        for (int mv = 0; mv < cp.moveObjs.Length; mv++)
                        {
                            if (cp.moveObjs[mv].readNameObj.Resolve(graph.GetResolver()) != null)
                            {
                                cp.moveObjs[mv].objName = cp.moveObjs[mv].readNameObj.Resolve(graph.GetResolver()).name;
                            }
                        }
                    }
                    //一つ以上指定あれば（構造体）
                    if (1 <= cp.motions.Length)
                    {
                        for (int m = 0; m < cp.motions.Length; m++)
                        {
                            if (cp.motions[m].readNameObj.Resolve(graph.GetResolver()) != null)
                            {
                                cp.motions[m].objName = cp.motions[m].readNameObj.Resolve(graph.GetResolver()).name;
                            }
                        }
                    }
                    //取得ミスが起きることあるので、クリップ上で処理をすることにしてみた
                    ////一つ以上指定あれば（構造体）SetActiveのためにRoot読み取って参照
                    //if (1 <= cp.setActiveObjs.Length)
                    //{
                    //    for (int sa = 0; sa < cp.setActiveObjs.Length; sa++)
                    //    {
                    //        if (cp.setActiveObjs[sa].readAddressObj.Resolve(graph.GetResolver()) != null)
                    //        {
                    //            #region RootObjNameとPathとObjName取得（ObjNameはエレメント名用）

                    //            //ReadObjまでのpathを取得
                    //            //とりあえずTrs取得（参考にしたブログにこう書いてあったが、よく把握してない→）直接Transformにキャストできないので一回GameObjectにする
                    //            var targetTrs = cp.setActiveObjs[sa].readAddressObj.Resolve(graph.GetResolver()).transform;
                    //            //■エレメント名用に名前取得
                    //            cp.setActiveObjs[sa].objName = targetTrs.name;

                    //            //Path作り用にリスト
                    //            var targetList = new List<Transform>();
                    //            //一個目とりあえずAdd
                    //            targetList.Add(targetTrs);

                    //            //親改装をすべて取得　//親が無くなるまでAddしていく
                    //            while (targetTrs.parent != null)
                    //            {
                    //                targetTrs = targetTrs.parent;
                    //                targetList.Add(targetTrs);
                    //            }
                    //            //↑最後のTrsはRootObjなので↓取得
                    //            //■RootObj参照
                    //            cp.setActiveObjs[sa].rootObjName = targetList[targetList.Count - 1].name;
                    //            //Pathリストからは外す
                    //            targetList.RemoveAt(targetList.Count - 1);

                    //            //Path作成(逆順for)
                    //            var path = "";
                    //            for (int a = targetList.Count - 1; a >= 0; a--)
                    //            {
                    //                path += "/" + targetList[a].name;
                    //            }
                    //            //最初の"/"要らない
                    //            path = path.Remove(0, 1);
                    //            //■代入
                    //            cp.setActiveObjs[sa].objPath = path;


                    //            #endregion
                    //        }
                    //    }
                    //}
                    #endregion


                    #region フラグ初期化
                    //トラックフラグ初期化
                    for (int f = 0; f < m_RMEventTrack.flagBoolList.Count; f++)
                    {
                        m_RMEventTrack.flagBoolList[f] = false;
                    }

                    //フラグ数セット
                    Array.Resize(ref cp.flagBoolArray, m_RMEventTrack.flagBoolList.Count);
                    Array.Resize(ref cp.aSentakuflagBoolArray, m_RMEventTrack.flagBoolList.Count);
                    Array.Resize(ref cp.bSentakuflagBoolArray, m_RMEventTrack.flagBoolList.Count);
                    Array.Resize(ref cp.m_behaviour.movePointEnterFlagBoolArray, m_RMEventTrack.flagBoolList.Count);
                    #endregion
                }
                isPreValueRead = true;
                //Debug.Log("テキストプレビューやオブジェクト取得完了");
            }
            #endregion


            #region 処理開始までの流れ
            //タイムラインの位置time
            var time = m_PlayableDirector.time;

            //クリップを一個ずつ読んで処理する
            for (int i = 0; i < m_clipsList.Count; i++)
            {
                //まずクリップの変数用behaviour読み込み
                var bh = (m_clipsList[i].asset as RMEventClip).m_behaviour; // クリップが持つBehaviourを参照（パラメータ）

                //↑ではインスペクターをカスタムしづらいので、Recordなどを諦めてクリップ参照
                var cp = (m_clipsList[i].asset as RMEventClip); // クリップを参照

                //クリップがtimeより前だったら（タイムラインがクリップに入った、通り過ぎた）
                if (time >= m_clipsList[i].start
                    //&& time <= m_clipsList[i].end 　//（クリップ内のみにする場合）
                    )
                {
                    //まだ処理してなかったら、してboolをTrue
                    if (!bh.isClipEnter)
                    {
                        bh.isClipEnter = true;
                        #endregion


                        #region フラグ確認（設定されていればそのフラグがONの時実行
                        if (cp.isUseFlagBool)
                        {
                            //まず結果判定用のフラグを生成
                            bool isTmpFlag = false;

                            //クリップに設定されたフラグを一個ずつ照らし合わせ (一個でも合ってなかったらfalseにして抜ける)
                            for (int f = 0; f < cp.flagBoolArray.Length; f++)
                            {
                                //クリップのフラグとトラックのフラグが合ってたらフラグON
                                if (cp.flagBoolArray[f] == m_RMEventTrack.flagBoolList[f])
                                { isTmpFlag = true; }
                                else //合ってなければオフして抜け
                                {
                                    isTmpFlag = false;
                                    break;
                                }
                            }

                            //結果、フラグたってなければ抜け(クリップ実行しない)
                            if (isTmpFlag == false) { goto クリップ抜け; }

                        }

                        //StaticFlag（セーブデータに書き込むフラグ）
                        if (cp.isUseStaticFlagBool)
                        {
                            //フラグがちゃんと設定されているか確認
                            if (DC.staticFlagDict.ContainsKey(cp.staticFlagKey))
                            {
                                //フラグたってなければ抜け(クリップ実行しない)
                                if (DC.staticFlagDict[cp.staticFlagKey] == false) { goto クリップ抜け; }
                            }
                            else
                            {
                                Debug.Log("■" + cp.staticFlagKey + "がStaticFlagDictにない？");
                            }

                        }


                        #endregion

                        #region プレイヤーカメラ処理
                        //カメラPosObjがあれば
                        if (cp.cameraObjectsTrsPosObj != null)
                        {
                            //移動
                            DC.CameraObjectsTrs.transform.DOLocalMove(
                                cp.cameraObjectsTrsPosObj.transform.localPosition
                                , cp.cameraObjectsTrsTime)
                                .SetEase(cp.cameraObjectsTrsEase);

                            //回転
                            DC.CameraObjectsTrs.DOLocalRotate(
                                cp.cameraObjectsTrsPosObj.transform.localEulerAngles
                                , cp.cameraObjectsTrsTime)
                                .SetEase(cp.cameraObjectsTrsEase)
                                .OnComplete(() =>
                                {
                                    //カメラリセット回転値設定
                                    DB.cameraObjectsResetLocalEul = cp.cameraObjectsTrsPosObj.transform.localEulerAngles;
                                });

                            //スケール
                            DOTween.To(() => DC.nowPlayerLocalScale, (x) => DC.nowPlayerLocalScale = x
                            , cp.cameraObjectsTrsPosObj.transform.localScale
                            , cp.cameraObjectsTrsTime)
                            .SetEase(cp.cameraObjectsTrsEase);
                        }
                        //目眩演出
                        if (cp.isUseMemai)
                        {
                            DC.PPv2MemaiLittle(1f);
                            DC.SEPlay("magic-attack-darkness1_long(-12)", 0.5f);
                        }
                        #endregion
                        #region 選択肢処理Cp
                        //両方キーが指定されていたらそれで再生
                        if (!string.IsNullOrWhiteSpace(cp.AKeySentakushi)
                            && !string.IsNullOrWhiteSpace(cp.BKeySentakushi))
                        {
                            DC.Sentakushi(cp.AKeySentakushi);
                            DC.Sentakushi(cp.BKeySentakushi);

                            //ウェイト（フラグ書き込みも兼ねる）
                            DC.StartCoroutine(DC.UTLSentakushiWait(
                                m_PlayableDirector
                                , m_RMEventTrack
                                , cp.aSentakuflagBoolArray
                                , cp.bSentakuflagBoolArray));

                        }
                        //tmpAキーあれば
                        else if (!string.IsNullOrWhiteSpace(cp.tmpAKeySentakushi)
                            && !string.IsNullOrWhiteSpace(cp.tmpBKeySentakushi))
                        {
                            //Dictに追加して
                            DC.serihuDict.Add("■" + cp.tmpAKeySentakushi + "_tmp", cp.tmpAValueSentakushi);
                            DC.engSerihuDict.Add("■" + cp.tmpAKeySentakushi + "_tmp", cp.tmpAValueSentakushi);
                            //Dictに追加して
                            DC.serihuDict.Add("■" + cp.tmpBKeySentakushi + "_tmp", cp.tmpBValueSentakushi);
                            DC.engSerihuDict.Add("■" + cp.tmpBKeySentakushi + "_tmp", cp.tmpBValueSentakushi);


                            //再生
                            DC.Sentakushi("■" + cp.tmpAKeySentakushi + "_tmp", cp.tmpAValueSentakushi);
                            DC.Sentakushi("■" + cp.tmpBKeySentakushi + "_tmp");


                            //削除
                            DC.serihuDict.Remove("■" + cp.tmpAKeySentakushi + "_tmp");
                            DC.engSerihuDict.Remove("■" + cp.tmpAKeySentakushi + "_tmp");
                            //削除
                            DC.serihuDict.Remove("■" + cp.tmpBKeySentakushi + "_tmp");
                            DC.engSerihuDict.Remove("■" + cp.tmpBKeySentakushi + "_tmp");

                            //ウェイト（フラグ書き込みも兼ねる）
                            DC.StartCoroutine(DC.UTLSentakushiWait(
                                m_PlayableDirector
                                , m_RMEventTrack
                                , cp.aSentakuflagBoolArray
                                , cp.bSentakuflagBoolArray));
                        }
                        #endregion
                        #region モーション関連 Cp

                        #region ■モーション 一括指定再生
                        //一つ以上指定あれば（MotionStructArray（構造体））
                        if (1 <= cp.motions.Length)
                        {
                            for (int m = 0; m < cp.motions.Length; m++)
                            {
                                //モーションObj名指定があれば、そのObjのAnimaterControllerで
                                if (!string.IsNullOrWhiteSpace(cp.motions[m].objName))
                                {
                                    //モーション名が指定されていたら
                                    if (!string.IsNullOrWhiteSpace(cp.motions[m].stateName))
                                    {
                                        #region モーションObj名とスポーンObj名が一緒なら1フレ待つフラグ立てる(単体モーション命令のと同じ)
                                        bool is1FrameWait = false;//1フレ待つか判定用

                                        //スポーンObj名が指定されていて
                                        if (!string.IsNullOrWhiteSpace(bh.spawnObjName))
                                        {
                                            //モーションObj名とスポーンObj名が一緒だったら
                                            if (cp.motions[m].objName == bh.spawnObjName)
                                            {
                                                //1フレ待つフラグON
                                                is1FrameWait = true;
                                            }
                                        }

                                        //一括スポーンObj名もチェック
                                        //一つ以上指定あれば（構造体）
                                        if (1 <= cp.spawnObjs.Length)
                                        {
                                            for (int s = 0; s < cp.spawnObjs.Length; s++)
                                            {
                                                if (cp.motions[m].objName == cp.spawnObjs[s].spawnObjName)
                                                {
                                                    //1フレ待つフラグON
                                                    is1FrameWait = true;
                                                }
                                            }
                                        }

                                        #endregion


                                        //■■※ SpawnするPrefab内部にオブジェクトがあると、判定が取れないため、一旦強制1フレ待ち
                                        is1FrameWait = true;



                                        //単体のコルーチンと同じのを使って処理（Obj設置と同時にモーション指定している場合は1フレ遅らせる処理）
                                        DC.StartCoroutine(DC.UTLMotionIEnum
                                            (cp.motions[m].objName
                                            , cp.motions[m].stateName
                                            , cp.motions[m].crossFadeTime
                                            , cp.motions[m].layer
                                            , is1FrameWait));
                                    }
                                }//else{} //obj名指定なければ何もしない

                            }

                        }


                        #endregion

                        #region ■モーションでウェイト（指定モーション完了までTL待機）
                        //ステート名指定があれば
                        if (!string.IsNullOrWhiteSpace(cp.waitMotionClipName))
                        {
                            //モーションウェイトコルーチン始動
                            DC.StartCoroutine(DC.UTLMotionWait
                                (m_PlayableDirector
                                , cp.waitMotionObjName
                                , cp.waitMotionClipName
                                , cp.waitMotionNormlizedTime
                                ));
                        }
                        #endregion

                        #endregion
                        #region IK関連 Cp
                        //IKEf指定されていたら
                        if (cp.useIKEf != RMEventClip.UseIKEf.__)
                        {
                            #region IKEfを取得
                            RootMotion.FinalIK.IKEffector tmpIKEf = null;

                            if (cp.useIKEf == RMEventClip.UseIKEf.LFootEf)
                            { tmpIKEf = DC.IKLFootEf;}
                            else if (cp.useIKEf == RMEventClip.UseIKEf.RFootEf)
                            { tmpIKEf = DC.IKRFootEf; }
                            else if (cp.useIKEf == RMEventClip.UseIKEf.LHandEf)
                            { tmpIKEf = DC.IKLHandEf; }
                            else if (cp.useIKEf == RMEventClip.UseIKEf.RHandEf)
                            { tmpIKEf = DC.IKRHandEf; }
                            #endregion

                            DC.DOTweenToIKEfPos(tmpIKEf
                                , cp.IKWeight
                                , cp.IKTime
                                , cp.IKEase);
                            DC.DOTweenToIKEfRot(tmpIKEf
                                , cp.IKWeight
                                , cp.IKTime
                                , cp.IKEase);
                        }

                        //IK位置戻し
                        if (cp.isGirlPosToIKTargetPosRot)
                        { DC.GirlPosToIKTargetPosRot(); }


                        #endregion
                        #region 宿題システム Cp
                        if (cp.hWSystem == RMEventClip.HWSystemONOFF.ON)
                        {
                            DC.StartCoroutine(DC.RMEHWSystemIEnum());
                        }
                        else if (cp.hWSystem == RMEventClip.HWSystemONOFF.OFF)
                        {
                            DC.isRMEHwSystem = false;
                        }
                        #endregion
                        #region オブジェクト 配置移動削除 関係

                        #region ■一括 プレファブをGameobjectsに設置
                        //一つ以上指定あれば（構造体）
                        if (1 <= cp.spawnObjs.Length)
                        {
                            for (int s = 0; s < cp.spawnObjs.Length; s++)
                            {
                                if (cp.spawnObjs[s].spawnObj != null)
                                {
                                    GameObject spawnObj
                                        = UnityEngine.Object.Instantiate(cp.spawnObjs[s].spawnObj, DC.GameObjectsTrs);
                                    spawnObj.name = cp.spawnObjs[s].spawnObjName;//名前指定
                                }
                            }
                        }

                        #endregion

                        #region ■一括 Obj移動 回転 拡縮

                        //一つ以上指定あれば（構造体）
                        if (1 <= cp.moveObjs.Length)
                        {
                            for (int mv = 0; mv < cp.moveObjs.Length; mv++)
                            {

                                //移動Objと移動先PosObjがあれば
                                if (cp.moveObjs[mv].objName != "" && cp.moveObjs[mv].posObj != null)
                                {
                                    //Nullチェック
                                    if (GameObject.Find(cp.moveObjs[mv].objName) != null)
                                    {
                                        var tmpObj = GameObject.Find(cp.moveObjs[mv].objName);

                                        //Obj移動
                                        tmpObj.transform.DOLocalMove(
                                            cp.moveObjs[mv].posObj.transform.localPosition
                                            , cp.moveObjs[mv].time)
                                            .SetEase(cp.moveObjs[mv].ease);

                                        //Obj回転
                                        tmpObj.transform.DOLocalRotate(
                                            cp.moveObjs[mv].posObj.transform.localEulerAngles
                                            , cp.moveObjs[mv].time)
                                            .SetEase(cp.moveObjs[mv].ease);

                                        //Objスケール
                                        //if (bh.isEnableScale)
                                        //{
                                        tmpObj.transform.DOScale(
                                            cp.moveObjs[mv].posObj.transform.localScale
                                            , cp.moveObjs[mv].time)
                                            .SetEase(cp.moveObjs[mv].ease);
                                        //}
                                    }
                                    else
                                    {
                                        Debug.Log("■" + bh.moveObjName + "がヒエラルキーにない？ 一括移動ObjName");
                                    }
                                }
                            }
                        }


                        #endregion

                        #region ■一括 ObjアクティブONOFF
                        //一つ以上指定あれば（構造体）
                        if (1 <= cp.setActiveObjs.Length)
                        {
                            for (int sa = 0; sa < cp.setActiveObjs.Length; sa++)
                            {
                                //readAddressObjが指定されていたら取得
                                if (cp.setActiveObjs[sa].readAddressObj.Resolve(graph.GetResolver()) != null)
                                {
                                    #region RootObjNameとPathとObjName取得（ObjNameはエレメント名用）

                                    //ReadObjまでのpathを取得
                                    //とりあえずTrs取得（参考にしたブログにこう書いてあったが、よく把握してない→）直接Transformにキャストできないので一回GameObjectにする
                                    var targetTrs = cp.setActiveObjs[sa].readAddressObj.Resolve(graph.GetResolver()).transform;
                                    //■エレメント名用に名前取得
                                    cp.setActiveObjs[sa].objName = targetTrs.name;

                                    //Path作り用にリスト
                                    var targetList = new List<Transform>();
                                    //一個目とりあえずAdd
                                    targetList.Add(targetTrs);

                                    //親改装をすべて取得　//親が無くなるまでAddしていく
                                    while (targetTrs.parent != null)
                                    {
                                        targetTrs = targetTrs.parent;
                                        targetList.Add(targetTrs);
                                    }
                                    //↑最後のTrsはRootObjなので↓取得
                                    //■RootObj参照
                                    cp.setActiveObjs[sa].rootObjName = targetList[targetList.Count - 1].name;
                                    //Pathリストからは外す
                                    targetList.RemoveAt(targetList.Count - 1);

                                    //Path作成(逆順for)
                                    var path = "";
                                    for (int a = targetList.Count - 1; a >= 0; a--)
                                    {
                                        path += "/" + targetList[a].name;
                                    }
                                    //最初の"/"要らない
                                    path = path.Remove(0, 1);
                                    //■代入
                                    cp.setActiveObjs[sa].objPath = path;


                                    #endregion
                                }

                                //rootObjNameがあれば
                                if (cp.setActiveObjs[sa].rootObjName != "")
                                {
                                    //rootObjNameをFind
                                    GameObject.Find(cp.setActiveObjs[sa].rootObjName)
                                        //Addressで参照
                                        .transform.Find(cp.setActiveObjs[sa].objPath)
                                        //セットActive
                                        .gameObject.SetActive(cp.setActiveObjs[sa].setActive);
                                }
                            }
                        }
                        #endregion

                        #region ■指定オブジェの子オブジェ全削除
                        if (!string.IsNullOrWhiteSpace(cp.childAllDelObjName))
                        {
                            //Nullチェック
                            if (GameObject.Find(cp.childAllDelObjName) != null)
                            {
                                var tmpObj = GameObject.Find(cp.childAllDelObjName);
                                //子オブジェ全削除
                                foreach (Transform trs in tmpObj.transform)
                                {
                                    UnityEngine.Object.Destroy(trs.gameObject);
                                }
                            }
                            else
                            {
                                Debug.Log("■" + cp.childAllDelObjName + "がヒエラルキーにない？" + nameof(cp.childAllDelObjName));
                            }

                        }
                        #endregion

                        #endregion
                        #region 特殊設定　ライト Frac爆発設置 ちえりスマホなど Cp
                        #region ■ポストプロセス交換 デフォルトライトのオンオフ

                        //PostProcess設定するなら
                        if (cp.defPostProcessSwitch != RMEventClip.DefPostProcessSwitch.__)
                        {
                            //ONなら
                            if (cp.defPostProcessSwitch == RMEventClip.DefPostProcessSwitch.ON)
                            {
                                //既存のポストプロセスON
                                DC.PostProcessVolume00DirectDataObj.SetActive(true);
                            }
                            //OFFなら
                            if (cp.defPostProcessSwitch == RMEventClip.DefPostProcessSwitch.off)
                            {
                                //既存のポストプロセスオフ
                                DC.PostProcessVolume00DirectDataObj.SetActive(false);
                            }
                        }


                        //ライト設定するなら
                        if (cp.defLightSwitch != RMEventClip.DefLightSwitch.__)
                        {
                            //ONなら
                            if (cp.defLightSwitch == RMEventClip.DefLightSwitch.ON)
                            {
                                for (int L = 0; L < DC.defLightList.Count; L++)
                                {
                                    DC.defLightList[L].enabled = DC.defLightONOFFBoolList[L];
                                }
                            }
                            //OFFなら
                            if (cp.defLightSwitch == RMEventClip.DefLightSwitch.off)
                            {
                                for (int L = 0; L < DC.defLightList.Count; L++)
                                {
                                    DC.defLightList[L].enabled = false;
                                }
                            }
                        }
                        #endregion

                        #region ■Frac爆発設定　ちえりコリダーが衝突したFracturedObjが爆発
                        //初期化なら元に戻す
                        if (cp.fracImpact == RMEventClip.FracImpact.初期化)
                        {
                            DC.UTLFracImpactSetting(false);
                        }
                        //設定なら設定する
                        else if (cp.fracImpact == RMEventClip.FracImpact.設定)
                        {
                            DC.UTLFracImpactSetting(true, cp.impactPosObj, cp.impactForce, cp.impactRadius, cp.bAlsoImpactFreeChunks);
                        }
                        #endregion

                        #region ちえりスマホ画面
                        if (cp.chieriSumahoMonitor == RMEventClip.ChieriSumahoMonitor.OFF)
                        {
                            DC.ChieriSumahoPower(false);
                            //■サイズ画面ON
                            DC.Status_SeeSizeObjClone.SetActive(true);
                            //□スマホカメラオフ
                            DC.ChieriSumahoObj.transform.Find("ChieriSmartPhoneCanvas/FrontCameraRenderTexture").gameObject.SetActive(false);
                        }
                        else if (cp.chieriSumahoMonitor == RMEventClip.ChieriSumahoMonitor.Size)
                        {
                            DC.ChieriSumahoPower(true);
                            //■サイズ画面ON
                            DC.Status_SeeSizeObjClone.SetActive(true);
                            //□スマホカメラオフ
                            DC.ChieriSumahoObj.transform.Find("ChieriSmartPhoneCanvas/FrontCameraRenderTexture").gameObject.SetActive(false);
                        }
                        else if (cp.chieriSumahoMonitor == RMEventClip.ChieriSumahoMonitor.FrontCamera)
                        {
                            DC.ChieriSumahoPower(true);
                            //□サイズ画面オフ
                            DC.Status_SeeSizeObjClone.SetActive(false);
                            //■スマホカメラON
                            DC.ChieriSumahoObj.transform.Find("ChieriSmartPhoneCanvas/FrontCameraRenderTexture").gameObject.SetActive(true);
                        }
                        #endregion

                        #region ちえり足跡系


                        if (cp.chieriFootstepObj == RMEventClip.ChieriFootstepObj.True)
                        { DC.GirlTrs.GetComponent<ChieriFootStepSpawn>().isFootStepObj = true; }
                        else if (cp.chieriFootstepObj == RMEventClip.ChieriFootstepObj.False)
                        { DC.GirlTrs.GetComponent<ChieriFootStepSpawn>().isFootStepObj = false; }


                        if (cp.chieriFootstepDecal == RMEventClip.ChieriFootstepDecal.True)
                        { DC.GirlTrs.GetComponent<ChieriFootStepSpawn>().isFootStepDecal = true; }
                        else if (cp.chieriFootstepDecal == RMEventClip.ChieriFootstepDecal.False)
                        { DC.GirlTrs.GetComponent<ChieriFootStepSpawn>().isFootStepDecal = false; }


                        #endregion

                        #region Fogリアルタイム設定
                        if (cp.fogRealtime != RMEventClip.FogRealtime.__)
                        {
                            #region オプションのFog ONOFF設定処理 ほぼそのまま(オプション設定ごと書き換える セーブされる)

                            if (cp.fogRealtime == RMEventClip.FogRealtime.False)
                            { DB.isUserFog = false; }
                            else if(cp.fogRealtime == RMEventClip.FogRealtime.True)
                            { DB.isUserFog = true; }

                            DC.PPv2FPSLayerComponent.fog.enabled =
                            DC.PPv2TPSLayerComponent.fog.enabled = DB.isUserFog;
                            RenderSettings.fog = DB.isUserFog;
                            //TogglleChange(itToggleObj, DB.isUserFog);

                            #endregion

                            //カラー
                            RenderSettings.fogColor = cp.fogColor;
                            RenderSettings.fogDensity = cp.fogDensity;
                        }

                        #endregion

                        #endregion

                        #region フキダシ処理
                        //キーが指定されていたらそれを再生
                        if (bh.serihuKey != "")
                        { DC.Hukidashi(bh.serihuKey); }
                        else if (bh.tmpSerihuKey != "") //tmpKeyあればtmpkeyとValueで再生
                        {
                            //Dictに追加して
                            DC.serihuDict.Add(bh.tmpSerihuKey + "_tmp", bh.tmpSerihuValue);
                            DC.engSerihuDict.Add(bh.tmpSerihuKey + "_tmp", bh.tmpSerihuValue);
                            //再生
                            DC.Hukidashi(bh.tmpSerihuKey + "_tmp");
                            //削除
                            DC.serihuDict.Remove(bh.tmpSerihuKey + "_tmp");
                            DC.engSerihuDict.Remove(bh.tmpSerihuKey + "_tmp");

                        }
                        else { }//どちらも指定なかったらなにもしない。

                        //キー待ちあればコルーチンでキー待ち
                        if (bh.isSerihuKeyWait)
                        { DC.StartCoroutine(DC.UTLKeyOrWait(m_PlayableDirector, 1)); }
                        #endregion
                        #region ノベル処理
                        //キーが指定されていたらそれを再生
                        if (bh.novelKey != "")
                        { DC.NovelSetVis(bh.novelKey); }
                        else if (bh.tmpNovelKey != "") //tmpKeyあればtmpkeyとValueで再生
                        {
                            //■UTF8じゃないせいか、*のあとに一文字入れないと改ページ時の頭一文字消えるので、*の後に一文字追加
                            var tmpValue
                                = bh.tmpNovelValue.Replace("*", "* ");

                            //Dictに追加して
                            DC.novelDict.Add(bh.tmpNovelKey + "_tmp", tmpValue);
                            DC.engNovelDict.Add(bh.tmpNovelKey + "_tmp", tmpValue);

                            //■再生
                            //オートだったらオートで
                            if (bh.isNovelAuto) { DC.NovelSetVis(bh.tmpNovelKey + "_tmp", true); }
                            else { DC.NovelSetVis(bh.tmpNovelKey + "_tmp"); }

                            //削除
                            DC.novelDict.Remove(bh.tmpNovelKey + "_tmp");
                            DC.engNovelDict.Remove(bh.tmpNovelKey + "_tmp");
                        }
                        else { }//どちらも指定なかったらなにもしない。

                        //ノベル表示終了待ちあればコルーチンで待ち
                        if (bh.isNovelEndWait)
                        { DC.StartCoroutine(DC.UTLNovelSetVisIngWait(m_PlayableDirector)); }
                        #endregion
                        #region プレファブをGameobjectsに設置
                        if (bh.spawnObj != null)
                        {
                            GameObject spawnObj = UnityEngine.Object.Instantiate(bh.spawnObj, DC.GameObjectsTrs);
                            spawnObj.name = bh.spawnObjName;//名前指定
                        }
                        #endregion
                        #region モーション、表情処理

                        //モーションObj名指定があれば、そのObjのAnimaterControllerで
                        if (!string.IsNullOrWhiteSpace(bh.motionObjName))
                        {
                            //モーション名が指定されていたら
                            if (!string.IsNullOrWhiteSpace(bh.motionStateName))
                            {
                                #region モーションObj名とスポーンObj名が一緒なら1フレ待つフラグ立てる(単体モーション命令のと同じ)
                                bool is1FrameWait = false;//1フレ待つか判定用

                                //スポーンObj名が指定されていて
                                if (!string.IsNullOrWhiteSpace(bh.spawnObjName))
                                {
                                    //モーションObj名とスポーンObj名が一緒だったら
                                    if (bh.motionObjName == bh.spawnObjName)
                                    {
                                        //1フレ待つフラグON
                                        is1FrameWait = true;
                                    }
                                }

                                //一括スポーンObj名もチェック
                                //一つ以上指定あれば（構造体）
                                if (1 <= cp.spawnObjs.Length)
                                {
                                    for (int s = 0; s < cp.spawnObjs.Length; s++)
                                    {
                                        if (bh.motionObjName == cp.spawnObjs[s].spawnObjName)
                                        {
                                            //1フレ待つフラグON
                                            is1FrameWait = true;
                                        }
                                    }
                                }

                                #endregion


                                //■■※ SpawnするPrefab内部にオブジェクトがあると、判定が取れないため、一旦強制1フレ待ち
                                is1FrameWait = true;


                                //コルーチンを使って処理（Obj設置と同時にモーション指定している場合は1フレ遅らせる処理）
                                DC.StartCoroutine(DC.UTLMotionIEnum
                                    (bh.motionObjName
                                    , bh.motionStateName
                                    , bh.motionCrossFadeTime
                                    , bh.motionLayer
                                    , is1FrameWait));
                            }
                        }
                        else//obj名指定なければちえり（従来どおり）
                        {
                            //モーション名が指定されていたらそれを再生
                            if (!string.IsNullOrWhiteSpace(bh.motionStateName))
                            { DC.ChieriMotion(bh.motionStateName, bh.motionCrossFadeTime, 0); }
                        }

                        //表情名が指定されていたら再生
                        if (!string.IsNullOrWhiteSpace(bh.faceStateName))
                        { DC.ChieriMotion(bh.faceStateName, bh.faceCrossFadeTime, 2); }


                        #endregion
                        #region IKLookAtPlyer見る
                        //プレイヤー見る
                        if (bh.iKLookAtPlayer == RMEventBehaviour.IKLookAtPlayer.プレイヤー見る)
                        {
                            #region IKLookAtプレイヤー見る
                            DC.ChieriMotion("まばたき", 0f, 4); DC.blinkTime = 0;
                            DC.FollowDOMove(DC.IKLookAtEyeTargetTrs, DC.PlayerEyeTargetTrs, 0f);
                            DC.DOTweenToLAIKSEyes(DC.LAIKEyeS, DC.LAIKSEyesDefWeight, 0f);
                            DC.FollowDOMove(DC.IKLookAtHeadTargetTrs, DC.PlayerHeadTargetTrs, new Vector3(0, -0.045f, 0));
                            DC.DOTweenToLAIKSHead(DC.LAIKHeadS, DC.LAIKSHeadDefWeight, 1f);
                            #endregion
                        }
                        //解除
                        else if (bh.iKLookAtPlayer == RMEventBehaviour.IKLookAtPlayer.解除)
                        {
                            #region IKLookAt解除
                            DC.ChieriMotion("まばたき", 0f, 4); DC.blinkTime = 0;
                            DC.DOTweenToLAIKSEyes(DC.LAIKEyeS, 0, 0f);
                            DC.DOTweenToLAIKSHead(DC.LAIKHeadS, 0, 1f);
                            #endregion
                        }

                        #endregion
                        #region 削除destroyObjNameあれば
                        if (!string.IsNullOrWhiteSpace(bh.destroyObjName))
                        {
                            //Nullチェック
                            if (GameObject.Find(bh.destroyObjName) != null)
                            {
                                //ヒエラルキーからObj削除
                                UnityEngine.Object.Destroy(GameObject.Find(bh.destroyObjName));
                            }
                            else
                            {
                                Debug.Log("■" + bh.destroyObjName + "がヒエラルキーにない？ destroyObjName");
                            }

                        }
                        #endregion
                        #region 削除destroyObjListあれば
                        if (bh.destroyObjList.Count != 0)
                        {
                            //ヒエラルキーからObj削除
                            for (int d = 0; d < bh.destroyObjList.Count; d++)
                            {
                                //Nullチェック
                                if (GameObject.Find(bh.destroyObjList[d]) != null)
                                {
                                    UnityEngine.Object.Destroy(GameObject.Find(bh.destroyObjList[d]));
                                }
                                else
                                {
                                    Debug.Log("■" + bh.destroyObjList[d] + "がヒエラルキーにない？ destroyObjList[" + d + "]");
                                }
                            }
                        }
                        #endregion
                        #region ペアレント
                        //ペアレントobj名、親と子 両方指定あれば
                        if (bh.ParentObjName != "" && bh.ChildObjName != "")
                        {
                            //ペアレント（Obj名で直接）
                            DC.LateAction(() =>
                            {
                                //Nullチェック
                                if (GameObject.Find(bh.ChildObjName) != null)
                                {
                                    GameObject.Find(bh.ChildObjName).transform
                                    .SetParent(GameObject.Find(bh.ParentObjName).transform, true);
                                }
                                else
                                {
                                    Debug.Log("■" + bh.ChildObjName + "がヒエラルキーにない？ ChildObjName");
                                }
                            });
                        }
                        #endregion
                        #region RigidBodyのisKinematic
                        //参照するObj名があれば
                        if (!string.IsNullOrWhiteSpace(bh.rigidbodyObjName))
                        {
                            //Nullチェック
                            if (GameObject.Find(bh.rigidbodyObjName) != null)
                            {
                                //そのObjのRigidbodyのisKinematicを設定
                                GameObject.Find(bh.rigidbodyObjName).GetComponent<Rigidbody>().isKinematic
                                    = bh.isKinematic;
                            }
                            else
                            {
                                Debug.Log("■" + bh.rigidbodyObjName + "がヒエラルキーにない？ rigidbodyObjName");
                            }
                        }
                        #endregion
                        #region プレイヤー位置
                        if (bh.playerLocalPosObj != null)
                        {
                            DC.LateAction(() =>
                            {
                                //プレイヤー位置指定
                                GameObject tmpPosObj
                                //= Resources.Load(path) as GameObject;
                                = bh.playerLocalPosObj;
                                DC.CameraObjectsTrs.localPosition = tmpPosObj.transform.localPosition;
                                DC.CameraObjectsTrs.localEulerAngles = tmpPosObj.transform.localEulerAngles;
                                //カメラリセット回転値設定
                                DB.cameraObjectsResetLocalEul = tmpPosObj.transform.localEulerAngles;
                            });
                        }
                        #endregion
                        #region プレイヤー立ち座り倒れ
                        //立ち指定あれば立ちカメラリセット
                        if (bh.playerStandSitFall == RMEventBehaviour.PlayerStandSit.立ち)
                        {
                            #region プレイヤー立ち（倒れてたら　倒れから復帰するように立つ）
                            if (DC.isPlayerFallDownSystem)
                            {
                                DC.playerFallDownDefCameraAnchorPos = DB.cameraStandAnchorDefLocalPos;
                                DC.playerFallDownDefCameraAnchorEul = Vector3.zero;
                                DC.isPlayerFallDownSystem = false;
                            }
                            else//倒れてないなら一瞬で
                            {
                                //立ちでカメラリセット（ユーザーカメラ動かさない）
                                DC.CameraReset(null,
                                    DB.cameraStandAnchorDefLocalPos//Anchorを立ちに
                                    , false, null, false, false);
                            }
                            #endregion
                        }
                        //座り
                        else if (bh.playerStandSitFall == RMEventBehaviour.PlayerStandSit.座り)
                        {
                            #region プレイヤー座り（倒れてたら　倒れから復帰するように座る）
                            if (DC.isPlayerFallDownSystem)
                            {
                                DC.playerFallDownDefCameraAnchorPos = DB.cameraSitAnchorDefLocalPos;
                                DC.playerFallDownDefCameraAnchorEul = Vector3.zero;
                                DC.isPlayerFallDownSystem = false;
                            }
                            else//倒れてないなら一瞬で
                            {
                                //座りでカメラリセット（ユーザーカメラ動かさない）
                                DC.CameraReset(null,
                                    DB.cameraSitAnchorDefLocalPos//Anchorを座りに
                                    , false, null, false, false);
                            }
                            #endregion
                        }
                        //倒れる
                        else if (bh.playerStandSitFall == RMEventBehaviour.PlayerStandSit.倒れる)
                        {
                            //倒れコルーチンスタート
                            DC.StartCoroutine(DC.PlayerFallDownSystemIEnum());
                        }
                        #endregion
                        #region カメラ揺れあれば
                        if (bh.isCameraDOShake)
                        {
                            DC.StartCoroutine(DC.UTLDOShakePosition
                                (bh.durationDOShake
                                , bh.strengthDOShake
                                , bh.vibratoDOShake
                                , 90 //Randomness
                                , false //Snaping
                                , bh.fadeOutDOShake)
                                );
                        }
                        #endregion
                        #region カメラリセット
                        if (bh.isCameraReset)
                        {
                            //強制カメラリセット（トラッキングも）
                            DC.CameraReset(null, null, true);
                        }
                        #endregion
                        #region 黒フェードインアウト
                        //フェードイン
                        if (bh.fadeBlack == RMEventBehaviour.FadeBlack.IN)
                        {
                            DC.FadeBlack(1, bh.fadeBlackTime);
                        }
                        //アウト
                        else if (bh.fadeBlack == RMEventBehaviour.FadeBlack.OUT)
                        {
                            DC.FadeBlack(0, bh.fadeBlackTime, false, Ease.InCubic);
                        }

                        #endregion
                        #region 白フェードインアウト
                        //フェードイン
                        if (bh.fadeWhite == RMEventBehaviour.FadeWhite.IN)
                        {
                            DC.FadeWhite(1, bh.fadeWhiteTime);
                        }
                        //アウト
                        else if (bh.fadeWhite == RMEventBehaviour.FadeWhite.OUT)
                        {
                            DC.FadeWhite(0, bh.fadeWhiteTime, false, Ease.InCubic);
                        }

                        #endregion
                        #region ちえりPosLock
                        //True
                        if (bh.chieriPosLock == RMEventBehaviour.ChieriPosLock.True)
                        { DB.isChieriPosLock = true; }
                        //False
                        else if (bh.chieriPosLock == RMEventBehaviour.ChieriPosLock.False)
                        { DB.isChieriPosLock = false; }

                        #endregion
                        #region Obj移動 回転 拡縮
                        //移動Objと移動先PosObjがあれば
                        if (bh.moveObjName != "" && bh.movePosObj != null)
                        {
                            //Nullチェック
                            if (GameObject.Find(bh.moveObjName) != null)
                            {
                                var tmpObj = GameObject.Find(bh.moveObjName);
                                //Obj移動（Obj名で直接）
                                tmpObj.transform.DOLocalMove(
                                    bh.movePosObj.transform.localPosition
                                    , bh.moveTime)
                                    .SetEase(bh.moveEase);

                                //Obj回転（Obj名で直接）
                                tmpObj.transform.DOLocalRotate(
                                    bh.movePosObj.transform.localEulerAngles
                                    , bh.moveTime)
                                    .SetEase(bh.moveEase);

                                //Objスケール（Obj名で直接）
                                if (bh.isEnableScale)
                                {
                                    tmpObj.transform.DOScale(
                                        bh.movePosObj.transform.localScale
                                        , bh.moveTime)
                                        .SetEase(bh.moveEase);
                                }
                            }
                            else
                            {
                                Debug.Log("■" + bh.moveObjName + "がヒエラルキーにない？ moveObjName");
                            }
                        }

                        #endregion
                        #region 移動ポイント設置起動 終了

                        //移動ポイント終了命令あれば
                        if (bh.isSystemOffMovePoint)
                        {
                            #region 終了と移動ポイント設置が同時だった場合は、移動ポイント削除のみに
                            if (bh.movePointPosObj != null)
                            {
                                DC.KO_isMovePosLock = false;//移動止め解除
                                DC.KO_isMovePosSet = false;//移動先なしに

                                //隠れ場所オブジェ削除
                                for (int k = 0; k < DC.KO_KakurePosObjsList.Count; k++)
                                {
                                    UnityEngine.Object.Destroy(DC.KO_KakurePosObjsList[k]);
                                }
                                //Enter待ちウェイトと、Enterフラグオフ
                                DC.isUTLKO_SimplePointObjWait = false;
                                DC.isKO_SimplePointObj_Enter = false;
                                //Resources.UnloadUnusedAssets();
                                Debug.Log("シンプル移動ポイント削除のみ処理完了");
                            }
                            else
                            #endregion
                            {
                                //ただ終了（ポイント削除命令も自動で行われる）
                                DC.isKOSystem = false;
                                //到着待ちフラグ残留を防ぐため、一応オフ（シークやスキップへの対応）
                                DC.isUTLKO_SimplePointObjWait = false;
                                DC.isKO_SimplePointObj_Enter = false;
                            }
                        }
                        //移動ポイント到着待ちウェイトあれば
                        if (bh.isEnterWaitMovePoint)
                        {
                            //到着まで待機コルーチン
                            DC.StartCoroutine(DC.UTLKO_SimplePointObjWait(m_PlayableDirector));
                        }
                        //移動ポイントPosObj設置と起動
                        if (bh.movePointPosObj != null)
                        {
                            ////移動ポイント終了命令あれば2フレ待つ
                            //if (bh.isSystemOffMovePoint) { yield return null; yield return null; }
                            //yield使えない

                            //移動ポイントObj本体
                            GameObject tmpMovePointObj
                                = UnityEngine.Object.Instantiate(Resources.Load("EventSystem/KakureOni/Prefab/KO_SimplePointObj") as GameObject
                                , DC.GameObjectsTrs);
                            //システム終了時削除するようにリストに入れ
                            DC.KO_KakurePosObjsList.Add(tmpMovePointObj);

                            //移動ポイントObjの位置大きさ
                            GameObject tmpPosObj
                                //= Resources.Load(path) as GameObject;
                                = bh.movePointPosObj;
                            tmpMovePointObj.transform.localPosition = tmpPosObj.transform.localPosition;
                            tmpMovePointObj.transform.localEulerAngles = tmpPosObj.transform.localEulerAngles;
                            tmpMovePointObj.transform.localScale = tmpPosObj.transform.localScale;

                            //シンプル移動システム開始
                            DC.StartCoroutine(DC.KakureOniSimpleSystemLoad());
                            //ポイント出現 演出
                            DC.KO_NewPosPointObjVis(tmpMovePointObj);


                            //※フラグ書き込み命令があれば
                            if (bh.isUseEnterFlagBool)
                            {
                                //到着でフラグ書き込みコルーチン開始(トラックとboolリストを送って、書き込ませる)
                                DC.StartCoroutine(DC.UTLKO_SimplePointObjFlagWrite
                                    (m_RMEventTrack, bh.movePointEnterFlagBoolArray));
                            }


                        }

                        #endregion
                        #region SE調整（SEのPrefabが設置されていること前提）
                        //SEObj指定があれば
                        if (!string.IsNullOrWhiteSpace(bh.SEObjName))
                        {
                            //Nullチェック
                            if (GameObject.Find(bh.SEObjName) != null)
                            {
                                //そのSEObjのAudio取得（Obj名で直接）
                                AudioSource tmpAS =
                                    GameObject.Find(bh.SEObjName).GetComponent<AudioSource>();

                                //フェード処理
                                tmpAS.DOFade
                                    (bh.SEVolume
                                    , bh.SEFadeTime);
                            }
                            else
                            {
                                Debug.Log("■" + bh.SEObjName + "がヒエラルキーにない？ SEObjName");
                            }
                        }

                        #endregion
                        #region UnityTimeline再生cp
                        //UnityTimelineObj指定があれば
                        if (cp.UnityTimelineObj != null)
                        {
                            //PD再生

                            //Prefabにして取り出し（TimelineAssetをスクリプトのみで読み出して再生が難しかったので）
                            GameObject tmpPDObj
                                = UnityEngine.Object.Instantiate(cp.UnityTimelineObj);
                            tmpPDObj.name = cp.UnityTimelineObj.name;

                            PlayableDirector tmpPD
                                = tmpPDObj.GetComponent<PlayableDirector>();

                            tmpPD.Play();
                            UnityEngine.Object.Destroy(m_PlayableDirector.gameObject);
                        }
                        //イベント移動
                        if (cp.isEventMove)
                        {
                            DC.isPDStopped = true;

                            //移動先イベント名指定あって
                            if (cp.MoveEventName != "")
                            {
                                //強制シーンジャンプ読み込みの場合（シーン読み込みなおし） 
                                if (cp.isForceSceneLoad)
                                {
                                    DC.isFlowChartEventMove = true;
                                    DC.EventMove(cp.MoveEventName, true, true);
                                }
                                else //引継ぎ移動ならそのまま
                                {
                                    DC.EventMove(cp.MoveEventName);
                                }
                            }
                        }
                        #endregion

                        クリップ抜け:;
                    }
                }
                //timeより後だったら（タイムラインがクリップに到達前位置）
                else
                {
                    //Enterしてたらfalse
                    if (bh.isClipEnter)
                    { bh.isClipEnter = false; }
                    else { } //falseだったら何もしない
                }

            }
        }
    }

    //public override void OnPlayableDestroy(Playable playable)
    //{
    //    m_FirstFrameHappened = false;

    //    if (m_TrackBinding == null)
    //        return;

    //    m_TrackBinding.color = m_DefaultColor;
    //}
}
