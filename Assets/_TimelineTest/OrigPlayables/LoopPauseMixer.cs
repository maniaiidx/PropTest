using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoopPauseMixer : PlayableBehaviour
{

    //この変数はトラックが作られた際（プレイ時でも？）にトラックから割り当てられる
    public PlayableDirector m_PlayableDirector;
    public LoopPauseTrack m_LoopPauseTrack;
    public List<TimelineClip> m_clipsList;

    //
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //現在ループ待ちクリップ番号より現在のクリップ数が少なかったら何もしない
        if (m_LoopPauseTrack.nowLoopClipInt > m_clipsList.Count-1) { return; }

        var time = m_PlayableDirector.time;
        var nowLoopClipInt = m_LoopPauseTrack.nowLoopClipInt;

        //現在再生位置が、ループ待ちクリップのendより奥だったら
        if (time >= m_clipsList[nowLoopClipInt].end)
        {
            //現在ループ待ちクリップのStart位置に移動
            m_PlayableDirector.time = m_clipsList[nowLoopClipInt].start;
        }



        //強制でループ終わりへ移動bool立ったら　移動してfalse
        if (m_LoopPauseTrack.isForceLoopEnd)
        {
            m_PlayableDirector.time = m_clipsList[nowLoopClipInt].end;
            m_LoopPauseTrack.isForceLoopEnd = false;
        }
    }

    //public override void OnPlayableDestroy(Playable playable)
    //{
    //    m_FirstFrameHappened = false;

    //    if (m_TrackBinding == null)
    //        return;

    //    m_TrackBinding.color = m_DefaultColor;
    //}
}
