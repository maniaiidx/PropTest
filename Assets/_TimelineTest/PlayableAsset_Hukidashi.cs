using UnityEngine;
using UnityEngine.Playables;

// PlayableTrackサンプル
[System.Serializable]
public class PlayableAsset_Hukidashi : PlayableAsset
{
    //// シーン上のオブジェクトはExposedReference<T>を使用する
    //public ExposedReference<GameObject> sceneObj;
    //// Project内であれば以下の定義でも可
    //public GameObject projectObj;

    public ExposedReference<DataCounter> DC;
    public string serihuKey = "OP0002";

    public override Playable CreatePlayable(PlayableGraph graph, GameObject obj)
    {

        var behaviour = new PlayableBehaviour_Hukidashi();

        //behaviour.sceneObj = sceneObj.Resolve(graph.GetResolver());
        //behaviour.projectObj = projectObj;
        behaviour.DC = GameObject.Find("Server").GetComponent<DataCounter>();
        behaviour.serihuKey = serihuKey;

        return ScriptPlayable<PlayableBehaviour_Hukidashi>.Create(graph, behaviour);
    }
}