using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;


public class AnimClipRecorder : MonoBehaviour
{
    public AnimationClip clip;

    GameObjectRecorder m_Recorder;

    bool isRecord = false;

    public string now = "stop";

    [SerializeField, Space(10)]
    [Button(nameof(RecordStart), "●録画")]
    public bool dummy1;//属性でボタンを作るので何かしら宣言が必要。使わない

    [SerializeField, Space(20)]
    [Button(nameof(RecordStart_IsKinematicOff), "●録画＋IsKinematicオフ")]
    public bool dummy2;//属性でボタンを作るので何かしら宣言が必要。使わない
    public Vector3 addForceValue;

    [SerializeField, Space(20)]
    [Button(nameof(RecordStop), "■録画停止")]
    public bool dummy3;//属性でボタンを作るので何かしら宣言が必要。使わない

    //録画時停止する用
    Animator anim;
    //物理演算録画する用（IsKinematicを同時にオフする用）
    Rigidbody rb;


    private void Awake()
    {
        //Gameobject取得
        m_Recorder = new GameObjectRecorder(gameObject);
        // Bind all the Transforms on the GameObject and all its children.
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true);

        //録画時停止する用
        if (GetComponent<Animator>() != null)
        {
            anim = GetComponent<Animator>();
        }
        //物理演算録画する用（IsKinematicを同時にオフする用）
        if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }
    void Start()
    {
    }

    void LateUpdate()
    {
        if (isRecord)
        {
            //録画(フレーム書き込み)
            m_Recorder.TakeSnapshot(Time.deltaTime);
        }
    }

    void RecordStart()
    {
        now = "Recording";
        isRecord = true;

        //アニメーターオフ
        anim.enabled = false;
    }

    void RecordStart_IsKinematicOff()
    {
        rb.isKinematic = false;
        rb.AddForce(addForceValue, ForceMode.Impulse);
        RecordStart();
    }

    void RecordStop()
    {
        now = "Stop";
        if (m_Recorder.currentTime > 0)
        { now = now + "_ExistData"; }

        if (m_Recorder.isRecording)
        {
            if (m_Recorder.currentTime > 0) { }

            // Save the recorded session to the clip.
            m_Recorder.SaveToClip(clip);

            //リセット（そのままだと追記されてしまうので）
            m_Recorder = new GameObjectRecorder(gameObject);
            // Bind all the Transforms on the GameObject and all its children.
            m_Recorder.BindComponentsOfType<Transform>(gameObject, true);

            rb.isKinematic = true;
        }
        isRecord = false;
    }


}

#endif