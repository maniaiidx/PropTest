using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class SamekuwareAtarihantei : MonoBehaviour
{
    //風呂サメちえりの口に当たったら、別タイムラインに切り替える用スクリプト

    public TimelineAsset kuwareTimelineAsset;

    #region DC取得
    DataCounter DC;
    IEnumerator Start()
    {
        while (GameObject.Find("Server") == null) { yield return new WaitForSeconds(0.5f); }
        DC = GameObject.Find("Server").GetComponent<DataCounter>();

        Debug.Log(DC.nowPlayPD);
        Debug.Log(DC.nowPlayPD.gameObject.name);
        yield break;
    }
    #endregion
    
    IEnumerator OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            //まず念の為DCのPublicなTimelineAssetに当てはめ(必要ないかも)
            DC.KDEventPlayable = kuwareTimelineAsset;

            //タイムラインを取り替えて再生（今のObjそのまま使う）
            //変数にする
            //それに入れて再生
            GameObject nowKDEventTLObj
                = DC.nowPlayPD.gameObject;

            //取得
            PlayableDirector nowKDEventTL
                = nowKDEventTLObj.GetComponent<PlayableDirector>();

            //当てはめ
            nowKDEventTL.playableAsset = kuwareTimelineAsset;

            nowKDEventTL.time = 0;
            //再生
            nowKDEventTL.Play();

           　//念の為このObj削除
            Destroy(this.gameObject);

        }
        yield break;
    }

    #region //IEnumerator OnTriggerExit(Collider collider)
    //{
    //    if (collider.tag == "Player")
    //    {
    //        //もしイベント中なら終了まで待機
    //        while (DC.isTansakuEnter) { yield return null; }

    //        //メソッドにObj送信
    //        DC.TansakuEnter(this.gameObject, DataCounter.RayOrColl.Exit);
    //    }
    //    yield break;
    //}
    #endregion
}
