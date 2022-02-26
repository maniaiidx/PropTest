//using追加も自由です（もし追加したら、後で教えてほしいかも）
using System.Collections;
using UnityEngine;

//クラスは↓の通りにしてください。
public partial class DataCounter
{
    //■ここからコルーチンのメソッドを追加していきます

    //型はIEnumerator（コルーチンメソッド）　メソッド名は自由（RME_が付いてると、後で管理しやすいかも）


    public IEnumerator Azlight_CameraAddComponent_Animator()
    {
        if (CameraObjectsTrs.gameObject.GetComponent<Animator>() == null)
        {
            CameraObjectsTrs.gameObject.AddComponent<Animator>().runtimeAnimatorController =
            GameObject.Find("Azlight_Resource").GetComponent<Azlight_ResourceFiles>().PlayerAnimatorController;
        }

        yield break;
    }

    public IEnumerator Azlight_SocksUpdateOffScreen()
    {
        GirlMeshTrs.Find("Socks").GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = false;
        yield break;
    }

}

//メソッド名をタイムラインで指定すれば、そこで実行されます。

