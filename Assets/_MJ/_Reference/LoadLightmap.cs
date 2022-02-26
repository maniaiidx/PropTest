using UnityEngine;
using System.Collections;

public class LoadLightmap : MonoBehaviour
{
    [SerializeField]
    Texture2D[] dir, light, shadowMask;
    [SerializeField]
    LightProbes lightprobes;
    [SerializeField]
    Cubemap cubemap;

    // Use this for initialization
    void Start()
    {

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LightmapData[] datas = new LightmapData[light.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                LightmapData data = new LightmapData();
                data.lightmapDir = dir[i];
                data.lightmapColor = light[i];
                data.shadowMask = shadowMask[i];
                datas[i] = data;
            }
            LightmapSettings.lightmaps = datas;
            LightmapSettings.lightProbes = lightprobes;
            RenderSettings.customReflection = cubemap;
        }
    }


}