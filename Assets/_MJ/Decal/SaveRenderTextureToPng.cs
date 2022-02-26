using UnityEngine;
using System.Collections;
using System.IO;

public class SaveRenderTextureToPng : MonoBehaviour
{
    #region テスト中にセッティングしたもの。今はいらない
    public RenderTexture RenderTextureRef;
    public Renderer setRenderer;

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    savePng();
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    DecalControll();
        //}
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    StartCoroutine(DecalSetDel());
        //}
    }
    
    void savePng()
    {
        #region RenderTextureをInspectorでアセット指定の時のもの（これだと複数個目からだめっぽい）
        Texture2D tex = new Texture2D(RenderTextureRef.width, RenderTextureRef.height, TextureFormat.RGB24, false);
        RenderTexture.active = RenderTextureRef;
        tex.ReadPixels(new Rect(0, 0, RenderTextureRef.width, RenderTextureRef.height), 0, 0);
        tex.Apply();

        setRenderer.material.SetTexture("_MainTex", tex);

        //// Encode texture into PNG
        //byte[] bytes = tex.EncodeToPNG();
        //Object.Destroy(tex);

        ////Write to a file in the project folder
        //File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
        #endregion

    }
    #endregion

    public IEnumerator DecalSetDel()
    {
        //RenderTexture複製で指定
        RenderTextureRef = Instantiate(Resources.Load("EventSystem/ChieriFootStep/ZimenSatueiRenderTexture") as RenderTexture);
        //それをカメラに当てはめ
        GetComponent<Camera>().targetTexture = RenderTextureRef;

        //撮影に1フレ
        yield return null;

        //テクスチャ生成
        Texture2D tex = new Texture2D(RenderTextureRef.width, RenderTextureRef.height, TextureFormat.RGB24, false);
        RenderTexture.active = RenderTextureRef;
        tex.ReadPixels(new Rect(0, 0, RenderTextureRef.width, RenderTextureRef.height), 0, 0);
        tex.Apply();

        //Textureに当てはめ
        transform.parent.Find("FootPrint/FootPrint").GetComponent<Renderer>().material.SetTexture("_MainTex", tex);

        //表示
        transform.parent.Find("FootPrint").gameObject.SetActive(true);

        //レイヤー変更前に1フレ（撮影と同時にしたら真っ黒になってしまった）
        yield return null;

        //消し
        Destroy(gameObject);

        yield break;
    }

}