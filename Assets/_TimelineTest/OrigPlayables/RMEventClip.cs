using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Pixeye.Unity;//FoldOut
using Candlelight;//プロパティいじったときのコールバック機能
using DG.Tweening;//DOTween
using UnityEngine.Rendering.PostProcessing;//PostProcessing Stack v2をスクリプトでいじるために必要
using UnityEngine.Events;//メソッド指定用
using System.Reflection;//変数管理

[Serializable]
public class RMEventClip : PlayableAsset, ITimelineClipAsset
{

    //Clipでも変数持てるが、メンバー変数はBehaviourのみにする（ちゃんとクリップごとに表示されるし編集できる）
    //必ずPublic [SerializedField] privateは不可　（あと、そうしないとRecordボタンも表示されない）
    public RMEventBehaviour m_behaviour =
        new RMEventBehaviour();

    //しかし↑だとインスペクターのカスタムができないので、諦めてここでも変数を持つ（移植中）

    #region ユーザー機能 メモなど
    [Foldout("ユーザー機能　メモなど", true)]
    [TextArea(1, 30)] public string userMemo;
    public bool clipMute = false;
    [TextArea(0, 30)] public string autoTextOutput;

    //コピー
    [Button(nameof(Copy), "コピー")]
    public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない
    void Copy()
    {
        StaticFields.clipClipBord = this;

        Debug.Log("コピーしました" + StaticFields.clipClipBord.userMemo);
    }

    //ペースト
    public bool pasteLock = true;
    [Button(nameof(Paste), "ペースト")]
    public bool dummy2;//属性でボタンを作るので何かしら宣言が必要。使わない
    void Paste()
    {
        if (pasteLock == false)
        {
            FieldInfo[]
                staticCpInfoArray = StaticFields.clipClipBord.GetType().GetFields(),
                thisCpInfoArray = this.GetType().GetFields();
            //↓BehaviourもCpに含まれてるからか、一緒にコピーできたのでいらなかった
            //staticBhInfoArray = StaticFields.clipClipBord.m_behaviour.GetType().GetFields(),
            //thisBhInfoArray = this.m_behaviour.GetType().GetFields();

            for (int i = 0; i < staticCpInfoArray.Length; i++)
            {
                thisCpInfoArray[i].SetValue(this, staticCpInfoArray[i].GetValue(StaticFields.clipClipBord));
            }
            Debug.Log("■ペーストしました");
            pasteLock = true;
        }
        else
        {
            Debug.Log("PasteLockを解除するとペースト実行できます。（アンドゥできないので注意！）");
        }


    }


    #endregion

    #region フラグ関連
    [Foldout("フラグ関連", true)]
    [HeaderAttribute("フラグ判定を使用するかどうか")]
    public bool
        isUseFlagBool;

    [HeaderAttribute("┗フラグが以下の状態であればこのクリップ実行")]
    public bool[]
        flagBoolArray;

    [HeaderAttribute("セーブデータのフラグを使用するかどうか（立っていればクリップ実行）")]
    public bool
        isUseStaticFlagBool;
    [HeaderAttribute("┗そのフラグ名")]
    public string
        staticFlagKey;
    [HeaderAttribute("┗そのフラグがTrueで実行？")]
    public bool
        staticFlagUseTrue = false;

    [HeaderAttribute("■デバッグ用 フラグ書き込みを使用するかどうか")]
    public bool
        isUseDebugFlagBool;
    [HeaderAttribute("┗現在のフラグを以下の状態に書き込む")]
    public bool[]
        debugFlagBoolArray;

    [HeaderAttribute("セーブデータのフラグ書き換え")]
    public StaticFlagWrite staticFlagWrite = StaticFlagWrite.__;
    public enum StaticFlagWrite { __, True, False }
    [HeaderAttribute("┗書き換えるセーブデータフラグ名")]
    public string
        writeStaticFlagKey;


    [Header("VRかPCか判定して実行")]
    public VRorPC vRorPC = VRorPC.__;
    public enum VRorPC { __, VR, PC }

    #endregion

    #region スクリプト・タイムライン切替・イベント移動(終了)

    [Foldout("スクリプト・タイムライン切替・イベント移動(終了)", true)]
    [Header("一括 使用メソッド名指定")]
    public String[] useMethodNameArray;

