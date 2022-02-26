#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class OutlineMeshBuilder : MonoBehaviour
{

    [Button("Build", "Build")]
    public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない

    public void Build()
    {
        GameObject TansakuOutLineMeshObj
            = Instantiate(gameObject, transform, false);
        TansakuOutLineMeshObj.transform.localEulerAngles = Vector3.zero;
        TansakuOutLineMeshObj.transform.localPosition = Vector3.zero;
        TansakuOutLineMeshObj.transform.localScale = Vector3.one;
        TansakuOutLineMeshObj.name = "TansakuOutlineMesh";
        DestroyImmediate(TansakuOutLineMeshObj.GetComponent<OutlineMeshBuilder>());

        TansakuOutLineMeshObj.GetComponent<Renderer>().material
            = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/EventSystem/Tansaku/Mat/TansakuOutLine1.mat");

        for (int i = 0; i < TansakuOutLineMeshObj.transform.childCount; i++)
        {
            DestroyImmediate(TansakuOutLineMeshObj.transform.GetChild(i).gameObject);
        }


        Debug.Log(TansakuOutLineMeshObj.transform.childCount);
        DestroyImmediate(this);

    }

}
#endif
