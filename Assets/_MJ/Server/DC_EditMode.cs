using System;//FuncやActionを使うために必要
using System.Linq;// evsからキー名指定でインデックス番号取り出すfor文に必要（Skipなど）
using System.Collections;
using System.Collections.Generic; //Listに必要
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using RootMotion.FinalIK;
using DG.Tweening;//DOTween
using System.Text.RegularExpressions;

public partial class DataCounter
{
    #region ■エディットモード

    #region 変数
    bool
        isEditModeSystem;

    public OrderedDictionary<string, Action>
        EM_eventDict = new OrderedDictionary<string, Action>();
    int
        EM_evNum = 0;

    float
        EM_waitFloat;
    RaycastHit
        EM_SeeRayHit;
    #endregion

    IEnumerator EditModeSystem()
    {
        #region 初期化
        if (isEditModeSystem) { yield break; }
        isEditModeSystem = true; Debug.Log("エディットモード開始");

        #region エディットモードポインタ
        GameObject //エディットモードポインタ
            EM_SeePointObj = Instantiate(SeePointTrs.gameObject);

        Transform
            EM_SeePointTrs = EM_SeePointObj.transform;
        Renderer
            EM_SeePointRenderer = EM_SeePointObj.GetComponent<Renderer>();
        Material
            EM_SeePointBlueMat = Resources.Load("EventSystem/Tansaku/Mat/TansakuSeePointBlueMat") as Material,
            EM_SeePointRedMat = Resources.Load("EventSystem/Tansaku/Mat/TansakuSeePointRedMat") as Material;

        #endregion

        bool
            isChieriCatch = false;

        #endregion

        //■■現在は智恵理をキャッチ・離す命令のみ。
        //原点じゃないモーションを、意図したところへ設置する方法が必要そう。
        //（ピボットを使って逆算する？）


        #region ループ
        while (isEditModeSystem)
        {
            #region エディットモードテスト
            if (Physics.Raycast(playerSeeRay, out EM_SeeRayHit, Mathf.Infinity, seeRayBlockLayerMask))
            {
                //キャッチしてなければタグ判定して
                if (isChieriCatch == false
                    && EM_SeeRayHit.collider.tag == "ChieriCollider")
                {
                    //決定でキャッチ
                    if (isKetteiDown == true)
                    {
                        isChieriCatch = true;
                        //ポインタ動作しないように智恵理コリダーオフ
                        foreach (GameObject collObj in ChieriColliderObjs)
                        { collObj.SetActive(false); }
                    }
                }
                //キャッチしてれば決定で離す
                else if (isChieriCatch == true
                    && isKetteiDown == true)
                {
                    isChieriCatch = false;
                    //コリダーオン
                    foreach (GameObject collObj in ChieriColliderObjs)
                    { collObj.SetActive(true); }
                }

                #region ■ポインタ処理
                //存在させる
                if (EM_SeePointTrs.gameObject.activeSelf != true)
                {
                    EM_SeePointTrs.gameObject.SetActive(true);
                }

                //ポインタの場所と大きさ
                EM_SeePointTrs.position = EM_SeeRayHit.point;
                EM_SeePointTrs.LookAt(PlayerTargetTrs, Vector3.forward);
                nearSizeAjust(EM_SeePointTrs, 80);

                //色変え
                if (EM_SeeRayHit.collider.tag == "ChieriCollider")
                {
                    if (EM_SeePointRenderer.material != EM_SeePointRedMat)
                    {
                        EM_SeePointRenderer.material = EM_SeePointRedMat;
                    }
                }
                else
                {
                    if (EM_SeePointRenderer.material != EM_SeePointBlueMat)
                    {
                        EM_SeePointRenderer.material = EM_SeePointBlueMat;
                    }
                }
                #endregion

            }
            //Rayが何にもヒットしてない場合
            else { }



            //キャッチ時智恵理移動
            if (isChieriCatch)
            {
                //設置
                Vector3 tmpPos;
                //RootボーンのLocalPosを引く
                tmpPos.x = EM_SeePointTrs.position.x - GameObjectsTrs.TransformPoint(Bip001Trs.localPosition).x;
                tmpPos.y = GirlTrs.position.y;
                tmpPos.z = EM_SeePointTrs.position.z - GameObjectsTrs.TransformPoint(Bip001Trs.localPosition).z;

                GirlTrs.position = tmpPos;
            }

            if (Input.GetKeyDown(KeyCode.V))
            {



            }
            #endregion

            yield return null;
        }
        #endregion

        #region 終了処理

        #endregion

        yield break;
    }


