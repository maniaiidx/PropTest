using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFractureActiveTriggerColliderGenerater : MonoBehaviour
{

    void Awake()
    {
        Set();
    }
    
    public void Set()
    {
        //シングルメッシュObj取得
        GameObject singleMeshObj =
            GetComponent<FracturedObject>().SingleMeshObject;

        //FracturedのObj（これ）から一つ上のPosフォルダに運ぶ
        singleMeshObj.transform.SetParent(transform.parent);


        
        
        ////コリダー付与（なければ（リセット用クローンで再度Awake実行時はなにもしないため））
        //if (singleMeshObj.GetComponent<BoxCollider>() == null)
        //{
        //    var tmpCollider = singleMeshObj.AddComponent<BoxCollider>();

        //    Vector3 tmpSizeV3 = new Vector3();
            
        //    tmpSizeV3.x = tmpCollider.size.x - (tmpCollider.size.x / 10);
        //    tmpSizeV3.y = tmpCollider.size.y - (tmpCollider.size.y / 3);
        //    tmpSizeV3.z = tmpCollider.size.z - (tmpCollider.size.z / 10);

        //    tmpCollider.size = tmpSizeV3;

        //}

        //メッシュコリダーConvex付与（本当は、指定したBoxコライダー移植にしたいが↑、時間ないため保留）
        if (singleMeshObj.GetComponent<MeshCollider>() == null)
        {
            var tmpCollider = singleMeshObj.AddComponent<MeshCollider>();
            tmpCollider.convex = true;

            tmpCollider.cookingOptions = MeshColliderCookingOptions.None;

        }

        
        
        
        
        //RigidBody付与(isKinematicにしながら)(なければ)
        if (singleMeshObj.GetComponent<Rigidbody>() == null)
        { singleMeshObj.AddComponent<Rigidbody>().isKinematic = true; }



        //トリガーコリダー用のスクリプト付与（なければ）
        CustomFractureActiveTriggerCollider tmpComp;
        if (singleMeshObj.GetComponent<CustomFractureActiveTriggerCollider>() == null)
        {
            tmpComp =
                singleMeshObj.AddComponent<CustomFractureActiveTriggerCollider>();
        }
        else //あればそれに再指定する
        {
            tmpComp =
                singleMeshObj.GetComponent<CustomFractureActiveTriggerCollider>();
        }

        //■↑にFractureのObj（これ）指定
        #region 破片なしにしているようならPrefab指定
        //中身が一個（破片がなさそう）なら
        if (transform.childCount == 0)
        {
            tmpComp.tmpObjName = gameObject.name;
            //一つ上の親（_Pos）のFracStorageからPrefab取得
            tmpComp.FracObj = gameObject.transform.parent.GetComponent<FracStorage>().thisFracPrefab;
            tmpComp.isFracPrefabLoad = true;
        }
        //破片ありそうなら今まで通り
        else
        {
            tmpComp.FracObj = gameObject;
        }
        #endregion


        //検知伝えるように、他のスクリプトにもコリダースクリプトを伝える
        if (GetComponent<CustomFractureDetachEvent>())
        { GetComponent<CustomFractureDetachEvent>().customFractureActiveTriggerCollider = tmpComp; }
        //if (GetComponent<CustomFractureAnimClipRecorder>())
        //{ GetComponent<CustomFractureAnimClipRecorder>().customFractureActiveTriggerCollider = tmpComp; }

        //Fracturedオフ（破片たち）
        gameObject.SetActive(false);
        
    }
}
