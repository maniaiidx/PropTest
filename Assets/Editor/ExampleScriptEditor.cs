using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExampleScript))]//拡張するクラスを指定
public class ExampleScriptEditor : Editor
{

    /// <summary>
    /// InspectorのGUIを更新
    /// </summary>
    public override void OnInspectorGUI()
    {
        //元のInspector部分を表示
        base.OnInspectorGUI();

        //targetを変換して対象を取得
        ExampleScript exampleScript = target as ExampleScript;

        //PrivateMethodを実行する用のボタン
        if (GUILayout.Button("PrivateMethod"))
        {
            //SendMessageを使って実行
            exampleScript.SendMessage("PrivateMethod", null, SendMessageOptions.DontRequireReceiver);
        }

        //PublicMethodを実行する用のボタン
        if (GUILayout.Button("PublicMethod"))
        {
            exampleScript.PublicMethod();
        }


        //
        if (GUILayout.Button("アニメーション"))
        {
            exampleScript.AnimationTest();


        }


    }

}