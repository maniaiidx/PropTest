//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic; // for List<>
//using UnityEngine.UI;//for Text
//using System.Text.RegularExpressions;

//public class WakuHukidasi : MonoBehaviour
//{
//    DataCounter DC;

//    //生成された自身のフキダシとシッポを取得
//    public Transform WakuHukidashiObjectsTrs, WakuHukiTrs, Shippo_UTrs, Shippo_RTrs, Shippo_DTrs, Shippo_LTrs,
//        HukiTextTrs;
//    public Text hukiText;
//    public Vector3 mouthPos;
//    public Vector3 sippoUDefPos, sippoDDefPos, sippoLDefPos, sippoRDefPos;

//    public MeshFilter wakuHukiMeshFilter, shippo_UMeshFilter, shippo_RMeshFilter, shippo_DMeshFilter, shippo_LMeshFilter;
//    public Vector3
//        wakuOut01Pos, wakuOut02Pos, wakuOut03Pos, wakuOut04Pos, wakuOut05Pos,//上辺
//        wakuOut06Pos, wakuOut07Pos, wakuOut08Pos, wakuOut09Pos,              //右辺
//        wakuOut10Pos, wakuOut11Pos, wakuOut12Pos, wakuOut13Pos,              //下辺
//        wakuOut14Pos, wakuOut15Pos, wakuOut16Pos,                            //左辺

//        wakuIn01Pos, wakuIn02Pos, wakuIn03Pos,                               //上辺
//        wakuIn04Pos, wakuIn05Pos,                                            //右辺
//        wakuIn06Pos, wakuIn07Pos,                                            //下辺
//        wakuIn08Pos,                                                         //左辺

//        shippoUPos, shippoUPosB, shippoRPos, shippoRPosB, shippoDPos, shippoDPosB, shippoLPos, shippoLPosB;

//    public Vector3[] wakuHukiMeshs, shippo_UMeshs, shippo_RMeshs, shippo_DMeshs, shippo_LMeshs;

//    //選択肢
//    public bool hukidashiBool;
//    public int sentakuListNumInt;
//    public float sentakuCountFloat, sentakuPrevCountFloat, sentakuNoCountFloat, sentakuPrevNoCountFloat;
//    public Transform SentakutyuuSliderTrs, SentakuNoSliderTrs;
//    public Slider sentakutyuuSlider, sentakuNoSlider;

//    //テスト空間
//    public string testString01;

//    void Start()
//    {
//        #region//基本群
//        DC = GameObject.Find("Server").GetComponent<DataCounter>();

//        HukiTextTrs = transform.Find("WakuHuki/Canvas/HukiText").transform;
//        hukiText = HukiTextTrs.GetComponent<Text>();

//        WakuHukidashiObjectsTrs = this.transform;
//        //ゼロ位置ワンスケールに
//        WakuHukidashiObjectsTrs.localPosition =
//        WakuHukidashiObjectsTrs.localEulerAngles = Vector3.zero;
//        WakuHukidashiObjectsTrs.localScale = Vector3.one;

//        WakuHukiTrs = transform.Find("WakuHuki").transform;
//        Shippo_UTrs = transform.Find("Shippo_U").transform;
//        Shippo_RTrs = transform.Find("Shippo_R").transform;
//        Shippo_DTrs = transform.Find("Shippo_D").transform;
//        Shippo_LTrs = transform.Find("Shippo_L").transform;

//        wakuHukiMeshFilter = WakuHukiTrs.GetComponent<MeshFilter>();
//        shippo_UMeshFilter = Shippo_UTrs.GetComponent<MeshFilter>();
//        shippo_RMeshFilter = Shippo_RTrs.GetComponent<MeshFilter>();
//        shippo_DMeshFilter = Shippo_DTrs.GetComponent<MeshFilter>();
//        shippo_LMeshFilter = Shippo_LTrs.GetComponent<MeshFilter>();

//        wakuHukiMeshs = wakuHukiMeshFilter.mesh.vertices;
//        shippo_UMeshs = shippo_UMeshFilter.mesh.vertices;
//        shippo_RMeshs = shippo_RMeshFilter.mesh.vertices;
//        shippo_DMeshs = shippo_DMeshFilter.mesh.vertices;
//        shippo_LMeshs = shippo_LMeshFilter.mesh.vertices;
//        #endregion

