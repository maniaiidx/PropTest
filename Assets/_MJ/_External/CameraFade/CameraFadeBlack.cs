using UnityEngine;
using System;

public class CameraFadeBlack : MonoBehaviour
{

    public Material material;

    //すべてのレンダリングが完了したら呼ばれる、ポストプロセスをかけるためのメソッド(カメラがAddされていないと呼ばれない)
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //_materialを使って、ポストプロセスをかける
        Graphics.Blit(src, dest, material);
    }

    private void Awake()
    {
        material = Instantiate(material); 
    }

    //public void testRend()
    //{
    //    material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 0.5f));
    //}

}