using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;//タイムラインを再生するコンポーネント
using UnityEngine.Timeline;//トラックやクリップの読み込みなど
using Pixeye.Unity;//FoldOut

//[System.Serializable]
[SerializeField]
public class FlowChartKoma : MonoBehaviour {

    //付与したら変更しないシリアルナンバー（セーブデータ互換のため。テスト）
    [Foldout("入力項目")]
    [HeaderAttribute("■シリアルナンバー■変更不可（セーブデータ・メソッド指定用）")]
    public string staticSerialNumber;

    //付与したら変更しないシリアルナンバー（セーブデータ互換のため。テスト）
    [Foldout("入力項目")]
    [Header("手動追加イベントならチェック入れる")]
    public bool isManualKomaData;

    [Foldout("入力項目")]
    [Header("タイムラインアセット指定")]
    public TimelineAsset playable;

    //破綻したので保留//コマデータ開いて1フレーム経ったことでコマ並ぶので、ライン引く用（obj表示でキャンバスからはみ出る）(プレイヤーチョイスenumの分岐追いがややこしい（Twoを選んだらTwoのままにしなきゃいけない。2重分岐でfourやfiveが来た時破綻）)）
    //public DataCounter.flowChartLine
    //    playerChoiceLine;

    //時刻（時系列順に並べる&時計のために）

    [Foldout("入力項目")]
    [HeaderAttribute("・時刻")]
    public int
        day = 0;
    [Foldout("入力項目")]
    public int
        hour = 0,
        minute = 0;

    //シーン選択
    [Foldout("入力項目")]
    [HeaderAttribute("・シーン選択")]
    public DataBridging.enumScene
        scene;

    [Foldout("入力項目")]
    [Header("Cityx100Fogシーン読み込み")]
    public bool isFogSceneLoad;

    [Foldout("入力項目")]
    [Header("Bathシーン読み込み")]
    public bool isBathSceneLoad;

    //UIに出すかどうか と プレイヤー到達したかどうか（表示非表示の判定）
    [Foldout("入力項目")]
    [HeaderAttribute("・シーンジャンプに搭載するかどうか")]
    public bool
        isFlowChartVis = true;
    public bool
        isPlayerVisFlag = false;
    public bool //プレイヤーが選んでいるルートコマかどうか（線ひくよう）
        isPlayerChoice = false;
    public bool //シーンジャンプ時にここの判定取って表示
        isEXTRA = false;
    public bool
        isHintVisFlag = false;
    [Multiline]
    public string
        hintMessage,
        hintMessageEng;

    //イベント名とナンバー（イベントに移動できるように）(Obj名を名前にすべきか)
    [Foldout("入力項目")]
    [HeaderAttribute("・イベント名")]
    public string
        eventName;
    [Foldout("入力項目")]
    public string
        eventNameEnglish;
    public int
        eventNum;
    public string
        mainEventName;
    public AudioClip
        BGMAudioClip;

    //画像（サムネイル）（null時は未設定画像）
    [Foldout("入力項目")]
    [HeaderAttribute("・サムネイル画像")]
    public Sprite
        thumbnailImageSprite;

    //フローチャートライン（時系列の横列）
    [Foldout("入力項目")]
    [HeaderAttribute("・時系列とフォルダ名")]
    public DataCounter.flowChartLine
        flowChartLine;
    [Foldout("入力項目")]
    public string
        folderName;


    //選択肢チェック穴（サムネイルで残選択肢を可視化）
    [HeaderAttribute("・選択肢List")]
    public List<string>
        sentakushiTitleList = new List<string>(),
        sentakushisList = new List<string>();

    //BADチェック穴（サムネイルで残BADを可視化）
    [HeaderAttribute("・BADチェックリスト")]
    public List<string>
        badEndList = new List<string>();

    ////ステータス（体力）
    //[HeaderAttribute("・ステータス保存")]
    //public float
    //    playerHP;


 //   // Use this for initialization
 //   void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}
