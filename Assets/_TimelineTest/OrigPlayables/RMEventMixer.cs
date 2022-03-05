using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;
using DG.Tweening;//DOTween
using System;//array
using UnityEngine.XR;
using System.Reflection;//メソッド取り扱い
using UnityEngine.PostProcessing; //Post-processing Stackをスクリプトでいじるために必要
using UnityEngine.Rendering.PostProcessing;//PostProcessing Stack v2をスクリプトでいじるために必要

public class RMEventMixer : PlayableBehaviour
{

    //この変数はトラックから割り当てられる（作られた際（プレイ時でも？））
    public PlayableDirector m_PlayableDirector;
    public RMEventTrack m_RMEventTrack;
    public List<TimelineClip> m_clipsList;
    public DataCounter DC;
    public DataBridging DB;
    public ResourceFiles ResourceFiles;
    public PlayableGraph graph;

    //ミキサー（これ）で、Clipの位置から処理を行う


    //再生中毎フレーム
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

        if (Application.isPlaying)//■再生中のみ
        {
            #region //イベント製作チーム用

            ////ちえりのアニメーターコントローラーを、それぞれ各個人のものに変更                        
            //if (m_RMEventTrack.useAnimator != null) //トラックに設定あれば
            //{
            //    //（var tmpAnim みたいに変数化できない（参照型じゃない？））
            //    if (DC.GirlTrs.GetComponent<Animator>().runtimeAnimatorController != m_RMEventTrack.useAnimator)
            //    { DC.GirlTrs.GetComponent<Animator>().runtimeAnimatorController = m_RMEventTrack.useAnimator; }
            //}
            #endregion

            #region 再生時1フレ処理。セリフとノベルのプレビューや、ReadObj（Gameobject名読み込み）　フラグ初期化
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
                    Array.Resize(ref cp.debugFlagBoolArray, m_RMEventTrack.flagBoolList.Count);
                    #endregion



                    //Hideで廃止したのでコメントアウト
                    ////■一括DestroyObjに移設するため処理（今後処理はCpの方で行う）（しばらくしたら（全タイムラインが移設されたら）削除）
                    //if (bh.destroyObjList.Count > 0)
                    //{
                    //    Debug.Log("■リスト版Obj削除にデータが残っています。トラックから「一括Obj削除へ移設」を押して移設しておいてください");
                    //}

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


                        //ユーザー機能でミュートしてれば抜け
                        if (cp.clipMute == true) { goto クリップ抜け; }

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

                        //StaticFlag（セーブデータに書き込んであるフラグを読み取る）
                        if (cp.isUseStaticFlagBool)
                        {
                            //フラグがたっていれば実行か、たっていなければ実行か判定。
                            if (cp.staticFlagUseTrue)//立っていれば実行の場合
                            {
                                //フラグが立っていなかったら抜け
                                if (DC.staticFlagDict[cp.staticFlagKey] == false)
                                {
                                    goto クリップ抜け;
                                }
                            }
                            else //立っていなければ実行
                            {
                                //フラグが立っていたら抜け
                                if (DC.staticFlagDict[cp.staticFlagKey])
                                {
                                    goto クリップ抜け;
                                }
                            }

                        }

                        //■デバッグ フラグ書き込み
                        if (cp.isUseDebugFlagBool)
                        {
                            if (m_RMEventTrack.flagBoolList.Count != cp.debugFlagBoolArray.Length)
                            {
                                Debug.Log("■フラグ書き込みの数がイベントフラグの数と違っています");
                            }
                            else
                            {
                                for (int f = 0; f < cp.debugFlagBoolArray.Length; f++)
                                {
                                    m_RMEventTrack.flagBoolList[f] = cp.debugFlagBoolArray[f];
                                }
                            }
                        }


                        //■VRかPCか判定して実行
                        if (cp.vRorPC != RMEventClip.VRorPC.__)
                        {
                            //VR時に、PCが指定されてたら抜け
                            if (XRSettings.enabled)
                            {
                                if (cp.vRorPC == RMEventClip.VRorPC.PC)
                                { goto クリップ抜け; }
                            }
                            //PC時に、VRが指定されてたら抜け
                            else
                            {
                                if (cp.vRorPC == RMEventClip.VRorPC.VR)
                                { goto クリップ抜け; }
                            }

                        }

                        //セーブデータフラグ書き換え
                        if (cp.staticFlagWrite != RMEventClip.StaticFlagWrite.__)
                        {
                            if (cp.staticFlagWrite == RMEventClip.StaticFlagWrite.True)
                            {
                                DC.staticFlagDict[cp.writeStaticFlagKey] = true;
                            }
                            else if (cp.staticFlagWrite == RMEventClip.StaticFlagWrite.False)
                            {
                                DC.staticFlagDict[cp.writeStaticFlagKey] = false;
                            }
                        }

                        #endregion

                        #region 一括 使用メソッド名指定 Cp
                        if (cp.useMethodNameArray.Length > 0)
                        {
                            //メソッドウェイトが指定されていたら、終了待ちコルーチンリストを初期化→追加しながら実行
                            if (cp.isMethodWait)
                            {
                                DC.UTLNowRunMethodCoroutineList.Clear();
                                for (int m = 0; m < cp.useMethodNameArray.Length; m++)
                                {
                                    //実行
                                    DC.UTLNowRunMethodCoroutineList.Add
                                        (
                                        DC.StartCoroutine(cp.useMethodNameArray[m])
                                        );
                                }
                                //ウェイト用コルーチン実行
                                DC.StartCoroutine(DC.UTLMethodWait(m_PlayableDirector));
                            }
                            else //メソッドウェイト指定されていなかったらただ実行
                            {
                                for (int m = 0; m < cp.useMethodNameArray.Length; m++)
                                {
                                    //実行
                                    DC.StartCoroutine(cp.useMethodNameArray[m]);
                                }
                            }
                        }
                        #endregion
                        #region プレイヤー 処理（カメラ 目眩 ダミーハンド 身長オフセット）

                        #region カメラPosObjがあれば
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

                            //ユーザーのカメラ回転をキープする場合(逆算でかけてく)
                            if (cp.isKeepUserCamRot)
                            {
                                //変化分を抜き出す　posObj回転 - 現カメラ回転
                                Vector3 tmpV3
                                    = cp.cameraObjectsTrsPosObj.transform.localEulerAngles - DC.CameraObjectsTrs.localEulerAngles;

                                //userコントロールから変化分を引いた数を
                                tmpV3 = DC.CameraUserControlTrs.localEulerAngles - tmpV3;

                                //DOTWeenTOで、現在のUserコントロールに与える
                                DOTween.To(() => DC.CameraUserControlTrs.localEulerAngles, (x) => DC.CameraUserControlTrs.localEulerAngles = x
                                , tmpV3
                                , cp.cameraObjectsTrsTime)
                                .SetEase(cp.cameraObjectsTrsEase);

                                //※ 計算の理屈が結局理解できなかったけど、これでうまくいった。
                                //逆回転してしまうことがあるので、基本0秒で変化させるべき
                            }


                            //スケール
                            DOTween.To(() => DC.nowPlayerLocalScale, (x) => DC.nowPlayerLocalScale = x
                            , cp.cameraObjectsTrsPosObj.transform.localScale
                            , cp.cameraObjectsTrsTime)
                            .SetEase(cp.cameraObjectsTrsEase);
                        }
                        #endregion

                        //目眩演出
                        if (cp.isUseMemai)
                        {
                            DC.PPv2MemaiLittle(1f);
                            DC.SEPlay("magic-attack-darkness1_long(-12)", 0.5f);
                        }

