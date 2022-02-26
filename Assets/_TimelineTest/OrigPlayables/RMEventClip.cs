using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Pixeye.Unity;//FoldOut
using Candlelight;//プロパティいじったときのコールバック機能
using DG.Tweening;//DOTween
using UnityEngine.Rendering.PostProcessing;//PostProcessing Stack v2をスクリプトでいじるために必要

[Serializable]
public class RMEventClip : PlayableAsset, ITimelineClipAsset
{

    //Clipでも変数持てるが、メンバー変数はBehaviourのみにする（ちゃんとクリップごとに表示されるし編集できる）
    //必ずPublic [SerializedField] privateは不可　（あと、そうしないとRecordボタンも表示されない）
    public RMEventBehaviour m_behaviour =
        new RMEventBehaviour();



    //しかし↑だとインスペクターのカスタムができないので、諦めてここでも変数を持つ（移植中）
    [Foldout("フラグ関連", true)]
    public bool
        isUseFlagBool;

    public bool[]
        flagBoolArray;

    [HeaderAttribute("セーブデータのフラグが立っていればこのクリップ実行")]
    public bool
        isUseStaticFlagBool;
    [HeaderAttribute("セーブデータのフラグ名")]
    public string
        staticFlagKey;

    [Foldout("タイムライン切替・イベント移動(終了)", true)]
    public GameObject
    UnityTimelineObj;
    public bool
        isEventMove;
    [Header("移動先イベント名（未入力時は今まで通りTimeline終了（スクリプトで後処理必要））")]
    public String
        MoveEventName;
    [Header("移動時シーン初期化（強制黒フェード）")]
    public bool
        isForceSceneLoad;


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


    [Foldout("宿題システム（工事中）", true)]

    public HWSystemONOFF hWSystem = HWSystemONOFF.__;
    public enum HWSystemONOFF { __, ON, OFF }



    [Foldout("プレイヤー カメラ", true)]
    [Header("プレイヤー移動 拡縮 回転")]
    public GameObject
        cameraObjectsTrsPosObj;
    public float
        cameraObjectsTrsTime;
    public Ease
        cameraObjectsTrsEase;

    [Header("目眩演出")]
    public bool
        isUseMemai;







    [Foldout("IK関連", true)]

    public UseIKEf useIKEf = UseIKEf.__;
    public enum UseIKEf { __, RHandEf, LHandEf, RFootEf, LFootEf }
    [Range(0, 1)]
    public float
        IKWeight;
    [Range(0, Mathf.Infinity)]
    public float
        IKTime;
    public Ease IKEase;
    [HeaderAttribute("全てのIKTargetを現在ポーズの位置へ移動")]
    public bool isGirlPosToIKTargetPosRot;




    [Foldout("モーション関連", true)]
    [HeaderAttribute("■モーション 一括指定再生")]
    public MotionStructArray[] motions = new MotionStructArray[0];
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
    }


    [HeaderAttribute("■モーションでウェイト（指定モーション完了までTL待機）")]
    public string
        waitMotionObjName;
    public string
        waitMotionClipName;
    public float
        waitMotionNormlizedTime = 1;






    [Foldout("オブジェクト設置移動削除 関係", true)]

    [HeaderAttribute("■一括 プレファブをGameobjectsに設置")]
    public SpawnObjStructArray[] spawnObjs = new SpawnObjStructArray[0];
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
        public float
            time;
        public Ease
            ease;
        //public bool //今後は標準で影響するようにするために一旦コメントアウト
        //    isEnableScale;
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
        //public bool //今後は標準で影響するようにするために一旦コメントアウト
        //    isEnableScale;
    }

    //[HeaderAttribute("■一括 削除 ヒエラルキーからObj削除")]
    //public String[] destroyObjs;


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





    [Foldout("特殊設定　ライト Frac爆発設置 スマホ画面など", true)]
    [HeaderAttribute("■ポストプロセス交換 デフォルトライトのオンオフ")]
    public DefPostProcessSwitch defPostProcessSwitch = DefPostProcessSwitch.__;
    public enum DefPostProcessSwitch { __, ON, off }

    public DefLightSwitch defLightSwitch = DefLightSwitch.__;
    public enum DefLightSwitch { __, ON, off }

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
    [Range(0, 2)] public float
        fogDensity;




}
