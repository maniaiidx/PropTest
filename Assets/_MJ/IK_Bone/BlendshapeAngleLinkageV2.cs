using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlendshapeAngleLinkageV2 : MonoBehaviour
{
    #region 変数
    public SkinnedMeshRenderer //blendShape入ってるObjを手動で指定
        skinnedMeshRenderer;
    SkinnedMeshRenderer //変更あった時更新する判定用
        prevSkinnedMeshRenderer;
    public int //動かすモーフの番号（手動で指定）
        blendShapeIndex = 0;
    public List<string> //参考にモーフの名前リスト
        __blendShapeNameList = new List<string>();

    //影響元のXYZ指定用（手動）
    public enum angles { X, Y, Z }
    public angles targetAngle;

    [Range(-1, 1)]
    public int//影響する範囲の何回転目か（手動）
        startRound = 0;
    public float //影響する範囲の角度（手動）
        startAngle = 90;

    [Range(-1, 1)]
    public int
        endRound = 0;
    public float
        endAngle = 180;

    public AnimationCurve //加えるカーブ（手動）
        curve = AnimationCurve.Linear(0, 0, 1, 1);//初期値はLinear

    public float //参考に現在のモーフウェイト量
        __nowWeight;

    float //360度を跨いだ回転を割り出す用 + 変更検知用
        nowAngle,
        prevAngle,
        diffValueAngleZ;

    public int
        nowRoundCount = 0;
    int //変更検知用
        prevRoundCount = 0;

    #endregion

    void Update()
    {
        #region 名前一覧取得 ※Unityエディタ上でのみ。（skinnedMeshRenderer変更あったらNameListなどデータ更新）
#if UNITY_EDITOR
        if (prevSkinnedMeshRenderer != skinnedMeshRenderer)
        {
            prevSkinnedMeshRenderer = skinnedMeshRenderer;

            //名前一覧取得
            __blendShapeNameList.Clear();//クリアして
            //nullじゃなければAdd
            if (skinnedMeshRenderer != null)
            {
                for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
                { __blendShapeNameList.Add(skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i)); }
            }
        }
#endif
        #endregion

        //nullじゃなければ
        if (skinnedMeshRenderer != null)
        {
            #region enumで指定のAngleを取得
            float
                targetAngle = 0;

            if (this.targetAngle == angles.X) { targetAngle = transform.localEulerAngles.x; }
            else if (this.targetAngle == angles.Y) { targetAngle = transform.localEulerAngles.y; }
            else if (this.targetAngle == angles.Z) { targetAngle = transform.localEulerAngles.z; }
            #endregion

            //前フレームから変更があれば
            if (
                nowAngle != targetAngle //角度が変わっていれば
                || prevRoundCount != nowRoundCount //回転数が変わっていれば
                )
            {
                #region targetAngleが360度を跨いだ場合、1回転として計算
                //前フレームと現フレームの値取得し、その差異を出す。
                prevAngle = nowAngle;
                nowAngle = targetAngle;
                diffValueAngleZ = (nowAngle - prevAngle);

                //1フレームでの差異が181以上（-181以下）の場合、”360を跨いだ"と推定して、回転数に代入　（180だと、他の回転の影響で切り替わる数値もカウントしてしまうので181）
                if (diffValueAngleZ > 181) { nowRoundCount--; }
                if (diffValueAngleZ < -181) { nowRoundCount++; }


                //変更検知用
                prevRoundCount = nowRoundCount;
                #endregion

                #region 指定Indexのモーフに、カーブを加え、スタートからエンドの間で現在の角度を0～1で割り出し、100倍（モーフは0～100なので）にして、セット
                skinnedMeshRenderer.SetBlendShapeWeight
                    (blendShapeIndex
                    , curve.Evaluate(//カーブ
                        Mathf.InverseLerp(//割り出し
                            startAngle + (startRound * 360)
                            , endAngle + (endRound * 360)
                            , targetAngle + (nowRoundCount * 360)
                            ))
                            * 100
                    );
                #endregion

                //参考用nowWeight更新
                __nowWeight = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex);
            }
        }

    }

    void Start()
    {
        //たまにいつの間にかシーン上で-1とかになってることがあるので
        nowRoundCount = 0;
    }
}
