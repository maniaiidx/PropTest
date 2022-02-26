using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracStorageAllAtOnce : MonoBehaviour
{
#if UNITY_EDITOR

    [Button(nameof(FracStorageListAdd), "このObj以下全てのFracStorage取得")] public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない
    public List<GameObject> AllFrac_PosObj = new List<GameObject>();
    void FracStorageListAdd()
    {
        List<GameObject> AllObjList = GetAllChildren.GetAll(gameObject);

        AllFrac_PosObj.Clear();
        foreach (GameObject obj in AllObjList)
        {
            if (obj.GetComponent<FracStorage>())
            {
                AllFrac_PosObj.Add(obj);
            }
        }
    }

    [Button(nameof(FracDel), "リスト全てで破片を削除")] public bool dummy3;//属性でボタンを作るので何かしら宣言が必要。使わない
    void FracDel()
    {
        for (int i = 0; i < AllFrac_PosObj.Count; i++)
        {
            AllFrac_PosObj[i].GetComponent<FracStorage>().Storage();
        }
    }

    [Button(nameof(PrefabLoad), "リスト全てでPrefab読み込み")] public bool dummy2;//属性でボタンを作るので何かしら宣言が必要。使わない
    void PrefabLoad()
    {
        for (int i = 0; i < AllFrac_PosObj.Count; i++)
        {
            AllFrac_PosObj[i].GetComponent<FracStorage>().Unpack();
        }
    }
#endif

}