//        #region//wakuHukiとSippoのメッシュ座標取得
//        //本体
//        wakuIn03Pos = wakuHukiMeshs[0];
//        wakuOut05Pos = wakuHukiMeshs[1];
//        wakuOut06Pos = wakuHukiMeshs[2];
//        wakuOut04Pos = wakuHukiMeshs[3];
//        wakuIn02Pos = wakuHukiMeshs[4];
//        wakuOut03Pos = wakuHukiMeshs[5];
//        wakuOut07Pos = wakuHukiMeshs[6];
//        wakuIn04Pos = wakuHukiMeshs[7];
//        wakuOut02Pos = wakuHukiMeshs[8];
//        wakuIn01Pos = wakuHukiMeshs[9];
//        wakuOut01Pos = wakuHukiMeshs[10];
//        wakuOut16Pos = wakuHukiMeshs[11];
//        wakuOut15Pos = wakuHukiMeshs[12];
//        wakuIn08Pos = wakuHukiMeshs[13];
//        //中心       = wakuHukiMeshs[14];
//        wakuIn05Pos = wakuHukiMeshs[15];
//        wakuIn06Pos = wakuHukiMeshs[16];
//        wakuOut10Pos = wakuHukiMeshs[17];
//        wakuOut09Pos = wakuHukiMeshs[18];
//        wakuOut08Pos = wakuHukiMeshs[19];
//        wakuOut11Pos = wakuHukiMeshs[20];
//        wakuOut12Pos = wakuHukiMeshs[21];
//        wakuIn07Pos = wakuHukiMeshs[22];
//        wakuOut13Pos = wakuHukiMeshs[23];
//        wakuOut14Pos = wakuHukiMeshs[24];

//        //シッポの分断された謎のメッシュ非表示（0位置に運ぶ）
//        shippo_UMeshs[0].y = 0;
//        shippo_UMeshs[5].y = 0;
//        shippo_RMeshs[0].x = 0;
//        shippo_RMeshs[4].x = 0;
//        shippo_DMeshs[0].y = 0;
//        shippo_DMeshs[4].y = 0;
//        shippo_LMeshs[0].x = 0;
//        shippo_LMeshs[4].x = 0;

//        //シッポ
//        shippoUPos = sippoUDefPos = shippo_UMeshs[6];
//        shippoUPosB = shippo_UMeshs[8];
//        shippoRPos = sippoRDefPos = shippo_RMeshs[7];
//        shippoRPosB = shippo_RMeshs[6];
//        shippoDPos = sippoDDefPos = shippo_DMeshs[7];
//        shippoDPosB = shippo_DMeshs[6];
//        shippoLPos = sippoLDefPos = shippo_LMeshs[7];
//        shippoLPosB = shippo_LMeshs[6];
//        #endregion

//        //レイヤーを変更
//        WakuHukiTrs.GetComponent<MeshRenderer>().sortingLayerName = "hukidashi";

//        //選択肢
//        SentakutyuuSliderTrs = transform.Find("WakuHuki/Canvas/SentakutyuuSlider").transform;
//        sentakutyuuSlider = SentakutyuuSliderTrs.GetComponent<Slider>();
//        SentakuNoSliderTrs = transform.Find("WakuHuki/Canvas/SentakuNoSlider").transform;
//        sentakuNoSlider = SentakuNoSliderTrs.GetComponent<Slider>();
//    }


//    void Update()
//    {
//        #region//テスト用 座標代入後 RecalculateBounds更新（メッシュのレンダリング再計算）
//        /*
//              wakuHukiMeshs[0]  = wakuIn03Pos  ;
//              wakuHukiMeshs[1]  = wakuOut05Pos ; 
//              wakuHukiMeshs[2]  = wakuOut06Pos ;
//              wakuHukiMeshs[3]  = wakuOut04Pos ;
//              wakuHukiMeshs[4]  = wakuIn02Pos  ;
//              wakuHukiMeshs[5]  = wakuOut03Pos ;
//              wakuHukiMeshs[6]  = wakuOut07Pos ;
//              wakuHukiMeshs[7]  = wakuIn04Pos  ;
//              wakuHukiMeshs[8]  = wakuOut02Pos ;
//              wakuHukiMeshs[9]  = wakuIn01Pos  ;
//              wakuHukiMeshs[10] = wakuOut01Pos ;
//              wakuHukiMeshs[11] = wakuOut16Pos ;
//              wakuHukiMeshs[12] = wakuOut15Pos ;
//              wakuHukiMeshs[13] = wakuIn08Pos  ;
//            //wakuHukiMeshs[14] = //中心       ;
//              wakuHukiMeshs[15] = wakuIn05Pos  ;
//              wakuHukiMeshs[16] = wakuIn06Pos  ;
//              wakuHukiMeshs[17] = wakuOut10Pos ;
//              wakuHukiMeshs[18] = wakuOut09Pos ;
//              wakuHukiMeshs[19] = wakuOut08Pos ;
//              wakuHukiMeshs[20] = wakuOut11Pos ;
//              wakuHukiMeshs[21] = wakuOut12Pos ;
//              wakuHukiMeshs[22] = wakuIn07Pos  ;
//              wakuHukiMeshs[23] = wakuOut13Pos ;
//              wakuHukiMeshs[24] = wakuOut14Pos ;

