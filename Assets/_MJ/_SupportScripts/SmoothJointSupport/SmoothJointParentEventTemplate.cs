using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Listに必要
using System.IO;
#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
#endif

//[ExecuteInEditMode]
public class SmoothJointParentEventTemplate : MonoBehaviour
{
    DataCounter DC;
    [HideInInspector]
    public List<Transform> SJIKtargets = new List<Transform>();

    [HideInInspector]
    public List<Transform> IkskParentDefLocalPosTrsList = new List<Transform>();

    //ペアレント戻し場所（元のフォルダ）
    [HideInInspector]
    public Transform SJIKtargetTrs;

    #region//変数とボタン
    [SerializeField, HeaderAttribute("適用先 指定（書き出し前なので変えても大丈夫）")]
    public int SmoothJoint_Iksk;
    public bool All;

    [Button("LoadData", "読み取り")]
    public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない。

    [SerializeField, HeaderAttribute("適用パラメータ")]
    public bool SetParent;

    public enum AddType
    { animAndEvName, anim }
    [SerializeField, HeaderAttribute("タイミングのタイプ")]
    public AddType addType;

    [SerializeField, HeaderAttribute("適用タイミング")]
    public string animClipName;
    public float animClipNomTime;
    public string evName;

    [Button("SaveDataObj", "書き出し＆Prefabセーブ")]
    public bool dummy3;
    [Button("PrefabSave", "停止前にPrefabセーブ")]
    public bool dummy4;
    #endregion


    void Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        SJIKtargetTrs = GameObject.Find("SJIKtarget").transform;

        #region ■スムースジョイント取得

        //ターゲット群まずクリア（Listは前に追加したものがMissingで残るため）
        SJIKtargets.Clear();
        //ターゲット群取得
        if (GameObject.Find("SJIKtarget") != null)
        {
            foreach (Transform i in SJIKtargetTrs)  //親のTransformに子一覧が入っている（配列として扱えて、順に子の数分ループする)
            { SJIKtargets.Add(i); }
        }
        #endregion ■スムースジョイント取得

        #region ■DefPosTrsを手動取得
        //まずクリア（Listは前に追加したものがMissingで残るため）
        IkskParentDefLocalPosTrsList.Clear();
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk00(Dummy)").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk01PelvisDefLocalPos").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk02LThighDefLocalPos").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk03LThighDefLocalPos").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk04LThighDefLocalPos").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk05LThighDefLocalPos").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk06LThighDefLocalPos").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk07PelvisDefLocalPos").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk08RThighDefLocalPos").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk09RThighDefLocalPos").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk10RThighDefLocalPos").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk11RThighDefLocalPos").transform);
        IkskParentDefLocalPosTrsList.Add(GameObject.Find("Iksk12RThighDefLocalPos").transform);
        #endregion ■DefPosTrsを手動取得


    }

    //再生中のシーンから読み取る
    public void LoadData()
    {
        evName = DC.evs[DC.DB.nowEventNum].Key;
        animClipName = DC.nowGirlAnimClipName;
        animClipNomTime = DC.girlAnimNomTime;

        //PrefabSave(); //読み取りではしない
    }

    //現在のオブジェクトに イベント名で子オブジェクトを作り、その中に孫オブジェクトを生成し、孫にスクリプトを付与してデータ書き込みへ
    //また、もし既にイベント名子オブジェクトが存在していたら、その中に
    public void SaveDataObj()
    {
        //まずTempObj生成
        GameObject TempObj = new GameObject();

        //ALLならオブジェクト名ALL
        if (All == true)
        {
            TempObj.name = "ParentAll";
        }
        //でなければ番号
        else
        {
            TempObj.name = "Parent0" + SmoothJoint_Iksk;
        }
        //ONOFF
        if (SetParent == true) { TempObj.name += "On"; }
        else { TempObj.name += "Off"; }


        //フォルダ名（evname）を、animのみなら"AnimOnly"　animAndEvならイベントネーム
        if (addType == AddType.anim) { evName = "AnimOnly"; }
        else if (addType == AddType.animAndEvName) { evName = DC.evs[DC.DB.nowEventNum].Key; }

        if (transform.Find(evName) == true)//既に子に存在していたら
        {
            //TempObjをフォルダObjにペアレント
            TempObj.transform.SetParent(transform.Find(evName).transform);
        }
        else if (transform.Find(evName) == null)//なかったら
        {
            //フォルダObj生成して
            GameObject EvNameFolderObj = new GameObject(evName);
            //現在のObjにペアレントして
            EvNameFolderObj.transform.SetParent(gameObject.transform);
            //TempObjペアレント
            TempObj.transform.SetParent(EvNameFolderObj.transform);
        }

        //孫にスクリプト付与 そして書き込み
        SmoothJointParentEvent tempSave = TempObj.AddComponent<SmoothJointParentEvent>();
        Save(tempSave);
    }

    //読み取った値を↑で生成したスクリプトに書き出し
    public void Save(SmoothJointParentEvent tempSave)
    {
        tempSave.SmoothJoint_Iksk = SmoothJoint_Iksk;
        tempSave.SetParent = SetParent;
        tempSave.All = All;
        tempSave.addType = addType;

        tempSave.evName = evName;
        tempSave.animClipName = animClipName;
        tempSave.animClipNomTime = animClipNomTime;

        //書き出し時はついでにセーブ
        PrefabSave();
    }

    //prefab保存
    public void PrefabSave()
    {
#if UNITY_EDITOR
        //prefabの保存
        PrefabUtility.CreatePrefab("Assets/_MJ/SmoothJointSupport/SmoothJointEvents.prefab", gameObject);
        AssetDatabase.SaveAssets();
#endif
    }
}
