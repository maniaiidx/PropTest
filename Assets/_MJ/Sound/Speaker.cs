using UnityEngine;
using System.Collections;

public class Speaker : MonoBehaviour
{

    DataCounter DC;
    public AudioSource audioSource;

    public GameObject ChaceObj;


    void Start()
    {
        //DC = GameObject.Find("Server").GetComponent<DataCounter>();
        audioSource = this.GetComponent<AudioSource>();

        ////シーン移動時になくなるので先に破棄
        //DC = null;
    }


    void Update()
    {
        //音鳴ってる時のみ
        if (audioSource.isPlaying)
        {
            //シーン移動してもスピーカー追跡
            if (ChaceObj != null)
            { transform.position = ChaceObj.transform.position; }
            else
            { ChaceObj = GameObject.Find("Speaker"); }
        }
    }
}
