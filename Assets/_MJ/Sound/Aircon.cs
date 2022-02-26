using UnityEngine;
using System.Collections;
using DG.Tweening;//DOTween

public class Aircon : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip bgmAircon;

    public Tweener volTweener;//Tweenerをスクリプトが所持してないとシーン移動時にエラーになりそうなので

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        bgmAircon = Resources.Load("Main/BGM/ambient_room_tone_01_lp") as AudioClip;
        audioSource.clip = bgmAircon;
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
