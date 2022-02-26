//using追加も自由です（もし追加したら、後で教えてほしいかも）
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//クラスは↓の通りにしてください。
public partial class DataBridging
{
    public int
       //着替えメニューで選択されてる衣装
       //【髪型】0:通常
       intCurrentHead,
       //【服装】0:通常（制服）,1:タンクトップ
       intCurrentCloth,
       //【靴】0:通常（靴下）,1:素足
       intCurrentShoes,
       //【全身】0:なし,1:ビキニ
       intCurrentALL;
}