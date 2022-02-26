using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlendshapeAngleLinkage : MonoBehaviour
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
    public angles angle;

    public float //影響する範囲の角度（手動）
        startAngle = 90,
        endAngle = 180;
    public AnimationCurve //加えるカーブ（手動）
        curve = AnimationCurve.Linear(0, 0, 1, 1);//初期値はLinear
    float //変更時のみ更新する用
        prevAngle;

    public float //参考に現在のモーフウェイト量
        __nowWeight;
    #endregion

    void Update()
    {
        //nullじゃなければ
        if (skinnedMeshRenderer != null)
        {
            #region 指定がXの場合
            if (angle == angles.X)
            {
                //前フレームと値が変わったら
                if (prevAngle != transform.localEulerAngles.x)
                {
                    //スタートからエンドの間で現在の角度を0～1で割り出し、カーブを加え、100倍にしてモーフにセット（モーフは0～100なので）
                    skinnedMeshRenderer.SetBlendShapeWeight
                        (blendShapeIndex
                        , curve.Evaluate(
                            Mathf.InverseLerp(
                                startAngle
                                , endAngle
                                , transform.localEulerAngles.x
                                ))
                                * 100
                        );

                    //prevAngle更新
                    prevAngle = transform.localEulerAngles.x;

                    //参考用nowWeight更新
                    __nowWeight = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex);
                }
            }
            #endregion
            #region 指定がYの場合
            else if (angle == angles.Y)
            {
                //前フレームと値が変わったら
                if (prevAngle != transform.localEulerAngles.y)
                {
                    //スタートからエンドの間で現在の角度を0～1で割り出し、カーブを加え、100倍にしてモーフにセット（モーフは0～100なので）
                    skinnedMeshRenderer.SetBlendShapeWeight
                        (blendShapeIndex
                        , curve.Evaluate(
                            Mathf.InverseLerp(
                                startAngle
                                , endAngle
                                , transform.localEulerAngles.y
                                ))
                                * 100
                        );

                    //prevAngle更新
                    prevAngle = transform.localEulerAngles.y;

                    //参考用nowWeight更新
                    __nowWeight = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex);
                }
            }
            #endregion
            #region 指定がYの場合
            else if (angle == angles.Z)
            {
                //前フレームと値が変わったら
                if (prevAngle != transform.localEulerAngles.z)
                {
                    //スタートからエンドの間で現在の角度を0～1で割り出し、カーブを加え、100倍にしてモーフにセット（モーフは0～100なので）
                    skinnedMeshRenderer.SetBlendShapeWeight
                        (blendShapeIndex
                        , curve.Evaluate(
                            Mathf.InverseLerp(
                                startAngle
                                , endAngle
                                , transform.localEulerAngles.z
                                ))
                                * 100
                        );

                    //prevAngle更新
                    prevAngle = transform.localEulerAngles.z;

                    //参考用nowWeight更新
                    __nowWeight = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex);
                }
            }
            #endregion
        }

        #region Unityエディタ上ではskinnedMeshRenderer変更あったらNameListなどデータ更新
#if UNITY_EDITOR
        if (prevSkinnedMeshRenderer != skinnedMeshRenderer)
        {
            prevSkinnedMeshRenderer = skinnedMeshRenderer;
            ReloadBlendShapeNameList();
        }
#endif
        #endregion
    }

    void ReloadBlendShapeNameList()//名前一覧更新
    {
        //名前一覧取得
        __blendShapeNameList.Clear();//クリアして
        //nullじゃなければAdd
        if (skinnedMeshRenderer != null)
        {
            for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
            { __blendShapeNameList.Add(skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i)); }
        }
    }
}
