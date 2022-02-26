using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // 追加しましょう

//enumを使うために必要
using System;

public class DownKeyChecker : MonoBehaviour
{
    public Text inputText
        , axis1Text
        , axis2Text
        , axis3Text
        , axis4Text
        , axis5Text
        , axis6Text
        , axis7Text

        , ocu1Text
        , ocu2Text
        , ocu3Text
        , ocu4Text
        , ocu5Text
        , ocu6Text
        , ocu7Text
        , ocu8Text
        , ocu9Text
        ;


    void Update()
    {
        DownKeyCheck();
        AxisCheck();
    }


    void DownKeyCheck()
    {
        if (Input.anyKey)
        {
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    //処理を書く
                    Debug.Log(code);
                    inputText.text += code.ToString() + "\n";
                    break;
                }
            }
        }
        else
        {
            inputText.text = "";
        }


    }

    void AxisCheck()
    {
        axis1Text.text = Input.GetAxis("XboxTriggerAxis").ToString();
        axis2Text.text = Input.GetAxis("XboxPadXAxis").ToString();
        axis3Text.text = Input.GetAxis("XboxPadYAxis").ToString();
        axis4Text.text = Input.GetAxis("XboxLStickXAxis").ToString();
        axis5Text.text = Input.GetAxis("XboxLStickYAxis").ToString();
        axis6Text.text = Input.GetAxis("XboxRStickXAxis").ToString();
        axis7Text.text = Input.GetAxis("XboxRStickYAxis").ToString();
        ocu1Text.text = Input.GetAxis("OculusTouchLStickXAxis").ToString();
        ocu2Text.text = Input.GetAxis("OculusTouchLStickYAxis").ToString();
        ocu3Text.text = Input.GetAxis("OculusTouchLRTriggerAxis").ToString();
        ocu4Text.text = Input.GetAxis("OculusTouchRStickXAxis").ToString();
        ocu5Text.text = Input.GetAxis("OculusTouchRStickYAxis").ToString();
        ocu6Text.text = Input.GetAxis("OculusTouchLTriggerAxis").ToString();
        ocu7Text.text = Input.GetAxis("OculusTouchRTriggerAxis").ToString();
        ocu8Text.text = Input.GetAxis("OculusTouchLGripAxis").ToString();
        ocu9Text.text = Input.GetAxis("OculusTouchRGripAxis").ToString();
    }

}