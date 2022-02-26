using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;

//複数選択有効
[CanEditMultipleObjects]
#endif


public class FracStorage : MonoBehaviour
{
#if UNITY_EDITOR

    [Button(nameof(Unpack), "Prefabの状態に戻す")] public bool dummy2;//属性でボタンを作るので何かしら宣言が必要。使わない
#endif

    public GameObject
        thisFracPrefab;

#if UNITY_EDITOR

    [Button(nameof(Storage), "破片を削除")] public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない



    public void Storage()
    {
        //if (thisFracPrefab == null)
        //{
        //    Debug.LogError("■■Prefabが指定されてない模様");
        //    return;
        //}

        //子のFrac親Obj取得
        GameObject koFracObj = transform.GetChild(0).gameObject;

        //#region 破片なしで保存しようとしてないかチェック
        //if (koFracObj.transform.childCount <= 2)
        //{
        //    Debug.LogError("■■破片Objない状態でPrefab保存しようとしているかも");
        //    return;
        //}
        //#endregion


        ////Prefabのパスを取得
        //string path = AssetDatabase.GetAssetPath(thisFracPrefab);

        ////■プレファブ作成
        //PrefabUtility.SaveAsPrefabAsset(koFracObj, path);

        //Prefab状態だったらUnpack
        if(PrefabUtility.GetCorrespondingObjectFromSource(koFracObj) != null)
        {
            //Unpack
            PrefabUtility.UnpackPrefabInstance(koFracObj
                , PrefabUnpackMode.OutermostRoot
                , InteractionMode.UserAction);
        }



        #region 破片削除

        //なぜか直接Trasnformからforやforechで削除しても半分しか削除されない。
        //List追加はできて、それを利用して削除すればうまくいくので
        //一度List化している。

        //FracObjをリスト取得（single meshを省く）
        List<Transform> FracList = new List<Transform>();
        foreach (Transform k in koFracObj.transform)
        {
            if (k.gameObject.name.Contains("single mesh"))
            { }
            else
            {
                FracList.Add(k);
            }
        }


        //削除
        for (int i = 0; i < FracList.Count; i++)
        {
            DestroyImmediate(FracList[i].gameObject);
        }

        #endregion

        #region LocalGravityコンポーネントオフ（Awakeで破片読みにいこうとするのでひとまずオフ）
        if (koFracObj.GetComponent<CustomFracturedSetLocalGravity>())
        {
            koFracObj.GetComponent<CustomFracturedSetLocalGravity>().enabled = false;
        }
        #endregion

        #region //コンポーネント削除 //とりあえず処理安定してから（ゲームスタート時にやってた処理を行う感じ）
        //if (koFracObj.GetComponent<FracturedObject>())
        //{
        //    Destroy(koFracObj.GetComponent<FracturedObject>());
        //}
        //if (koFracObj.GetComponent<CustomFracturedSetLocalGravity>())
        //{
        //    Destroy(koFracObj.GetComponent<CustomFracturedSetLocalGravity>());
        //}
        //if (koFracObj.GetComponent<CustomFractureDetachEvent>())
        //{
        //    Destroy(koFracObj.GetComponent<CustomFractureDetachEvent>());
        //}
        //if (koFracObj.GetComponent<CustomFractureActiveTriggerColliderGenerater>())
        //{
        //    Destroy(koFracObj.GetComponent<CustomFractureActiveTriggerColliderGenerater>());
        //}
        #endregion
    }

    public void Unpack()
    {
        if (thisFracPrefab == null)
        {
            Debug.LogError("■■Prefabが指定されてない模様\n" + gameObject.name);
            return;
        }

        string tmpObjName = thisFracPrefab.name;

        //Objectあれば 名前取得して削除して
        if (transform.childCount == 1)
        {
            tmpObjName = transform.GetChild(0).gameObject.name;
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        //2個以上あるなら注意出す
        else if(transform.childCount >= 2)
        {
            Debug.LogError("■■Fractured_Posの中に二つ以上Objectがあるので確認");
            return;
        }

        //Prefab呼び出して名前割り当て
        GameObject tmpObj =
            PrefabUtility.InstantiatePrefab(thisFracPrefab,transform) as GameObject;
        tmpObj.name = tmpObjName;
    }

#endif

}
