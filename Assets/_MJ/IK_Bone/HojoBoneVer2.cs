using UnityEngine;
using System.Collections;


//■■■■■■■補助ボーンの回転値を、親ボーンの回転値 プラマイ反転してdivideで割った数値にするスクリプト
[ExecuteInEditMode]//エディタ上で動作する

public class HojoBoneVer2 : MonoBehaviour
{
    //インスペクター上に手動で登録する
    public Transform
        oya;
    Vector3 //親の回転に変更があった時のみ更新する用プレ値
        preOyaLclEul;

    [System.Serializable]//インスペクター上で登録 構造体
    public struct Hojo
    {
        public Transform
            transform;

        [SerializeField, Range(-1f, 1f)]
        public float //加える回転量（1が100%）
            rate;

        public bool //影響する軸（切っても、クォータニオン計算により多少は影響してしまう）
            isX, isY, isZ;

        [HideInInspector]
        public Vector3 //補助ボーンのもともとの回転値（最後に加算する）
            defV3;

#if UNITY_EDITOR
        [HideInInspector]
        public float //更新し続けないよう ＆ 編集時更新する用にプレ値
            preRate;
        [HideInInspector]
        public Vector3
            preDefV3;
        [HideInInspector]
        public Transform
            preTransform;
#endif
    }

    public Hojo[]
        hojos;

    Vector3 //代入用
        tmpV3;

    ////Transform指定した時に当てはめるのでボタンはいらなそう
    //[Button("SetHojoLclRotDef", "全ての補助ボーン回転デフォルト値設定")]
    //public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない


    void Awake()
    {
        //ボーンが指定されてない場合はオフ
        if (oya == null)
        { GetComponent<HojoBoneVer2>().enabled = false; }

        else//現在値をプレ値に読み込み
        {
            preOyaLclEul = oya.localEulerAngles;

#if UNITY_EDITOR
            for (int i = 0; i < hojos.Length; i++)
            {
                hojos[i].preRate = hojos[i].rate;
                hojos[i].preDefV3 = hojos[i].defV3;
                hojos[i].preTransform = hojos[i].transform;
            }
#endif
        }
    }

    void Update()
    {
        //親の値が変わったら補助計算メソッド実行
        if (oya.localEulerAngles != preOyaLclEul)
        {
            hojoKeisan();
            //実行し続けないようにプレ値現在値更新
            preOyaLclEul = oya.localEulerAngles;
        }

#if UNITY_EDITOR
        //編集用にrate・defV3・Transformをいじってもメソッド実行
        for (int i = 0; i < hojos.Length; i++)
        {
            if (
                hojos[i].preRate != hojos[i].rate ||
                hojos[i].preDefV3 != hojos[i].defV3
                )
            {
                hojoKeisan();
                //実行し続けないようにプレ値現在値更新
                hojos[i].preRate = hojos[i].rate;
                hojos[i].preDefV3 = hojos[i].defV3;
                hojos[i].preTransform = hojos[i].transform;
            }

            //Trs変わった瞬間はDefV3値を当てはめ
            if (hojos[i].preTransform != hojos[i].transform)
            {
                //nullじゃなければ
                if(hojos[i].transform != null)
                { hojos[i].defV3 = hojos[i].transform.localEulerAngles; }

                //実行し続けないようプレTrs更新
                hojos[i].preTransform = hojos[i].transform;
            }
        }
#endif
    }

    public void hojoKeisan()
    {
        //補助ボーンの数だけ
        for (int i = 0; i < hojos.Length; i++)
        {
            //回転軸チェックがついてなければ何もしない
            if (hojos[i].isX == false && hojos[i].isY == false && hojos[i].isZ == false) { goto 次のボーンへ; }
            //nullでも何もしない
            if(hojos[i].transform == null) { goto 次のボーンへ; }


            //rateが正数ならそのまま加算で代入値に
            if (hojos[i].rate > 0)
            {
                tmpV3 =
                    Quaternion.Slerp(Quaternion.identity
                    , oya.localRotation
                    , hojos[i].rate)
                    .eulerAngles;
            }
            //rateが負数なら減算で代入値に
            else if (hojos[i].rate < 0)
            {
                tmpV3 =
                    Quaternion.Slerp(Quaternion.identity
                    , Inverse(oya.localRotation)//反転し
                    , -hojos[i].rate)//rateも反転
                    .eulerAngles;
            }
            //rateが0の場合はデフォルト値にしてスキップ
            if (hojos[i].rate == 0)
            {
                hojos[i].transform.localEulerAngles = hojos[i].defV3;
                goto 次のボーンへ;
            }


            //チェックが付いている回転軸にデフォルト値を追加
            //チェックが付いてない回転軸は変更しない（補助ボーンの値そのままにする）
            if (hojos[i].isX)
            { tmpV3.x = tmpV3.x + hojos[i].defV3.x; }
            else
            { tmpV3.x = hojos[i].transform.localEulerAngles.x; }

            if (hojos[i].isY)
            { tmpV3.y = tmpV3.y + hojos[i].defV3.y; }
            else
            { tmpV3.y = hojos[i].transform.localEulerAngles.y; }

            if (hojos[i].isZ)
            { tmpV3.z = tmpV3.z + hojos[i].defV3.z; }
            else
            { tmpV3.z = hojos[i].transform.localEulerAngles.z; }

            //補助ボーンに適用
            hojos[i].transform.localEulerAngles = tmpV3;

            次のボーンへ:;
        }

    }

    void SetHojoLclRotDef()
    {
        for (int i = 0; i < hojos.Length; i++)
        {
            hojos[i].defV3 = hojos[i].transform.localEulerAngles;
        }
    }

    public static Quaternion Inverse(Quaternion q)
    {
        return new Quaternion(-q.x, -q.y, -q.z, q.w);
    }

}