    IEnumerator EditDataRun()
    {
        for (int i = 0; i < EM_eventDict.Count; i++)
        {
            //走らせて
            EM_eventDict[i].Value();

            //■キーからイベントナンバーだけ削除
            string tmpEvString =
            Regex.Replace(EM_eventDict[i].Key, "[0-9]", "");//[^0-9]だと数字"以外"削除

            //イベントがウェイトだったらウェイトする
            if (tmpEvString == "ウェイト")
            {
                yield return new WaitForSeconds(EM_waitFloat);
                EM_waitFloat = 0;
            }

        }
        yield break;
    }
    void EM_WaitAdd(float waitTime)
    {
        EM_eventDict.Add(EM_evNum + "ウェイト", () =>
        {
            EM_waitFloat = waitTime;
        });
        EM_evNum++;
    }
    void EM_HukidashiAdd()
    {
        EM_eventDict.Add(EM_evNum + "フキダシ", () =>
        {
            Hukidashi("test01");
        });
        EM_evNum++;
    }

    #endregion

    #region 背比べモード
    IEnumerator SekurabeMode()
    {
        if (debugEventMoveFlag == true)
        {
            FadeBlack(1, 0, true);

            ChieriMotion("背比べ直前", 0f, 0);//デバッグ用アニメ直接再生

            //プレイヤー見
            DOTweenToLAIKSEyes(LAIKEyeS, LAIKSEyesDefWeight, 0);
            DOTweenToLAIKSHead(LAIKHeadS, LAIKSHeadDefWeight, 0);
            FollowDOMove(IKLookAtHeadTargetTrs, PlayerHeadTargetTrs);
            FollowDOMove(IKLookAtEyeTargetTrs, PlayerEyeTargetTrs);

            nowPlayerLocalScale = Vector3.one;

            ////宿題システムスタート
            //StartCoroutine(HomeworkSystem());
        }
        //■背比べ時に手でAreaOutになるので、暫定で一時的にコリダーオフ（あとで手の位置自体調整か元に戻す）
        #region ■智恵理紐摘む手のコリダーオフ（ちゃんとあとで元に戻す）
        for (int i = 0; i < ChieriColliderObjs.Length; i++)
        {
            if (
                ChieriColliderObjs[i].name == "UpperArm_R_coll" ||
                ChieriColliderObjs[i].name == "Forearm_R_coll" ||
                ChieriColliderObjs[i].name == "Hand_R_coll" ||
                ChieriColliderObjs[i].name == "Hitosashi00_R_coll" ||
                ChieriColliderObjs[i].name == "Hitosashi01_R_coll" ||
                ChieriColliderObjs[i].name == "Hitosashi02_R_coll" ||
                ChieriColliderObjs[i].name == "Ko00_R_coll" ||
                ChieriColliderObjs[i].name == "Ko01_R_coll" ||
                ChieriColliderObjs[i].name == "Ko02_R_coll" ||
                ChieriColliderObjs[i].name == "Kusuri00_R_coll" ||
                ChieriColliderObjs[i].name == "Kusuri01_R_coll" ||
                ChieriColliderObjs[i].name == "Kusuri02_R_coll" ||
                ChieriColliderObjs[i].name == "Naka00_R_coll" ||
                ChieriColliderObjs[i].name == "Naka01_R_coll" ||
                ChieriColliderObjs[i].name == "Naka02_R_coll" ||
                ChieriColliderObjs[i].name == "Oya00_R_coll" ||
                ChieriColliderObjs[i].name == "Oya01_R_coll" ||
                ChieriColliderObjs[i].name == "Oya02_R_coll"
                )
            {
                ChieriColliderObjs[i].SetActive(false);
            }
        }
        #endregion

        #region 暗幕中設定
        FadeBlack(1, 0, true);

        //フキダシノベル近くに

        #region プレイヤー背比べ位置に移動と回転設定

        //暗幕中に位置移動
        GameObject Player_00SekurabeStandPosObj
            = Resources.Load("_PosObj/02Sekurabe/Player_00SekurabeStandPosObj") as GameObject;

        if (isSekurabeRealStand == false)
        {
            CameraObjectsTrs.transform.localPosition = Player_00SekurabeStandPosObj.transform.localPosition;
            //カメラリセット値変更してリセット
            CameraReset(Player_00SekurabeStandPosObj.transform.localEulerAngles
                , DB.cameraStandAnchorDefLocalPos);//Anchorを立ちに
        }
        else //VR立ちしてたら、手動で回転のリセット値更新して、同じ分アンカー回転 
        {
            //cameraobjectsのリセット回転値と回転を90（指定）にし
            DB.cameraObjectsResetLocalEul =
            CameraObjectsTrs.transform.localEulerAngles = Player_00SekurabeStandPosObj.transform.localEulerAngles;


            //アンカー回転を、現在のアンカー回転から90（指定）を引いた数に
            CameraAnchorTrs.localEulerAngles =
                -(Player_00SekurabeStandPosObj.transform.localEulerAngles
                - CameraAnchorTrs.localEulerAngles);
            nowPlayerStand = true;
        }
        #endregion

        //スマホ片手いじりPosへ
        GameObject ChieriSumaho_LHand_SekurabeKatatePosObj = Resources.Load("_PosObj/_ParentPoseObjs/ChieriSumaho_LHand_SekurabeKatatePosObj") as GameObject;
        ChieriSumahoObj.transform.localPosition = ChieriSumaho_LHand_SekurabeKatatePosObj.transform.localPosition;
        ChieriSumahoObj.transform.localEulerAngles = ChieriSumaho_LHand_SekurabeKatatePosObj.transform.localEulerAngles;
        ChieriMotion("背比べ直前", 0f, 0);

        //LookAtターゲットプレイヤーの顔へ
        ChieriMotion("まばたき", 0f, 4); blinkTime = 0;
        DOTweenToLAIKSEyes(LAIKEyeS, LAIKSEyesDefWeight, 0f);
        FollowDOMove(IKLookAtEyeTargetTrs, PlayerEyeTargetTrs, 0f);
        DOTweenToLAIKSHead(LAIKHeadS, 1, 1);
        FollowDOMove(IKLookAtHeadTargetTrs, PlayerHeadTargetTrs);

        #endregion

        #region 暗幕中 背比べモード変数設定

        bool isShrinkComplete;

        Tweener shrinkTweener =
            DOTween.To(() => nowPlayerLocalScale, (x) => nowPlayerLocalScale = x
            , nowPlayerLocalScale / 1.1f, 3f)
            .OnPlay(() => { isShrinkComplete = false; })
            .OnComplete(() => { isShrinkComplete = true; })
            .Pause()
            .SetAutoKill(false);



        #endregion
        yield return new WaitForSeconds(1f);

        //開幕
        FadeBlack(0, 1, true);

        Debug.Log("準備完了");

        while (true)
        {
            if (isKetteiDown)
            {
                //縮小
                PPv2MemaiLittle(0.8f);
                shrinkTweener.ChangeValues(nowPlayerLocalScale, nowPlayerLocalScale / 1.1f, 3f)
                    .Restart();
            }

            if (Input.GetButtonDown(DB.inputDict["セレクト"]))
            {
                //リセット
                FadeBlack(1, 0.5f);
                yield return new WaitForSeconds(0.5f);

                shrinkTweener.Pause();
                nowPlayerLocalScale = Vector3.one;

                yield return null;
                FadeBlack(0, 0.5f);
            }


            yield return null;
        }

    }

