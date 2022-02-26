using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadFracturedSingleMeshObj : MonoBehaviour
{

    [Button("Reload", "シングルメッシュ再指定")]
    public bool dummy1;//属性でボタンを作るので何かしら宣言が必要。使わない。

    void Reload()
    {
        bool isReloadComplete = false;

        var tmpFO = GetComponent<FracturedObject>();

        foreach(Transform k in transform)
        {
            if (k.name.Contains("(single mesh)"))
            {
                tmpFO.SingleMeshObject = k.gameObject;
                Debug.Log(transform.parent.name + "再指定成功");
                isReloadComplete = true;
            }
        }

        if(isReloadComplete == false)
        { Debug.Log(transform.parent.name + "■再指定失敗 (single mesh)がない？"); }

    }
}
