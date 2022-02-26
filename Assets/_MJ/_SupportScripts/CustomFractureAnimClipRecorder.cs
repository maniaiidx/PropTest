using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public class CustomFractureAnimClipRecorder : MonoBehaviour
{
#if UNITY_EDITOR

    public AnimationClip clip;
    public GameObjectRecorder m_Recorder;

    FracturedObject thisFracturedObject;
    //Generaterから付与される
    [HideInInspector]
    public CustomFractureActiveTriggerCollider customFractureActiveTriggerCollider;

    Animator anim;

    public bool isRecord = false;
    bool isAnimEnd = false;
    bool isAnimPlayOnece = false;

    float time = 0;
    public float recordTime = 3.5f;

    private void Awake()
    {
        m_Recorder = new GameObjectRecorder(gameObject);
        // Bind all the Transforms on the GameObject and all its children.
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true);
    }
    void Start()
    {

        thisFracturedObject = GetComponent<FracturedObject>();
        anim = GetComponent<Animator>();

        //録画状態ならアニメーターオフ
        if (isRecord)
        { anim.enabled = false; }
        else //再生状態ならコンポーネント削除
        {
            anim.enabled = true;
            #region ■Fracture関係コンポーネント削除、メッシュON（レコード状態でない（再生状態）の場合）

            for (int i = 0; i < thisFracturedObject.ListFracturedChunks.Count; i++)
            {
                //削除されてない時のみ
                if (thisFracturedObject.ListFracturedChunks[i])
                {
                    thisFracturedObject.ListFracturedChunks[i].enabled = false;

                    Destroy(thisFracturedObject.ListFracturedChunks[i].gameObject
                        .GetComponent<Collider>());
                    Destroy(thisFracturedObject.ListFracturedChunks[i].gameObject
                        .GetComponent<LocalGravity>());
                    Destroy(thisFracturedObject.ListFracturedChunks[i].thisRigidBody);

                    //メッシュON
                    thisFracturedObject.ListFracturedChunks[i].gameObject
                        .GetComponent<MeshRenderer>().enabled = true;

                    Destroy(thisFracturedObject.ListFracturedChunks[i]);

                    //DieTimerはあったりなかったり多かったりなのでGetComponent"s"で削除
                    for (int k = 0; k < thisFracturedObject.ListFracturedChunks[i].gameObject.GetComponents<UltimateFracturing.DieTimer>().Length; k++)
                    { Destroy(thisFracturedObject.ListFracturedChunks[i].gameObject.GetComponents<UltimateFracturing.DieTimer>()[k]); }

                }
                Destroy(thisFracturedObject.gameObject.GetComponent<CustomFracturedSetLocalGravity>());
                //Destroy(thisFracturedObject.gameObject.GetComponent<CustomFractureActiveTriggerColliderGenerater>());
                //Destroy(thisFracturedObject.gameObject.GetComponent<CustomFractureDetachEvent>());
                //Destroy(thisFracturedObject);
            }
            #endregion
        }

    }

    void LateUpdate()
    {
        if (isRecord)
        {
            //シングルコリダー検知で録画開始
            if (customFractureActiveTriggerCollider.isCrash)
            {
                if(time < recordTime)
                {
                    m_Recorder.TakeSnapshot(Time.deltaTime);
                    time += 1 * Time.deltaTime;
                }
            }
        }
        else
        {
            //シングルコリダー検知で再生開始
            if (customFractureActiveTriggerCollider.isCrash)
            {
                if (isAnimPlayOnece) { }
                else
                {
                    isAnimPlayOnece = true;
                    anim.CrossFadeInFixedTime(clip.name, 0, 0);

                    //シングルコリダーOFF （コリダー自体がオフにすると1フレ見えなくなる（Fracture自体がオフ処理する）のでこっちで）
                    customFractureActiveTriggerCollider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }

            if (isAnimEnd) { }
            else
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                {
                    isAnimEnd = true;
                    foreach (Transform k in transform)
                    {
                        if (k.localScale.x < 0.1f)
                        { Destroy(k.gameObject); }
                    }
                }
            }
        }


    }

    //終了時に録画保存
    void OnDisable()
    {
        if (clip == null)
            return;

        if (isRecord)
        {
            if (m_Recorder.isRecording)
            {
                // Save the recorded session to the clip.
                m_Recorder.SaveToClip(clip);
            }
        }
    }
#endif

}
