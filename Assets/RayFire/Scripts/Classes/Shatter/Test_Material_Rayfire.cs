using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Presets;
using RayFire;
using Sirenix.OdinInspector;

public class Test_Material_Rayfire : MonoBehaviour
{
    public List<Material[]> materialsList;
    public List<Material> tmp0;
    public List<Material> tmp1;
    public List<Material> tmp2;

    [ContextMenu("Execute")]
    public void Execute()
    {

        for (int i = 0; i < materialsList.Count; i++)
        {
            for (int k = 0; k < materialsList[i].Length; k++)
            {
                if (k >= 3) return;
                if (k == 0)
                {
                    tmp0.Add(materialsList[i][k]);
                }
                if (k == 1)
                {
                    tmp1.Add(materialsList[i][k]);
                }
                if (k == 2)
                {
                    tmp2.Add(materialsList[i][k]);
                }

            }
        }
        
        //Debug.Log("materials.Length is "+ materialsList.Length);
        //Debug.Log(materialsList[0]);
    }

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        materialsList = new List<Material[]>();
        tmp0 = new List<Material>();
        tmp1 = new List<Material>();
        tmp2 = new List<Material>();
    }
    public void getMaterials(Material[] mats)
    {
        materialsList.Add(mats);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
