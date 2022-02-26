using UnityEngine;
using UnityEngine.Playables;

// PlayableTrackサンプル
public class TestPlayableBehaviour : PlayableBehaviour
{
    //public GameObject sceneObj;
    //public GameObject projectObj;

    //GameObject obj;


    public DataCounter DC;

    // タイムライン開始時実行
    public override void OnGraphStart(Playable playable)
    {
        Debug.Log("タイムライン開始時実行");
    }
    // タイムライン停止時実行
    public override void OnGraphStop(Playable playable)
    {
        Debug.Log("タイムライン停止時実行");
    }
    // PlayableTrack再生時実行
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log("PlayableTrack再生時実行");
        //if (Application.isPlaying && obj == null)
        //{
        //    obj = Object.Instantiate(projectObj) as GameObject;
        //}
    }
    // PlayableTrack停止時実行
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        Debug.Log("PlayableTrack停止時実行");
        //if (obj != null)
        //{
        //    Object.Destroy(obj);
        //}
    }
    // PlayableTrack再生時毎フレーム実行
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        Debug.Log("PlayableTrack再生時毎フレーム実行");
        //if (sceneObj != null)
        //{
        //    sceneObj.transform.Translate(new Vector3(0.1f, 0));
        //}
    }
}