//              shippo_UMeshs[6]  = shippoUPos  ;
//              shippo_UMeshs[8]  = shippoUPosB ;
//              shippo_RMeshs[7]  = shippoRPos  ;
//              shippo_RMeshs[6]  = shippoRPosB ;
//              shippo_DMeshs[7]  = shippoDPos  ;
//              shippo_DMeshs[6]  = shippoDPosB ;
//              shippo_LMeshs[7]  = shippoLPos  ;
//              shippo_LMeshs[6]  = shippoLPosB ;
        
//              wakuHukiMeshFilter.mesh.vertices = wakuHukiMeshs;
//              wakuHukiMeshFilter.mesh.RecalculateBounds();
//              shippo_UMeshFilter.mesh.vertices = shippo_UMeshs;
//              shippo_UMeshFilter.mesh.RecalculateBounds();
//              shippo_RMeshFilter.mesh.vertices = shippo_RMeshs;
//              shippo_RMeshFilter.mesh.RecalculateBounds();
//              shippo_DMeshFilter.mesh.vertices = shippo_DMeshs;
//              shippo_DMeshFilter.mesh.RecalculateBounds();
//              shippo_LMeshFilter.mesh.vertices = shippo_LMeshs;
//              shippo_LMeshFilter.mesh.RecalculateBounds();
//       */
//        #endregion

//        if (hukidashiBool == true)
//        {
//            Hukidashi();
//        }
//        if (hukidashiBool == false)
//        {
//            Sentakushi();
//        }
//    }



//    public void HukiSetOn(float time, string serihuKey, int ListNum)
//    {
//        //選択肢かフキダシかのフラグ
//        hukidashiBool = true;

//        //選択肢スライダー消す
//        Destroy(SentakutyuuSliderTrs.gameObject);
//        Destroy(SentakuNoSliderTrs.gameObject);

//        //セリフの詳細を受け取りまずフキダシの方へ
//        hukiText.text = DC.serihuDict[serihuKey];

//        //■■■文字内容に合わせてフキダシの大きさを変える
//        //リッチテキストのタグを取り除いて大きさ計算するためにもう一個受け取る
//        string tempString = DC.serihuDict[serihuKey];
//        tempString = new Regex("<.*?>", RegexOptions.Singleline).Replace(tempString,"");

//        //各行をList化することで行数と各行の文字数を取り出す
//        string[] tempStringArray;
//        tempStringArray = tempString.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

//        //↑で取った各行の文字数を読み取り、一番大きいのが横幅になるようにする
//        int tempMojisuuInt = 0;
//        foreach (string e in tempStringArray)
//        {
//            if (tempMojisuuInt < e.Length)
//            {
//                tempMojisuuInt = e.Length;
//            }
//        }

//        float yokoHaba = tempMojisuuInt * 0.05f;
//        float tateHaba = tempStringArray.Length * 0.06f;

//        #region//フキダシ大きさ設定(メッシュ座標を移動させる)
//        //上辺
//        wakuOut01Pos.y = wakuOut02Pos.y = wakuOut03Pos.y = wakuOut04Pos.y = wakuOut05Pos.y
//            += tateHaba;
//        wakuIn01Pos.y = wakuIn02Pos.y = wakuIn03Pos.y
//            += tateHaba;

//        //下辺
//        wakuOut09Pos.y = wakuOut10Pos.y = wakuOut11Pos.y = wakuOut12Pos.y = wakuOut13Pos.y
//            -= tateHaba;
//        wakuIn05Pos.y = wakuIn06Pos.y = wakuIn07Pos.y
//            -= tateHaba;

