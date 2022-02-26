using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

//足跡をPrefabに保存できるようにするもの

public class PrefabBuilder : MonoBehaviour
{

#if UNITY_EDITOR
    [Button(nameof(PrefabBuild), "Prefab保存")] public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない

    void PrefabBuild()
    {
        #region ファイル名用に時間取得
        var tmpTimeStr = System.DateTime.Now.Year.ToString("D4");
        tmpTimeStr += System.DateTime.Now.Month.ToString("D2");
        tmpTimeStr += System.DateTime.Now.Day.ToString("D2");
        tmpTimeStr += System.DateTime.Now.Hour.ToString("D2");
        tmpTimeStr += System.DateTime.Now.Minute.ToString("D2");
        tmpTimeStr += System.DateTime.Now.Second.ToString("D2");
        #endregion
        //保存するパス 基本ファイル名を設定
        string path = "Assets/" + gameObject.name + tmpTimeStr;

        //現在テクスチャを持つマテリアル取得
        var footPrintMat = transform.Find("FootPrint/FootPrint").GetComponent<Renderer>().material;

        #region ■Pngテクスチャアセット作成

        //テクスチャを取得
        Texture2D tex = (Texture2D)footPrintMat.GetTexture("_MainTex");

        // PNG 画像を保存
        File.WriteAllBytes(path + "Tex.png", tex.EncodeToPNG());
        // アセットとしてインポート（データベース更新的な感じ？）
        AssetDatabase.ImportAsset(path + "Tex.png");

        #endregion
        //マテリアルに↑のテクスチャアセットを関連付ける
        footPrintMat.SetTexture("_MainTex"
            , AssetDatabase.LoadAssetAtPath(path + "Tex.png", typeof(Texture2D)) as Texture2D);

        //■マテリアルアセット作成
        AssetDatabase.CreateAsset(footPrintMat, path + "Mat.mat");
        //このObjに↑のマテリアルアセットを関連付ける
        footPrintMat =
            AssetDatabase.LoadAssetAtPath<Material>(path + "Mat.mat");

        //■プレファブ作成
        PrefabUtility.SaveAsPrefabAsset(gameObject, path + ".prefab");

    }
#endif

}
