using UnityEngine;
using UnityEngine.UI;//スライダー
using System.Collections;//IENumeratorを使う用
using System.Collections.Generic; //Listに必要
using Pixeye.Unity;

public partial class DataBridging : MonoBehaviour
{

    //シングルトンのこのクラスに、シーン間で橋渡ししたいデータを渡す。
    //DataCounterその他はここからデータを読み込むようにする。

    static public DataBridging DB;//シングルトン用

    #region AssetBundle含ませるデータ（フォントAsset）

    [Foldout("AssetBundle含ませるデータ")]
    public GameObject
        hukidashiCanvasObj,
        novelSystem;

    #endregion

    #region データベース（関数名一覧など）
    public List<string> RMEMethodNameList;
    public List<string> TansakuMethodNameList;
    #endregion

    #region シーンを跨ぐ共有変数群（風圧音のスクリプトリストなど）

    public List<WindnoiseSound> nowRun_WindNoiseList = new List<WindnoiseSound>();

    #endregion

    #region 環境設定群
    //プレイヤーVR環境
    public enum PlayerVREnvironment
    { VR, DeskTop, NoVR }
    public PlayerVREnvironment playerVR;
    public bool isPlayerVREnvironment = false;//初回起動時の環境設定を行ったかどうか

    //プレイヤーコントローラー環境
    public enum PlayerVRController
    { Xbox, HTCVive, OculusTouch, WindowsMR}
    public PlayerVRController playerController;
    public OrderedDictionary<string, string> inputDict = new OrderedDictionary<string, string>();
    public bool isPlayerControllerEnvironment = false;//初回起動時の環境設定を行ったかどうか
    
    //酔い対策の刻み角度変更用 角度（この数値角度ごとに回転）
    public float angleJagFloat = 25;

    public bool //英語版用
        isEnglish = false,
        isDebugEngUnLoad = false;
    public bool
        isLanguageDebug = false;

    //エンド回数（予定）
    public int storyLoopCountInt = 0;
    //デバッグモード
    public bool isDebugMode;
    //文字送りボタン式切り替えBool
    public bool
        isButtonWaitMode = true,
        isSkipMode,
        isKidokuOnlySkip = true;
    //セーブデータデバッグプレイで毎回作らない用
    public bool
        isSaveDataCreate;

    //智恵理ゼロ位置固定bool
    public bool isChieriPosLock = true;

    public bool isScreenShotVkey = false;
    public bool isScenarioLogOutput = false;
    #endregion

    #region TimeLine

    //イベント移動用
    public bool isKankinFlag = false;//監禁フラグが立ったら、宿題シーンで告白無しに

    public bool isEXTRAEnter = false;//シチュエーションモードフラグ（入ってすぐオフ）

    [SerializeField, HeaderAttribute("開始イベント番号(■■イベント■■)")]
    public int nowEventNum = 1;
    [SerializeField, HeaderAttribute("イベント番号・名前一覧")]
    public List<string> eventNamesList = new List<string>();

    #region イベントの朝昼夜シーンセレクト
    public enum enumScene
    { 指定無し, あさ, Hiru, 夕方, 夜, 深夜 };
    [SerializeField, HeaderAttribute("イベントとシーンの組み合わせ")]
    public bool isSceneChange;
    //public List<enumScene> enumSceneList = new List<enumScene>();
    public bool isDebugEventChange = false;

    #endregion
    public string setEventKey;
    //private bool eventMoveFlag = true, debugEventMoveFlag = false;

    //public float storyTimer;//使ってない？

    public List<GameObject> evMoveDelObjList = new List<GameObject>();
    public List<GameObject> sceneMoveDelObjList = new List<GameObject>();

    public List<string>
        kidokuSerihuKeyList = new List<string>(),
        kiSentakuSerihuKeyList = new List<string>(),
        kidokuNovelKeyList = new List<string>();

    
    //イベントを跨ぐ変数群
    public bool
        isBunki_Day1_SleepBedPos;

    #endregion

    #region シーン移動 画面フェード タイムスケール系
    [SerializeField, HeaderAttribute("画面フェード")]
    //白で移動してきたか黒で移動してきたか、ここの値を変えることで指定する
    public Color nowFadeWhiteEndColor = new Vector4(1, 1, 1, 1);
    public Color nowFadeBlackEndColor = new Vector4(0, 0, 0, 0);
    public Color nowFadeBlackSmartPhoneEndColor = new Vector4(0, 0, 0, 0);
    public Color nowFadeColorEndColor = new Vector4(1, 0, 0, 0);

    #endregion

    #region サイズ・ステータス系
    [SerializeField, HeaderAttribute("サイズ・ステータス系")]