//        //右辺
//        wakuOut05Pos.x = wakuOut06Pos.x = wakuOut07Pos.x = wakuOut08Pos.x = wakuOut09Pos.x
//            += yokoHaba;
//        wakuIn03Pos.x = wakuIn04Pos.x = wakuIn05Pos.x
//            += yokoHaba;

//        //左辺
//        wakuOut13Pos.x = wakuOut14Pos.x = wakuOut15Pos.x = wakuOut16Pos.x = wakuOut01Pos.x
//            -= yokoHaba;
//        wakuIn07Pos.x = wakuIn08Pos.x = wakuIn01Pos.x
//            -= yokoHaba;
//        #endregion
//        #region//メッシュに代入
//        wakuHukiMeshs[0] = wakuIn03Pos;
//        wakuHukiMeshs[1] = wakuOut05Pos;
//        wakuHukiMeshs[2] = wakuOut06Pos;
//        wakuHukiMeshs[3] = wakuOut04Pos;
//        wakuHukiMeshs[4] = wakuIn02Pos;
//        wakuHukiMeshs[5] = wakuOut03Pos;
//        wakuHukiMeshs[6] = wakuOut07Pos;
//        wakuHukiMeshs[7] = wakuIn04Pos;
//        wakuHukiMeshs[8] = wakuOut02Pos;
//        wakuHukiMeshs[9] = wakuIn01Pos;
//        wakuHukiMeshs[10] = wakuOut01Pos;
//        wakuHukiMeshs[11] = wakuOut16Pos;
//        wakuHukiMeshs[12] = wakuOut15Pos;
//        wakuHukiMeshs[13] = wakuIn08Pos;
//        //wakuHukiMeshs[14] = //中心       ;
//        wakuHukiMeshs[15] = wakuIn05Pos;
//        wakuHukiMeshs[16] = wakuIn06Pos;
//        wakuHukiMeshs[17] = wakuOut10Pos;
//        wakuHukiMeshs[18] = wakuOut09Pos;
//        wakuHukiMeshs[19] = wakuOut08Pos;
//        wakuHukiMeshs[20] = wakuOut11Pos;
//        wakuHukiMeshs[21] = wakuOut12Pos;
//        wakuHukiMeshs[22] = wakuIn07Pos;
//        wakuHukiMeshs[23] = wakuOut13Pos;
//        wakuHukiMeshs[24] = wakuOut14Pos;
//        #endregion

//        //メッシュ再計算
//        wakuHukiMeshFilter.mesh.vertices = wakuHukiMeshs;
//        wakuHukiMeshFilter.mesh.RecalculateBounds();

//        //■フキダシサイズ決定したのでシッポ位置を設定
//        //子オブジェクト化
//        Shippo_UTrs.parent = Shippo_RTrs.parent = Shippo_DTrs.parent = Shippo_LTrs.parent
//            = WakuHukiTrs;
//        //位置移動
//        Shippo_UTrs.localPosition = wakuOut03Pos;
//        Shippo_RTrs.localPosition = wakuOut07Pos;
//        Shippo_DTrs.localPosition = wakuOut11Pos;
//        Shippo_LTrs.localPosition = wakuOut15Pos;
//        //回転0化
//        Shippo_UTrs.localRotation = Shippo_RTrs.localRotation = Shippo_DTrs.localRotation = Shippo_LTrs.localRotation
//            = Quaternion.Euler(0, 0, 0);

//        //テキスト合わせる
//        RectTransform textRect = HukiTextTrs.GetComponent<RectTransform>();
//        textRect.sizeDelta = new Vector2(tempMojisuuInt * 100, tempStringArray.Length * 120);

//        //文字数に合わせて口パクアニメーション
//        DC.girlAnim.CrossFade("口パク", 0.3f, 3);
//        //コルーチンで文字数分ウェイトして口パクやめメソッド実行
//        StartCoroutine(kutipakuOff(DC.serihuDict[serihuKey].Length * 0.13f));

//        //フキダシ消滅命令　//コルーチンで文字数分ウェイトしてメソッド実行
//        StartCoroutine(HukiSetOff(DC.serihuDict[serihuKey].Length * 0.2f + 3, ListNum));

//    }

//    public IEnumerator kutipakuOff(float delay)//口パクオフ
//    {
//        yield return new WaitForSeconds(delay);//引数分待ってから
//        DC.girlAnim.CrossFade("_noData", 0.3f, 3);

//    }