    [Header("↑スクリプト処理終了までタイムライン待機")]
    public bool
        isMethodWait = false;

    [Header("タイムライン切り替え ※ こちらは旧版なので一つ↓を使います")]
    public GameObject
        UnityTimelineObj;
    public TimelineAsset
        nextTimelineAsset;
    public bool
        isEventMove;

    [Header("移動先イベント名（未入力時は今まで通りTimeline終了（スクリプトで後処理必要））")]
    public String
        MoveEventName;
    [Header("移動時シーン初期化（強制黒フェード）")]
    public bool
        isForceSceneLoad;


    [Header("ランダムで飛ぶタイムライン")]
    public TimelineAsset[]
    randomNextTimelineAsset;

    #endregion

    #region 選択肢
    [Foldout("選択肢", true)]
    public string
        AKeySentakushi;
    public string
        previewAValueSentakushi;
    public string
        tmpAKeySentakushi;
    [TextArea(2, 5)]
    public string
        tmpAValueSentakushi;
    public bool[]
        aSentakuflagBoolArray;

    public string
        BKeySentakushi;
    public string
        previewBValueSentakushi;
    public string
        tmpBKeySentakushi;
    [TextArea(2, 5)]
    public string
        tmpBValueSentakushi;
    public bool[]
        bSentakuflagBoolArray;

    [Header("選択中タイムラインを止めるかどうか")]
    public bool
        isSentakushiWait = true;
    public bool[]
        throughSentakuflagBoolArray;
    [Header("選択肢解除（解除時は選択肢設置時のthroughフラグが書き込まれる（このクリップのは書き込まれない））")]
    public bool
        isSentakushiThrough;

    #endregion

    #region 宿題システム
    [Foldout("宿題システム（工事中）", true)]

    public HWSystemONOFF hWSystem = HWSystemONOFF.__;
    public enum HWSystemONOFF { __, ON, OFF }

    [Header("宿題中ループ地点設定")]
    public HWLoop hWloop = HWLoop.__;
    public enum HWLoop { __, Start, End }

    [Header("宿題完了後に切り替えるタイムライン")]
    public TimelineAsset
        HWEndGoTimelineAsset;
    //public GameObject
    //    HWEndGoTimelineObj;

    #endregion

    #region プレイヤー カメラ
    [Foldout("プレイヤー カメラ IK　特殊など", true)]
    [Header("プレイヤー移動 拡縮 回転")]
    public GameObject
        cameraObjectsTrsPosObj;
    public float
        cameraObjectsTrsTime;
    public Ease
        cameraObjectsTrsEase;
    [Header("ユーザーのカメラ回転をキープ（基本0秒で行う）")]
    public bool
        isKeepUserCamRot = false;

    [Header("目眩演出")]
    public bool
        isUseMemai;

    [Header("■New! プレイヤーPosを他ObjのPosと同期")]
    public PlayerObjDouki playerObjDouki = PlayerObjDouki.__;
    public enum PlayerObjDouki { __, 同期開始, 同期Off, }
    public string
        doukiObjName;
    public bool
        isPosDouki = false,
        isRotDouki = false,
        isSclDouki = false;

    [Header("プレイヤーの手表示")]
    public DummyHand dummyHandEnum = DummyHand.__;
    public enum DummyHand { __, ON, Off }

    [Header("プレイヤーのIK（TPSや手の表示時に使用するイメージ）")]
    public UseMakotoIKEf useMakotoIKEf = UseMakotoIKEf.__;
    public enum UseMakotoIKEf { __, RHandEf, LHandEf, RFootEf, LFootEf, RLHand, RLFoot, ALL }

    [Range(0, 1)]
    public float
        makotoIKWeight;

    [Range(0, Mathf.Infinity)]
    public float
        makotoIKTime;

    public Ease
        makotoIKEase;

    [HeaderAttribute("全てのプレイヤーIKTargetを現在ポーズの位置へ移動")]
    public bool isPlayerPosToIKTargetPosRot;

    [Header("プレイヤーのアニメーション オフ（宙ぶらりんのようなポーズになります）")]
    public PlayerUniqueMotion playerUniqueMotion = PlayerUniqueMotion.__;
    public enum PlayerUniqueMotion { __, True, False }


    [Header("カメラエフェクト演出")]
    public CameraEffect cameraEffect = CameraEffect.__;
    public enum CameraEffect { __, 時間補正中の目眩, 時間補正目眩の点滅のみ, OFF }
    public float cameraEffectTime;

