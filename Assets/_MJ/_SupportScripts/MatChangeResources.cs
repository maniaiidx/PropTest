using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MatChangeResources : MonoBehaviour
{
    [HeaderAttribute("切り替えるマテリアル保持用スクリプト（スパッツや咥内など）")]
    [FormerlySerializedAs("defaultMat")]
    public Material
        defaultMat;
    [FormerlySerializedAs("hiDetailMat")]
    public Material
        otherMat;

    [HeaderAttribute("List版")]
    public List<Material>
        defaultMatList = new List<Material>();
    public List<Material>
        otherMatList = new List<Material>();

}
