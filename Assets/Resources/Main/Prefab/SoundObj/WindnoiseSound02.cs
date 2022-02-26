using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;//DOTween

public class WindnoiseSound02 : MonoBehaviour
{
    DataCounter DC;
    AudioSource
        aSource;
    Vector3
        nowPos,
        prevPos;
    float
        nowMag,
        prevMag;

    Tweener
        pitchTweener;

    void Awake()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        aSource = GetComponent<AudioSource>();
        nowPos = transform.position;
        prevPos = nowPos;

        pitchTweener = DOTween.To(() => aSource.pitch, (x) => aSource.pitch = x,
                nowMag, 0.0001f)
                .SetAutoKill(false)
                .SetEase(Ease.OutQuint);
    }

    public float PitchP
    {
        get { return aSource.pitch; }
        set { aSource.pitch = Mathf.Clamp(value, 0, 2); }
    }

    void Update()
    {
        if (nowPos != transform.position)
        { nowPos = transform.position; }
        nowMag = (prevPos - nowPos).magnitude;


        pitchTweener
            .ChangeEndValue(nowMag)
            .Restart();

        Debug.Log(nowMag);



        ////加速したらピッチ上げる
        //if (nowMag > prevMag)
        //{ PitchP = nowMag / DC.girlAnim.speed; }//ためしにスピードが低いほど上げ率UP
        ////静止したらピッチ段々下げる
        //else if(nowMag == prevMag && PitchP >= 0)
        //{
        //    PitchP -= 2f * Time.deltaTime;
        //    ////ピッチ高すぎる場合急激に下げる（1以上なら1フレームで半分）
        //    //if (PitchP > 1) { PitchP = PitchP / 2; } 
        //}

        if (aSource.pitch < 0) { aSource.pitch = 0; }

        prevPos = nowPos;
        prevMag = nowMag;
    }
}
