using UnityEngine;
using System.Collections;

public class skinnedMeshRendererVisible : MonoBehaviour {
    
    public SkinnedMeshRenderer SkinnedMeshRenderer;

    public int SkinnedSortingOrder, SkinnedSortingLayerID;
    public string SkinnedSortingLayerName;

    // Use this for initialization
    void Start () {
        SkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        SkinnedSortingOrder = SkinnedMeshRenderer.sortingOrder;
        SkinnedSortingLayerID = SkinnedMeshRenderer.sortingLayerID;
        SkinnedSortingLayerName = SkinnedMeshRenderer.sortingLayerName;
    }
	
	// Update is called once per frame
	void Update () {

        SkinnedMeshRenderer.sortingOrder = SkinnedSortingOrder;
        SkinnedMeshRenderer.sortingLayerID = SkinnedSortingLayerID;
        SkinnedMeshRenderer.sortingLayerName = SkinnedSortingLayerName;



    }
}
