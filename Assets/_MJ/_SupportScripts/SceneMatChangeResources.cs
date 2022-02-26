using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class SceneMatChangeResources : MonoBehaviour
{
    [Header("■シーンごとにマテリアル自動で切り替えるスクリプト（ちえりヘアーなど）■")]
    public Material[]
        asaMat;
    public Material[]
        hiruMat,
        yugataMat,
        yoruMat,
        shinyaMat;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "TH_Asa" && asaMat.Length > 0)
        {
            GetComponent<Renderer>().materials = asaMat;
        }
        else if (SceneManager.GetActiveScene().name == "TH_Hiru" && hiruMat.Length > 0)
        {
            GetComponent<Renderer>().materials = hiruMat;
        }
        else if (SceneManager.GetActiveScene().name == "TH_Yugata" && yugataMat.Length > 0)
        {
            GetComponent<Renderer>().materials = yugataMat;
        }
        else if (SceneManager.GetActiveScene().name == "TH_Yoru" && yoruMat.Length > 0)
        {
            GetComponent<Renderer>().materials = yoruMat;
        }
        else if (SceneManager.GetActiveScene().name == "TH_Shinya" && shinyaMat.Length > 0)
        {
            GetComponent<Renderer>().materials = shinyaMat;
        }
        else
        {
            Debug.Log(gameObject.name + "■シーン別マテリアルが割り当てられてない　もしくは該当シーンがない");
        }
    }
}