                        #region プレイヤーPosを他ObjのPosと同期

                        if (cp.playerObjDouki == RMEventClip.PlayerObjDouki.同期開始)
                        {
                            //nullチェック
                            if (GameObject.Find(cp.doukiObjName) != null)
                            {
                                //同期するObjをDCに渡し
                                DC.UTLPlayerDoukiObj =
                                GameObject.Find(cp.doukiObjName);
                            }
                            else
                            {
                                Debug.Log("同期するObjがヒエラルキーにないかオフ？");
                                goto 抜け;
                            }

                            //同期する項目bool設定
                            DC.isUTLPlayerPosDouki = cp.isPosDouki;
                            DC.isUTLPlayerRotDouki = cp.isRotDouki;
                            DC.isUTLPlayerSclDouki = cp.isSclDouki;

                            //同期コルーチンスタート
                            DC.StartCoroutine(DC.UTLPlayerObjDouki());

                            抜け:;
                        }
                        else if (cp.playerObjDouki == RMEventClip.PlayerObjDouki.同期Off)
                        {
                            //同期設定初期化
                            DC.isUTLPlayerObjDoukiSystem =
                            DC.isUTLPlayerPosDouki =
                            DC.isUTLPlayerRotDouki =
                            DC.isUTLPlayerSclDouki = false;

                            DC.UTLPlayerDoukiObj = null;
                        }

                        #endregion

                        #region プレイヤーの手表示
                        if (cp.dummyHandEnum != RMEventClip.DummyHand.__)
                        {
                            if (cp.dummyHandEnum == RMEventClip.DummyHand.ON)
                            {
                                //Dummyハンド表示
                                DC.isDummyHandVis = true;
                                //FPSなら手表示に（TPS時はDummyの手があるので表示しない）
                                if (DC.tPSModeInt == 0)
                                {
                                    DC.Dummy_Hand.SetActive(true);
                                }
                            }
                            else if (cp.dummyHandEnum == RMEventClip.DummyHand.Off)
                            {
                                //Dummyハンド非表示
                                DC.isDummyHandVis = false;
                                DC.Dummy_Hand.SetActive(false);
                            }
                        }
                        #endregion

                        #region プレイヤーIK（TPSや手の表示時に仕様するイメージ）
                        //IKEf指定されていたら
                        if (cp.useMakotoIKEf != RMEventClip.UseMakotoIKEf.__)
                        {
                            #region IKEfを取得
                            RootMotion.FinalIK.IKEffector tmpIKEf = null;

                            if (cp.useMakotoIKEf == RMEventClip.UseMakotoIKEf.LFootEf)
                            { tmpIKEf = DC.PlayerIKLFootEf; }
                            else if (cp.useMakotoIKEf == RMEventClip.UseMakotoIKEf.RFootEf)
                            { tmpIKEf = DC.PlayerIKRFootEf; }
                            else if (cp.useMakotoIKEf == RMEventClip.UseMakotoIKEf.LHandEf)
                            { tmpIKEf = DC.PlayerIKLHandEf; }
                            else if (cp.useMakotoIKEf == RMEventClip.UseMakotoIKEf.RHandEf)
                            { tmpIKEf = DC.PlayerIKRHandEf; }
                            else if (cp.useMakotoIKEf == RMEventClip.UseMakotoIKEf.RLHand)
                            {
                                #region RLHandEf

                                tmpIKEf = DC.PlayerIKRHandEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);

                                tmpIKEf = DC.PlayerIKLHandEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);