    [Header("プレイヤー身長表示オフセット")]
    public MakotoSintyouVisOffset makotoSintyouVisOffset = MakotoSintyouVisOffset.__;
    public enum MakotoSintyouVisOffset { __, ON, OFF }

    public String offsetObjName;

    #endregion

    #region IK関連


    [Foldout("ちえりIK関連", true)]

    public UseIKEf useIKEf = UseIKEf.__;
    public enum UseIKEf { __, RHandEf, LHandEf, RFootEf, LFootEf, RLHand, RLFoot, ALL }
    [Range(0, 1)]
    public float
        IKWeight;
    [Range(0, Mathf.Infinity)]
    public float
        IKTime;
    public Ease IKEase;

    public ChgIKBendGoalWeight chgIKBendGoalWeight = ChgIKBendGoalWeight.__;
    public enum ChgIKBendGoalWeight { __, RHand, LHand, RFoot, LFoot, RLHand, RLFoot, ALL }
    [Range(0, 1)]
    public float
        bendGoalWeight;

    [HeaderAttribute("全てのIKTargetを現在ポーズの位置へ移動")]
    public bool isGirlPosToIKTargetPosRot;

    [Header("ちえりIKLookAt")]
    public IKLookAtPlayerEnum iKLookAtPlayer = IKLookAtPlayerEnum.__;
    public enum IKLookAtPlayerEnum { __, 通常On, プレイヤー見続ける機能On, 解除 }
    public float
        IKLookAtOnOffTime = 1;
    public bool
        isIKLookAtHead = true;
    [HideInInspector]//Bodyは挙動難しいので一旦なし　（ちなみにEyeとHeadで別LookAtに分けてるのは、LookAtのターゲットを一箇所で指定すると真正面向きになってしまうから（俯き気味で見るには分ける必要ある））
    public bool
        isIKLookAtBody = false;

    #endregion

    #region モーション関連
    [Foldout("モーション関連", true)]
    [HeaderAttribute("■モーション 一括指定再生")]
    public MotionStructArray[] motions = new MotionStructArray[0];

    //AnimatorのOnOff設定用Enum（Boolだと初期値設定できないため仕方なくEnum）
    public enum AnimatorOnOffEnum { __, On, Off }

    //プロパティStruct
    [Serializable]
    public struct MotionStructArray
    {
        public string
            objName;
        public ExposedReference<GameObject> //名前読み取るのみ
            readNameObj;
        public string
            stateName;
        public float
            crossFadeTime;
        public int
            layer;
        public AnimatorOnOffEnum
            animatorOnOff;
    }

    [HeaderAttribute("■モーションのセッティング変更 一括")]
    public MotionSettingsStructArray[] motionSettings = new MotionSettingsStructArray[0];
    //プロパティStruct
    [Serializable]
    public struct MotionSettingsStructArray
    {
        public string
            objName;
        public float
            animSpeed;
    }


    [HeaderAttribute("■モーションでウェイト（指定モーション完了までTL待機）")]
    public string
        waitMotionObjName;
    public string
        waitMotionClipName;
    public float
        waitMotionNormlizedTime = 1;

    #endregion

    #region オブジェクト設置移動削除 関係
    [Foldout("オブジェクト設置移動削除 関係", true)]

    [HeaderAttribute("■一括 プレファブをGameobjectsに設置")]
    public SpawnObjStructArray[] spawnObjs = new SpawnObjStructArray[0];

    //既に置かれてる時用Enum
    public enum SpawnObjAlreadyEnum { __, 設置しない, 削除して再設置 }

    //プロパティStruct
    [Serializable]
    public struct SpawnObjStructArray
    {
        public string
            spawnObjName;

        [SerializeField, PropertyBackingField]//[SerializeField, PropertyBackingField( "spawnObj" )]とすれば　→　PropertyBackingField 属性にプロパティ名を指定することで 変数を好きなプロパティと紐付けることができます
        public GameObject
        m_spawnObj;
        public GameObject spawnObj //↑のObj指定時にspawnObjNameへObj.nameを入れる
        {
            //getはそのまま
            get { return m_spawnObj; }
            //名前取得
            set
            {
                //まずはそのままデータ代入
                m_spawnObj = value;
                //nullじゃなければ名前へ代入
                if (m_spawnObj != null)
                {
                    spawnObjName = m_spawnObj.name;
                }
            }
        }
        [Header("既に同じ名前のObjが置かれている場合")]
        public SpawnObjAlreadyEnum spawnObjAlreadyEnum;
    }

