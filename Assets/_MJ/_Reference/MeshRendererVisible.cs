using UnityEngine;
using System.Collections;
[ExecuteInEditMode]//エディタ上で動作する
public class MeshRendererVisible : MonoBehaviour {

    public MeshRenderer meshRenderer;

    public int sortingOrder, sortingLayerID;
    public string sortingLayerName;

    // Use this for initialization
    void Start () {
        meshRenderer = GetComponent<MeshRenderer>();

        sortingOrder = meshRenderer.sortingOrder;
        sortingLayerID = meshRenderer.sortingLayerID;
        sortingLayerName = meshRenderer.sortingLayerName;
    }
	
	// Update is called once per frame
	void Update () {
        meshRenderer.sortingOrder = sortingOrder;
        meshRenderer.sortingLayerID = sortingLayerID;
        meshRenderer.sortingLayerName = sortingLayerName;

        
    }
}