//    public IEnumerator HukiSetOff(float delay, int ListNum)//削除
//    {
//        yield return new WaitForSeconds(delay);//引数分待ってから
//        WakuHukidashiObjectsTrs.localScale = new Vector3(0, 0, 0);

//        //フキダシPointを口に戻すためBoolをFlase
//        DC.hukiPointBools[ListNum] = false;

//        Destroy(this.gameObject);
//    }

//    public void Hukidashi()
//    {

//        #region//口にシッポを伸ばす

//        //まず口の位置取得
//        mouthPos = DC.MouthTargetTrs.position;

//        //リアルタイムに尻尾頂点のデフォルト座標を取る（この後で判定とって伸ばす場所を変えるため）
//        Vector3 cSippoUDefPos = WakuHukidashiObjectsTrs.position + sippoUDefPos;
//        Vector3 cSippoDDefPos = WakuHukidashiObjectsTrs.position + sippoDDefPos;
//        Vector3 cSippoLDefPos = WakuHukidashiObjectsTrs.position + sippoLDefPos;
//        Vector3 cSippoRDefPos = WakuHukidashiObjectsTrs.position + sippoRDefPos;

//        //カメラから見てデフォルトより高い位置にあれば上尻尾が作動
//        if (DC.VRCamera.WorldToViewportPoint(cSippoUDefPos).y <
//            DC.VRCamera.WorldToViewportPoint(DC.MouthTargetTrs.position).y)
//        {
//            //shippoUPos = WakuHukidashiObjects.transform.InverseTransformPoint(mouthPos) / 10 * 3;//尻尾の座標をフキダシObjのローカル座標に変換して代入
//            shippoUPos = Vector3.MoveTowards(sippoUDefPos, WakuHukidashiObjectsTrs.InverseTransformPoint(mouthPos), 0.3f);
//            shippoUPosB = new Vector3(0, 0, 0);
//        }
//        else//でなければ形デフォルトに戻す
//        {
//            if (shippoUPos != sippoDDefPos)
//            {
//                shippoUPos =
//                    shippoUPosB = new Vector3(0, 0, 0);
//            }
//        }

//        //デフォルトより低い位置にあれば下尻尾が作動
//        if (DC.VRCamera.WorldToViewportPoint(cSippoDDefPos).y >
//            DC.VRCamera.WorldToViewportPoint(DC.MouthTargetTrs.position).y)
//        {
//            //shippoDPos = WakuHukidashiObjects.transform.InverseTransformPoint(mouthPos) / 10 * 3;//尻尾の座標をフキダシObjのローカル座標に変換
//            shippoDPos = Vector3.MoveTowards(sippoDDefPos, WakuHukidashiObjectsTrs.InverseTransformPoint(mouthPos), 0.3f);
//            shippoDPosB = new Vector3(0, 0, 0);
//        }
//        else//でなければ形デフォルトに戻す
//        {
//            if (shippoDPos != sippoDDefPos)
//            {
//                shippoDPos =
//                    shippoDPosB = new Vector3(0, 0, 0);
//            }
//        }

//        if (//デフォルトより左位置にあり、上下尻尾の圏外なら左尻尾が作動
//            DC.VRCamera.WorldToViewportPoint(cSippoLDefPos).x > DC.VRCamera.WorldToViewportPoint(DC.MouthTargetTrs.position).x &&
//            DC.VRCamera.WorldToViewportPoint(cSippoUDefPos).y > DC.VRCamera.WorldToViewportPoint(DC.MouthTargetTrs.position).y &&
//            DC.VRCamera.WorldToViewportPoint(cSippoDDefPos).y < DC.VRCamera.WorldToViewportPoint(DC.MouthTargetTrs.position).y
//           )
//        {
//            //shippoLPos = WakuHukidashiObjects.transform.InverseTransformPoint(mouthPos) / 10 * 5;//尻尾の座標をフキダシObjのローカル座標に変換
//            shippoLPos = Vector3.MoveTowards(sippoLDefPos, WakuHukidashiObjectsTrs.InverseTransformPoint(mouthPos), 0.3f);
//            shippoLPosB = new Vector3(0, 0, 0);
//        }
//        else//でなければ形デフォルトに戻す
//        {
//            if (shippoLPos != sippoDDefPos)
//            {
//                shippoLPos =
//                    shippoLPosB = new Vector3(0, 0, 0);
//            }
//        }


