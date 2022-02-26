using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class LoopPauseClip : PlayableAsset, ITimelineClipAsset
{

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject obj)
    {
        var behaviour = new LoopPauseBehaviour();        
        return ScriptPlayable<LoopPauseBehaviour>.Create(graph, behaviour);
    }
}
