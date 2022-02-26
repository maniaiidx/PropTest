using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HukidashiAutoVisOff : MonoBehaviour
{
    //DCでメニューついたら ノベルログ表示したらCanvasオフ


    DataCounter DC;
    Canvas thisCanvas;
    void Awake()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        thisCanvas = GetComponent<Canvas>();
    }


    bool //1フレで処理する用
        tmpMenuVis = true,
        tmpNovelLogVis = true;
    void Update()
    {
        //メニュー表示時
        if (tmpMenuVis && DC.isMenuSystem)
        {
            tmpMenuVis =
            thisCanvas.enabled = false;
        }
        else if (tmpMenuVis == false && DC.isMenuSystem == false)
        {
            tmpMenuVis = true;
            //ログ表示していない場合にON（メニューログ表示しながらメニューオフした時は戻さない）
            if (tmpNovelLogVis)
            { thisCanvas.enabled = true; }
        }

        //ノベルログ表示時も
        if (tmpNovelLogVis && DC.isNovelLogVisIng)
        {
            tmpNovelLogVis =
            thisCanvas.enabled = false;
        }
        else if (tmpNovelLogVis == false && DC.isNovelLogVisIng == false)
        {
            tmpNovelLogVis =
            thisCanvas.enabled = true;
        }
    }
}