//        if (//デフォルトより右位置にあり、上下尻尾の圏外なら右尻尾が作動
//            DC.VRCamera.WorldToViewportPoint(cSippoRDefPos).x < DC.VRCamera.WorldToViewportPoint(DC.MouthTargetTrs.position).x &&
//            DC.VRCamera.WorldToViewportPoint(cSippoUDefPos).y > DC.VRCamera.WorldToViewportPoint(DC.MouthTargetTrs.position).y &&
//            DC.VRCamera.WorldToViewportPoint(cSippoDDefPos).y < DC.VRCamera.WorldToViewportPoint(DC.MouthTargetTrs.position).y
//           )
//        {
//            //shippoRPos = WakuHukidashiObjects.transform.InverseTransformPoint(mouthPos) / 10 * 5;//尻尾の座標をフキダシObjのローカル座標に変換
//            shippoRPos = Vector3.MoveTowards(sippoRDefPos, WakuHukidashiObjectsTrs.InverseTransformPoint(mouthPos), 0.3f);
//            shippoRPosB = new Vector3(0, 0, 0);
//        }
//        else//でなければ形デフォルトに戻す
//        {
//            if (shippoRPos != sippoDDefPos)
//            {
//                shippoRPos =
//                    shippoRPosB = new Vector3(0, 0, 0);
//            }
//        }

//        //メッシュコンポーネントのプロパティboundsを再計算する（レンダリング実行みたいなことと思う）
//        shippo_UMeshs[6] = shippoUPos;
//        shippo_UMeshs[8] = shippoUPosB;
//        shippo_RMeshs[7] = shippoRPos;
//        shippo_RMeshs[6] = shippoRPosB;
//        shippo_DMeshs[7] = shippoDPos;
//        shippo_DMeshs[6] = shippoDPosB;
//        shippo_LMeshs[7] = shippoLPos;
//        shippo_LMeshs[6] = shippoLPosB;
//        shippo_UMeshFilter.mesh.vertices = shippo_UMeshs;
//        shippo_UMeshFilter.mesh.RecalculateBounds();
//        shippo_RMeshFilter.mesh.vertices = shippo_RMeshs;
//        shippo_RMeshFilter.mesh.RecalculateBounds();
//        shippo_DMeshFilter.mesh.vertices = shippo_DMeshs;
//        shippo_DMeshFilter.mesh.RecalculateBounds();
//        shippo_LMeshFilter.mesh.vertices = shippo_LMeshs;
//        shippo_LMeshFilter.mesh.RecalculateBounds();
//        #endregion
//    }


//    //選択肢
//    public void SentakuSetOn(float time, string serihuKey, int ListNum)
//    {
//        //List（sentakuSyss）の番号で、何番選択肢なのか取得（最後にTimeLineに渡す）
//        sentakuListNumInt = ListNum;
//        //テクスチャ変更
//        WakuHukiTrs.GetComponent<Renderer>().material
//            = Instantiate(Resources.Load("EventSystem/Hukidashi/Materials/sentakushiCanvasMat") as Material);
//        //Rayにヒットさせるためタグ変更
//        WakuHukiTrs.tag = "sentakushi";
//        //Sippo削除
//        Destroy(Shippo_UTrs.gameObject);
//        Destroy(Shippo_DTrs.gameObject);
//        Destroy(Shippo_LTrs.gameObject);
//        Destroy(Shippo_RTrs.gameObject);

//        //セリフの詳細を受け取り
//        hukiText.text = DC.serihuDict[serihuKey];

//        //■■■文字内容に合わせて選択肢の大きさを変える
//        //リッチテキストのタグを取り除いて大きさ計算するためにもう一個受け取る
//        string tempString = DC.serihuDict[serihuKey];
//        tempString = new Regex("<.*?>", RegexOptions.Singleline).Replace(tempString, "");

//        //各行をList化することで行数と各行の文字数を取り出せる
//        string[] tempStringArray;
//        tempStringArray = tempString.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

//        //各行の文字数を読み取り、一番大きいのが横幅になるようにする
//        int yoko = 0;
//        foreach (string e in tempStringArray)
//        {
//            if (yoko < e.Length)
//            {
//                yoko = e.Length;
//            }
//        }


//        float yokoHaba = yoko * 0.05f;
//        float tateHaba = tempStringArray.Length * 0.06f;
//        #region//選択肢大きさ設定(メッシュ座標を移動させる)
//        //上辺
//        wakuOut01Pos.y = wakuOut02Pos.y = wakuOut03Pos.y = wakuOut04Pos.y = wakuOut05Pos.y
//            += tateHaba;
//        wakuIn01Pos.y = wakuIn02Pos.y = wakuIn03Pos.y
//            += tateHaba;

