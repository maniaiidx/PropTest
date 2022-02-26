using UnityEngine;
using UnityEngine.Playables;

// PlayableTrackサンプル
[System.Serializable]
public class PlayableAsset_AlphaFade : PlayableAsset
{
    //// シーン上のオブジェクトはExposedReference<T>を使用する
    //public ExposedReference<GameObject> sceneObj;
    //// Project内であれば以下の定義でも可
    //public GameObject projectObj;

    public PlayableOrigSetting_AlphaFade setting;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject obj)
    {

        var behaviour = new PlayableBehaviour_AlphaFade();

        //behaviour.sceneObj = sceneObj.Resolve(graph.GetResolver());
        //behaviour.projectObj = projectObj;
        behaviour.setting = obj.GetComponent<PlayableOrigSetting_AlphaFade>();

        return ScriptPlayable<PlayableBehaviour_AlphaFade>.Create(graph, behaviour);
    }
}