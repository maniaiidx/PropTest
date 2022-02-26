using UnityEngine;
using UnityEngine.Playables;

// PlayableTrackサンプル
public class PlayableBehaviour_ChieriMotion : PlayableBehaviour
{
    //public GameObject sceneObj;
    //public GameObject projectObj;

    //GameObject obj;


    public DataCounter DC;
    public string stateName;
    public float durationTime;
    public int layerInt;

    // PlayableTrack再生時実行
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (Application.isPlaying)
        {
            DC.ChieriMotion(stateName, durationTime, layerInt);
        }
    }
}