//        //下辺
//        wakuOut09Pos.y = wakuOut10Pos.y = wakuOut11Pos.y = wakuOut12Pos.y = wakuOut13Pos.y
//            -= tateHaba;
//        wakuIn05Pos.y = wakuIn06Pos.y = wakuIn07Pos.y
//            -= tateHaba;

//        //右辺
//        wakuOut05Pos.x = wakuOut06Pos.x = wakuOut07Pos.x = wakuOut08Pos.x = wakuOut09Pos.x
//            += yokoHaba;
//        wakuIn03Pos.x = wakuIn04Pos.x = wakuIn05Pos.x
//            += yokoHaba;

//        //左辺
//        wakuOut13Pos.x = wakuOut14Pos.x = wakuOut15Pos.x = wakuOut16Pos.x = wakuOut01Pos.x
//            -= yokoHaba;
//        wakuIn07Pos.x = wakuIn08Pos.x = wakuIn01Pos.x
//            -= yokoHaba;
//        #endregion
//        #region//メッシュに代入
//        wakuHukiMeshs[0] = wakuIn03Pos;
//        wakuHukiMeshs[1] = wakuOut05Pos;
//        wakuHukiMeshs[2] = wakuOut06Pos;
//        wakuHukiMeshs[3] = wakuOut04Pos;
//        wakuHukiMeshs[4] = wakuIn02Pos;
//        wakuHukiMeshs[5] = wakuOut03Pos;
//        wakuHukiMeshs[6] = wakuOut07Pos;
//        wakuHukiMeshs[7] = wakuIn04Pos;
//        wakuHukiMeshs[8] = wakuOut02Pos;
//        wakuHukiMeshs[9] = wakuIn01Pos;
//        wakuHukiMeshs[10] = wakuOut01Pos;
//        wakuHukiMeshs[11] = wakuOut16Pos;
//        wakuHukiMeshs[12] = wakuOut15Pos;
//        wakuHukiMeshs[13] = wakuIn08Pos;
//        //wakuHukiMeshs[14] = //中心       ;
//        wakuHukiMeshs[15] = wakuIn05Pos;
//        wakuHukiMeshs[16] = wakuIn06Pos;
//        wakuHukiMeshs[17] = wakuOut10Pos;
//        wakuHukiMeshs[18] = wakuOut09Pos;
//        wakuHukiMeshs[19] = wakuOut08Pos;
//        wakuHukiMeshs[20] = wakuOut11Pos;
//        wakuHukiMeshs[21] = wakuOut12Pos;
//        wakuHukiMeshs[22] = wakuIn07Pos;
//        wakuHukiMeshs[23] = wakuOut13Pos;
//        wakuHukiMeshs[24] = wakuOut14Pos;
//        #endregion

//        //メッシュ再計算
//        wakuHukiMeshFilter.mesh.vertices = wakuHukiMeshs;
//        wakuHukiMeshFilter.mesh.RecalculateBounds();

//        //テキスト合わせる
//        RectTransform textRect = HukiTextTrs.GetComponent<RectTransform>();
//        textRect.sizeDelta = new Vector2(yoko * 100, tempStringArray.Length * 120);

//        //スライダー合わせる (非選択のも)
//        RectTransform SliderSentakutyuuRect = SentakutyuuSliderTrs.GetComponent<RectTransform>();
//        //文字数に合わせての拡大に、基本のメッシュの大きさがプラスされる
//        SliderSentakutyuuRect.sizeDelta = new Vector2(yoko * 0.1f + 0.05f, tempStringArray.Length * 0.240f + 0.05f);
//        RectTransform SliderSentakuNoRect = SentakuNoSliderTrs.GetComponent<RectTransform>();
//        //文字数に合わせての拡大に、基本のメッシュの大きさがプラスされる
//        SliderSentakuNoRect.sizeDelta = new Vector2(yoko * 0.1f + 0.05f, tempStringArray.Length * 0.240f + 0.05f);


//        //フキダシ消滅命令　//コルーチンで文字数分ウェイトしてメソッド実行
//        StartCoroutine(HukiSetOff(DC.serihuDict[serihuKey].Length * 99.2f + 3, ListNum));

