using UnityEngine;
using System.Collections;


//■■■■■■■補助ボーンの回転値を、親ボーンの回転値 プラマイ反転してdivideで割った数値にするスクリプト
[ExecuteInEditMode]//エディタ上で動作する

public class HojoBone : MonoBehaviour
{
    //インスペクター上に手動で登録する
    public GameObject oyaBoneObj, hojoBoneObj;

    //割り算のウェイト量（インスペクターで調整できるようにRange）
    [SerializeField, Range(1f, 5f)]
    public float divideFloat = 2;

    //計算後補助ボーンに代入する用
    Vector3 tempV3;

    //変更が合った時のみに更新する用プレ値群
    Vector3 oyaBoneLclEulPre;
    float divideFloatPre, addFloatPre;

    void Start()
    {
        //ボーンが指定されてない場合はオフ
        if (oyaBoneObj == null || hojoBoneObj == null)
        {
            GetComponent<HojoBone>().enabled = false;
        }

        else
        {
            //現在値をプレ値に読み込み
            oyaBoneLclEulPre = oyaBoneObj.transform.localEulerAngles;
            divideFloatPre = divideFloat;
        }
    }

    void Update()
    {
        //親の値、またはdivideの値が変わったら補助計算メソッド実行
        if (oyaBoneObj.transform.localEulerAngles != oyaBoneLclEulPre
            || divideFloat != divideFloatPre)
        {
            hojoKeisan();

            //実行し続けないようにプレ値現在値更新
            oyaBoneLclEulPre = oyaBoneObj.transform.localEulerAngles;
            divideFloatPre = divideFloat;
        }
    }

    public void hojoKeisan()
    {
        //eulerAnglesは、360に収めて解釈する（-1なら359、361なら1）ので、200（180だとギリギリ過ぎて危うい）以上の場合は-360する。
        //ひとつ200以上の時、他も-360されないようxyz別々にやる

        //Vector3のxyzは配列指定でアクセスできるので、forループで回す。
        for (int i = 0; i < 3; i++)
        {
            if (oyaBoneObj.transform.localEulerAngles[i] >= 200)
            {
                tempV3[i] = (oyaBoneObj.transform.localEulerAngles[i] - 360) * -1;
            }
            else
            {
                tempV3[i] = oyaBoneObj.transform.localEulerAngles[i] * -1;
            }

        }

        //最後に代入
        hojoBoneObj.transform.localEulerAngles
            = new Vector3(tempV3.x / divideFloat, tempV3.y / divideFloat, tempV3.z / divideFloat);
    }
}
