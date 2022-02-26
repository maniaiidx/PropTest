using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TwistRelaxer_CustomV2 : MonoBehaviour
{
    public bool
        isNemotoOnly = true;

    public Transform Nemoto;

    #region ■カスタム要素 targetを構造体化
    [System.Serializable]//インスペクター上で登録 構造体
    public struct Twists
    {
        [HideInInspector]
        public Vector3
            twistAxis,
            axis,
            axisRelativeToOyaDefault,
            axisRelativeTokoDefault;

        public Transform
            transform;

        [Tooltip("If 0.5, this Transform will be twisted half way from parent to child. If 1, the twist angle will be locked to the child and will rotate with along with it.")]
        [Range(0f, 1f)] public float rate;

        [Tooltip("Rotation offset around the twist axis.")]
        [Range(-180f, 180f)] public float twistAngleOffset;

    }

    public Twists[]
        twists;
    #endregion

    public Transform sakippo;

    //■カスタム要素 スタート時のkoRot保持用
    Quaternion startSakippoRot;


    [Tooltip("The weight of relaxing the twist of this Transform")]
    [Range(0f, 1f)] public float transTime = 1f;





    void Start()
    {
        for (int i = 0; i < twists.Length; i++)
        {
            //■元々はクラスで宣言してた
            twists[i].twistAxis = Vector3.right;
            twists[i].axis = Vector3.forward;


            //ひねりの軸算出？ targetから見て、koの位置からtargetの位置を引いた位置　へのベクトルを取っている
            twists[i].twistAxis = twists[i].transform.InverseTransformDirection(sakippo.position - twists[i].transform.position);

            //なぜか直接twistAxisを代入すると、レートの効果が出ない
            //yzx　と、なぜかずらして代入している
            twists[i].axis = new Vector3(twists[i].twistAxis.y, twists[i].twistAxis.z, twists[i].twistAxis.x);

            // Axis in world space　（これでなぜかワールドの軸が取れる？）
            Vector3 axisWorld = twists[i].transform.rotation * twists[i].axis;

            // Store the axis in worldspace relative to the rotations of the parent and child
            //（翻訳:軸を親と子の回転に関連してワールド空間に格納する）
            twists[i].axisRelativeToOyaDefault = Quaternion.Inverse(Nemoto.rotation) * axisWorld;
            twists[i].axisRelativeTokoDefault = Quaternion.Inverse(sakippo.rotation) * axisWorld;
        }


        startSakippoRot = sakippo.rotation;

    }



    /// <summary>
    /// Rotate this Transform to relax it's twist angle relative to the "parent" and "child" Transforms.
    /// （翻訳:この変換を回転させると、「親」と「子」の変換に対する捻れ角度が緩和されます。）
    /// </summary>

    public void Relax()
    {
        if (transTime <= 0f) return; // 0なら何もしない


        for (int i = 0; i < twists.Length; i++)
        {
            Quaternion
            tmpTargetRot
            = twists[i].transform.rotation;

            Quaternion
                twistOffset
                = Quaternion.AngleAxis(twists[i].twistAngleOffset, tmpTargetRot * twists[i].twistAxis);

            tmpTargetRot = twistOffset * tmpTargetRot;


            // Find the world space relaxed axes of the parent and child
            //（翻訳:親と子の世界空間緩和軸を求める）
            Vector3
                relaxedAxisNemoto
                = twistOffset * Nemoto.rotation * twists[i].axisRelativeToOyaDefault;
            Vector3
                relaxedAxisSakippo
                = twistOffset * sakippo.rotation * twists[i].axisRelativeTokoDefault;

            Vector3 //■カスタム　スタート時のKoの回転保持しておいて、それを当てはめる
                relaxedAxisStartSakippo
                = twistOffset * startSakippoRot * twists[i].axisRelativeTokoDefault;


            // Cross-fade between the parent and child
            // （翻訳:親と子の間でクロスフェードする）
            Vector3
                relaxedAxis;
            if (isNemotoOnly)
            {
                relaxedAxis
                    = Vector3.Slerp(relaxedAxisNemoto, relaxedAxisStartSakippo, twists[i].rate);
            }
            else
            {
                relaxedAxis
                    = Vector3.Slerp(relaxedAxisNemoto, relaxedAxisSakippo, twists[i].rate);
            }

            // Convert relaxedAxis to (axis, twistAxis) space so we could calculate the twist angle
            //（翻訳:relaxedAxisを（axis、twistAxis）空間に変換して、ねじれ角を計算できます）
            Quaternion
                r = Quaternion.LookRotation(tmpTargetRot * twists[i].axis, tmpTargetRot * twists[i].twistAxis);

            relaxedAxis = Quaternion.Inverse(r) * relaxedAxis;

            // Calculate the angle by which we need to rotate this Transform around the twist axis.
            // （翻訳書き換え:ねじれ軸を中心にこのTransform（target）を回転させるために必要な角度を計算します。）
            float
                angle
                = Mathf.Atan2(relaxedAxis.x, relaxedAxis.z) * Mathf.Rad2Deg;

            // Store the rotation of the child so it would not change with twisting this Transform
            //（翻訳書き換え:子の回転を格納して、この変形（targetを）をねじると変化しないようにします）
            Quaternion
                defSakippoRot = sakippo.rotation;

            // Twist the bone
            twists[i].transform.rotation = Quaternion.AngleAxis(angle * transTime, tmpTargetRot * twists[i].twistAxis) * tmpTargetRot;

            // Revert the rotation of the child
            //（翻訳:子の回転を元に戻す）
            sakippo.rotation = defSakippoRot;
        }

    }




    void LateUpdate()
    {
        Relax();
    }

}