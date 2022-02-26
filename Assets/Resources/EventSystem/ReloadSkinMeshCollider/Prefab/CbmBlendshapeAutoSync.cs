using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CbmBlendshapeAutoSync : MonoBehaviour
{
    //BodyのBlendshapeと同期する（Blendshapeの数が同じ前提）

    DataCounter DC;

    SkinnedMeshRenderer
        origSkinnedMeshRenderer,
        thisSkinnedMeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();

        origSkinnedMeshRenderer = DC.Body.GetComponent<SkinnedMeshRenderer>();
        thisSkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i< thisSkinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
        {
            thisSkinnedMeshRenderer.SetBlendShapeWeight(i, origSkinnedMeshRenderer.GetBlendShapeWeight(i));
        }
    }
}
