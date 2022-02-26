using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HwEnemyColliderSet : MonoBehaviour {

#if UNITY_EDITOR

    [Button("SetColliderSize", "コリダーの大きさをRectのSizeと同じに")]
    public bool dummy1;//属性でボタンを作るので何かしら宣言が必要。使わない。
    public void SetColliderSize()
    {
        float
            thisWidth = GetComponent<RectTransform>().sizeDelta.x,
            thisHeight = GetComponent<RectTransform>().sizeDelta.y;

        BoxCollider
            thisBoxCollider = GetComponent<BoxCollider>();

        thisBoxCollider.size = new Vector3(thisWidth, thisHeight, 0.002f);
    }



    [Button("SetRescaleAndImageAndChildIcon", "Scale1にして大きさ合わせてImageつけて子にアイコン")]
    public bool dummy2;//属性でボタンを作るので何かしら宣言が必要。使わない。
    public void SetRescaleAndImageAndChildIcon()
    {
        float
            thisScaleX = transform.localScale.x,
            thisScaleY = transform.localScale.y,
            thisScaleZ = transform.localScale.z;

        Image //同時にRectTransform化される
            thisImage = gameObject.AddComponent<Image>();

        RectTransform
            thisRectTransform = GetComponent<RectTransform>();

        //ScaleをWidthHeightに
        thisRectTransform.sizeDelta = new Vector2(thisScaleX,thisScaleY);

        //Scale1に
        transform.localScale = Vector3.one;


        //子にアイコンImage
        var HusenIconImageObj = new GameObject("HusenIconImage");
        HusenIconImageObj.transform.SetParent(transform,false);

        var husenIconImage = HusenIconImageObj.AddComponent<Image>();

        husenIconImage.sprite = Resources.Load<Sprite>("EventSystem/Homework/DrillTextures/HusenIconAlpha");
        husenIconImage.raycastTarget = false;
        husenIconImage.preserveAspect = true;

        //大きさ
        HusenIconImageObj.GetComponent<RectTransform>().sizeDelta = thisRectTransform.sizeDelta;

        SetColliderSize();
    }
#endif

}
