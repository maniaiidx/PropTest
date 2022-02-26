using UnityEngine;

public class SmoothJointParentEvent : MonoBehaviour
{
    DataCounter DC;
    SmoothJointParentEventTemplate SmoothJointParentEventTemplate;

    #region//変数とボタン
    [SerializeField, HeaderAttribute("適用先 決定 (適用タイミング中に↓変更するとそのまま即適用される)")]
    public int SmoothJoint_Iksk;
    public bool All;

    [Button("LoadData", "読み取り")]
    public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない。

    [SerializeField, HeaderAttribute("適用パラメータ")]
    public bool SetParent;

    [SerializeField, HeaderAttribute("タイミングのタイプ")]
    public SmoothJointParentEventTemplate.AddType addType;

    [SerializeField, HeaderAttribute("適用タイミング")]
    public string animClipName;
    public float animClipNomTime;
    public string evName;

    [Button("PrefabSave", "停止前にPrefabセーブ")]
    public bool dummy4;
    #endregion


    void Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        SmoothJointParentEventTemplate = GameObject.Find("SmoothJointEvents").GetComponent<SmoothJointParentEventTemplate>();
    }

    bool animAndEvAddBool = false;//アニメとイベントタイマー両方時はbool判定で一回だけ処理
    void Update()
    {
        if (addType == SmoothJointParentEventTemplate.AddType.anim)
        {
            if (DC.nowGirlAnimClipName == animClipName &&
                DC.girlAnimNomTime >= animClipNomTime && DC.girlAnimNomPrevTime < animClipNomTime)
            {
                AddData();
            }
        }


        if (addType == SmoothJointParentEventTemplate.AddType.animAndEvName
            && animAndEvAddBool == false)
        {
            if (DC.nowGirlAnimClipName == animClipName &&
                DC.evs[DC.DB.nowEventNum].Key == evName &&
                DC.girlAnimNomTime >= animClipNomTime
                )
            {
                AddData();
                animAndEvAddBool = true;
            }
        }
        //Bool戻し（繰り返し再生用に）
        if (addType == SmoothJointParentEventTemplate.AddType.animAndEvName
            && animAndEvAddBool == true)
        {
            if (DC.nowGirlAnimClipName == animClipName &&
                DC.evs[DC.DB.nowEventNum].Key == evName &&
                DC.girlAnimNomTime <= animClipNomTime
                )
            {
                animAndEvAddBool = false;
            }
        }
    }

    //処理する
    public void AddData()
    {
        if (All == true)
        {
            if (SetParent == true)
            {
                //全部Defと同じフォルダにペアレントし、localPositionも合わせる（0はダミー）
                for (int i = 0; i < SmoothJointParentEventTemplate.SJIKtargets.Count; i++)
                {
                    SmoothJointParentEventTemplate.SJIKtargets[i]
                        .SetParent(SmoothJointParentEventTemplate.IkskParentDefLocalPosTrsList[i].parent.gameObject.transform);

                    SmoothJointParentEventTemplate.SJIKtargets[i].localPosition
                        = SmoothJointParentEventTemplate.IkskParentDefLocalPosTrsList[i].localPosition;
                }
            }
            else
            {
                //全部もとのフォルダにペアレント
                for (int i = 0; i < SmoothJointParentEventTemplate.SJIKtargets.Count; i++)
                {
                    SmoothJointParentEventTemplate.SJIKtargets[i]
                        .SetParent(SmoothJointParentEventTemplate.SJIKtargetTrs);
                }
            }
        }
        else
        {
            if (SetParent == true)
            {
                //指定されたIkskペアレントをDefと同じフォルダ同じLocalPosに
                SmoothJointParentEventTemplate.SJIKtargets[SmoothJoint_Iksk]
                    .SetParent(SmoothJointParentEventTemplate.IkskParentDefLocalPosTrsList[SmoothJoint_Iksk].parent.gameObject.transform);

                SmoothJointParentEventTemplate.SJIKtargets[SmoothJoint_Iksk].localPosition
                    = SmoothJointParentEventTemplate.IkskParentDefLocalPosTrsList[SmoothJoint_Iksk].localPosition;
            }
            else
            {
                //指定されたIkskペアレントを元のフォルダに
                SmoothJointParentEventTemplate.SJIKtargets[SmoothJoint_Iksk]
                    .SetParent(SmoothJointParentEventTemplate.SJIKtargetTrs);
            }
        }
        //Debug.Log("スムースジョイントイベンター発動");
    }

    //再生中のシーンから読み取る
    public void LoadData()
    {
        evName = DC.evs[DC.DB.nowEventNum].Key;
        animClipName = DC.nowGirlAnimClipName;
        animClipNomTime = DC.girlAnimNomTime;


        //名づけ
        //ALLならオブジェクト名ALL
        if (All == true)
        {
            gameObject.name = "ParentAll";
        }
        //でなければ番号
        else
        {
            gameObject.name = "Parent0" + SmoothJoint_Iksk;
        }

        //ONOFF
        if (SetParent == true) { gameObject.name += "On"; }
        else { gameObject.name += "Off"; }

        //SmoothJointEvents.PrefabSave(); //読み取りでしない
    }


    //prefab保存
    public void PrefabSave()
    {
        SmoothJointParentEventTemplate.PrefabSave();
    }

}
