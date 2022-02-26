using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class FadeClip : PlayableAsset, ITimelineClipAsset
{
    //public bool isFadeIn = false;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject obj)
    {
        var behaviour = new FadeBehaviour();        
        return ScriptPlayable<FadeBehaviour>.Create(graph, behaviour);
    }
}
