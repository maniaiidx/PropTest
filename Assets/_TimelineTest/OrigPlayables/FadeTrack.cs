using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;
using System.Collections.Generic;


[TrackColor(0.875f, 0.5944853f, 0.1737132f)]
[TrackClipType(typeof(FadeClip))]
[TrackBindingType(typeof(GameObject))]

public class FadeTrack : TrackAsset
{

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject obj, int inputCount)
    {
        var mixer = ScriptPlayable<FadeMixer>.Create(graph, inputCount);
        mixer.GetBehaviour().m_PlayableDirector = obj.GetComponent<PlayableDirector>();
        //mixer.GetBehaviour().m_FadeTrack = this;
        mixer.GetBehaviour().m_clipsList = GetClips().ToList();

        #region renderer取得
        if (obj.GetComponent<PlayableDirector>().GetGenericBinding(this) != null)
        {

            List<Renderer> tmpRendererList = new List<Renderer>();

            List<GameObject> tmpObjList =
                GetAllChildren.GetAll(obj.GetComponent<PlayableDirector>().GetGenericBinding(this) as GameObject);
            foreach (GameObject tmpObj in tmpObjList)
            {
                if (tmpObj.GetComponent<Renderer>() != null)
                {
                    tmpRendererList.Add(tmpObj.GetComponent<Renderer>());
                }
            }

            mixer.GetBehaviour().m_rendererList = tmpRendererList;
        }
        #endregion

        return mixer;
    }

    //    //終了時初期値に戻す命令
    //    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    //    {
    //#if UNITY_EDITOR
    //        GameObject trackBinding = director.GetGenericBinding(this) as GameObject;
    //        if (trackBinding == null)
    //            return;

    //        #region renderer取得
    //        List<Renderer> tmpRendererList = new List<Renderer>();

    //        List<GameObject> tmpObjList =
    //            GetAllChildren.GetAll(director.GetGenericBinding(this) as GameObject);
    //        foreach (GameObject tmpObj in tmpObjList)
    //        {
    //            if (tmpObj.GetComponent<Renderer>() != null)
    //            {
    //                tmpRendererList.Add(tmpObj.GetComponent<Renderer>());
    //            }
    //        }

    //        #endregion

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