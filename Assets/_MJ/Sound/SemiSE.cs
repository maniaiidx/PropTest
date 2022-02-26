using UnityEngine;
using System.Collections;
using DG.Tweening;//DOTween

public class SemiSE : MonoBehaviour
{

    public AudioSource audioSource;

    public Tweener volTweener;//Tweenerをスクリプトが所持してないとシーン移動時にエラーになりそうなので

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        //音量で自動オンオフ
        if (audioSource.volume == 0)
        { if (audioSource.isPlaying) { audioSource.Stop(); } }
        else
        { if (audioSource.isPlaying == false) { audioSource.Play(); } }
    }
}
