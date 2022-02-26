using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;//DOTween

public class MobControl : MonoBehaviour
{

    DataCounter DC;
    DataBridging DB;

    public float ninzuu = 1;

    void Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        DB = GameObject.Find("DataBridging").GetComponent<DataBridging>();
        StartCoroutine(MobControlCor());
    }

    IEnumerator MobControlCor()
    {
        while (true)
        {
            //キャラ生成
            if (Input.GetKeyDown(KeyCode.G))
            {
                for (int i = 0; i < ninzuu; i++)
                {
                    //生成
                    GameObject Mob00Taichi
                        = Instantiate(Resources.Load("_FractureTest02/MobRun/Mob00Taichi") as GameObject
                        , DC.GameObjectsTrs, false);
                    DB.evMoveDelObjList.Add(Mob00Taichi);
                    Mob00Taichi.name = nameof(Mob00Taichi);
                    Mob00Taichi.transform.localScale = DC.CameraObjectsTrs.localScale;
                    Mob00Taichi.transform.Find("Taichi").GetComponent<Animator>().CrossFadeInFixedTime("sprint_00_Re", 0, 0);

                    #region 移動
                    //位置
                    GameObject MobRunStartPosObj
                        = Resources.Load("_FractureTest02/MobRun/MobRunStartPosObj") as GameObject;
                    //位置
                    GameObject MobRunGoalPosObj
                        = Resources.Load("_FractureTest02/MobRun/MobRunGoalPosObj") as GameObject;

                    Mob00Taichi.transform.localPosition = MobRunStartPosObj.transform.localPosition
                        + new Vector3(0, 0, 0.01f * i);//人数分ずらし

                    Mob00Taichi.transform.DOLocalMove(MobRunGoalPosObj.transform.localPosition
                        , 10)
                        .SetEase(Ease.Linear)
                        .OnComplete(() => { if (Mob00Taichi) { Destroy(Mob00Taichi); } });

                    //方向は設置してから変えるためにDO
                    Mob00Taichi.transform.DOLookAt(MobRunGoalPosObj.transform.position, 0.01f);
                    #endregion

                }

            }

            //GPUInstansing
            if (Input.GetKeyDown(KeyCode.H))
            {

            }

            yield return null;
        }
        yield break;
    }


}
