using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using System.Collections.Generic;

public class FadeMixer : PlayableBehaviour
{
    public PlayableDirector m_PlayableDirector;
    //public FadeTrack m_FadeTrack;
    public List<TimelineClip> m_clipsList;
    public List<Renderer> m_rendererList;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //ゲームが走っているときのみ
        if (Application.isPlaying)
        {
            var time = m_PlayableDirector.time;
            float fade = 1;
            //クリップ一覧から
            foreach (var clip in m_clipsList)
            {
                //現在タイムが今のクリップ内だったら
                if (time >= clip.start && time <= clip.end)
                {
                    fade = (float)((time - clip.start) / clip.duration);

                    fade = 1.0f - fade;//fade out

                }
                else if (time > clip.end)
                {
                    fade = 0f;//fadeout
                    //if (clip.isFadeIn) { fade = 1.0f; }
                }

                for (int i = 0; i < m_rendererList.Count; i++)
                {
                    //マテリアル複数の場合もあるのでmaterial　ではなく　materials　をforで
                    for (int k = 0; k < m_rendererList[i].materials.Length; k++)
                    {
                        m_rendererList[i].materials[k].color = new Color(1, 1, 1
                        , fade);
                    }
                }
            }

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