    #endregion

    //(ビルドして確かめたが、Confinedはウインドウサイズ変更でおかしくなるし、Lockedはそもそも中心から動かなくなるので今はやめとくべきか)
    #region //マウスカーソル

    //#region 変数
    //bool
    //    isMouseSystem;
    //GameObject
    //    MouseCursor;
    //Vector3
    //    mousePos,
    //    mousePrePos,
    //    screenToWorldPointPosition;
    //#endregion

    //IEnumerator MouseSystem()
    //{
    //    #region 初期化
    //    if (isMouseSystem) { yield break; }
    //    isMouseSystem = true;

    //    MouseCursor = GameObject.Find("MouseCursor");

    //    // カーソルをウィンドウから出さない
    //    Cursor.lockState = CursorLockMode.Confined;
    //    //(ビルドして確かめたが、Confinedはウインドウサイズ変更でおかしくなるし、Lockedはそもそも中心から動かなくなるので今はやめとくべきか)

    //    float mouseVisTimeCount = 0;
    //    #endregion

    //    #region ループ
    //    while (isMouseSystem)
    //    {
    //        MouseCursor.transform.position = Input.mousePosition;

    //        // Vector3でマウス位置座標を取得する
    //        mousePos = Input.mousePosition;
    //        // Z軸修正
    //        mousePos.z = 50f;
    //        // マウス位置座標をスクリーン座標からワールド座標に変換する
    //        screenToWorldPointPosition = VRCamera.ScreenToWorldPoint(mousePos);
    //        // ワールド座標に変換されたマウス座標を代入
    //        MouseCursor.transform.position = screenToWorldPointPosition;


    //        #region 2秒マウス止まったら表示消す
    //        if (mousePos != mousePrePos)
    //        { mouseVisTimeCount = 0; }

    //        mouseVisTimeCount += 1 * Time.deltaTime;

    //        if (mouseVisTimeCount > 2)
    //        { Cursor.visible = false; }
    //        else { Cursor.visible = true; }

    //        mousePrePos = mousePos;
    //        #endregion

    //        yield return null;
    //    }
    //    #endregion

    //    #region 終了処理

    //    #endregion

    //    yield break;
    //}

    #endregion
}
