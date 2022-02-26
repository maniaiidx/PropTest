using UnityEngine;
using UnityEngine.Playables;

// PlayableTrackサンプル
public class PlayableBehaviour_Hukidashi : PlayableBehaviour
{
    //public GameObject sceneObj;
    //public GameObject projectObj;

    //GameObject obj;


    public DataCounter DC;
    public string serihuKey;

    // PlayableTrack再生時実行
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (Application.isPlaying)
        {
            DC.Hukidashi(serihuKey);
        }
    }
}