    //身長（後で実際の視界と同期できるようにして、プレイヤーに入力させたい？）
    public float
        sintyouFloat = 170,
        taizyuuFloat = 60;

    public float
        playerHPFloat = 2550,
        playerHPMaxFloat = 2550;

    public Vector3 //ゲーム中書き換えてカメラリセットに対応
        cameraObjectsResetLocalScl = Vector3.one,
        cameraObjectsResetLocalPos = new Vector3(-1.2f, 0, 0.8f),
        cameraObjectsResetLocalEul = Vector3.zero,
        cameraAnchorResetLocalPos = new Vector3(0, 1.55f, 0f),
        cameraAnchorResetLocalRot = Vector3.zero,
        cameraUserResetLocalEul = Vector3.zero,
        //した二つは書き換えない
        cameraSitAnchorDefLocalPos = new Vector3(0, 0.65f, 0f),
        cameraStandAnchorDefLocalPos = new Vector3(0, 1.55f, 0f),
        //した二つは書き換えない
        pcOculusCameraSitAnchorDefLocalPos = new Vector3(0, 0.65f, 0f),
        pcOculusCameraStandAnchorDefLocalPos = new Vector3(0, 1.55f, 0f),
        //した二つは書き換えない
        steamVRcameraSitAnchorDefLocalPos = new Vector3(0, 0f, 0f),
        steamVRcameraStandAnchorDefLocalPos = new Vector3(0, 1f, 0f);

    public float
        defLAIKEyeClamp,
        defLAIKHeadClamp;

    public Vector3
        playerScale_HwZero = new Vector3(0.96f, 0.96f, 0.96f),
        playerScale_HwOne = new Vector3(0.92f, 0.92f, 0.92f),
        playerScale_HwTwo = new Vector3(0.86f, 0.86f, 0.86f),

        playerScale_SekurabeZero,
        playerScale_SekurabeOne,
        playerScale_SekurabeTwo,

        playerScale_MotiageZero,
        playerScale_MotiageOne,
        playerScale_DokomadeChizimuZero,
        playerScale_BaziraGuraiZero,
        playerScale_BaziraGuraiOne,
        playerScale_JougiDeHakariZero,
        playerScale_PetbottleIreZero = new Vector3(0.015f, 0.015f, 0.015f),
        playerScale_City01 = new Vector3(0.01f, 0.01f, 0.01f),
        playerScale_Vore01 = new Vector3(0.01f, 0.01f, 0.01f);

    #endregion

    #region 宿題系
    [SerializeField, HeaderAttribute("宿題系")]
    //攻撃力, 集中力, 集中力回復量, お金, 値段, 集中力減退量
    public float
        HwPowFloat = 1;            //攻撃力
    public float
        HwConcFloat = 20,          //集中力(スタミナ)
        HwConcCureFloat = 0.5f,    //集中力回復量
        HwConcDownFloat = 1,       //集中力減退量
        HwMoneyFloat = 0,          //お金
        HwPriceFloat = 10,         //値段（倍数）
        HwConcCurRhythmFloat = 3,  //回復が始まるまでの時間
        HwAttackRhythmMaxFloat = 1.0f;//攻撃の間隔スピード

    public Vector3
        HwPointLocalScale = new Vector3(0.01f, 0.0001f, 0.01f);

    //現在宿題ステージ・クリアした宿題ステージ数・倒した問題数
    [SerializeField, HeaderAttribute("開始宿題ステージ番号(■宿題■)")]
    public int HwNowStageInt = 1;
    public int HwStageClearCountInt, HwAllEnmKillCountInt;

    #endregion

    #region メニュー（音量・マウス・PostProcessing v2）のユーザーカスタム設定

    public bool
        isUserInitialSetting = false, //初期起動設定済んでるかどうか
        isUserContinue = false;//続きがあるかどうか（現在はタイトル画面でContinue出るかどうか）

    //音量系
    public float
        userMasterVolume = 80,
        userBGMVolume = 80,
        userSEVolume = 80;

    public float
        userMouseCameraSensitivityFlt = 3,
        userStickCameraSensitivity = 1;

    public bool
        isUserFixityMakotoHeightVis;

    //PostProcessing関係
    public bool
        isUserAntialiasing,
        isUserAmbientOcclusion,
        isUserBloom,
        isUserDepthOfFieldV1,
        isUserDepthOfFieldV2,
        isUserFog,
        isUserTPSMode,
        isUserVRUpDownRotate,
        isUserVRSmoothRotate;
        //isUserFreeCameraMode;

    public float
        userDepthOfFieldV1Float,
        userDepthOfFieldV2Float,
        userFieldOfViewFloat,

        adjustDepthOfFieldV1RangeFloat = 1,//レンジから、後の最大値最小値を設定

