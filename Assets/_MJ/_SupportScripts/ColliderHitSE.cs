using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHitSE : MonoBehaviour
{

    DataCounter DC;
    void Awake()
    { if (GameObject.Find("Server") != null) { DC = GameObject.Find("Server").GetComponent<DataCounter>(); } }

    public AudioClip
        audioClip;
    [Range(0, 1)]
    public float
        volume = 1f;

    //bool //怪獣と当たったとき
    //    isHitKaijuSEPlayed = false;

    float //音が出る強さ境目
        velocityMagnitudeBorder = 4;

    float //一定以上ベロシティ上がった時、少なくなった時に音出す用に(様子見るためにpublic表示)
        nowVelMag = 0;
    bool
        isMaxOverVelMag = false,
        isDownVelMag = false;


    void Update()
    {
        //まず計測
        if (isMaxOverVelMag == false)
        {
            //VelMag小さければ
            if (nowVelMag < 4)
            {
                //計測
                nowVelMag = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            }
            //↑越えたら音出してbool(何かにぶつかって勢いよく移動開始したとして)
            else
            {
                //DC.SEPlay(DC.Other3DSEObj, audioClip, gameObject, volume);

                isMaxOverVelMag = true;
                nowVelMag = 0;
            }
        }
        //VelMag小さくなったら(何かにぶつかって勢いよく移動停止したとして)
        else if (isMaxOverVelMag
            && 3f > gameObject.GetComponent<Rigidbody>().velocity.magnitude)
        {
            //ベロシティ弱くなったら 
            DC.SEPlay(DC.Other3DSEObj, audioClip, gameObject, volume);

            //また計測へ
            isMaxOverVelMag = false;
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    //一度だけ 怪獣とぶつかった時のみ大きめ音
    //    if (isHitKaijuSEPlayed == false && collision.collider.tag == "kaiju")
    //    {
    //        DC.SEPlay(DC.KashibakoSEObj, DC.kashibakoSEStrList[0], gameObject, volume + 0.1f);

    //        isHitKaijuSEPlayed = true;
    //    }
    //}
}
