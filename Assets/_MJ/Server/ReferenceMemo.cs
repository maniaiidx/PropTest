using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceMemo : MonoBehaviour
{

    //■足登りシステム

    //ループ
    #region//■ジャンプ (一時保留)
    ////どちらか片手離れたらカウントスタート
    //if (!RLGrapBool && GrapJumpCountFloat < 0.05f)
    //{
    //    GrapJumpCountFloat += 1 * Time.deltaTime;
    //    //カウント中両手すぐ離したら
    //    if (!RGrapBool && !LGrapBool)
    //    {
    //        SEPlay("ui_notification_04");
    //        GrapJumpBool = true;
    //        GrapJumpCountFloat = 99;
    //    }
    //    //処理はFixedUpdateにある
    //}
    #endregion

    #region //プレイヤー掴み先にペアレント（角度戻すなら必要なさそう）
    ////プレイヤーもペアレント
    //if (RGrapDownBool)
    //{ CameraObjectsTrs.SetParent(Hand_R_RootTrs.parent); }
    //if (LGrapDownBool)
    //{ CameraObjectsTrs.SetParent(Hand_L_RootTrs.parent); }

    ////両手離したらGameobjectsへ
    //if (!RGrapBool && !LGrapBool &&
    //    CameraObjectsTrs.parent != GameObjectsTrs)
    //{ CameraObjectsTrs.SetParent(GameObjectsTrs); }

    //////角度を変えない
    ////if (CameraObjectsTrs.eulerAngles.x != 0 || CameraObjectsTrs.eulerAngles.z != 0)
    ////{ CameraObjectsTrs.eulerAngles = new Vector3(0, CameraObjectsTrs.eulerAngles.y, 0); }
    #endregion

    #region //掴み時の力抜け方（旧）
    //            //■■両手で掴んだら 
    //        if (RLGrapBool)
    //        {
    //            //1フレーム目は現在の距離で両手リミットを固定し
    //            if (RLGrapDownBool)
    //            {
    //                RLimitTweener.Pause();
    //                LLimitTweener.Pause();
    //                if (tmpRLimit.limit != Vector3.Distance(CameraObjectsTrs.position, PlayerIKRHandTargetTrs.position))
    //                { tmpRLimit.limit = Vector3.Distance(CameraObjectsTrs.position, PlayerIKRHandTargetTrs.position); }
    //                if (tmpLLimit.limit != Vector3.Distance(CameraObjectsTrs.position, PlayerIKLHandTargetTrs.position))
    //                { tmpLLimit.limit = Vector3.Distance(CameraObjectsTrs.position, PlayerIKLHandTargetTrs.position); }

    //                //掴み時間カウントリセット
    //                if (RLGrapCountFloat != -2) { RLGrapCountFloat = -2; }
    //                if (RGrapCountFloat != 0 || LGrapCountFloat != 0) { RGrapCountFloat = LGrapCountFloat = 0; }
    //                if (GrapJumpCountFloat != 0) { GrapJumpCountFloat = 0; }
    //            }
    ////リミット最大値までカウントして
    //if (RLGrapCountFloat<nowGrapLimitlength)
    //{ RLGrapCountFloat += 1 * Time.deltaTime; }

    ////リミットがカウントを越え始めたらリミットをその数値にする
    //if (tmpRLimit.limit<RLGrapCountFloat)
    //{ tmpRLimit.limit = RLGrapCountFloat; }
    //if (tmpLLimit.limit<RLGrapCountFloat)
    //{ tmpLLimit.limit = RLGrapCountFloat; }

    ////パワー溜まり （↑の下がる状態になったらパワー減る）
    //if (RLGrapPowMax > RLGrapPowFloat && tmpRLimit.limit > RLGrapCountFloat)
    //{ RLGrapPowFloat += RLGrapPowAdd* Time.deltaTime; }
    //else if (tmpRLimit.limit == RLGrapCountFloat && RLGrapPowMin<RLGrapPowFloat)
    //{ RLGrapPowFloat -= RLGrapPowSub* Time.deltaTime; }

    //        }
    //        else //■■片手で掴んでる場合
    //        {
    //            if (RGrapBool)
    //            {
    //                //リミット最大値までカウントする
    //                if (RGrapCountFloat<nowGrapLimitlength)
    //                { RGrapCountFloat += (40 * nowPlayerLocalScl.x) * Time.deltaTime; }
    //                //リミットより上がったら代入する
    //                if (tmpRLimit.limit<RGrapCountFloat)
    //                { tmpRLimit.limit = RGrapCountFloat; }
    //            }
    //            if (LGrapBool)
    //            {
    //                if (LGrapCountFloat<nowGrapLimitlength)
    //                { LGrapCountFloat += (40 * nowPlayerLocalScl.x) * Time.deltaTime; }
    //                if (tmpLLimit.limit<LGrapCountFloat)
    //                { tmpLLimit.limit = LGrapCountFloat; }
    //            }

    //            //パワー減り
    //            if (RLGrapPowMin<RLGrapPowFloat)
    //            { RLGrapPowFloat -= RLGrapPowSub* Time.deltaTime; }
    //        }
    #endregion
    #region//手(IK移動先のObj) 大きさリアルタイム変化(対象の大きさに対応したが、位置が変わる？のでだめだった)
    //if (Hand_R_RootTrs.localScale != new Vector3(
    //    nowPlayerLocalScl.x / Hand_R_RootTrs.parent.localScale.x,
    //    nowPlayerLocalScl.y / Hand_R_RootTrs.parent.localScale.y,
    //    nowPlayerLocalScl.z / Hand_R_RootTrs.parent.localScale.z
    //    ))
    //{
    //    Hand_R_RootTrs.localScale = new Vector3(
    //      nowPlayerLocalScl.x / Hand_R_RootTrs.parent.localScale.x,
    //      nowPlayerLocalScl.y / Hand_R_RootTrs.parent.localScale.y,
    //      nowPlayerLocalScl.z / Hand_R_RootTrs.parent.localScale.z
    //      );
    //}
    #endregion

    //AsinoboriHandGrap
    #region //コリジョンサーチオブジェ生成
    //rHandAreaCollisionObj = Instantiate(Resources.Load("MiniGame/Asinobori/Prefab/PlayerRHandAreaSphereCollision") as GameObject);
    //rHandAreaCollisionObj.transform.position = PlayerRHandAreaSphereTrs.position;
    //rHandAreaCollisionObj.transform.localScale = PlayerRHandAreaSphereTrs.lossyScale;

    //while (asinoboriRHandAreaCollList.Count == 0) { Debug.Log("ゼロ"); yield return null; }

    ////抽出した座標の中で一番視点先に近い所へ移動
    //float nearDistance = 0;
    //Vector3 nearV3 = new Vector3();
    //for (int m = 0; m < asinoboriRHandAreaCollList.Count; m++)
    //{
    //    float
    //        tempDistance = Vector3.Distance(asinoboriRRayHit.point, asinoboriRHandAreaCollList[m]);

    //    if (nearDistance == 0 || nearDistance > tempDistance)
    //    {
    //        nearDistance = tempDistance;
    //        nearV3 = asinoboriRHandAreaCollList[m];
    //    }
    //}
    //if (nearV3 == Vector3.zero)
    //{
    //    Debug.Log("ゼロ");
    //}
    //else
    //{
    //    Hand_R_RootTrs.position = nearV3;
    //    Debug.Log(nearV3);
    //}

    //asinoboriRHandAreaCollList.Clear();
    #endregion //コリジョンサーチオブジェ生成
    #region //15度ずらし
    //if (AsinoboriRHandDownBool)
    //{
    //    Debug.Log("押し瞬間");
    //    //■手の届く範囲にコリダーが存在する場合
    //    if (AsinoboriRContactGrapColliderList.Count != 0)
    //    {
    //    Debug.Log("//■手の届く範囲にコリダーが存在する場合");
    //        //Ray用変数
    //        RaycastHit tempRayHit;
    //        float tempDisFloat = PlayerRHandAreaSphereCollider.radius * GameObjectsTrs.localScale.z * nowPlayerLocalScl.z * PlayerRHandAreaSphereTrs.localScale.z;
    //        List<Vector3> tempHitV3 = new List<Vector3>();

    //        //■リストの中身を順に
    //        for (int i = 0; i < AsinoboriRContactGrapColliderList.Count; i++)
    //        {
    //            //■そのコリダーRayを手範囲中心から 範囲分前へ飛ばし
    //            if (!AsinoboriRContactGrapColliderList[i].Raycast(PlayerRHandAreaSphereFowerdRay, out tempRayHit, tempDisFloat))
    //            {
    //                //存在しなければRay5度下へ向けて再打ち
    //                for (int k = 5; k <= 95; k += 5)
    //                {
    //    Debug.Log("//存在しなければRay5度下へ向けて再打ち");
    //                    PlayerRHandAreaSphereTrs.localEulerAngles = new Vector3
    //                        (
    //                        PlayerRHandAreaSphereTrs.localEulerAngles.x + k,
    //                        PlayerRHandAreaSphereTrs.localEulerAngles.y,
    //                        PlayerRHandAreaSphereTrs.localEulerAngles.z
    //                        );

    //                    if (AsinoboriRContactGrapColliderList[i].Raycast(new Ray(PlayerRHandAreaSphereTrs.position, PlayerRHandAreaSphereTrs.localEulerAngles), out tempRayHit, tempDisFloat))
    //                    {
    //    Debug.Log("//■存在すればListに座標追加して次のコリダーに移る");
    //                        //■存在すればListに座標追加して次のコリダーに移る
    //                        tempHitV3.Add(tempRayHit.point);
    //                        PlayerRHandAreaSphereTrs.localEulerAngles = Vector3.zero;//元に戻す
    //                        k = 120;//ループ終らし
    //                    }
    //                }
    //            }
    //            else
    //            {
    //    Debug.Log("//■存在すればListに座標追加して次のコリダーに移る");
    //                //■存在すればListに座標追加して次のコリダーに移る
    //                tempHitV3.Add(tempRayHit.point);
    //            }
    //        }

    //        float nearDistance = 0;
    //        Vector3 nearV3 = new Vector3();
    //        //抽出した座標の中で一番視点先に近い所へ移動
    //        for (int m = 0; m < tempHitV3.Count; m++)
    //        {
    //            Debug.Log("//抽出した座標の中で一番視点先に近い場所更に抽出");
    //            float
    //                tempDistance = Vector3.Distance(asinoboriRRayHit.point, tempHitV3[m]);

    //            if (nearDistance == 0 || nearDistance > tempDistance)
    //            {
    //                nearDistance = tempDistance;
    //                nearV3 = tempHitV3[m];
    //            }
    //        }
    //        if (nearV3 == Vector3.zero)
    //        {
    //            PlayerRHandAreaSphereTrs.localEulerAngles = Vector3.zero;//元に戻す
    //            Debug.Log("ゼロ");
    //        }
    //        else
    //        {
    //            Hand_R_RootTrs.position = nearV3;
    //        }

    //        Debug.Log(nearV3);


    //    }
    #endregion 15度ずらし
    #region//跳ね返しRayによる、コリダー外側point求め方
    //Collider tempPlayerCollider = CameraObjectsTrs.GetComponent<Collider>();

    //// ターゲットオブジェクトとの差分を求め(プレイヤーからサークルの中心)
    //Vector3 tempV3 = CameraTrs.position - Hand_R_Trs.position;
    //// 正規化して方向ベクトルを求める(Rayはターゲットへの場所ではなく"方向"を指定するため)
    //Vector3 normal = tempV3.normalized;
    //// Rayの作成
    //Ray tempRhandToPlayerRay = new Ray(Hand_R_Trs.position, normal);
    //RaycastHit tempRhandToPlayerRayHit;

    //if (tempPlayerCollider.Raycast(tempRhandToPlayerRay, out tempRhandToPlayerRayHit, Mathf.Infinity))
    //{
    //    SEPlay("UI_po");
    //    PlayerIKRHandTargetTrs.position = tempRhandToPlayerRayHit.point;
    //}
    #endregion
    #region //■手を離した位置をプレオブジェに入れておく
    ////■手を離した位置をプレオブジェに入れておく
    ////設置ポイントに移動し
    //PreMoveCubeRTrs.transform.position = asinoboriRRayHit.point;
    ////設置面からみて正面の角度にし
    //PreMoveCubeRTrs.transform.rotation = Quaternion.LookRotation(asinoboriRRayHit.normal);
    ////前へ少し進ませる（設置面から少し浮いた位置になる）
    //PreMoveCubeRTrs.transform.Translate(new Vector3(0, 0, nowPlayerLocalScl.z));
    #endregion
    #region//掴み対象が1,1,1のスケールでなかった場合のために、lossyScale取得して当てはめ(だめだった)
    //Vector3 tmplossyV3 = HandRootTrs.lossyScale;
    //Vector3 tmplocalV3 = HandRootTrs.localScale;
    //HandRootTrs.SetParent(asinoboriRayHit.transform,true);
    //HandRootTrs.localScale = new Vector3(
    //    tmplocalV3.x / tmplossyV3.x * nowPlayerLocalScl.x,
    //    tmplocalV3.y / tmplossyV3.y * nowPlayerLocalScl.y,
    //    tmplocalV3.z / tmplossyV3.z * nowPlayerLocalScl.z
    //    );
    #endregion


    //AsinoboriHandLeave
    #region////踏ん張りをコルーチンにしていた時の
    //if (RLGrapBool)
    //{
    //    //右手を離した場合
    //    if (!RGrapBool)
    //    {
    //        StartCoroutine(AsinoboriHandLeaveHoldOut
    //            (
    //            playerLHandJoint,
    //            PlayerIKLHandTargetTrs
    //            ));
    //    }
    //    //左手を離した場合
    //    else if (!LGrapBool)
    //    {
    //        StartCoroutine(AsinoboriHandLeaveHoldOut
    //            (
    //            playerRHandJoint,
    //            PlayerIKRHandTargetTrs
    //            ));
    //    }
    //}
    #endregion

}
