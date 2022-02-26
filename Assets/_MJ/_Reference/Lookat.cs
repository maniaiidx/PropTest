using UnityEngine;
using System.Collections;

public class Lookat : MonoBehaviour
{

    //目LookAt
    /*・目のズレ修正について解説（したけど、目の"内側"後ろがピボット位置だったので結局だめだった）
まずlookAtはデフォルトだと、Y軸プラス方向を上方としたZ軸プラス方向へ向く。
下図は画面こちらをLookAtしている場合。（Ｚがこちらにプラス方向）

　　 Ｙ
　　 ↑
　Ｘ←Ｚ

今回のずれた目は、こちらを向かせると下図のようになっていたので

　　　Ｙ→Ｚ
　　　↓
　　　Ｘ

この通りに、X軸マイナス方向を上方に、Ｙ軸プラス方向へ向くようにしなければならない。


第二引数で上方を指定することができるので、-Vector3.rightと指定。
（デフォルトでは、図１のように"そのオブジェクトにとって"の右方向がX軸プラス方向なので
「上方は右にマイナスを掛けた方向」という指定になる）

これで図２のようにX軸マイナス方向が上方とはなったが、Z軸プラス方向に向くので

　Ｙ←Ｚ
　　 ↓
　　 Ｘ

となってしまう。

これに、「この状態から、ワールド座標軸でY軸に-90度」をすることで
図2のようにYがこちらを向いて、成功した。


    */

    public bool lookAtOn = true;
    public Transform eyeTarget;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (lookAtOn)
        {
            transform.LookAt(eyeTarget, Vector3.forward);
        }
    }

}