        adjustDepthOfFieldV1x50MinFloat,
        defaultDepthOfFieldV1x50Float,
        adjustDepthOfFieldV1x50MaxFloat,

        adjustDepthOfFieldV1x100MinFloat,
        defaultDepthOfFieldV1x100Float,
        adjustDepthOfFieldV1x100MaxFloat,

        adjustDepthOfFieldV2MinFloat,
        defaultDepthOfFieldV2Float,
        adjustDepthOfFieldV2MaxFloat,

        adjustFieldOfViewMinFloat,
        defaultFieldOfViewFloat,
        adjustFieldOfViewMaxFloat;

    public bool
        isUserPSControllerFix = false;//PS4コン繋ぐと上選択おしっぱになるのを防ぐ（その代わりSteamVRで右スティックで選択操作できない）
    
    public bool //210711廃止ユーザー固定設定以外 
                //isUserClothsBarefoot,
                //isUserClothsTankTop,
                //isUserClothsBikini,
        isUserFixityOutfit;



    #endregion

    #region アンロック系bool InfoVis機能など
    public bool
        isUserInfoVisMakotoHeightUnlock = false;

    #endregion

    IEnumerator Start()
    {
        #region シングルトン命令
        //DBインスタンスがなかったら
        if (DB == null)
        {
            //このDBをインスタンスとする
            DB = this;
            //シーンを跨いでもDBインスタンスを破棄しない
            DontDestroyOnLoad(this.gameObject);
        }
        else //DBインスタンスが存在したら
        {
            //今回インスタンス化したDBを破棄
            Destroy(this.gameObject);
        }

        #endregion

        #region スタートシーンならやる処理（AssetBundleからシーンロード）
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Start_DataBridging")
        {
            #region ローディングスライダーON
            var loadSlider =
            transform.Find("VRFadeCameraObj/FadeCanvas/Slider_Loading").gameObject.GetComponent<Slider>();
            loadSlider.value = 0;
            loadSlider.gameObject.SetActive(true);
            #endregion

            //非同期でシーンデータ系ロードする用変数（プログレスバー出せる 読み込み完了判定取れる）
            AsyncOperation async;

            #region Sceneデータあれば読み込み

            //アセットバンドル読み込んで　有無チェック（Nullチェックできなかったのでエラー上等でまず読み込み）
            //async読み込みで非同期処理
            async = AssetBundle.LoadFromFileAsync("AssetBundleData/hiruscene");

            ////アセットバンドル読み込んで　有無チェック（asyncにしたのでオフ）
            //AssetBundle HiruSceneAB =
            //    AssetBundle.LoadFromFile("AssetBundleData/hiruscene");

            //■アセットバンドル読み込めてたら
            if (async != null)
            {
                //Debug.Log("AssetBundle Hiruシーン展開開始");

                //アセットバンドル読み込みプログレスバー
                loadSlider.transform.Find("Text").GetComponent<Text>().text
                    = "Loading 0/2";
                while (async.isDone == false)
                {
                    loadSlider.value = async.progress;
                    yield return null;
                }

                Debug.Log("□AssetBundle Hiruシーンロード開始");
                ////アセットバンドルシーン0個目のシーン名読み込み（asyncで読み込んだので、シーン名で直接読み込むことにした。これはTH_Hiruの元あった場所のアドレスだった）
                //Debug.Log(HiruSceneAB.GetAllScenePaths()[0]);
            }
            else//昼シーンアセットバンドル読み込めなかったら
            {
                Debug.Log("Hiruシーンアセットバンドルないので、直接読み込みを開始");
                Debug.Log("□Hiruシーンをロード開始");
            }


            //■シーン移動(async読み込み)
            async =
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("TH_Hiru");

            //シーン読み込みプログレスバー
            loadSlider.transform.Find("Text").GetComponent<Text>().text
                = "Loading 1/2";
            while (async.isDone == false)//Doneするまで待機
            {
                //Debug.Log("ABHiruシーンロード中" + async.progress);
                loadSlider.value = async.progress;
                yield return null;
            }
            Debug.Log("■Hiruシーンロード終了");

            //AudioListener削除
            Destroy(GetComponent<AudioListener>());

            //バー消し
            loadSlider.gameObject.SetActive(false);

            #endregion

        }
        #endregion

        //VRFadeCameraのclearFlagsをDepthに（ローディングゲージのテキストを表示更新するためにSolidColorにしてあるため（戻さないと真っ黒））
        transform.Find("VRFadeCameraObj").GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;

        yield break;
    }


    #region セーブ関連
    [HideInInspector]
    public //セーブデータが既にあったかどうか
        bool isSaveDataLoad = false;

    public Dictionary<string, string> saveDataDict = new Dictionary<string, string>();

    #endregion


}
