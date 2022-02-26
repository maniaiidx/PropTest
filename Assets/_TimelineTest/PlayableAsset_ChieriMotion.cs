using UnityEngine;
using UnityEngine.Playables;

// PlayableTrackサンプル
[System.Serializable]
public class PlayableAsset_ChieriMotion : PlayableAsset
{
    //// シーン上のオブジェクトはExposedReference<T>を使用する
    //public ExposedReference<GameObject> sceneObj;
    //// Project内であれば以下の定義でも可
    //public GameObject projectObj;

    public ExposedReference<DataCounter> DC;
    public string stateName = "_noData";
    public float durationTime = 0;
    public int layerInt = 0;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject obj)
    {

        var behaviour = new PlayableBehaviour_ChieriMotion();

        //behaviour.sceneObj = sceneObj.Resolve(graph.GetResolver());
        //behaviour.projectObj = projectObj;
        behaviour.DC = GameObject.Find("Server").GetComponent<DataCounter>();
        behaviour.stateName = stateName;
        behaviour.durationTime = durationTime;
        behaviour.layerInt = layerInt;

        return ScriptPlayable<PlayableBehaviour_ChieriMotion>.Create(graph, behaviour);
    }
}