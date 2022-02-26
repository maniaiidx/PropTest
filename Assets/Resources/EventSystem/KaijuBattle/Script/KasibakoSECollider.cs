using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KasibakoSECollider : MonoBehaviour
{

    public DataCounter DC;
    void Awake()
    { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }

    float //音量
        volume = 0.1f;

    bool //怪獣と当たったとき
        isHitKaijuSEPlayed = false;


    float //一定以上ベロシティ上がった時、少なくなった時に音出す用に
        maxVelMag = 0;
    bool
        isMaxOverVelMag = false,
        isDownVelMag = false;


    void Update()
    {
        //まず計測
        if (isMaxOverVelMag == false)
        {
            //VelMag小さければ
            if (maxVelMag < 4)
            {
                //計測
                maxVelMag = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            }
            //↑越えたら音出してbool
            else
            {
                DC.SEPlay(DC.KashibakoSEObj, DC.kashibakoSEStrList[0], gameObject, volume);
                //リストトップをお尻に入れ替え
                string tmpTopStr = DC.kashibakoSEStrList[0];
                DC.kashibakoSEStrList.Remove(tmpTopStr);
                DC.kashibakoSEStrList.Add(tmpTopStr);

                isMaxOverVelMag = true;
                maxVelMag = 0;
            }
        }
        //今度は 小さくなったら音　また計測へ
        else if (isMaxOverVelMag
            && 3f > gameObject.GetComponent<Rigidbody>().velocity.magnitude)
        {
            //ベロシティ弱くなったら 
            DC.SEPlay(DC.KashibakoSEObj, DC.kashibakoSEStrList[0], gameObject, volume);

            //リストトップをお尻に入れ替え
            string tmpTopStr = DC.kashibakoSEStrList[0];
            DC.kashibakoSEStrList.Remove(tmpTopStr);
            DC.kashibakoSEStrList.Add(tmpTopStr);

            isMaxOverVelMag = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //一度だけ 怪獣とぶつかった時のみ大きめ音
        if (isHitKaijuSEPlayed == false && collision.collider.tag == "kaiju")
        {
            DC.SEPlay(DC.KashibakoSEObj, DC.kashibakoSEStrList[0], gameObject, volume + 0.1f);

            //リストトップをお尻に入れ替え
            string tmpTopStr = DC.kashibakoSEStrList[0];
            DC.kashibakoSEStrList.Remove(tmpTopStr);
            DC.kashibakoSEStrList.Add(tmpTopStr);

            isHitKaijuSEPlayed = true;
        }
    }
}
