using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Listに必要

//エディタフォルダのResourceFilesEditorで自動読み込み

public class ResourceFiles : MonoBehaviour
{
    //EV_C_系PrefabなどをResouces読み込みから退避（それぞれのシステム起動時に読み込んでいたもの）
    public GameObject
        EV_C_KaijuBattle,
        EV_C_KakureOni,
        KO_SimplePointObj,
        MenuFolder,
        yd_loadClothsWindowCanvas;//ゆろーどさん着替えシステムテスト環境用

    #region SE_BGM
    public UnityEngine.Audio.AudioMixer audioMixer;
    public Dictionary<string, AudioClip>//ファイル名で指定するためにDictionary化
        SE = new Dictionary<string, AudioClip>(),
        BGM = new Dictionary<string, AudioClip>();
    [SerializeField, HeaderAttribute("SE_BGM"), TooltipAttribute("Dictionaryはシリアライズできない（事前に読んでもゲーム時読み込めない）ので、Listに入れてAwakeでDictionary化している")]
    public List<AudioClip> SEAudioClipList = new List<AudioClip>();
    public List<AudioClip> BGMAudioClipList = new List<AudioClip>();
    [HideInInspector]
    public List<string>//Key名
        SEKeyNameList = new List<string>(),
        BGMKeyNameList = new List<string>();

    public GameObject tempSEObj;
    #endregion

    #region UI
    [SerializeField, HeaderAttribute("UI")]
    public GameObject
        TobecontinuedObj;

    #endregion


    #region ChieriKomono
    [SerializeField, HeaderAttribute("ChieriKomono")]
    public GameObject SkirtBoneQuad;
    public GameObject SharpenObj, KeshigomuObj, ChieriSumahoObj;
    public PhysicMaterial keshigomuPMat;
    #endregion
    
    #region ColliderObj
    [SerializeField, HeaderAttribute("ColliderObj")]
    //実ファイル
    public List<GameObject> colliderObjGameobjectList = new List<GameObject>();
    //キー名
    [HideInInspector]
    public List<string> colliderObjKeyNameList = new List<string>();
    //組み合わせたDictionary
    public Dictionary<string, GameObject> colliderDic = new Dictionary<string, GameObject>();
    #endregion

    #region デバッグ
    [SerializeField, HeaderAttribute("デバッグ")]
    public string testString01;
    public AudioClip testAudioClip01, testAudioClip02;
    #endregion

    void Awake()
    {
        // AssetLoad();

        //各ListをDictionary化
        #region//SE_BGM
        //SE
        for (int i = 0; i < SEAudioClipList.Count; i++)
        { SE.Add(SEKeyNameList[i], SEAudioClipList[i]); }

        for (int i = 0; i < BGMAudioClipList.Count; i++)
        { BGM.Add(BGMKeyNameList[i], BGMAudioClipList[i]); }
        #endregion

        //Collider
        for (int i = 0; i < colliderObjGameobjectList.Count; i++)
        { colliderDic.Add(colliderObjKeyNameList[i], colliderObjGameobjectList[i]); }


    }

}