                                #endregion
                                goto 抜け;
                            }
                            else if (cp.useMakotoIKEf == RMEventClip.UseMakotoIKEf.RLFoot)
                            {
                                #region RLFootEf

                                tmpIKEf = DC.PlayerIKRFootEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);

                                tmpIKEf = DC.PlayerIKLFootEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);

                                #endregion
                                goto 抜け;
                            }
                            else if (cp.useMakotoIKEf == RMEventClip.UseMakotoIKEf.ALL)
                            {
                                #region ALL
                                tmpIKEf = DC.PlayerIKRHandEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);

                                tmpIKEf = DC.PlayerIKLHandEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);

                                tmpIKEf = DC.PlayerIKRFootEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);

                                tmpIKEf = DC.PlayerIKLFootEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.makotoIKWeight
                                    , cp.makotoIKTime
                                    , cp.makotoIKEase);
                                #endregion
                                goto 抜け;
                            }

                            #endregion

                            DC.DOTweenToIKEfPos(tmpIKEf
                                , cp.makotoIKWeight
                                , cp.makotoIKTime
                                , cp.makotoIKEase);
                            DC.DOTweenToIKEfRot(tmpIKEf
                                , cp.makotoIKWeight
                                , cp.makotoIKTime
                                , cp.makotoIKEase);

                            抜け:;
                        }

                        //IK位置戻し
                        if (cp.isPlayerPosToIKTargetPosRot)
                        { DC.PlayerPosToIKTargetPosRot(); }
                        #endregion

                        #region プレイヤーモーションオフ（宙ぶらりんのようなポーズになります）
                        if (cp.playerUniqueMotion != RMEventClip.PlayerUniqueMotion.__)
                        {
                            if (cp.playerUniqueMotion == RMEventClip.PlayerUniqueMotion.True)
                            {
                                DC.isPlayerUniqueMotion = true;
                                DC.PlayerMotion("_noData", 0f, 0);
                            }
                            else
                            {
                                DC.isPlayerUniqueMotion = false;
                            }
                        }

                        #endregion

                        #region カメラエフェクト演出

                        if (cp.cameraEffect != RMEventClip.CameraEffect.__)
                        {
                            if (cp.cameraEffect == RMEventClip.CameraEffect.時間補正中の目眩)
                            {
                                DC.PPEffect_ZikanHosei();
                            }

                            if (cp.cameraEffect == RMEventClip.CameraEffect.時間補正目眩の点滅のみ)
                            {
                                DC.PPEffect_ZikanHoseiIntro();
                            }

                            if (cp.cameraEffect == RMEventClip.CameraEffect.OFF)
                            {
                                DC.PPEffect_FadeToDef(cp.cameraEffectTime);
                            }

                        }

                        #endregion

                        #region 身長表示オフセット

                        if (cp.makotoSintyouVisOffset != RMEventClip.MakotoSintyouVisOffset.__)
                        {
                            if (cp.makotoSintyouVisOffset == RMEventClip.MakotoSintyouVisOffset.OFF)
                            {
                                DC.isMakotoSizeOffset = false;
                            }
                            else if (cp.makotoSintyouVisOffset == RMEventClip.MakotoSintyouVisOffset.ON)
                            {
                                DC.isMakotoSizeOffset = true;
                                if (cp.offsetObjName != "")
                                {
                                    //nullチェック
                                    if (GameObject.Find(cp.offsetObjName) != null)
                                    {
                                        DC.MakotoSizeOffsetObj = GameObject.Find(cp.offsetObjName);
                                    }
                                    else
                                    {
                                        Debug.Log("身長表示オフセット対象Objが見つからない？"+ cp.offsetObjName);
                                    }
                                }

                            }


                        }

                        #endregion

                        #endregion
                        #region 選択肢処理Cp
                        //両方キーが指定されていたらそれで再生
                        if (!string.IsNullOrWhiteSpace(cp.AKeySentakushi)
                            && !string.IsNullOrWhiteSpace(cp.BKeySentakushi))
                        {
                            #region 念の為選択肢初期化（デバッグ移動の持ってきた）
                            DC.sentakuListNum = 99;//選択肢ナンバーデフォルトへ

                            //選択肢消し
                            DC.DelAll_Sentakushi();
                            //一時保持選択肢Listクリア
                            DC.sentakushiTempLogList.Clear();

                            #endregion


                            DC.Sentakushi(cp.AKeySentakushi);
                            DC.Sentakushi(cp.BKeySentakushi);

                            //ウェイト（フラグ書き込みも兼ねる）
                            DC.StartCoroutine(DC.UTLSentakushiWait(
                                m_PlayableDirector
                                , m_RMEventTrack
                                , cp.aSentakuflagBoolArray
                                , cp.bSentakuflagBoolArray
                                , cp.isSentakushiWait
                                , cp.throughSentakuflagBoolArray));


                        }
                        //tmpAキーあれば
                        else if (!string.IsNullOrWhiteSpace(cp.tmpAKeySentakushi)
                            && !string.IsNullOrWhiteSpace(cp.tmpBKeySentakushi))
                        {
                            #region 念の為選択肢初期化（デバッグ移動の持ってきた）
                            DC.sentakuListNum = 99;//選択肢ナンバーデフォルトへ
                            //選択肢消し
                            DC.DelAll_Sentakushi();
                            //一時保持選択肢Listクリア
                            DC.sentakushiTempLogList.Clear();
                            #endregion

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
                                , cp.bSentakuflagBoolArray
                                , cp.isSentakushiWait
                                , cp.throughSentakuflagBoolArray));

                        }

                        //選択肢解除（スルー）命令があれば 選択待ちのループ解除
                        if (cp.isSentakushiThrough)
                        { DC.isUTLSentakushiWaitLoop = false; }
                        #endregion
                        #region モーション関連 Cp

                        #region ■モーション 一括指定再生
                        //一つ以上指定あれば（MotionStructArray（構造体））
                        if (1 <= cp.motions.Length)
                        {
                            for (int m = 0; m < cp.motions.Length; m++)
                            {
                                //モーションObj名指定があれば、
                                if (!string.IsNullOrWhiteSpace(cp.motions[m].objName))
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
                                        , is1FrameWait
                                        , cp.motions[m].animatorOnOff
                                        ));
                                }

                            }

                        }


                        #endregion

                        #region ■モーションセッティング変更 一括
                        //一つ以上指定あれば（MotionSettingsStructArray（構造体））
                        if (1 <= cp.motionSettings.Length)
                        {
                            for (int m = 0; m < cp.motionSettings.Length; m++)
                            {
                                //モーションObj名指定があれば、
                                if (!string.IsNullOrWhiteSpace(cp.motionSettings[m].objName))
                                {
                                    #region モーションObj名とスポーンObj名が一緒なら1フレ待つフラグ立てる(単体モーション命令のと同じ)
                                    bool is1FrameWait = false;//1フレ待つか判定用

                                    //スポーンObj名が指定されていて
                                    if (!string.IsNullOrWhiteSpace(bh.spawnObjName))
                                    {
                                        //モーションObj名とスポーンObj名が一緒だったら
                                        if (cp.motionSettings[m].objName == bh.spawnObjName)
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
                                            if (cp.motionSettings[m].objName == cp.spawnObjs[s].spawnObjName)
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
                                    DC.StartCoroutine(DC.UTLMotionSettingIEnum
                                        (cp.motionSettings[m].objName
                                        , cp.motionSettings[m].animSpeed
                                        , is1FrameWait
                                        ));
                                }

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
                            { tmpIKEf = DC.IKLFootEf; }
                            else if (cp.useIKEf == RMEventClip.UseIKEf.RFootEf)
                            { tmpIKEf = DC.IKRFootEf; }
                            else if (cp.useIKEf == RMEventClip.UseIKEf.LHandEf)
                            { tmpIKEf = DC.IKLHandEf; }
                            else if (cp.useIKEf == RMEventClip.UseIKEf.RHandEf)
                            { tmpIKEf = DC.IKRHandEf; }
                            else if (cp.useIKEf == RMEventClip.UseIKEf.RLHand)
                            {
                                #region RLHandEf

                                tmpIKEf = DC.IKLHandEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);

                                tmpIKEf = DC.IKRHandEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);

                                #endregion
                                goto 抜け;
                            }
                            else if (cp.useIKEf == RMEventClip.UseIKEf.RLFoot)
                            {
                                #region RLFootEf

                                tmpIKEf = DC.IKLFootEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);

                                tmpIKEf = DC.IKRFootEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);

                                #endregion
                                goto 抜け;
                            }
                            else if (cp.useIKEf == RMEventClip.UseIKEf.ALL)
                            {
                                #region ALL
                                tmpIKEf = DC.IKLFootEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);

                                tmpIKEf = DC.IKRFootEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);

                                tmpIKEf = DC.IKLHandEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);

                                tmpIKEf = DC.IKRHandEf;
                                DC.DOTweenToIKEfPos(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);
                                DC.DOTweenToIKEfRot(tmpIKEf
                                    , cp.IKWeight
                                    , cp.IKTime
                                    , cp.IKEase);
                                #endregion
                                goto 抜け;
                            }

                            #endregion

                            DC.DOTweenToIKEfPos(tmpIKEf
                                , cp.IKWeight
                                , cp.IKTime
                                , cp.IKEase);
                            DC.DOTweenToIKEfRot(tmpIKEf
                                , cp.IKWeight
                                , cp.IKTime
                                , cp.IKEase);

                            抜け:;

                        }

                        #region BendGoalWeight調整
                        //BendGoalWeight指定されていたら
                        if (cp.chgIKBendGoalWeight != RMEventClip.ChgIKBendGoalWeight.__)
                        {
                            if (cp.chgIKBendGoalWeight == RMEventClip.ChgIKBendGoalWeight.LFoot)
                            { DC.FBBIK.solver.leftLegChain.bendConstraint.weight = cp.bendGoalWeight; }
                            else if (cp.chgIKBendGoalWeight == RMEventClip.ChgIKBendGoalWeight.RFoot)
                            { DC.FBBIK.solver.rightLegChain.bendConstraint.weight = cp.bendGoalWeight; }
                            else if (cp.chgIKBendGoalWeight == RMEventClip.ChgIKBendGoalWeight.LHand)
                            { DC.FBBIK.solver.leftArmChain.bendConstraint.weight = cp.bendGoalWeight; }
                            else if (cp.chgIKBendGoalWeight == RMEventClip.ChgIKBendGoalWeight.RHand)
                            { DC.FBBIK.solver.rightArmChain.bendConstraint.weight = cp.bendGoalWeight; }
                            else if (cp.chgIKBendGoalWeight == RMEventClip.ChgIKBendGoalWeight.RLHand)
                            {
                                DC.FBBIK.solver.leftArmChain.bendConstraint.weight =
                                DC.FBBIK.solver.rightArmChain.bendConstraint.weight = cp.bendGoalWeight;
                            }
                            else if (cp.chgIKBendGoalWeight == RMEventClip.ChgIKBendGoalWeight.RLFoot)
                            {
                                DC.FBBIK.solver.leftLegChain.bendConstraint.weight =
                                DC.FBBIK.solver.rightLegChain.bendConstraint.weight = cp.bendGoalWeight;
                            }
                            else if (cp.chgIKBendGoalWeight == RMEventClip.ChgIKBendGoalWeight.ALL)
                            {
                                DC.FBBIK.solver.leftArmChain.bendConstraint.weight =
                                DC.FBBIK.solver.rightArmChain.bendConstraint.weight =
                                DC.FBBIK.solver.leftLegChain.bendConstraint.weight =
                                DC.FBBIK.solver.rightLegChain.bendConstraint.weight = cp.bendGoalWeight;
                            }
                        }
                        #endregion

                        //IK位置戻し
                        if (cp.isGirlPosToIKTargetPosRot)
                        { DC.GirlPosToIKTargetPosRot(); }


                        #region IKLookAtPlyer見る
                        //「__」以外が指定されていたら専用コルーチン実行
                        if (cp.iKLookAtPlayer != RMEventClip.IKLookAtPlayerEnum.__)
                        {
                            DC.StartCoroutine(DC.RME_IKLookAt
                                (cp.iKLookAtPlayer
                                , cp.IKLookAtOnOffTime
                                , cp.isIKLookAtHead
                                , cp.isIKLookAtBody)
                                );
                        }
                        #endregion

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

                        //宿題中で
                        if (DC.isRMEHwSystem)
                        {
                            //ループ地点が設定されていたら
                            //スタート地点設定(クリップのお尻を、トラックに持たせた変数スタートタイムに設定)
                            if (cp.hWloop == RMEventClip.HWLoop.Start)
                            {
                                m_RMEventTrack.RMEHWLoopStartTime = m_clipsList[i].end;
                            }
                            //エンド地点設定（トラック変数に入ってるスタートタイムに戻る）
                            else if (cp.hWloop == RMEventClip.HWLoop.End)
                            {
                                m_PlayableDirector.time = m_RMEventTrack.RMEHWLoopStartTime;
                            }
                        }

                        //宿題後用タイムライン指定あれば
                        if (cp.HWEndGoTimelineAsset != null)
                        {
                            //DCの変数に送信
                            DC.RMEHWPlayDirector = m_PlayableDirector;//再生するこのDirector
                            DC.RMEHWEndGoTimelineAsset = cp.HWEndGoTimelineAsset;//タイムラインアセット自体
                        }
                        //宿題終了時UnityTimelineObj指定があれば
                        //else if (cp.HWEndGoTimelineObj != null)
                        //{
                        //    //PD再生

                        //    //Prefabにして取り出し（TimelineAssetをスクリプトのみで読み出して再生が難しかったので）
                        //    GameObject tmpPDObj
                        //        = UnityEngine.Object.Instantiate(cp.HWEndGoTimelineObj);
                        //    tmpPDObj.name = cp.HWEndGoTimelineObj.name;

                        //    PlayableDirector tmpPD
                        //        = tmpPDObj.GetComponent<PlayableDirector>();

                        //    tmpPD.Play();
                        //    UnityEngine.Object.Destroy(m_PlayableDirector.gameObject);
                        //}


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
                                    //Enumで、既に有ったら削除して設置
                                    if (cp.spawnObjs[s].spawnObjAlreadyEnum == RMEventClip.SpawnObjAlreadyEnum.削除して再設置)
                                    {
                                        if (GameObject.Find(cp.spawnObjs[s].spawnObjName) != null)
                                        {
                                            UnityEngine.Object.Destroy(GameObject.Find(cp.spawnObjs[s].spawnObjName));
                                        }
                                        //設置
                                        GameObject tmpSpawnObj
                                            = UnityEngine.Object.Instantiate(cp.spawnObjs[s].spawnObj, DC.GameObjectsTrs);
                                        tmpSpawnObj.name = cp.spawnObjs[s].spawnObjName;//名前指定
                                    }
                                    else if (cp.spawnObjs[s].spawnObjAlreadyEnum == RMEventClip.SpawnObjAlreadyEnum.設置しない)
                                    {
                                        if (GameObject.Find(cp.spawnObjs[s].spawnObjName) != null)
                                        {
                                            //何もしない
                                        }
                                        else
                                        {
                                            //設置
                                            GameObject tmpSpawnObj
                                                = UnityEngine.Object.Instantiate(cp.spawnObjs[s].spawnObj, DC.GameObjectsTrs);
                                            tmpSpawnObj.name = cp.spawnObjs[s].spawnObjName;//名前指定
                                        }
                                    }
                                    else
                                    {
                                        //設置
                                        GameObject tmpSpawnObj
                                            = UnityEngine.Object.Instantiate(cp.spawnObjs[s].spawnObj, DC.GameObjectsTrs);
                                        tmpSpawnObj.name = cp.spawnObjs[s].spawnObjName;//名前指定
                                    }

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
                                //移動Objと移動先PosObj(及び、ヒエラルキーのPosObjの名前指定)があれば
                                if (cp.moveObjs[mv].objName != "" &&
                                    (cp.moveObjs[mv].posObj != null || cp.moveObjs[mv].hierarchyPosObjName != ""))
                                {
                                    //Nullチェック
                                    if (GameObject.Find(cp.moveObjs[mv].objName) != null)
                                    {
                                        var tmpObj = GameObject.Find(cp.moveObjs[mv].objName);

                                        #region PosObj取得して実行（PosObj = local座標　　HierarchyPosObj = ワールド座標）
                                        GameObject tmpPosObj = null;

                                        //Sequence 作成（nextMoveがあるかもなので）（Appendで動きを追加していく）
                                        Sequence
                                            moveObjMovSeq = DOTween.Sequence(),
                                            moveObjRotSeq = DOTween.Sequence(),
                                            moveObjSclSeq = DOTween.Sequence();

                                        //PosObj指定あればそれでローカル動作
                                        if (cp.moveObjs[mv].posObj != null)
                                        {
                                            tmpPosObj = cp.moveObjs[mv].posObj;

                                            #region ■PosObjでローカル動作
                                            //Obj移動
                                            moveObjMovSeq.Append(
                                                tmpObj.transform.DOLocalMove(
                                                tmpPosObj.transform.localPosition
                                                , cp.moveObjs[mv].time)
                                                .SetEase(cp.moveObjs[mv].ease)
                                            );

                                            //Obj回転
                                            moveObjRotSeq.Append(
                                                tmpObj.transform.DOLocalRotate(
                                                    tmpPosObj.transform.localEulerAngles + cp.moveObjs[mv].addRotate //追加回転値を追加
                                                    , cp.moveObjs[mv].time
                                                    , cp.moveObjs[mv].rotateMode)//回転モード追加
                                                    .SetEase(cp.moveObjs[mv].ease)
                                            );

                                            //Objスケール
                                            //if (bh.isEnableScale)
                                            //{
                                            moveObjSclSeq.Append(
                                                tmpObj.transform.DOScale(
                                                    tmpPosObj.transform.localScale
                                                    , cp.moveObjs[mv].time)
                                                    .SetEase(cp.moveObjs[mv].ease)
                                            );

                                            //}
                                            #endregion
                                        }
                                        else //PosObj指定なければ HierarchyPosObjNameで取得（PosObj指定が優先される）(IKTargetの指定が主なので、現在はスケール処理が保留)
                                        {
                                            //Nullチェック
                                            if (GameObject.Find(cp.moveObjs[mv].hierarchyPosObjName) != null)
                                            {
                                                //HierarchyPosObjNameから取得
                                                tmpPosObj = GameObject.Find(cp.moveObjs[mv].hierarchyPosObjName);
                                            }

                                            #region ■ヒエラルキーのObjをPosObjとする場合はワールド座標

                                            //Obj移動
                                            moveObjMovSeq.Append(
                                                tmpObj.transform.DOMove(
                                                    tmpPosObj.transform.position
                                                    , cp.moveObjs[mv].time)
                                                    .SetEase(cp.moveObjs[mv].ease)
                                                    );

                                            //Obj回転
                                            moveObjRotSeq.Append(
                                                tmpObj.transform.DORotate(
                                                    tmpPosObj.transform.eulerAngles + cp.moveObjs[mv].addRotate //追加回転値を追加
                                                    , cp.moveObjs[mv].time
                                                    , cp.moveObjs[mv].rotateMode)//回転モード追加
                                                    .SetEase(cp.moveObjs[mv].ease)
                                                    );

                                            //Objスケール
                                            //if (bh.isEnableScale)
                                            //{
                                            moveObjSclSeq.Append(
                                                tmpObj.transform.DOScale(
                                                    tmpPosObj.transform.lossyScale
                                                    , cp.moveObjs[mv].time)
                                                    .SetEase(cp.moveObjs[mv].ease)
                                                    );
                                            //}

                                            #endregion
                                        }

                                        #region ■NextMove指定が1つ以上あれば（入れ子動作）
                                        if (cp.moveObjs[mv].nextMove.Length > 0)
                                        {
                                            //その数だけSequenceの末尾に追加していく
                                            for (int nmv = 0; nmv < cp.moveObjs[mv].nextMove.Length; nmv++)
                                            {
                                                GameObject nextTmpPosObj = null;
                                                //PosObj指定であればそれでローカル動作
                                                if (cp.moveObjs[mv].nextMove[nmv].posObj != null)
                                                {
                                                    nextTmpPosObj = cp.moveObjs[mv].nextMove[nmv].posObj;

                                                    #region ■PosObjでローカル動作（NextMove）
                                                    //Obj移動
                                                    moveObjMovSeq.Append(
                                                        tmpObj.transform.DOLocalMove(
                                                            nextTmpPosObj.transform.localPosition
                                                            , cp.moveObjs[mv].nextMove[nmv].time)
                                                            .SetEase(cp.moveObjs[mv].nextMove[nmv].ease)
                                                            );

                                                    //Obj回転
                                                    moveObjRotSeq.Append(
                                                        tmpObj.transform.DOLocalRotate(
                                                            nextTmpPosObj.transform.localEulerAngles + cp.moveObjs[mv].nextMove[nmv].addRotate //追加回転値を追加
                                                            , cp.moveObjs[mv].nextMove[nmv].time
                                                            , cp.moveObjs[mv].nextMove[nmv].rotateMode)//回転モード追加
                                                            )
                                                            .SetEase(cp.moveObjs[mv].nextMove[nmv].ease
                                                            );

                                                    //Obj拡縮
                                                    moveObjSclSeq.Append(
                                                        tmpObj.transform.DOScale(
                                                            nextTmpPosObj.transform.localScale
                                                            , cp.moveObjs[mv].nextMove[nmv].time)
                                                            .SetEase(cp.moveObjs[mv].nextMove[nmv].ease)
                                                            );
                                                    #endregion
                                                }
                                                else//PosObj指定なければ HierarchyPosObjNameで取得（PosObj指定が優先される）
                                                {
                                                    //Nullチェック
                                                    if (GameObject.Find(cp.moveObjs[mv].nextMove[nmv].hierarchyPosObjName) != null)
                                                    {
                                                        //HierarchyPosObjNameから取得
                                                        nextTmpPosObj = GameObject.Find(cp.moveObjs[mv].nextMove[nmv].hierarchyPosObjName);
                                                    }

                                                    #region ■ヒエラルキーのObjをPosObjとする場合はワールド座標

                                                    //Obj移動
                                                    moveObjMovSeq.Append(
                                                        tmpObj.transform.DOMove(
                                                            nextTmpPosObj.transform.position
                                                            , cp.moveObjs[mv].nextMove[nmv].time)
                                                            .SetEase(cp.moveObjs[mv].nextMove[nmv].ease)
                                                            );

                                                    //Obj回転
                                                    moveObjRotSeq.Append(
                                                        tmpObj.transform.DORotate(
                                                            nextTmpPosObj.transform.eulerAngles + cp.moveObjs[mv].nextMove[nmv].addRotate //追加回転値を追加
                                                            , cp.moveObjs[mv].nextMove[nmv].time
                                                            , cp.moveObjs[mv].nextMove[nmv].rotateMode)//回転モード追加
                                                            .SetEase(cp.moveObjs[mv].nextMove[nmv].ease)
                                                            );

                                                    //Objスケール
                                                    //if (bh.isEnableScale)
                                                    //{
                                                    moveObjSclSeq.Append(
                                                        tmpObj.transform.DOScale(
                                                            nextTmpPosObj.transform.lossyScale
                                                            , cp.moveObjs[mv].nextMove[nmv].time)
                                                            .SetEase(cp.moveObjs[mv].nextMove[nmv].ease)
                                                            );
                                                    //}

                                                    #endregion

                                                }

                                            }

                                        }

                                        #endregion

                                        #endregion
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
                                    //Null確認
                                    if (//rootObjNameをFind
                                        GameObject.Find(cp.setActiveObjs[sa].rootObjName)
                                        //Addressで参照
                                        .transform.Find(cp.setActiveObjs[sa].objPath)
                                        //セットActive
                                        .gameObject != null)
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
                        }
                        #endregion

                        #region ■一括 Objペアレント
                        //一つ以上指定あれば（構造体）
                        if (1 <= cp.setParentObjs.Length)
                        {
                            for (int sp = 0; sp < cp.setParentObjs.Length; sp++)
                            {
                                //LateActionをforでやるとArrayのindex範囲外エラー　になるので
                                //一旦名前を取得して行う
                                var tmpParentObjName = cp.setParentObjs[sp].ParentObjName;
                                var tmpChildObjName = cp.setParentObjs[sp].ChildObjName;

                                //boolでmoreLateActionもできるようにした（以前のは、以前の挙動のまま残しておく）
                                if (cp.setParentObjs[sp].moreLateAction)
                                {
                                    DC.MoreLateAction(() =>
                                    {
                                        //Nullチェック
                                        if (GameObject.Find(tmpChildObjName) != null)
                                        {
                                            GameObject.Find(tmpChildObjName).transform
                                            .SetParent(GameObject.Find(tmpParentObjName).transform, true);
                                        }
                                        else
                                        {
                                            Debug.Log("■" + tmpChildObjName + "がヒエラルキーにない？ ChildObjName");
                                        }
                                    });
                                }
                                else
                                {
                                    //ペアレント（Obj名で直接）
                                    DC.LateAction(() =>
                                    {
                                        //Nullチェック
                                        if (GameObject.Find(tmpChildObjName) != null)
                                        {
                                            GameObject.Find(tmpChildObjName).transform
                                            .SetParent(GameObject.Find(tmpParentObjName).transform, true);
                                        }
                                        else
                                        {
                                            Debug.Log("■" + tmpChildObjName + "がヒエラルキーにない？ ChildObjName");
                                        }
                                    });
                                }

                            }
                        }
                        #endregion

                        #region ■一括 Obj削除
                        if (cp.destroyObjArray.Length != 0)
                        {
                            //ヒエラルキーからObj削除
                            for (int d = 0; d < cp.destroyObjArray.Length; d++)
                            {
                                //Nullチェック
                                if (GameObject.Find(cp.destroyObjArray[d]) != null)
                                {
                                    UnityEngine.Object.Destroy(GameObject.Find(cp.destroyObjArray[d]));
                                }
                                else
                                {
                                    Debug.Log("■" + cp.destroyObjArray[d] + "がヒエラルキーにない？ destroyObjArray[" + d + "]");
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
                        #region ちえり着替え
                        if (cp.isClothsChg)
                        {
                            //ユーザー着替え維持設定かどうか
                            if (DB.isUserFixityOutfit)
                            {
                                //それでも強制で着替えるなら
                                if (cp.isForceClothsChg)
                                { goto 着替え処理; }
                                else //強制でないなら
                                { goto 抜け; }
                            }
                            else //着替え維持設定でないなら
                            { goto 着替え処理; }


                            着替え処理:
                            //DB.isUserClothsBarefoot = cp.isBarefoot;
                            if (cp.isBarefoot)
                            {
                                DB.intCurrentShoes = 1;
                            }
                            else if (cp.isBarefoot == false)
                            {
                                DB.intCurrentShoes = 0;
                            }

                            //DB.isUserClothsTankTop = cp.isTankTop;
                            if (cp.isTankTop)
                            {
                                DB.intCurrentCloth = 1;
                            }
                            else if (cp.isTankTop == false)
                            {
                                DB.intCurrentCloth = 0;
                            }

                            //DB.isUserClothsBikini = cp.isBikini;
                            if (cp.isBikini)
                            {
                                DB.intCurrentALL = 1;
                            }
                            else if (cp.isBikini == false)
                            {
                                DB.intCurrentALL = 0;
                            }

                            //DC.ClothsApply();
                            DC.ClothsApply_ydload();

                            if (cp.isTankTop && cp.isBikini) { Debug.Log("ビキニとタンクトップは同時にできません"); }
                            if (cp.isBarefoot == false && cp.isBikini) { Debug.Log("ビキニと靴下は同時にできません"); }

                            抜け:;
                        }

                        #endregion
                        #region 特殊設定　ライト Frac爆発設置 ちえりスマホなど Cp
                        #region ■ポストプロセス交換用 既存のONOFF 

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

                        #region ■設置したポストプロセス ウェイト量調整

                        if (!string.IsNullOrWhiteSpace(cp.postProcessObjName))
                        {
                            //Nullチェック
                            if (GameObject.Find(cp.postProcessObjName) != null)
                            {
                                //PostProcess取得
                                PostProcessVolume tmpPPVol =
                                    GameObject.Find(cp.postProcessObjName).GetComponent<PostProcessVolume>();

                                //フェード処理

                                //DOTWeenTO
                                DOTween.To(() => tmpPPVol.weight, (x) => tmpPPVol.weight = x
                                , cp.postProcessWeight
                                , cp.postProcessWeightFadetime);
                            }
                            else
                            {
                                Debug.Log("■" + cp.postProcessObjName + "がヒエラルキーにない？ postProcessObjName");
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
                            else if (cp.fogRealtime == RMEventClip.FogRealtime.True)
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

                        #region ちえりブレスController

                        if (cp.breathController != RMEventClip.BreathController.__)
                        {
                            if (cp.breathController == RMEventClip.BreathController.True)
                            { DC.OriBreathController.enabled = true; }
                            else if (cp.breathController == RMEventClip.BreathController.False)
                            { DC.OriBreathController.enabled = false; }
                        }

                        #endregion

                        #region ちえりブレスControllerをFinalIKより前に実行

                        if (cp.preRunbreathController != RMEventClip.PreRunBreathController.__)
                        {
                            //以前はFinalIKより後に実行していたのがデフォだったので、TrueFlaseが逆でややこしい
                            if (cp.preRunbreathController == RMEventClip.PreRunBreathController.True)
                            { DC.isBreathControllerMoreMoreLate = false; }
                            else if (cp.preRunbreathController == RMEventClip.PreRunBreathController.False)
                            { DC.isBreathControllerMoreMoreLate = true; }
                        }

                        #endregion

                        #region ちえり口パク
                        if (cp.chieriKuchipaku != RMEventClip.ChieriKuchipaku.__)
                        {
                            if (cp.chieriKuchipaku == RMEventClip.ChieriKuchipaku.True)
                            { DC.isChieriKuchipaku = true; }
                            else if (cp.chieriKuchipaku == RMEventClip.ChieriKuchipaku.False)
                            { DC.isChieriKuchipaku = false; }
                        }

                        #endregion

                        #region ちえりアニメスピード
                        if (cp.isChieriAnimSpeedChange)
                        {
                            //DOTWeenTO
                            DOTween.To(() => DC.girlAnim.speed, (x) => DC.girlAnim.speed = x
                            , cp.chieriAnimSpeed
                            , cp.chieriAnimSpeedChgTime)
                            .SetEase(cp.chieriAnimSpeedChgEase);
                        }

                        #endregion
                        #region ちえり足音ボリューム 足接地揺れ量設定
                        if (cp.isChieriAshiotoVolumeChange)
                        { DC.audioMixer.SetFloat("AsiotoVol", cp.chieriAshiotoVolume); }
                        if (cp.isChieriFootYurePowMulChange)
                        { DC.RMEFootYurePowMul = cp.chieriFootYurePowMul; }

                        #endregion

                        #region フキダシノベル距離変更
                        if (cp.isTextDistChg)
                        {
                            DC.HukidashiNovelDistanceChange(cp.textDistance, cp.textDistChgSpeed);
                        }
                        #endregion

                        #region 環境音ボリューム
                        if (cp.bGMSetting != RMEventClip.BGMSetting.__)
                        {
                            if (cp.bGMSetting == RMEventClip.BGMSetting.環境音)
                            {
                                DC.KankyouBGMVolumer(cp.bGMVolume, cp.bGMVolumeSpeed);
                            }
                        }
                        #endregion

                        #region ループ系SE再生

                        if (cp.loopSE != RMEventClip.LoopSE.__)
                        {
                            if (cp.loopSE == RMEventClip.LoopSE.マコト足音)
                            {
                                //マコト足音ループ
                                DC.StartCoroutine(DC.ActionLoopSystemCor(() =>
                                {
                                    DC.SEPlay(DC.UISEObj, DC.KO_PlayerAsioto_DefList[UnityEngine.Random.Range(0, DC.KO_PlayerAsioto_DefList.Count)], 0.3f);
                                }
                                , cp.loopSEIntervalTime
                                , cp.loopSEPlayTime));
                            }
                        }

                        //ストップあればストップ（現在はアクションループシステム一個を使って行っているので、複数指定できるようにするには別システム必要）
                        if (cp.stopLoopSE)
                        {
                            DC.isActionLoopSystem = false;
                        }

                        #endregion

                        #region VRトラッキング オンオフ

                        if (cp.vRTrackingEnable != RMEventClip.VRTrackingEnable.__)
                        {
                            if (cp.vRTrackingEnable == RMEventClip.VRTrackingEnable.ON)
                            {
                                #region トラッキングオンとリセット
                                InputTracking.disablePositionalTracking = false;
                                if (XRSettings.enabled)//VRポジションリセット
                                {
                                    DC.MoreLateAction(() =>
                                    {
                                        DC.VRUICameraTrs.localPosition = Vector3.zero;
                                        DC.VRCameraTrs.localPosition = Vector3.zero;
                                    });
                                }
                                #endregion
                            }
                            else if (cp.vRTrackingEnable == RMEventClip.VRTrackingEnable.OFF)
                            {
                                #region トラッキングオフとリセット
                                InputTracking.disablePositionalTracking = true;
                                if (XRSettings.enabled)//VRポジションリセット
                                {
                                    DC.MoreLateAction(() =>
                                    {
                                        DC.VRUICameraTrs.localPosition = Vector3.zero;
                                        DC.VRCameraTrs.localPosition = Vector3.zero;
                                    });
                                }
                                #endregion
                            }
                        }

                        #endregion

                        #region カメラグレーアウトONOFF

                        if (cp.cameraGrayOutMode != RMEventClip.VRBlockBlackOutMode.__)
                        {
                            if (cp.cameraGrayOutMode == RMEventClip.VRBlockBlackOutMode.ON)
                            {
                                DC.isVRBlockBlackOutMode = true;
                            }
                            else if (cp.cameraGrayOutMode == RMEventClip.VRBlockBlackOutMode.OFF)
                            {
                                DC.isVRBlockBlackOutMode = false;
                            }
                        }

                        #endregion

                        #region ちえり照れ頬
                        if (cp.isTerehohoUse)
                        {
                            //照れ頬作ったときのメソッド利用しているので若干ややこしい
                            DC.TereHohoAlphaChange(cp.terehohoValue, cp.terehohoTime);
                            DC.TereHoho(true, cp.terehohoTime);
                        }
                        #endregion

                        #region ちえり胸揺れ設定
                        if (cp.breastSpring != RMEventClip.BreastSpring.__)
                        {
                            Debug.Log("設定有り");
                            if (cp.breastSpring == RMEventClip.BreastSpring.ON)
                            {
                                Debug.Log("オン");
                                DC.rBreastSpringBone.enabled = true;
                                DC.lBreastSpringBone.enabled = true;
                            }
                            else if (cp.breastSpring == RMEventClip.BreastSpring.OFF)
                            {
                                Debug.Log("オフ");
                                DC.rBreastSpringBone.enabled = false;
                                DC.lBreastSpringBone.enabled = false;
                            }
                        }
                        #endregion

                        #region ちえり足から足音と揺れ強制再生
                        if (cp.isAnimTriggerRForcePlay)
                        { DC.isAnimTriggerRForcePlay = true; }
                        if (cp.isAnimTriggerLForcePlay)
                        { DC.isAnimTriggerLForcePlay = true; }
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
                        {
                            //オートだったらオートで
                            if (bh.isNovelAuto) { DC.NovelSetVis(bh.novelKey, true); }
                            else { DC.NovelSetVis(bh.novelKey); }
                        }
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
                            { DC.ChieriMotion(bh.motionStateName, bh.motionCrossFadeTime, bh.motionLayer); }
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
                        #region 削除destroyObjListあれば ※移設して廃止する //Hideで廃止済み 念の為処理だけはされるように残し
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
                            //Nullチェック
                            if (GameObject.Find(bh.ChildObjName) != null)
                            {
                                //専用のコルーチン始動
                                DC.StartCoroutine(DC.RMEParentIEnum(
                                    GameObject.Find(bh.ChildObjName), GameObject.Find(bh.ParentObjName)
                                    ));
                            }
                            else
                            {
                                Debug.Log("■" + bh.ChildObjName + "がヒエラルキーにない？ ChildObjName");
                            }
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
                            //DC.LateAction(() =>
                            //{
                            //    //プレイヤー位置指定
                            //    GameObject tmpPosObj
                            //    //= Resources.Load(path) as GameObject;
                            //    = bh.playerLocalPosObj;
                            //    DC.CameraObjectsTrs.localPosition = tmpPosObj.transform.localPosition;
                            //    DC.CameraObjectsTrs.localEulerAngles = tmpPosObj.transform.localEulerAngles;
                            //    //カメラリセット回転値設定
                            //    DB.cameraObjectsResetLocalEul = tmpPosObj.transform.localEulerAngles;
                            //});
                            DC.MoreLateAction(() =>
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
                            DC.StartCoroutine(DC.PlayerFallDownSystemIEnum(bh.fallSpeed));
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
                        #region カラーフェードインアウト
                        //フェードイン
                        if (bh.fadeColor == RMEventBehaviour.FadeColor.IN)
                        {
                            DC.FadeColor(bh.fadeColorColor, bh.fadeColorColor.a, bh.fadeColorTime);
                        }
                        //アウト
                        else if (bh.fadeColor == RMEventBehaviour.FadeColor.OUT)
                        {
                            DC.FadeColor(bh.fadeColorColor, 0, bh.fadeColorTime, Ease.InCubic);
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
                        //■終了時にカメラの向き維持設定あれば適用
                        if (bh.isSystemOffMovePointWithCamRot) { DC.KO_isSystemOffWithCamRot = true; }

                        //■移動ポイント終了命令あれば
                        if (bh.isSystemOffMovePoint)
                        {
                            #region 終了と移動ポイント設置が同時だった場合は、移動ポイント削除のみに
                            if (bh.movePointPosObj != null)
                            {
                                DC.KO_isMovePosLock = false;//移動止め解除
                                DC.KO_isMovePosSet = false;//移動先なしに

                                //隠れ場所オブジェ削除
                                for (int k = 0; k < DC.KO_SimpleKakurePosObjsList.Count; k++)
                                {
                                    UnityEngine.Object.Destroy(DC.KO_SimpleKakurePosObjsList[k]);
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

                        //■移動ポイント到着待ちウェイトあれば
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
                                = UnityEngine.Object.Instantiate(ResourceFiles.KO_SimplePointObj
                                , DC.GameObjectsTrs);
                            //システム終了時削除するようにリストに入れ
                            DC.KO_SimpleKakurePosObjsList.Add(tmpMovePointObj);

                            //移動ポイントObjの位置大きさ
                            GameObject tmpPosObj
                                //= Resources.Load(path) as GameObject;
                                = bh.movePointPosObj;
                            tmpMovePointObj.transform.localPosition = tmpPosObj.transform.localPosition;
                            tmpMovePointObj.transform.localEulerAngles = tmpPosObj.transform.localEulerAngles;
                            tmpMovePointObj.transform.localScale = tmpPosObj.transform.localScale;

                            //到着判定のコリダーを大きく（0.9倍 = ポイント表示消すのとほぼ同じ判定になる）
                            if (bh.isMovePointColliderBig)
                            {
                                tmpMovePointObj.transform.Find("KO_SimplePointObj_").localScale
                                    = new Vector3(bh.movePointColliderScale, bh.movePointColliderScale, bh.movePointColliderScale);
                            }


                            #region プレイヤーのアニメーション設定（移動システム起動前に設定）
                            if (bh.moveAnimation != RMEventBehaviour.MoveAnimation.__)
                            {
                                if (bh.moveAnimation == RMEventBehaviour.MoveAnimation.True)
                                { DC.isPlayerUniqueMotion = false; }
                                else if (bh.moveAnimation == RMEventBehaviour.MoveAnimation.False)
                                { DC.isPlayerUniqueMotion = true; }
                                else if (bh.moveAnimation == RMEventBehaviour.MoveAnimation.歩き)
                                { DC.isKO_PlayerWalk = true; }
                                else if (bh.moveAnimation == RMEventBehaviour.MoveAnimation.四つんばい)
                                { DC.isKO_PlayerYotsunbai = true; }
                            }
                            //何も設定されてない場合は
                            else
                            {
                                //移動速度が2以下だったら歩きに
                                if (2 > bh.moveSpeed)
                                { DC.isKO_PlayerWalk = true; }
                                else //でなければ走り
                                { DC.isKO_PlayerWalk = false; }
                            }
                            #endregion


                            //■シンプル移動システム開始
                            if (bh.isMovePlayerSmallest == false)
                            { DC.StartCoroutine(DC.KakureOniSimpleSystemLoad()); }
                            else //プレイヤー小さすぎるとうまく挙動しないため、応急処置的処理（普通はオフ）
                            { DC.StartCoroutine(DC.KakureOniSimpleSystemLoad(true)); }

                            //ポイント出現 演出
                            DC.KO_NewPosPointObjVis(tmpMovePointObj);

                            //※到着時自動システムオフ命令があれば
                            if (bh.isEnterAutoSystemOffMovePoint)
                            {
                                //到着で自動システムオフコルーチン開始
                                DC.StartCoroutine(DC.UTLKO_SimplePointObjAutoSystemOff());
                            }

                            //※フラグ書き込み命令があれば
                            if (bh.isUseEnterFlagBool)
                            {
                                //到着でフラグ書き込みコルーチン開始(トラックとboolリストを送って、書き込ませる)
                                DC.StartCoroutine(DC.UTLKO_SimplePointObjFlagWrite
                                    (m_RMEventTrack, bh.movePointEnterFlagBoolArray));
                            }

                            //移動速度
                            if (DC.playerMoveSpeed != bh.moveSpeed)
                            { DC.playerMoveSpeed = bh.moveSpeed; }



                            //足音リスト選択をDCの方に適用（enumとリストの管理をDCでやるようにしたため）
                            if (bh.moveSEList != DataCounter.KO_PlayerAsiotoListEnum.__)
                            {
                                DC.nowKO_PlayerAsiotoListEnum = bh.moveSEList;

                                //1ループ秒数指定（足音リスト設定時のみ適用）
                                DC.KO_AsiotoTimeCountMaxFloat = bh.moveSELoopSecond;
                            }

                        }

                        //バック移動をさせない機能
                        if (bh.moveBackLock != RMEventBehaviour.MoveBackLock.__)
                        {
                            if (bh.moveBackLock == RMEventBehaviour.MoveBackLock.True)
                            {
                                DC.KO_isBackLock = true;
                            }
                            else if (bh.moveBackLock == RMEventBehaviour.MoveBackLock.False)
                            {
                                DC.KO_isBackLock = false;
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
                            //PD再生(旧版)

                            //Prefabにして取り出し（TimelineAssetをスクリプトのみで読み出して再生が難しかったので）
                            GameObject tmpPDObj
                                = UnityEngine.Object.Instantiate(cp.UnityTimelineObj);
                            tmpPDObj.name = cp.UnityTimelineObj.name;

                            PlayableDirector tmpPD
                                = tmpPDObj.GetComponent<PlayableDirector>();

                            tmpPD.Play();
                            UnityEngine.Object.Destroy(m_PlayableDirector.gameObject);
                        }
                        //UnityTimelineAsset指定があれば
                        if (cp.nextTimelineAsset != null)
                        {
                            //まず念の為DCのPublicなTimelineAssetに当てはめ(必要ないかも)
                            DC.KDEventPlayable = cp.nextTimelineAsset;

                            //タイムラインを取り替えて再生（今のObjそのまま使う）
                            //変数にする
                            //それに入れて再生
                            GameObject nowKDEventTLObj
                                = m_PlayableDirector.gameObject;

                            //取得
                            PlayableDirector nowKDEventTL
                                = nowKDEventTLObj.GetComponent<PlayableDirector>();

                            //当てはめ
                            nowKDEventTL.playableAsset = cp.nextTimelineAsset;

                            nowKDEventTL.time = 0;
                            //再生
                            nowKDEventTL.Play();


                            #region //少し前の PD再生まで
                            ////既にシーンに「KDEventTLObj」があれば（Evsで再生されていたら（通常ゲームプレイ時））
                            //if (GameObject.Find("KDEventTLObj"))
                            //{
                            //    //それに入れて再生
                            //    GameObject KDEventTLObj
                            //        = GameObject.Find("KDEventTLObj");
                            //    KDEventTLObj.name = nameof(KDEventTLObj);

                            //    //取得
                            //    PlayableDirector KDEventTL
                            //        = KDEventTLObj.GetComponent<PlayableDirector>();

                            //    //当てはめ
                            //    KDEventTL.playableAsset = cp.nextTimelineAsset;

                            //    KDEventTL.time = 0;
                            //    //再生
                            //    KDEventTL.Play();

                            //}
                            ////シーンに「KDEventTLObj」がない（デバッグで再生中）
                            //else
                            //{
                            //    //PlayableDirectorObjを生成
                            //    GameObject KDEventTLObj
                            //        = UnityEngine.Object.Instantiate(Resources.Load("Main/Timeline/UTLTest/KDEventTLObj") as GameObject);
                            //    KDEventTLObj.name = nameof(KDEventTLObj);

                            //    //取得
                            //    PlayableDirector KDEventTL
                            //        = KDEventTLObj.GetComponent<PlayableDirector>();

                            //    //当てはめ
                            //    KDEventTL.playableAsset = cp.nextTimelineAsset;

                            //    KDEventTL.time = 0;

                            //    //再生
                            //    KDEventTL.Play();
                            //}

                            #endregion

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
                        #region タイムラインランダム再生
                        //RandomTimelineAsset指定があれば
                        if (cp.randomNextTimelineAsset.Length > 0)
                        {
                            //Arrayの中からランダムでセレクト
                            var tmpTimelineAsset = cp.randomNextTimelineAsset
                                [
                                UnityEngine.Random.Range(0, cp.randomNextTimelineAsset.Length - 1)
                                ];

                            //まず念の為DCのPublicなTimelineAssetに当てはめ(必要ないかも)
                            DC.KDEventPlayable = tmpTimelineAsset;

                            //タイムラインを取り替えて再生（今のObjそのまま使う）
                            //変数にする
                            //それに入れて再生
                            GameObject nowKDEventTLObj
                                = m_PlayableDirector.gameObject;

                            //取得
                            PlayableDirector nowKDEventTL
                                = nowKDEventTLObj.GetComponent<PlayableDirector>();

                            //当てはめ
                            nowKDEventTL.playableAsset = tmpTimelineAsset;

                            nowKDEventTL.time = 0;
                            //再生
                            nowKDEventTL.Play();

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



    bool isPreValueRead = false;
    bool isAnimatorLoad = false;
    //再生時（一時停止から復帰でも）
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (Application.isPlaying)//■再生中のみ
        {
            #region イベント製作チーム用

            #region //IEnum実行(タイムライン上でスクリプト実行)

            //for(int i=0;i< m_RMEventTrack.useIEnumList.Count; i++)
            //{
            //    //実行
            //    DC.StartCoroutine(m_RMEventTrack.useIEnumList[i]);
            //}

            #endregion

            #region ちえりアニメーター切り替え
            if (isAnimatorLoad == false)
            {
                //ちえりのアニメーターコントローラーを、それぞれ各個人のものに変更                        
                if (m_RMEventTrack.useAnimator != null) //トラックに設定あれば
                {
                    //（var tmpAnim みたいに変数化できない（参照型じゃない？））
                    if (DC.GirlTrs.GetComponent<Animator>().runtimeAnimatorController != m_RMEventTrack.useAnimator)
                    { DC.GirlTrs.GetComponent<Animator>().runtimeAnimatorController = m_RMEventTrack.useAnimator; }
                }
                Debug.Log("タイムライン ちえりにアニメーター適用");
                isAnimatorLoad = true;
            }
            #endregion

            #endregion

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

                    //Hideで廃止したのでコメントアウト   
                    ////■一括DestroyObjに移設するため処理（今後処理はCpの方で行う）（しばらくしたら（全タイムラインが移設されたら）削除）
                    //if (bh.destroyObjList.Count > 0)
                    //{
                    //    Debug.Log("■リスト版Obj削除にデータが残っています。トラックから「一括Obj削除へ移設」を押して移設しておいてください");
                    //}

                }

                //DCに、このPlayableが今再生中のPlayableであると送信（スクリプトでアクセスできる様にする）
                DC.nowPlayPD = m_PlayableDirector;


                isPreValueRead = true;
                //Debug.Log("テキストプレビューやオブジェクト取得完了");
            }
            #endregion

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
