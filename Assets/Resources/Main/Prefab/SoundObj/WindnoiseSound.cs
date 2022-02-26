using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations; // これがないと Constraintは使えない
using DG.Tweening;//DOTween

public class WindnoiseSound : MonoBehaviour
{
    DataCounter DC;
    public AudioSource
        aSource;
    public Vector3
        nowPos,
        prevPos;
    public float
        nowMag,
        prevMag,
        pitchUpRateAdjustFloat = 1,
        pitchDownRateAdjustFloat = 2,
        pitchMax = 0.5f;//前は2だった
    
    float defVolume;

    void Awake()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        aSource = GetComponent<AudioSource>();
        nowPos = transform.position;
        prevPos = nowPos;
    }

    public float PitchP
    {
        //ピッチの上限値設定
        get { return aSource.pitch; }
        set { aSource.pitch = Mathf.Clamp(value, 0, pitchMax); }
    }

    void Update()
    {

        #region WindnoiseSound処理
        //位置が変わってたら更新
        if (nowPos != transform.position)
        { nowPos = transform.position; }
        nowMag = (prevPos - nowPos).magnitude;


        //加速したらピッチ上げる //ためしにanimスピードが低いほど上げ率UP //かつ補正値
        if (nowMag > prevMag && DC.girlAnim != null)
        { PitchP = (nowMag / DC.girlAnim.speed) * pitchUpRateAdjustFloat; }

        //静止したらピッチ段々下げる (静止してて && ピッチが上がってたら)
        else if (nowMag == prevMag && PitchP >= 0)
        {
            PitchP -= pitchDownRateAdjustFloat * Time.fixedUnscaledTime;//ポーズ時止まるようにUnscaled

            ////ピッチ高すぎる場合急激に下げる（1以上なら1フレームで半分）
            //if (PitchP > 1) { PitchP = PitchP / 2; }
        }

        //0.03より下の場合は0
        if (aSource.pitch < 0.03) { aSource.pitch = 0; }

        //更新
        prevPos = nowPos;
        prevMag = nowMag;
        #endregion

    }

}
