using System;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TwistRelaxer_CustomV4 : MonoBehaviour
{
    public Transform
        nemotoTrs;

    public Target[] targets;

    [Serializable]
    public struct Target
    {
        public Transform transform;
        [Range(-2, 1)] public float rate;

        public float
            clamp;

        [HideInInspector]
        public Vector3
            twistAxisDefV3,
            axisDefV3,
            axisWorldDefV3,
            axisRelativeToNemotoDefV3;
    }

    public Transform
    sakippoTrs;

    Quaternion
        nemotoDefRot;

    void Start()
    {
        #region ■スタート時の状況で、ツイスト軸Xやローカル正面軸Zを算出し、ワールドのZ軸も算出。
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)//再生中のみ
        {
#endif
            nemotoDefRot
                = nemotoTrs.localRotation;

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].transform != null)
                {
                    //ツイストの軸算出（targetから見て、debugTrsの位置へのベクトルを取っている＝ツイスト軸）
                    targets[i].twistAxisDefV3
                    = sakippoTrs.position - targets[i].transform.position;

                    //yzx　と、ずらして代入している(これでツイストの軸を倒してると思われる)(ーを｜こう)
                    //■ツイスト軸に対してのローカル正面Z軸を出してる
                    targets[i].axisDefV3
                        = new Vector3(targets[i].twistAxisDefV3.y, targets[i].twistAxisDefV3.z, targets[i].twistAxisDefV3.x);

                    // Axis in world space　（算出したローカル正面Z軸を、現在の回転と掛けると相殺されて ワールドの正面Z軸が取れる？）
                    targets[i].axisWorldDefV3
                        = targets[i].transform.rotation * targets[i].axisDefV3;

                    //（翻訳:Nemotoの回転に対してのワールド空間軸を格納する）
                    //nemotoのスタート時反回転量分、ワールドz軸を足して相殺し、nemotoのワールド真背面を算出
                    targets[i].axisRelativeToNemotoDefV3
                        = Quaternion.Inverse(nemotoTrs.rotation) * targets[i].axisWorldDefV3;

                }
            }
#if UNITY_EDITOR
        }
#endif
#endregion
    }

        void Update()
    {
        #region ■nemotoのツイスト軸回転量抽出して、ツイスト軸のみ加算減算できる状態のTargetに与える（打ち消す）仕様
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)//再生中のみ
        {
#endif
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].transform != null)//transformが指定されていれば
                {
                    float //貰ったメソッドを使って、nemotoのツイスト量だけ算出（Clamp付き）
                    nemotoTwistAngle
                    = Mathf.Clamp(
                        GetTwistAroundAxis(nemotoDefRot, nemotoTrs.localRotation, targets[i].twistAxisDefV3)//メソッド
                        , -targets[i].clamp
                        , targets[i].clamp);
                    
                    #region Targetのツイスト軸のみ加算減算できる状態にする
                    Vector3 //■現在nemoto分回転したワールドZの方向 //スタート時のnemotoワールド反Z軸を現nemotoRot分回転したもの //nemotoにとってリアルタイムbackな感じ
                        tmpRelaxedAxisNemotoV3
                        = nemotoTrs.rotation * targets[i].axisRelativeToNemotoDefV3;

                    //（翻訳:relaxedAxisを（axis、twistAxis）空間に変換して、ツイスト角（回転量 angle）を計算できます）
                    Quaternion //まず現在TargetのXZ回転を出す 
                        tmpRot = Quaternion.LookRotation
                        (
                            targets[i].transform.rotation * targets[i].axisDefV3,
                            targets[i].transform.rotation * targets[i].twistAxisDefV3
                        );


                    //さっき出した"現在nemoto分回転したワールドZ"の方向から、現在TargetのXZ回転を打ち消す。 
                    //　= 傾きが残る（現在TargetのZ軸XAngle量と、nemotoZの元の向きを比較）　//（axis、twistAxis）空間を反転した分だけ、現nemotoRot分回転した反軸を向かせ　　(↓をaxisRelativeToNemotoDefV3にしたらisNemotoOnly状態になったので、多分relaxedAxisNemotoV3が増加分と化してる)
                    tmpRelaxedAxisNemotoV3 = Quaternion.Inverse(tmpRot) * tmpRelaxedAxisNemotoV3;

                    // （翻訳書き換え:ツイスト軸を中心にこのTransform（target）を回転させるために必要な角度を計算します。）
                    //"現在のTargetツイスト回転状態"から、"現在のnemotoツイスト回転状態"までに足りない量が算出される。//残った傾きの角度差を出している
                    float
                        tmpRemainAngle
                        = Mathf.Atan2
                        (tmpRelaxedAxisNemotoV3.x, tmpRelaxedAxisNemotoV3.z) * Mathf.Rad2Deg;

                    //これで、ツイスト角度のみを加算減算できる状態で、親と同じ回転をしている状態が作れる。
                    #endregion

                    //■ツイスト処理
                    targets[i].transform.rotation
                        = Quaternion.AngleAxis(
                            tmpRemainAngle//これを入れないと加算され続けるので必要
                            + (nemotoTwistAngle//序盤で出したnemotoのツイスト量
                            * targets[i].rate)//ここで、ツイスト量を加算したり減算したり
                            //+ rootTwistAngle//root分
                            ,
                            targets[i].transform.rotation * targets[i].twistAxisDefV3)//現Targetツイスト軸（スタート時算出のツイスト軸を現Target回転分回転させたもの）
                            * targets[i].transform.rotation;//現targetの回転合算

                }
            }
