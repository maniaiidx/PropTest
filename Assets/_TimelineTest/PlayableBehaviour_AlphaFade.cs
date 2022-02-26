using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;//DOTween

// PlayableTrackサンプル
public class PlayableBehaviour_AlphaFade : PlayableBehaviour
{

    public PlayableOrigSetting_AlphaFade setting;

    // タイムライン開始時 実行
    public override void OnGraphStart(Playable playable)
    {
        //ゲームが走っているときのみ
        if (Application.isPlaying)
        {
            for (int i = 0; i < setting.renderers.Count; i++)
            {
                setting.renderers[i].material.color = new Color(1, 1, 1, 1);
            }
        }
    }

    // PlayableTrack再生時実行
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //ゲームが走っているときのみ
        if (Application.isPlaying)
        {
            for (int i = 0; i < setting.renderers.Count; i++)
            {
                setting.renderers[i].material.DOColor(new Color(1, 1, 1, 0), setting.fadeTime);
            }
        }
    }
}