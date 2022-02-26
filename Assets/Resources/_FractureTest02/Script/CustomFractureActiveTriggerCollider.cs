using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFractureActiveTriggerCollider : MonoBehaviour
{
    //付与時に指定する
    public GameObject FracObj;
    public bool isFracPrefabLoad; //破片なしの場合はPrefabを読み込むのでtrueにする
    public string tmpObjName = "破片なしにしている場合、クラッシュ時Prefab読み込むので、もとの名前割り当てるために取得用";

    [HideInInspector]
    public bool
        isCrash = false;

    void OnTriggerEnter(Collider collider)
    {
        if (isCrash == false)
        {
            if (collider.tag == "ChieriCollider")
            {
                //現在はHitEnergyが無ければ何もしない
                if (collider.GetComponent<CustomFracturingColliderHitEnergy>() != null)
                {
                    //Debug.Log(collider.name);
                    isCrash = true;
                    gameObject.GetComponent<Collider>().enabled = false;

                    //破片なし状態ならPrefab読み込みとして処理（FracObjがPrefab指定されているはず）
                    if (isFracPrefabLoad)
                    {
                        GameObject tmpObj =
                            Instantiate(FracObj, transform.parent, false);
                        tmpObj.name = tmpObjName;//受け取っていた名前付与
                        gameObject.SetActive(false);//このsinglemeshはオフ
                    }
                    else//今まで通り
                    {
                        FracObj.SetActive(true);
                        FracObj.SetActive(true);//なぜかわからないが、リセットでのInstantiate後は二回やらないとTrueにならない。（本当になぞ）
                    }

                }
            }
        }
    }
    //public void Resets()
    //{
    //    isCrash = false;
    //    FracObj.SetActive(false);
    //    gameObject.GetComponent<Collider>().enabled = true;
    //    gameObject.GetComponent<MeshRenderer>().enabled = true;
    //}
}
