using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class PostProcess : MonoBehaviour
{
    [SerializeField]
    PostProcessResources postProcessResource;       //	PostProcessリソース

    // Use this for initialization
    void Start()
    {
        PostProcessLayer layer = gameObject.AddComponent<PostProcessLayer>();

        layer.volumeLayer = LayerMask.GetMask("PostProcessing");        //	PostProcessのレイヤーを設定
        layer.volumeTrigger = gameObject.transform;                 //	PostProcessをつけるカメラオブジェクト

        //	PostProcessレイヤーの初期設定
        layer.Init(postProcessResource);
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			//	PostProcessingStackの有効/無効切り替え
			PostProcessLayer layer = gameObject.GetComponent<PostProcessLayer>();
			layer.enabled = layer.enabled ? false : true;
		}
    }
}
