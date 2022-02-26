using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;//DOTween
using Candlelight;//プロパティいじったときのコールバック機能
using System.Collections.Generic;//List
using Pixeye.Unity;

//Clip用変数を持つのみ
//本来はクリップの範囲内で毎フレーム呼び出しできるが、処理落ちでスキップされることがあるので処理はしない。
//ミキサーで位置を読み取って処理するべき。

#region 一応呼び出しメソッドメモ
//◆開始/終了
//OnGraphStartグラフの再生時に呼ばれる
//OnGraphStopグラフの停止時に呼ばれる

//◆生成/破棄
//OnPlayableCreate生成時のコールバック
//OnPlayableDestroy破棄時のコールバック

//◆実行時
//OnBehaviourPausePlayableの状態がポーズになったら呼ばれる
//OnBehaviourPlayPlayableの状態がPlayになったら呼ばれる

//PrepareFrameProcessFrameより前にPlay状態のNode全体で再帰的に呼ばれる
//ProcessFramePrepareFrameより後にPlay状態のNode全体で再帰的に呼ばれる

#endregion


[Serializable]//必須
public class RMEventBehaviour : PlayableBehaviour
{
    public bool //そのクリップ内にもう入ったかどうか
        isClipEnter = false;


    #region 旧変数（cpに移行前の）
    //[HeaderAttribute("■フラグ読み込み")]
    //public bool
    //    isUseFlagBool;
    //public bool[]
    //    flagBoolArray;

    //[HeaderAttribute("■選択肢")]
    //public string
    //    tmpAKeySentakushi;
    //public string
    //    tmpAValueSentakushi;
    //public bool[]
    //    aSentakuflagBoolArray;
    //public string
    //    tmpBKeySentakushi;
    //public string
    //    tmpBValueSentakushi;
    //public bool[]
    //    bSentakuflagBoolArray;
    #endregion


    [HeaderAttribute("■セリフ")]
    public string
        serihuKey;
    public string
        previewSerihuValue;
    public string
        tmpSerihuKey;
    [TextArea(2, 5)]
    public string
        tmpSerihuValue;

    public bool
        isSerihuKeyWait;

    [HeaderAttribute("■ノベル")]
    public string
        novelKey;
    public string
        previewNovelValue;
    public string
        tmpNovelKey;
    [TextArea(2, 5)]
    public string
        tmpNovelValue;

    public bool
        isNovelAuto,
        isNovelEndWait;


    [HeaderAttribute("■プレファブをGameobjectsに設置")]
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



    [HeaderAttribute("■モーション（ObjName未入力時はちえり）")]
    public string
        motionObjName;
    public string
        motionStateName;
    public float
        motionCrossFadeTime;
    public int
        motionLayer;
    public string
        faceStateName;
    public float
        faceCrossFadeTime;
    public enum IKLookAtPlayer { __, プレイヤー見る, 解除}
    public IKLookAtPlayer iKLookAtPlayer = IKLookAtPlayer.__;



    [HeaderAttribute("■削除ヒエラルキーからObj削除")]
    public string
        destroyObjName;

    [HeaderAttribute("■リスト版Obj削除")]
    public List<string>
        destroyObjList;



    [HeaderAttribute("■ペアレント")]
    public ExposedReference<GameObject>
        ReadParentObj;
    public string
        ParentObjName;
    public ExposedReference<GameObject>
        ReadChildObj;
    public string
        ChildObjName;

    [HeaderAttribute("■リジッドボディのisKinematic")]
    public string
        rigidbodyObjName;
    public bool
        isKinematic = false;

    [HeaderAttribute("■プレイヤー（カメラ）位置ResourcesからPrefab指定")]
    public GameObject
        playerLocalPosObj;

    public enum PlayerStandSit { __,立ち,座り,倒れる}
    [HeaderAttribute("■カメラアンカー立ち座り倒れる")]
    public PlayerStandSit playerStandSitFall = PlayerStandSit.__;

    [HeaderAttribute("■カメラ揺れ")]
    public bool
        isCameraDOShake = false;
    public float
        durationDOShake = 1f,
        strengthDOShake = 0.05f;
    public int
        vibratoDOShake = 20;
    public bool
        fadeOutDOShake = true;

    [HeaderAttribute("■カメラリセット")]
    public bool
        isCameraReset;

    public enum FadeBlack { __, IN, OUT }
    [HeaderAttribute("■黒フェードインアウト")]
    public FadeBlack fadeBlack = FadeBlack.__;
    public float fadeBlackTime = 2;

    public enum FadeWhite { __, IN, OUT }
    [HeaderAttribute("■白フェードインアウト")]
    public FadeWhite fadeWhite = FadeWhite.__;
    public float fadeWhiteTime = 2;

    public enum ChieriPosLock { __, True, False }
    [HeaderAttribute("■ちえりPosLock")]
    public ChieriPosLock chieriPosLock = ChieriPosLock.__;

    [HeaderAttribute("■オブジェ移動 回転 拡縮")]
    public string
        moveObjName;
    public ExposedReference<GameObject>
        readMoveObj;
    public GameObject
        movePosObj;
    public float
        moveTime;
    public Ease 
        moveEase;
    public bool
        isEnableScale;

    [HeaderAttribute("■移動ポイント")]
    public bool
        isSystemOffMovePoint;
    public bool
        isEnterWaitMovePoint;
    public GameObject
        movePointPosObj;
    public bool
        isUseEnterFlagBool;
    public bool[]
        movePointEnterFlagBoolArray;


    [HeaderAttribute("■SE調整（SEのPrefabが設置されていること前提）")]
    public string
        SEObjName;
    public float
        SEVolume;
    public float
        SEFadeTime = 2;




}


#if UNITY_EDITOR
////カスタムエディタ拡張（インスペクターの表示編集）
//[CustomPropertyDrawer(typeof(RMEventBehaviour))]
//public class RMEventBehaviourDrawer : PropertyDrawer
//{
//}

#endif
