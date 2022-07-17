using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Presets;
using RayFire;

public class Tmp_AddRayfireScript : MonoBehaviour
{
    public Preset PresetSource;

    public string searchTagName = "Tmp_building";

    public bool isOverride = false;

    [SerializeField, Space(20)]

    [Button(nameof(addRayfireRigid), "Add RayfireRigid Component")]
    public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない
    void addRayfireRigid()
    {
        //Preset preset = new Preset(PresetSource);
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(searchTagName);
        for (int i = 0; i < gameObjects.Length; i++)
        {
            RayfireRigid RFrigid = gameObjects[i].transform.GetComponent<RayfireRigid>();
            if (RFrigid == null) //RayfireRigidが付与されてなければ以下を実行
            {
                RFrigid = gameObjects[i].AddComponent<RayfireRigid>();
                PresetSource.ApplyTo(RFrigid) ;
            }
            else
            {
                Debug.Log("RayfireRigid is exist.");
                if (isOverride)
                {
                    Debug.Log("上書きしました");
                    PresetSource.ApplyTo(RFrigid);
                }
            }
        }
    }

    //[SerializeField, Space(20)]

    ///*〇構想について
    // 　CopyFileをgameObjects[]にforeachで付与する*/


    //public GameObject[] gameObjects;

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
