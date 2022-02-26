using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

[TrackColor(0.875f, 0.5944853f, 0.1737132f)]
[TrackClipType(typeof(LoopPauseClip))]

public class LoopPauseTrack : TrackAsset
{
    public int nowLoopClipInt = 0;
    public bool isForceLoopEnd = false;

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject obj, int inputCount)
    {
        var mixer = ScriptPlayable<LoopPauseMixer>.Create(graph, inputCount);

        //ミキサーの変数にDirectorを割り当て
        mixer.GetBehaviour().m_PlayableDirector = obj.GetComponent<PlayableDirector>();
        //ミキサーの変数にトラックを割り当て
        mixer.GetBehaviour().m_LoopPauseTrack = this;
        //ミキサーの変数にトラック（This）からクリップ数読み取って入れる
        mixer.GetBehaviour().m_clipsList = GetClips().ToList();
        return mixer;
    }

    //終了時初期値に戻す命令
//    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
//    {
//#if UNITY_EDITOR
//        Image trackBinding = director.GetGenericBinding(this) as Image;
//        if (trackBinding == null)
//            return;

//        var serializedObject = new UnityEditor.SerializedObject(trackBinding);
//        var iterator = serializedObject.GetIterator();
//        while (iterator.NextVisible(true))
//        {
//            if (iterator.hasVisibleChildren)
//                continue;

//            driver.AddFromName<Image>(trackBinding.gameObject, iterator.propertyPath);
//        }
//#endif
//        base.GatherProperties(director, driver);
//    }
}