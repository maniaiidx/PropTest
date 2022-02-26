using UnityEngine;
using System.Collections;
using DG.Tweening;//DOTween
using System.Collections.Generic; //Listに必要

public class ClockSE : MonoBehaviour
{

    public AudioSource audioSource;

    public Tweener volTweener;//Tweenerをスクリプトが所持してないとシーン移動時にエラーになりそうなので
    public List<AudioClip> clipList;

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    public void SEPlay()
    {
        audioSource.clip = clipList[Random.Range(0, clipList.Count)];
        audioSource.Play();
    }

}