#if UNITY_EDITOR
        }
#endif
#endregion

        #region エディタ用初期化などの処理
#if UNITY_EDITOR
        if (targets != null)//targets自体があるかどうか
        {
            //targets数変更あったら
            if (targets.Length != preTargetsLength)
            {
                //targets数前より多かったら
                if (targets.Length > preTargetsLength)
                {
                    //targets数増えた分だけ初期化
                    for (int i = preTargetsLength; i < targets.Length; i++)
                    {
                        targets[i].clamp = 360;
                    }
                }
                //targets数更新
                preTargetsLength = targets.Length;
            }
        }
#endif
        #endregion
    }

    //指定軸（今回はツイスト軸）の回転角度をfloatで出すメソッド（教えて貰った）
    public static float GetTwistAroundAxis(Quaternion ra, Quaternion rb, Vector3 axis)
    {
        // 軸を正規化する
        if (axis == Vector3.zero)
        {
            axis = Vector3.forward;
        }
        axis.Normalize();

        // da、db、rab、rdadbを求める
        var da = ra * axis;
        var db = rb * axis;
        var rab = rb * Quaternion.Inverse(ra);
        var rdadb = Quaternion.FromToRotation(da, db);

        // rdadbからrabへの回転を求めたのち、その軸と角度を抽出する
        Vector3 deltaAxis;
        float deltaAngle;
        var delta = rab * Quaternion.Inverse(rdadb);
        delta.ToAngleAxis(out deltaAngle, out deltaAxis);

        // dbとdeltaAxisは同一直線上にあるはずだが、向きは逆かもしれない
        // 角度の正負を統一するため、向きの逆転の有無を調べる
        // deltaAngleSignはdbとdeltaAxisの向きが一致していれば1、逆転していれば-1になる
        var deltaAngleSign = Mathf.Sign(Vector3.Dot(db, deltaAxis));


        // 角度の符号を補正した上で0°～360°におさめて返す
        var result = (deltaAngleSign * deltaAngle) % 360.0f;
        //if (result < 0.0f)//■この処理があるとrateを変えた場合反転が生まれるので一旦コメントアウト
        //{
        //    result += 360.0f;
        //}
        return result;
    }

    #region エディタ用 初期化などの処理用
    int preTargetsLength = 0;

#if UNITY_EDITOR
    private void Awake()
    { nemotoTrs = this.transform; }
#endif

    #endregion
}
