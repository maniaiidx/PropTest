using UnityEngine;
using UnityEngine.Playables;

// PlayableTrackサンプル
[System.Serializable]
public class TestPlayableAsset : PlayableAsset
{
    //// シーン上のオブジェクトはExposedReference<T>を使用する
    //public ExposedReference<GameObject> sceneObj;
    //// Project内であれば以下の定義でも可
    //public GameObject projectObj;

    public ExposedReference<DataCounter> DC;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject obj)
    {

        var behaviour = new TestPlayableBehaviour();
        //behaviour.sceneObj = sceneObj.Resolve(graph.GetResolver());
        //behaviour.projectObj = projectObj;
        behaviour.DC = GameObject.Find("Server").GetComponent<DataCounter>();
        return ScriptPlayable<TestPlayableBehaviour>.Create(graph, behaviour);
    }
}