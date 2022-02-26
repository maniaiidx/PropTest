using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovelAutoVisOff : MonoBehaviour
{
    //DCでメニューついたらCanvasオフ

    DataCounter DC;
    Canvas thisCanvas;
    void Awake()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        thisCanvas = GetComponent<Canvas>();
    }


    bool tmpVis = true;
    void Update()
    {
        if(tmpVis && DC.isMenuSystem)
        {
            tmpVis =
            thisCanvas.enabled = false;
        }
        else if (tmpVis == false && DC.isMenuSystem == false)
        {
            tmpVis =
            thisCanvas.enabled = true;
        }
    }
}
