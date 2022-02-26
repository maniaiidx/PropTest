//using UnityEngine;
//using System.Collections;
//using UnityEditor;

//[CustomEditor(typeof(SmoothJointEventTemplate))]
//public class SmoothJointEventEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        SmoothJointEventTemplate obj = target as SmoothJointEventTemplate;
//        obj.onSmoothJoint = (SmoothJoint)EditorGUILayout.ObjectField("SmoothJoint", obj.onSmoothJoint, typeof(SmoothJoint), true);
//        obj.IkskTrs = (Transform)EditorGUILayout.ObjectField("Iksk", obj.IkskTrs, typeof(Transform), false);
//        EditorGUILayout.Separator();

//        obj.FixedTargetOn = EditorGUILayout.Toggle("FixedTargetOn", obj.FixedTargetOn);
//        obj.IkskToLocalPosition = EditorGUILayout.Vector3Field("Iksk配置localPosition",obj.IkskToLocalPosition);

//        obj.addType = (SmoothJointEventTemplate.AddType)EditorGUILayout.EnumPopup("適用条件のタイプ", obj.addType);






//        EditorUtility.SetDirty(target);
//    }

//}