    [HeaderAttribute("■一括 Obj移動 回転 拡縮")]
    public MoveObjStructArray[] moveObjs = new MoveObjStructArray[0];
    //プロパティStruct
    [Serializable]
    public struct MoveObjStructArray
    {
        public string
            objName;
        public ExposedReference<GameObject> //名前読み取るのみ
            readNameObj;
        public GameObject
            posObj;
        public string
            hierarchyPosObjName;
        public Vector3
            addRotate;
        public RotateMode
            rotateMode;
        public float
            time;
        public Ease
            ease;

        public NextMoveObjStructArray[] nextMove;

        //public bool //今後は標準で影響するようにするために一旦コメントアウト
        //    isEnableScale;
    }

    //NextMove用プロパティStruct
    [Serializable]
    public struct NextMoveObjStructArray
    {
        public GameObject
            posObj;
        public string
            hierarchyPosObjName;
        public Vector3
            addRotate;
        public RotateMode
            rotateMode;
        public float
            time;
        public Ease
            ease;
    }



    [HeaderAttribute("■一括 ObjアクティブONOFF")]
    public SetActiveObjStructArray[] setActiveObjs = new SetActiveObjStructArray[0];
    //プロパティStruct
    [Serializable]
    public struct SetActiveObjStructArray
    {
        public string
            objName;
        public string
            objPath;
        public string
            rootObjName;
        [HeaderAttribute("上3つは自動で入力される")]
        public ExposedReference<GameObject> //アドレス読み取るのみ
            readAddressObj;

        public bool
            setActive;
    }

    [HeaderAttribute("■一括 Objぺアレント")]
    public SetParentObjStructArray[] setParentObjs = new SetParentObjStructArray[0];
    //プロパティStruct
    [Serializable]
    public struct SetParentObjStructArray
    {
        //ReadObjName系は一旦やめ
        //public ExposedReference<GameObject>
        //    ReadParentObj;

        public string
            ParentObjName;
        //public ExposedReference<GameObject>
        //    ReadChildObj;

        public string
            ChildObjName;

        public bool
            moreLateAction;
    }


    //[HeaderAttribute("■一括 削除 ヒエラルキーからObj削除")]
    //public String[] destroyObjs;


    [HeaderAttribute("■一括Obj削除")]
    public string[]
        destroyObjArray;


    [HeaderAttribute("■子オブジェのみ全削除")]
    public string
        childAllDelObjName;

