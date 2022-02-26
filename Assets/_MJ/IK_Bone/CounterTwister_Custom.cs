using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[ExecuteInEditMode]
public class CounterTwister_Custom : MonoBehaviour
{
    public Segment[] Segments;
    private Quaternion previousRotation;
    private Quaternion[] currentSegmentRotations = new Quaternion[0];

    private void OnEnable()
    {
        this.previousRotation = this.transform.rotation;
    }

    private void Update()
    {
        var rootTransform = this.transform;
        var currentRotation = rootTransform.rotation;

        // 前のフレームとのルートオブジェクトの回転差分を求める
        var deltaRotation = currentRotation * Quaternion.Inverse(this.previousRotation);
        float deltaRotationAngle;
        Vector3 deltaRotationAxis;
        deltaRotation.ToAngleAxis(out deltaRotationAngle, out deltaRotationAxis);

        // 回転量が0なら何もする必要はない
        if (deltaRotationAngle == 0.0f)
        {
            return;
        }

        var segmentCount = this.Segments.Length;
        if (this.currentSegmentRotations.Length < segmentCount)
        {
            Array.Resize(ref this.currentSegmentRotations, segmentCount);
        }

        // 今回は各セグメントが親子関係を作っているので、ねじれを解消する過程で
        // セグメントを回転させると、他のセグメントも回転してしまう
        // そこで、ねじれ解消に入る前に各セグメントの現在の回転を覚えておく
        for (var i = 0; i < segmentCount; i++)
        {
            var segmentTransform = this.Segments[i].Transform;
            if (segmentTransform != null)
            {
                this.currentSegmentRotations[i] = segmentTransform.rotation;
            }
        }

        // ねじれ解消処理を行う
        // Segmentsを0から順番に操作していますので、Segmentsには親子関係がルートに近いセグメントほど
        // 若い番号になるようTransformをセットしてください
        // さもないと、せっかく設定した子の回転が親の回転設定時に崩れてしまうかもしれません
        for (var i = 0; i < segmentCount; i++)
        {
            var segment = this.Segments[i];
            var segmentTransform = segment.Transform;
            var weight = -segment.Weight;

            // セグメントが未設定なら何もする必要はない
            if (segmentTransform == null)
            {
                continue;
            }

            // ねじれを抽出する軸...さしあたりルートに対するセグメントの相対位置の方角としました
            //var axis = (segmentTransform.position - rootTransform.position).normalized;
            var axis = (this.Segments[segmentCount - 1].Transform.position - rootTransform.position).normalized;
            //var axis = (this.Segments[0].Transform.position - this.Segments[1].Transform.position).normalized;

            // ルートの差分回転のうち、セグメントのねじれに寄与する成分の割合を求める
            var twistFactor = Vector3.Dot(deltaRotationAxis, axis);

            // ねじれ軸周りの逆回転を作成する
            // 回転角にセグメントごとに決めたWeightを掛け、Weightが1に近いほどねじれが解消されるようにする
            var counterTwist = Quaternion.AngleAxis(-deltaRotationAngle * twistFactor * weight, axis);

            // セグメントに回転を適用する
            segmentTransform.rotation = counterTwist * this.currentSegmentRotations[i];
        }

        this.previousRotation = currentRotation;
    }

    [Serializable]
    public struct Segment
    {
        public Transform Transform;
        [Range(-2f, 1f)] public float Weight;
    }

    // おまけ機能...インスペクタにセグメントのlocalRotationを無回転にするボタンを追加する
    // 差分回転からねじれ成分のみ抽出してセグメント回転を調整するようにした都合上、ルートの回転を無回転に戻しても
    // セグメントの回転が元に戻るとは限らなくなりました
    // いちいちセグメントを選択して戻すのが面倒でしたので、一括設定するボタンを設けました
#if UNITY_EDITOR
    [CustomEditor(typeof(CounterTwister_Custom))]
    public class CounterTwisterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("子の回転値ゼロにするボタン"))
            {
                var counterTwister = target as CounterTwister_Custom;

                foreach (var segment in counterTwister.Segments)
                {
                    var segmentTransform = segment.Transform;
                    if (segmentTransform != null)
                    {
                        segmentTransform.localRotation = Quaternion.identity;
                    }
                }
            }
            this.DrawDefaultInspector();
        }
    }
#endif
}