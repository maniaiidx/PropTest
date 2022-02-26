using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFracturedSetLocalGravity : MonoBehaviour
{
    [SerializeField, HeaderAttribute("適用する重力")]
    public Vector3 setLocalGravity = new Vector3(0, -9.81f, 0);

    [Button("SetLocalGravity", "ローカル重力に書き換え")]
    public bool dummy1;//属性でボタンを作るので何かしら宣言が必要。使わない。

    //子の破片群オブジェクトのリスト
    public List<GameObject> fracturedObjectList = new List<GameObject>();

    [Button("SetDefault", "解除")]
    public bool dummy2;

    //子の破片群オブジェクト読み込み
    void FracturedObjectsLoad()
    {
        fracturedObjectList.Clear();
        foreach (Transform FracTrs in this.transform)
        {
            //Rigidbody持ってること前提
            if (FracTrs.GetComponent<Rigidbody>() != null)
            { fracturedObjectList.Add(FracTrs.gameObject); }
        }
    }

#if UNITY_EDITOR
    void Reset()//アタッチ時とリセット時に実行
    { FracturedObjectsLoad(); }
#endif

    //ローカル重力を適用
    public void SetLocalGravity()
    {
        FracturedObjectsLoad();//念のためリストをリロード

        for (int i = 0; i < fracturedObjectList.Count; i++)
        {
            //RigidbodyのuseGravity外す
            fracturedObjectList[i].GetComponent<Rigidbody>().useGravity = false;

            //重力適用（スクリプト持ってなければAdd）
            if (fracturedObjectList[i].GetComponent<LocalGravity>() == null)
            { fracturedObjectList[i].gameObject.AddComponent<LocalGravity>().localGravity = setLocalGravity; }
            else { fracturedObjectList[i].GetComponent<LocalGravity>().localGravity = setLocalGravity; }

        }
        Debug.Log(gameObject.name + "の子（破片）にローカル重力を適用完了");
    }

    //元に戻す
    public void SetDefault()
    {
        FracturedObjectsLoad();//念のためリストをリロード

        for (int i = 0; i < fracturedObjectList.Count; i++)
        {
            //RigidbodyのuseGravityつける
            fracturedObjectList[i].GetComponent<Rigidbody>().useGravity = true;

            //重力適用（スクリプト持ってたらDestroy）
            if (fracturedObjectList[i].GetComponent<LocalGravity>() != null)
            { DestroyImmediate(fracturedObjectList[i].gameObject.GetComponent<LocalGravity>()); }

        }
        Debug.Log(gameObject.name + "の子（破片）からローカル重力を解除完了");
    }


    #region リアルタイムで変更処理（ゲーム時にのみsetGravityに変更があれば全部に適用）

    Vector3 prevLocalGravity;
    List<LocalGravity> localGravityList = new List<LocalGravity>();
    private void Start()
    {
        prevLocalGravity = setLocalGravity;

        localGravityList.Clear();
        for (int i = 0; i < fracturedObjectList.Count; i++)
        {
            //あれば
            if(fracturedObjectList[i].GetComponent<LocalGravity>() != null)
            {
                localGravityList.Add(fracturedObjectList[i].GetComponent<LocalGravity>());
            }
        }
    }

    private void Update()
    {
        if (prevLocalGravity != setLocalGravity)
        {
            for (int i = 0; i < localGravityList.Count; i++)
            { localGravityList[i].localGravity = setLocalGravity; }

            prevLocalGravity = setLocalGravity;
        }
    }

    #endregion

}