//        switch (sentakuListNumInt)//選択肢リストナンバーで位置チェンジ）
//        {
//            case 0:
//                DC.SentakushiPointPos = DC.SentakushiPointA.transform.position;
//                break;
//            case 1:
//                DC.SentakushiPointPos = DC.SentakushiPointB.transform.position;
//                break;
//            case 2:
//                DC.SentakushiPointPos = DC.SentakushiPointC.transform.position;
//                break;
//            case 3:
//                DC.SentakushiPointPos = DC.SentakushiPointD.transform.position;
//                break;
//        }

//        //↑の位置を代入
//        WakuHukidashiObjectsTrs.position = DC.SentakushiPointPos;//point　と　points　の表記間違えないように注意

//        //選択肢置く　//コルーチンで文字数分ウェイトしてメソッド実行
//        StartCoroutine(SentakushiOku(0.3f));

//    }//選択肢属性与えて大きさ設定後配置

//    public void Sentakushi()
//    {
//        //選択肢に視点送ってなければ
//        if (DC.HitWakuHukidasi == null)
//        {
//            SentakuNo();//選択肢選ばないメソッド実行（カウントスタート）
//        }

//    }

//    public IEnumerator SentakushiOku(float delay)//選択肢追従止める コリダー追加
//    {
//        yield return new WaitForSeconds(delay);//引数分待ってから
//        WakuHukidashiObjectsTrs.parent = null;
//        WakuHukiTrs.gameObject.AddComponent<BoxCollider>();
//    }

//    public void Sentakutyuu()//見つめたらカウント後TimeLineに送信
//    {
//        //カウンター
//        sentakuPrevCountFloat = sentakuCountFloat;//1フレーム実行用にPrevCount
//        sentakuCountFloat += 1 * Time.deltaTime;
//        //選択ゲージ
//        sentakutyuuSlider.value = sentakuCountFloat;

//        //選択中XBOXコンのAで即決定
//        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
//        {
//            sentakuCountFloat = 2;
//        }

//        //送信とポインター外し
//        if (sentakuPrevCountFloat < 2 && sentakuCountFloat >= 2)//カウントしきったら
//        {
//            //選択した番号をTimeLineに送信
//            DC.sentakuListNum = sentakuListNumInt;

//            #region//ログ用に一時保持したListの、選択した選択肢テキスト文頭に「□」追加（ログ上で色変え判定のために）
//            DC.sentakushiTempLogList[sentakuListNumInt] = DC.sentakushiTempLogList[sentakuListNumInt].Insert(0, "□");

//            //一時保持した選択肢Listのテキストを全部ログに書き込む
//            for (int i = 0; i < DC.sentakushiTempLogList.Count; i++)
//            {
//                DC.TalkLogStringListAdd(DC.sentakushiTempLogList[i]);
//            }
//            //一時保持Listクリア
//            DC.sentakushiTempLogList.Clear();
//            #endregion

//            //コルーチンで1フレームウェイトして選択終了メソッド実行
//            StartCoroutine(SentakuSyuuryou(0));
//        }


//    }

//    public void SentakuNo()
//    {
//        sentakuPrevNoCountFloat = sentakuNoCountFloat;
//        sentakuNoCountFloat += 1 * Time.deltaTime;
//        sentakuNoSlider.value = sentakuNoCountFloat;

//        //送信して終了メソッド
//        if (sentakuPrevNoCountFloat < 10 && sentakuNoCountFloat >= 10)//カウントしきったら
//        {
//            //選択しなかったという意味づけの番号（-1）をTimeLineに送信
//            DC.sentakuListNum = -1;

//            #region//一時保持した選択肢Listのテキストを全部ログに書き込む
//            for (int i = 0; i < DC.sentakushiTempLogList.Count; i++)
//            {
//                DC.TalkLogStringListAdd(DC.sentakushiTempLogList[i]);
//            }
//            //一時保持Listクリア
//            DC.sentakushiTempLogList.Clear();
//            #endregion

//            //コルーチンで1フレームウェイトして選択終了メソッド実行
//            StartCoroutine(SentakuSyuuryou(0));

//        }
//    }

//    public IEnumerator SentakuSyuuryou(float delay)
//    {
//        yield return new WaitForSeconds(delay);//引数分待ってから

//        //ポインタ外し
//        DC.SeePointTrs.position = Vector3.zero;

//        //消す (foreachでsentakushis[]の中身（GameObject）をxとし繰り返す) //なぜかforでできなかった
//        foreach (GameObject x in DC.sentakushis)
//        {
//            Destroy(x);
//        }
//    }
//}