    //他のクリップとミックスできるかどうか
    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }
    //必須っぽいので記述してある（動画から直入力しただけでなにも調査してない）
    public override Playable CreatePlayable(PlayableGraph graph, GameObject obj)
    {
        var behaviour = new RMEventBehaviour();
        return ScriptPlayable<RMEventBehaviour>.Create(graph, behaviour);
    }

    #endregion

    #region ちえり着替え
    [Foldout("ちえり着替え", true)]

    [HeaderAttribute("■着替える")]
    public bool isClothsChg = false;
    [HeaderAttribute("ユーザーが維持設定でも強制で着替える")]
    public bool isForceClothsChg = false;

    public bool
        isBarefoot,
        isTankTop,
        isBikini;


    #endregion

    #region 特殊設定　ライト Frac爆発設置 スマホ画面など　時間など
    [Foldout("特殊設定　ライト Frac爆発設置 スマホ画面など", true)]
    [HeaderAttribute("■ポストプロセス交換用 既存のONOFF ※現在挙動不安定")]
    public DefPostProcessSwitch defPostProcessSwitch = DefPostProcessSwitch.__;
    public enum DefPostProcessSwitch { __, ON, off }

    public DefLightSwitch defLightSwitch = DefLightSwitch.__;
    public enum DefLightSwitch { __, ON, off }

    [HeaderAttribute("■設置したポストプロセスのウェイト量 調整")]
    public String
        postProcessObjName;
    public float
        postProcessWeight = 1,
        postProcessWeightFadetime = 0;


    [HeaderAttribute("■Frac爆発設定　ちえりコリダーが衝突したFracturedObjが爆発")]
    public FracImpact fracImpact = FracImpact.__;
    public enum FracImpact { __, 初期化, 設定 }

    public GameObject
        impactPosObj;
    public float
        impactForce,
        impactRadius;
    public bool
        bAlsoImpactFreeChunks;

    [HeaderAttribute("■ちえりのスマホ画面操作")]
    public ChieriSumahoMonitor chieriSumahoMonitor = ChieriSumahoMonitor.__;
    public enum ChieriSumahoMonitor { __, OFF, Size, FrontCamera }

    [HeaderAttribute("■ちえりの足跡系")]
    public ChieriFootstepObj chieriFootstepObj = ChieriFootstepObj.__;
    public enum ChieriFootstepObj { __, False, True }
    public ChieriFootstepDecal chieriFootstepDecal = ChieriFootstepDecal.__;
    public enum ChieriFootstepDecal { __, False, True }


    [HeaderAttribute("■Fogリアルタイム設定")]
    public FogRealtime fogRealtime = FogRealtime.__;
    public enum FogRealtime { __, True, False }

    public Color
        fogColor;
    [Range(0, 2)]
    public float
        fogDensity;

    [HeaderAttribute("■ちえりブレスController自体をONOFF")]
    public BreathController breathController = BreathController.__;
    public enum BreathController { __, True, False }

    [HeaderAttribute("■ちえりブレスControllerをIKより前に処理")]
    public PreRunBreathController preRunbreathController = PreRunBreathController.__;
    public enum PreRunBreathController { __, True, False }


    [HeaderAttribute("■ちえり口パク")]
    public ChieriKuchipaku chieriKuchipaku = ChieriKuchipaku.__;
    public enum ChieriKuchipaku { __, False, True }

    [HeaderAttribute("■ちえりアニメスピード")]
    public bool isChieriAnimSpeedChange = false;
    [Range(0.0000001f, 1)]
    public float chieriAnimSpeed = 1;
    public float chieriAnimSpeedChgTime = 0;
    public Ease chieriAnimSpeedChgEase;

    [HeaderAttribute("■ちえり足音ボリューム")]
    public bool isChieriAshiotoVolumeChange = false;
    [Range(-80, 20)]
    public float chieriAshiotoVolume = 0;

    [HeaderAttribute("■ちえり足接地揺れ量")]
    public bool isChieriFootYurePowMulChange = false;
    [Range(-0.001f, 10000)]
    public float chieriFootYurePowMul = 1f;


    [HeaderAttribute("■フキダシノベル距離変更")]
    public bool isTextDistChg = false;
    public float textDistance = 0.5f;
    public float textDistChgSpeed = 0;

    [HeaderAttribute("■BGM環境音ボリューム")]
    public BGMSetting bGMSetting = BGMSetting.__;
    public enum BGMSetting { __, 環境音 }
    public float bGMVolume = 1f;
    public float bGMVolumeSpeed = 3f;

    [HeaderAttribute("■ループ系SEループ再生（※現在はマコト足音のみ）")]
    public LoopSE loopSE = LoopSE.__;
    public enum LoopSE { __, マコト足音 }
    public float loopSEIntervalTime = 0.25f;
    public float loopSEPlayTime = Mathf.Infinity;
    public bool stopLoopSE = false;

    [HeaderAttribute("■VRトラッキング オンオフ")]
    public VRTrackingEnable vRTrackingEnable = VRTrackingEnable.__;
    public enum VRTrackingEnable { __, ON, OFF }

    [HeaderAttribute("■カメラ埋まりグレーアウト オンオフ")]
    public VRBlockBlackOutMode cameraGrayOutMode = VRBlockBlackOutMode.__;
    public enum VRBlockBlackOutMode { __, ON, OFF }

    [HeaderAttribute("■ちえり照れ頬")]
    public bool isTerehohoUse = false;
    public float
        terehohoValue = 0,
        terehohoTime = 2;

    [HeaderAttribute("■ちえり胸揺れ オンオフ")]
    public BreastSpring breastSpring = BreastSpring.__;
    public enum BreastSpring { __, ON, OFF }

    [HeaderAttribute("■ちえり足から足音と揺れ強制再生")]
    public bool
        isAnimTriggerRForcePlay = false;
    public bool
        isAnimTriggerLForcePlay = false;

    #endregion


}
