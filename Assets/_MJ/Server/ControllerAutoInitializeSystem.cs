using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class ControllerAutoInitializeSystem : MonoBehaviour
{
    //■コントローラー自動判別設定 入力管理システム(オーダー早くするために単独スクリプト)

    DataCounter DC;
    DataBridging DB;

    void Awake()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        DB = GameObject.Find("DataBridging").GetComponent<DataBridging>();

        //コントローラーひとまずデフォルトのままで設定
        ControllerSetting(DB.playerController, true);
    }

    bool //仮想ボタンのDown取り用
        isVivePadTouchAndGripButton = false;

    float //コントローラー判別 時間あけてやる用
        controllerCheckTimeCount,
        controllerCheckTime = 2;

    void Update()
    {
        //コントローラーチェックとセットを時間間隔開けて処理
        controllerCheckTimeCount += 1 * Time.deltaTime;
        if (controllerCheckTimeCount > controllerCheckTime)
        {
            controllerCheckTimeCount = 0;
            ControllerCheckAndSet();
        }

        #region //コントローラー判定

        //#region xbox A ocu A　vive RMenu　押されたときに
        //if (Input.GetKeyDown(KeyCode.JoystickButton0))
        //{
        //    //ocu A タッチがされてたら ＝ocu
        //    if (Input.GetKey(KeyCode.JoystickButton10))
        //    {
        //        //DC.SubTitleVis(true, "ocuR A");
        //        ControllerSetting(DataBridging.PlayerVRController.OculusTouchR);
        //    }
        //    //されてない ＝xbox or vive
        //    else
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "ViveR MENU");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCViveR);
        //        }
        //        else
        //        {
        //            //DC.SubTitleVis(true, "xbox A");
        //            ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //        }
        //    }
        //}
        //#endregion
        //#region xbox B ocu B　押されたときに
        //if (Input.GetKeyDown(KeyCode.JoystickButton1))
        //{
        //    //ocu B タッチがされてたら ＝ocu
        //    if (Input.GetKey(KeyCode.JoystickButton11))
        //    {
        //        //DC.SubTitleVis(true, "ocuR B");
        //        ControllerSetting(DataBridging.PlayerVRController.OculusTouchR);
        //    }
        //    //されてない ＝xbox
        //    else
        //    {
        //        //DC.SubTitleVis(true, "xbox B");
        //        ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //    }
        //}
        //#endregion
        //#region xbox X ocuL X　vive LMenu　押されたときに
        //if (Input.GetKeyDown(KeyCode.JoystickButton2))
        //{
        //    //ocu Y タッチがされてたら ＝ocu
        //    if (Input.GetKey(KeyCode.JoystickButton12))
        //    {
        //        //DC.SubTitleVis(true, "ocuL X");
        //        ControllerSetting(DataBridging.PlayerVRController.OculusTouch);
        //    }
        //    //されてない ＝xbox or Vive
        //    else
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "ViveL MENU");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCVive);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "xboxX ");
        //            ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //        }
        //    }
        //}
        //#endregion
        //#region xbox Y ocuL Y　押されたときに
        //if (Input.GetKeyDown(KeyCode.JoystickButton3))
        //{
        //    //ocu Y タッチがされてたら ＝ocu
        //    if (Input.GetKey(KeyCode.JoystickButton13))
        //    {
        //        //DC.SubTitleVis(true, "ocuL Y");
        //        ControllerSetting(DataBridging.PlayerVRController.OculusTouch);
        //    }
        //    //されてない ＝xbox
        //    else
        //    {
        //        //DC.SubTitleVis(true, "xbox Y");
        //        ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //    }
        //}
        //#endregion
        //#region xbox BACK oculusL START押されたときに(使用予定ないので、とりあえず判定方法は保留)
        //if (Input.GetKeyDown(KeyCode.JoystickButton6))
        //{
        //    //Oculus接続されてる（これだとOculus繋いだ上でのXBOX反応ができない）
        //    if (tmpJoystickNameMargeStr.IndexOf("Oculus") >= 0)
        //    {
        //        //DC.SubTitleVis(true, "OculusLEnableSTART(6)");
        //        ControllerSetting(DataBridging.PlayerVRController.OculusTouch);
        //    }
        //    //されてない
        //    else
        //    {
        //        //DC.SubTitleVis(true, "xbox BACK");
        //        ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //    }
        //}
        //#endregion
        //#region xbox LSTICKBUTTON ocuL STICKBUTTON　viveL　TRACKPADDOWN　押されたときに
        //if (Input.GetKeyDown(KeyCode.JoystickButton8))
        //{
        //    //ocuL STICKBUTTON タッチ  viveL TRACKPADタッチ　がされてたら ＝ocu or vive
        //    if (Input.GetKey(KeyCode.JoystickButton16))
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "ViveL TRACKPADDOWN");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCVive);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "OcuL STICKBUTTON");
        //            ControllerSetting(DataBridging.PlayerVRController.OculusTouch);
        //        }
        //    }
        //    //されてない ＝xbox
        //    else
        //    {
        //        //DC.SubTitleVis(true, "xbox LSTICKBUTTON");
        //        ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //    }
        //}
        //#endregion
        //#region xbox RSTICKBUTTON ocuR STICKBUTTON　viveR　TRACKPADDOWN　押されたときに
        //if (Input.GetKeyDown(KeyCode.JoystickButton9))
        //{
        //    //ocuR STICKBUTTON タッチ  viveR TRACKPADタッチ　がされてたら ＝ocu or vive
        //    if (Input.GetKey(KeyCode.JoystickButton17))
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "ViveR TRACKPADDOWN");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCViveR);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "OcuR STICKBUTTON");
        //            ControllerSetting(DataBridging.PlayerVRController.OculusTouchR);
        //        }
        //    }
        //    //されてない ＝xbox
        //    else
        //    {
        //        //DC.SubTitleVis(true, "xbox RSTICKBUTTON");
        //        ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //    }
        //}
        //#endregion

        ////■■AXIS
        //#region xbox LTRIGGER  ocuL INDEXTRIGGER　viveL TRIGGER　押されたときに
        //if (Input.GetAxis("OculusTouchLTriggerAxis") >= 0.2f)
        //{
        //    //ocuL INDEXTRIGGERタッチ  viveL TRIGGERタッチ　がされてたら ＝ocu or vive
        //    if (Input.GetKey(KeyCode.JoystickButton14))
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "viveL TRIGGER");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCVive);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "ocuL INDEXTRIGGER");
        //            ControllerSetting(DataBridging.PlayerVRController.OculusTouch);
        //        }
        //    }
        //    //されてない ＝xbox or Vive
        //    else
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "viveL TRIGGER");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCVive);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "xbox LTRIGGER");
        //            ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //        }
        //    }
        //}
        //#endregion
        //#region xbox RTRIGGER  ocuR INDEXTRIGGER　viveR TRIGGER　押されたときに
        //if (Input.GetAxis("OculusTouchRTriggerAxis") >= 0.2f)
        //{
        //    //ocuR INDEXTRIGGERタッチ  viveR TRIGGERタッチ　がされてたら ＝ocu or vive
        //    if (Input.GetKey(KeyCode.JoystickButton15))
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "viveR TRIGGER");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCViveR);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "ocuR INDEXTRIGGER");
        //            ControllerSetting(DataBridging.PlayerVRController.OculusTouchR);
        //        }
        //    }
        //    //されてない ＝xbox or Vive
        //    else
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "viveR TRIGGER");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCViveR);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "xbox RTRIGGER");
        //            ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //        }
        //    }
        //}
        //#endregion
        //#region xbox LBUTTON  ocuL HANDTRIGGER　viveL GRIPBUTTON　押されたときに
        //if (Input.GetKey(KeyCode.JoystickButton4))
        //{
        //    //ocuL HANDTRIGGERAxis  viveL GRIPBUTTONAxis　がされてたら ＝ocu or vive
        //    if (Input.GetAxis("OculusTouchLGripAxis") >= 0.4f)
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "viveL GRIPBUTTON");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCVive);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "ocuL HANDTRIGGER");
        //            ControllerSetting(DataBridging.PlayerVRController.OculusTouch);
        //        }
        //    }
        //    //されてない ＝xbox
        //    else
        //    {
        //        //DC.SubTitleVis(true, "xbox LB");
        //        ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //    }
        //}
        //#endregion
        //#region xbox RBUTTON  ocuR HANDTRIGGER　viveR GRIPBUTTON　押されたときに
        //if (Input.GetKeyDown(KeyCode.JoystickButton5))
        //{
        //    //ocuR HANDTRIGGERAxis  viveR GRIPBUTTONAxis　がされてたら ＝ocu or vive
        //    if (Input.GetAxis("OculusTouchRGripAxis") >= 0.4f)
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "viveR GRIPBUTTON");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCViveR);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "ocuR HANDTRIGGER");
        //            ControllerSetting(DataBridging.PlayerVRController.OculusTouchR);
        //        }
        //    }
        //    //されてない ＝xbox
        //    else
        //    {
        //        //DC.SubTitleVis(true, "xbox RB");
        //        ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //    }
        //}
        //#endregion
        //#region xbox LSTICKX  ocuL STICKX　viveL TRACKPADX　動かされたときに
        //if (Input.GetAxis("XboxLStickXAxis") >= 0.2f || Input.GetAxis("XboxLStickXAxis") <= -0.2f)
        //{
        //    //ocuL STICKBUTTON タッチ  viveL TRACKPADタッチ　がされてたら ＝ocu or vive
        //    if (Input.GetKey(KeyCode.JoystickButton16))
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "ViveL TRACKPADX");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCVive);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "OcuL STICKX");
        //            ControllerSetting(DataBridging.PlayerVRController.OculusTouch);
        //        }
        //    }
        //    //されてない ＝xbox
        //    else
        //    {
        //        //DC.SubTitleVis(true, "xbox LSTICKX");
        //        ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //    }
        //}
        //#endregion
        //#region xbox LSTICKY  ocuL STICKY　viveL TRACKPADY　動かされたときに
        //if (Input.GetAxis("XboxLStickYAxis") >= 0.2f || Input.GetAxis("XboxLStickYAxis") <= -0.2f)
        //{
        //    //ocuL STICKBUTTON タッチ  viveL TRACKPADタッチ　がされてたら ＝ocu or vive
        //    if (Input.GetKey(KeyCode.JoystickButton16))
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "ViveL TRACKPADY");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCVive);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "OcuL STICKY");
        //            ControllerSetting(DataBridging.PlayerVRController.OculusTouch);
        //        }
        //    }
        //    //されてない ＝xbox
        //    else
        //    {
        //        //DC.SubTitleVis(true, "xbox LSTICKY");
        //        ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //    }
        //}
        //#endregion
        //#region xbox RSTICKX  ocuR STICKX　viveR TRACKPADX　動かされたとき
        //if (Input.GetAxis("XboxRStickXAxis") >= 0.2f || Input.GetAxis("XboxRStickXAxis") <= -0.2f)
        //{
        //    //ocuR STICKBUTTON タッチ  viveR TRACKPADタッチ　がされてたら ＝ocu or vive
        //    if (Input.GetKey(KeyCode.JoystickButton17))
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "ViveR TRACKPADX");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCViveR);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "OcuR STICKX");
        //            ControllerSetting(DataBridging.PlayerVRController.OculusTouchR);
        //        }
        //    }
        //    //されてない ＝xbox
        //    else
        //    {
        //        //DC.SubTitleVis(true, "xbox RSTICKX");
        //        ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //    }
        //}
        //#endregion
        //#region xbox RSTICKY  ocuR STICKY　viveR TRACKPADY　動かされたとき
        //if (Input.GetAxis("XboxRStickYAxis") >= 0.2f || Input.GetAxis("XboxRStickYAxis") <= -0.2f)
        //{
        //    //ocuR STICKBUTTON タッチ  viveR TRACKPADタッチ　がされてたら ＝ocu or vive
        //    if (Input.GetKey(KeyCode.JoystickButton17))
        //    {
        //        //Vive接続されてる
        //        if (tmpJoystickNameMargeStr.IndexOf("OpenVR") >= 0)
        //        {
        //            //DC.SubTitleVis(true, "ViveR TRACKPADY");
        //            ControllerSetting(DataBridging.PlayerVRController.HTCViveR);
        //        }
        //        //されてない
        //        else
        //        {
        //            //DC.SubTitleVis(true, "OcuR STICKY");
        //            ControllerSetting(DataBridging.PlayerVRController.OculusTouchR);
        //        }
        //    }
        //    //されてない ＝xbox
        //    else
        //    {
        //        //DC.SubTitleVis(true, "xbox RSTICKY");
        //        ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //    }
        //}
        //#endregion

        //#region 単体　XBOX十字AxisとBACK　オキュラスの指置きなど
        //if (Input.GetAxis("XboxPadXAxis") >= 0.2f || Input.GetAxis("XboxPadXAxis") <= -0.2f)
        //{
        //    //DC.SubTitleVis(true, "xboxPADX");
        //    ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //}
        //if (Input.GetAxis("XboxPadYAxis") >= 0.2f || Input.GetAxis("XboxPadYAxis") <= -0.2f)
        //{
        //    //DC.SubTitleVis(true, "xboxPADY");
        //    ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //}
        //if (Input.GetKeyDown(KeyCode.JoystickButton7))
        //{
        //    //DC.SubTitleVis(true, "xboxSTART");
        //    ControllerSetting(DataBridging.PlayerVRController.Xbox);
        //}
        //if (Input.GetKeyDown(KeyCode.JoystickButton18))
        //{
        //    //DC.SubTitleVis(true, "OcuL ThumbRest");
        //    ControllerSetting(DataBridging.PlayerVRController.OculusTouch);
        //}
        //if (Input.GetKeyDown(KeyCode.JoystickButton19))
        //{
        //    //DC.SubTitleVis(true, "OcuR ThumbRest");
        //    ControllerSetting(DataBridging.PlayerVRController.OculusTouchR);
        //}
        //#endregion

        #endregion
        #region //コントローラー入力サブタイトルでテスト表示（AXISが効きっ放し判定になるので失敗）
        #region //WindowsMR設定テスト

        //if (Input.GetButtonDown("MRPadLTouch")) { DC.SubTitleVis(true, "MRPadLTouch"); }
        //if (Input.GetButtonDown("MRPadRTouch")) { DC.SubTitleVis(true, "MRPadRTouch"); }
        //if (Input.GetButtonDown("MRPadLPress")) { DC.SubTitleVis(true, "MRPadLPress"); }
        //if (Input.GetButtonDown("MRPadRPress")) { DC.SubTitleVis(true, "MRPadRPress"); }
        //if (Input.GetButtonDown("MRStickLPress")) { DC.SubTitleVis(true, "MRStickLPress"); }
        //if (Input.GetButtonDown("MRStickRPress")) { DC.SubTitleVis(true, "MRStickRPress"); }
        //if (Input.GetButtonDown("MRTriggerLPress")) { DC.SubTitleVis(true, "MRTriggerLPress"); }
        //if (Input.GetButtonDown("MRTriggerRPress")) { DC.SubTitleVis(true, "MRTriggerRPress"); }
        //if (Input.GetButtonDown("MRGripLPress")) { DC.SubTitleVis(true, "MRGripLPress"); }
        //if (Input.GetButtonDown("MRGripRPress")) { DC.SubTitleVis(true, "MRGripRPress"); }
        //if (Input.GetButtonDown("MRMenuL")) { DC.SubTitleVis(true, "MRMenuL"); }
        //if (Input.GetButtonDown("MRMenuR")) { DC.SubTitleVis(true, "MRMenuR"); }

        //if (Input.GetAxis("MRPadLAxisX") > 0.3f) { DC.SubTitleVis(true, "MRPadLAxisX+"); }
        //if (Input.GetAxis("MRPadLAxisX") < 0.3f) { DC.SubTitleVis(true, "MRPadLAxisX-"); }
        //if (Input.GetAxis("MRPadRAxisX") > 0.3f) { DC.SubTitleVis(true, "MRPadRAxisX+"); }
        //if (Input.GetAxis("MRPadRAxisX") < 0.3f) { DC.SubTitleVis(true, "MRPadRAxisX-"); }

        //if (Input.GetAxis("MRPadLAxisY") > 0.3f) { DC.SubTitleVis(true, "MRPadLAxisY+"); }
        //if (Input.GetAxis("MRPadLAxisY") < 0.3f) { DC.SubTitleVis(true, "MRPadLAxisY-"); }
        //if (Input.GetAxis("MRPadRAxisY") > 0.3f) { DC.SubTitleVis(true, "MRPadRAxisY+"); }
        //if (Input.GetAxis("MRPadRAxisY") < 0.3f) { DC.SubTitleVis(true, "MRPadRAxisY-"); }

        //if (Input.GetAxis("MRStickLAxisX") > 0.3f) { DC.SubTitleVis(true, "MRStickLAxisX+"); }
        //if (Input.GetAxis("MRStickLAxisX") < 0.3f) { DC.SubTitleVis(true, "MRStickLAxisX-"); }
        //if (Input.GetAxis("MRStickRAxisX") > 0.3f) { DC.SubTitleVis(true, "MRStickRAxisX+"); }
        //if (Input.GetAxis("MRStickRAxisX") < 0.3f) { DC.SubTitleVis(true, "MRStickRAxisX-"); }

        //if (Input.GetAxis("MRStickLAxisY") > 0.3f) { DC.SubTitleVis(true, "MRStickLAxisY+"); }
        //if (Input.GetAxis("MRStickLAxisY") < 0.3f) { DC.SubTitleVis(true, "MRStickLAxisY-"); }
        //if (Input.GetAxis("MRStickRAxisY") > 0.3f) { DC.SubTitleVis(true, "MRStickRAxisY+"); }
        //if (Input.GetAxis("MRStickRAxisY") < 0.3f) { DC.SubTitleVis(true, "MRStickRAxisY-"); }

        //if (Input.GetAxis("MRTriggerLAxis") > 0.3f) { DC.SubTitleVis(true, "MRTriggerLAxis+"); }
        //if (Input.GetAxis("MRTriggerLAxis") < 0.3f) { DC.SubTitleVis(true, "MRTriggerLAxis-"); }
        //if (Input.GetAxis("MRTriggerRAxis") > 0.3f) { DC.SubTitleVis(true, "MRTriggerRAxis+"); }
        //if (Input.GetAxis("MRTriggerRAxis") < 0.3f) { DC.SubTitleVis(true, "MRTriggerRAxis-"); }

        //if (Input.GetAxis("MRGripLAxis") > 0.3f) { DC.SubTitleVis(true, "MRGripLAxis+"); }
        //if (Input.GetAxis("MRGripLAxis") < 0.3f) { DC.SubTitleVis(true, "MRGripLAxis-"); }
        //if (Input.GetAxis("MRGripRAxis") > 0.3f) { DC.SubTitleVis(true, "MRGripRAxis+"); }
        //if (Input.GetAxis("MRGripRAxis") < 0.3f) { DC.SubTitleVis(true, "MRGripRAxis-"); }

        #endregion
        #region //XBOX設定テスト

        //if (Input.GetButtonDown("XboxA")) { DC.SubTitleVis(true, "XboxA"); }
        //if (Input.GetButtonDown("XboxB")) { DC.SubTitleVis(true, "XboxB"); }
        //if (Input.GetButtonDown("XboxX")) { DC.SubTitleVis(true, "XboxX"); }
        //if (Input.GetButtonDown("XboxY")) { DC.SubTitleVis(true, "XboxY"); }
        //if (Input.GetButtonDown("XboxLB")) { DC.SubTitleVis(true, "XboxLB"); }
        //if (Input.GetButtonDown("XboxRB")) { DC.SubTitleVis(true, "XboxRB"); }
        //if (Input.GetButtonDown("XboxBACK")) { DC.SubTitleVis(true, "XboxBACK"); }
        //if (Input.GetButtonDown("XboxSTART")) { DC.SubTitleVis(true, "XboxSTART"); }
        //if (Input.GetButtonDown("XboxLStickButton")) { DC.SubTitleVis(true, "XboxLStickButton"); }
        //if (Input.GetButtonDown("XboxRStickButton")) { DC.SubTitleVis(true, "XboxRStickButton"); }

        //if (Input.GetAxis("XboxTriggerAxis") > 0.3f) { DC.SubTitleVis(true, "XboxTriggerAxis+"); }
        //if (Input.GetAxis("XboxTriggerAxis") < 0.3f) { DC.SubTitleVis(true, "XboxTriggerAxis-"); }

        //if (Input.GetAxis("XboxPadXAxis") > 0.3f) { DC.SubTitleVis(true, "XboxPadXAxis+"); }
        //if (Input.GetAxis("XboxPadXAxis") < 0.3f) { DC.SubTitleVis(true, "XboxPadXAxis-"); }
        //if (Input.GetAxis("XboxPadYAxis") > 0.3f) { DC.SubTitleVis(true, "XboxPadYAxis+"); }
        //if (Input.GetAxis("XboxPadYAxis") < 0.3f) { DC.SubTitleVis(true, "XboxPadYAxis-"); }

        //if (Input.GetAxis("XboxLStickXAxis") > 0.3f) { DC.SubTitleVis(true, "XboxLStickXAxis+"); }
        //if (Input.GetAxis("XboxLStickXAxis") < 0.3f) { DC.SubTitleVis(true, "XboxLStickXAxis-"); }
        //if (Input.GetAxis("XboxLStickYAxis") > 0.3f) { DC.SubTitleVis(true, "XboxLStickYAxis+"); }
        //if (Input.GetAxis("XboxLStickYAxis") < 0.3f) { DC.SubTitleVis(true, "XboxLStickYAxis-"); }

        //if (Input.GetAxis("XboxRStickXAxis") > 0.3f) { DC.SubTitleVis(true, "XboxRStickXAxis+"); }
        //if (Input.GetAxis("XboxRStickXAxis") < 0.3f) { DC.SubTitleVis(true, "XboxRStickXAxis-"); }
        //if (Input.GetAxis("XboxRStickYAxis") > 0.3f) { DC.SubTitleVis(true, "XboxRStickYAxis+"); }
        //if (Input.GetAxis("XboxRStickYAxis") < 0.3f) { DC.SubTitleVis(true, "XboxRStickYAxis-"); }

        #endregion
        #endregion

        #region ■仮想ボタン処理
        #region ■仮想VivePadボタン
        //DownとUpを解除
        if (DC.isVivePadUp) { DC.isVivePadUp = false; }
        if (DC.isVivePadDown) { DC.isVivePadDown = false; }

        //元もとのDownUpを適用
        if (Input.GetButtonDown(DB.inputDict["VivePadDown"]) || Input.GetButtonDown(DB.inputDict["VivePadDown2"]))
        { DC.isVivePadDown = true; }
        if (Input.GetButtonUp(DB.inputDict["VivePadDown"]) || Input.GetButtonDown(DB.inputDict["VivePadDown2"]))
        { DC.isVivePadUp = true; }

        //Axisをエミュレート
        if ((Input.GetAxis(DB.inputDict["AxisVivePadDown"]) >= 0.5f)
            || (Input.GetAxis(DB.inputDict["AxisVivePadDown2"]) >= 0.5f))
        {
            //ここに入ってからisVivePad判別で処理　（↑で同時にやると↓elseのUpが選ばれてしまうので）
            if (DC.isVivePad == false)
            {
                DC.isVivePadDown =
                DC.isVivePad = true;
                DC.isVivePadUp = false;
            }
        }
        else if ((Input.GetAxis(DB.inputDict["AxisVivePadDown"]) < 0.5f && DC.isVivePad)
            || (Input.GetAxis(DB.inputDict["AxisVivePadDown2"]) < 0.5f && DC.isVivePad))
        {
            DC.isVivePadUp = true;
            DC.isVivePad = false;
        }

        //元もとのボタンを適用
        if (Input.GetButton(DB.inputDict["VivePadDown"])
            || Input.GetButton(DB.inputDict["VivePadDown2"]))
        { DC.isVivePad = true; }

        #endregion

        #region ■仮想決定ボタン（OculusTouchなどでトリガーAxisを決定ボタンにする用）
        //DownとUpを解除
        if (DC.isKetteiUp) { DC.isKetteiUp = false; }
        if (DC.isKetteiDown) { DC.isKetteiDown = false; }

        //元もとの決定ボタンDownUpを適用
        if (Input.GetButtonDown(DB.inputDict["決定"]) || Input.GetButtonDown(DB.inputDict["決定2"]))
        { DC.isKetteiDown = true; }
        if (Input.GetButtonUp(DB.inputDict["決定"]) || Input.GetButtonDown(DB.inputDict["決定2"]))
        { DC.isKetteiUp = true; }

        //Axisをエミュレート
        if ((Input.GetAxis(DB.inputDict["Axis決定"]) >= 0.8f)
            || (Input.GetAxis(DB.inputDict["Axis決定2"]) >= 0.8f))
        {
            //ここに入ってからKettei判別で処理　（↑で同時にやると↓elseのUpが選ばれてしまうので）
            if (DC.isKettei == false)
            {
                DC.isKetteiDown =
                DC.isKettei = true;
                DC.isKetteiUp = false;
            }
        }
        else if ((Input.GetAxis(DB.inputDict["Axis決定"]) < 0.8f && DC.isKettei)
            || (Input.GetAxis(DB.inputDict["Axis決定2"]) < 0.8f && DC.isKettei))
        {
            DC.isKetteiUp = true;
            DC.isKettei = false;
        }

        //元もとの決定ボタンを適用
        if (Input.GetButton(DB.inputDict["決定"])
            || Input.GetButton(DB.inputDict["決定2"]))
        { DC.isKettei = true; }

        #endregion
        #region ■仮想切り替えボタン
        //DownとUpを解除
        if (DC.isBackUp) { DC.isBackUp = false; }
        if (DC.isBackDown) { DC.isBackDown = false; }

        //元もとのDownUpを適用
        if (Input.GetButtonDown(DB.inputDict["切り替え・バック"]) || Input.GetButtonDown(DB.inputDict["切り替え・バック2"]))
        { DC.isBackDown = true; }
        if (Input.GetButtonUp(DB.inputDict["切り替え・バック"]) || Input.GetButtonDown(DB.inputDict["切り替え・バック2"]))
        { DC.isBackUp = true; }

        //Axisをエミュレート
        if ((Input.GetAxis(DB.inputDict["Axis切り替え・バック"]) >= 0.5f)
            || (Input.GetAxis(DB.inputDict["Axis切り替え・バック2"]) >= 0.5f))
        {
            //ここに入ってからBack判別で処理　（↑で同時にやると↓elseのUpが選ばれてしまうので）
            if (DC.isBack == false)
            {
                DC.isBackDown =
                DC.isBack = true;
                DC.isBackUp = false;
            }
        }
        else if ((Input.GetAxis(DB.inputDict["Axis切り替え・バック"]) < 0.4f && DC.isBack)
            || (Input.GetAxis(DB.inputDict["Axis切り替え・バック2"]) < 0.4f && DC.isBack))
        {
            DC.isBackUp = true;
            DC.isBack = false;
        }

        //元もとのボタンを適用
        if (Input.GetButton(DB.inputDict["切り替え・バック"])
            || Input.GetButton(DB.inputDict["切り替え・バック2"]))
        { DC.isBack = true; }

        #endregion
        #region ■仮想メニューポーズボタン
        //DownとUpを解除
        if (DC.isMenuPauseUp) { DC.isMenuPauseUp = false; }
        if (DC.isMenuPauseDown) { DC.isMenuPauseDown = false; }

        //元もとのDownUpを適用
        if (Input.GetButtonDown(DB.inputDict["メニュー・ポーズ"]) || Input.GetButtonDown(DB.inputDict["メニュー・ポーズ2"]))
        { DC.isMenuPauseDown = true; }
        if (Input.GetButtonUp(DB.inputDict["メニュー・ポーズ"]) || Input.GetButtonDown(DB.inputDict["メニュー・ポーズ2"]))
        { DC.isMenuPauseUp = true; }

        //Axisをエミュレート
        if ((Input.GetAxis(DB.inputDict["Axisメニュー・ポーズ"]) >= 0.5f)
            || (Input.GetAxis(DB.inputDict["Axisメニュー・ポーズ2"]) >= 0.5f))
        {
            //ここに入ってからisMenuPause判別で処理　（↑で同時にやると↓elseのUpが選ばれてしまうので）
            if (DC.isMenuPause == false)
            {
                DC.isMenuPauseDown =
                DC.isMenuPause = true;
                DC.isMenuPauseUp = false;
            }
        }
        else if ((Input.GetAxis(DB.inputDict["Axisメニュー・ポーズ"]) < 0.4f && DC.isMenuPause)
            || (Input.GetAxis(DB.inputDict["Axisメニュー・ポーズ2"]) < 0.4f && DC.isMenuPause))
        {
            DC.isMenuPauseUp = true;
            DC.isMenuPause = false;
        }

        //元もとのボタンを適用
        if (Input.GetButton(DB.inputDict["メニュー・ポーズ"])
            || Input.GetButton(DB.inputDict["メニュー・ポーズ2"]))
        { DC.isMenuPause = true; }

        #endregion
        #region ■仮想選択AXIS (ViveはisVivePad必要)

        //Viveの時はisVivePad必要に
        if (DB.playerController == DataBridging.PlayerVRController.HTCVive)
        {
            if (DC.isVivePad)
            {
                //今の所選択左右はなし
                //DC.sentakuAxisX =
                //    Input.GetAxis(DB.inputDict["選択左右"]) + Input.GetAxis(DB.inputDict["選択左右2"]);
                DC.sentakuAxisY =
                    Input.GetAxisRaw(DB.inputDict["選択上下"]) + Input.GetAxisRaw(DB.inputDict["選択上下2"]);
            }
            else//押されてない時0
            { if (DC.sentakuAxisY != 0) DC.sentakuAxisY = 0; }
        }
        else
        {
            //今の所選択左右はなし
            //DC.sentakuAxisX =
            //    Input.GetAxis(DB.inputDict["選択左右"]) + Input.GetAxis(DB.inputDict["選択左右2"]);
            DC.sentakuAxisY =
                Input.GetAxisRaw(DB.inputDict["選択上下"]) + Input.GetAxisRaw(DB.inputDict["選択上下2"]);

        }

        #endregion
        #region ■仮想カメラスティックAXIS (ViveはisVivePad必要)
        //Viveの時はisVivePad必要に
        if (DB.playerController == DataBridging.PlayerVRController.HTCVive)
        {
            if (DC.isVivePad)
            {
                DC.cameraStickAxisX =
                    Input.GetAxis(DB.inputDict["カメラスティック回転X"]) + Input.GetAxis(DB.inputDict["カメラスティック回転X2"]);

                DC.cameraStickAxisY =
                    Input.GetAxis(DB.inputDict["カメラスティック回転Y"]) + Input.GetAxis(DB.inputDict["カメラスティック回転Y2"]);
            }
            else//押されてない時0
            {
                if (DC.cameraStickAxisX != 0) { DC.cameraStickAxisX = 0; }
                if (DC.cameraStickAxisY != 0) { DC.cameraStickAxisY = 0; }
            }
        }
        else
        {
            DC.cameraStickAxisX =
                Input.GetAxis(DB.inputDict["カメラスティック回転X"]) + Input.GetAxis(DB.inputDict["カメラスティック回転X2"]);

            DC.cameraStickAxisY =
                Input.GetAxis(DB.inputDict["カメラスティック回転Y"]) + Input.GetAxis(DB.inputDict["カメラスティック回転Y2"]);
        }

        #endregion
        #region ■仮想カメラリセットボタンDown（VIVE用に同時押し対応にする用）

        //Down解除
        if (DC.isCameraResetDown) { DC.isCameraResetDown = false; }

        #region Viveのパッドタッチ+中指ボタン同時押し　を　Down化

        //まず、前フレで押されていたら どちらか離しているかどうかを判定
        if (isVivePadTouchAndGripButton)
        {
            if (DB.playerController == DataBridging.PlayerVRController.HTCVive)
            {
                //ViveLのパッドかグリップが離れていれば 解除
                if (Input.GetButton("HTCViveLTrackPadTouch") == false
                    || DC.isBack == false)
                {
                    isVivePadTouchAndGripButton = false;
                }
                //ViveRのパッドかグリップが離れていれば 解除
                else if (Input.GetButton("HTCViveRTrackPadTouch") == false
                    || DC.isBack == false)
                {
                    isVivePadTouchAndGripButton = false;
                }
            }
        }
        //Viveの同時押し（L）
        else if (DB.playerController == DataBridging.PlayerVRController.HTCVive
            && Input.GetButton("HTCViveLTrackPadTouch")
            && DC.isBack)
        {
            DC.isCameraResetDown = isVivePadTouchAndGripButton = true;
        }
        //Viveの同時押し（R）
        else if (DB.playerController == DataBridging.PlayerVRController.HTCVive
            && Input.GetButton("HTCViveRTrackPadTouch")
            && DC.isBack)
        {
            DC.isCameraResetDown = isVivePadTouchAndGripButton = true;
        }

        //元々のカメラリセットボタンを適用
        if (Input.GetButtonDown(DB.inputDict["カメラリセット"])
            || Input.GetButtonDown(DB.inputDict["カメラリセット2"]))
        { DC.isCameraResetDown = true; }

        #endregion
        #region //↑キーを仮想化する前のViveのパッドタッチ+中指ボタン同時押し　を　Down化

        ////まず、前フレで押されていたら どちらか離しているかどうかを判定
        //if (isVivePadTouchAndGripButton)
        //{
        //    //ViveLのパッドかグリップが離れていれば 解除
        //    if (DB.playerController == DataBridging.PlayerVRController.HTCVive
        //        && (Input.GetButton("HTCViveLTrackPadTouch") == false
        //        || Input.GetButton(DB.inputDict["切り替え・バック"]) == false))
        //    {
        //        isVivePadTouchAndGripButton = false;
        //    }
        //    //ViveRのパッドかグリップが離れていれば 解除
        //    else if (DB.playerController == DataBridging.PlayerVRController.HTCViveR
        //        && (Input.GetButton("HTCViveRTrackPadTouch") == false
        //        || Input.GetButton(DB.inputDict["切り替え・バック"]) == false))
        //    {
        //        isVivePadTouchAndGripButton = false;
        //    }
        //}
        ////Viveの同時押し（L）
        //else if (DB.playerController == DataBridging.PlayerVRController.HTCVive
        //    && Input.GetButton("HTCViveLTrackPadTouch")
        //    && Input.GetButton(DB.inputDict["切り替え・バック"]))
        //{
        //    DC.isCameraResetDown = isVivePadTouchAndGripButton = true;
        //}
        ////Viveの同時押し（R）
        //else if (DB.playerController == DataBridging.PlayerVRController.HTCViveR
        //    && Input.GetButton("HTCViveRTrackPadTouch")
        //    && Input.GetButton(DB.inputDict["切り替え・バック"]))
        //{
        //    DC.isCameraResetDown = isVivePadTouchAndGripButton = true;
        //}
        //#endregion

        ////元々のカメラリセットボタンを適用
        //if (Input.GetButtonDown(DB.inputDict["カメラリセット"]))
        //{ DC.isCameraResetDown = true; }

        #endregion

        #endregion

        #region sentakuAxisYはXBOXとそれ以外（スティック）は数値逆にする
        if (DB.playerController == DataBridging.PlayerVRController.HTCVive)
        {
            //HTCViveコントローラー（数値Xboxとたぶん逆　と　isVivePad）
            DC.sentakuAxisY = DC.sentakuAxisY * -1;
        }
        else if (DB.playerController == DataBridging.PlayerVRController.OculusTouch)
        {
            //OculusTouchコントローラー（数値Xboxと逆）
            DC.sentakuAxisY = DC.sentakuAxisY * -1;
        }
        else if (DB.playerController == DataBridging.PlayerVRController.WindowsMR)
        {
            //MRコントローラー（数値Xboxと逆）
            DC.sentakuAxisY = DC.sentakuAxisY * -1;
        }
        #endregion

        #endregion

    }

    void Start()
    {
        ControllerCheckAndSet();
    }

    //接続されているVRとXBOXコントローラー有無で設定
    void ControllerCheckAndSet()
    {
        string tmpModelStr = "";
        //まずXBOX
        var tmpController = DataBridging.PlayerVRController.Xbox;

        //VR接続されてたら
        if (XRSettings.enabled)
        {
            //Vive接続
            if (XRDevice.model.IndexOf("VIVE") >= 0
                || XRDevice.model.IndexOf("Vive") >= 0)
            {
                tmpModelStr += "VIVE\n";
                tmpController = DataBridging.PlayerVRController.HTCVive;

                #region コントローラーに"VIVE" の文字がなかったら XBOX
                //（リストなので一旦1行のテキストに変換）
                string[] tmpGetjoystickNames = Input.GetJoystickNames();
                string tmpJoystickNameMargeStr = "";
                for (int i = 0; i < tmpGetjoystickNames.Length; i++)
                { tmpJoystickNameMargeStr += tmpGetjoystickNames[i]; }

                //VIVEコンが刺さって"なかったら" XBOX
                if (tmpJoystickNameMargeStr.IndexOf("VIVE") == -1
                    && tmpJoystickNameMargeStr.IndexOf("Vive") == -1)
                {
                    tmpModelStr += "Xboxコントローラー設定";
                    tmpController = DataBridging.PlayerVRController.Xbox;
                }
                #endregion
            }
            //Oculus接続
            else if (XRDevice.model.IndexOf("OCULUS") >= 0
                || XRDevice.model.IndexOf("Oculus") >= 0)
            {
                tmpModelStr += "Oculus\n";
                tmpController = DataBridging.PlayerVRController.OculusTouch;

                #region コントローラーに"Oculus" の文字がなかったら XBOX
                //（リストなので一旦1行のテキストに変換）
                string[] tmpGetjoystickNames = Input.GetJoystickNames();
                string tmpJoystickNameMargeStr = "";
                for (int i = 0; i < tmpGetjoystickNames.Length; i++)
                { tmpJoystickNameMargeStr += tmpGetjoystickNames[i]; }

                //Oculusコンが刺さって"なかったら" XBOX
                if (tmpJoystickNameMargeStr.IndexOf("OCULUS") == -1
                    && tmpJoystickNameMargeStr.IndexOf("Oculus") == -1)
                {
                    tmpModelStr += "Xboxコントローラー設定";
                    tmpController = DataBridging.PlayerVRController.Xbox;
                }
                #endregion
            }
            //MR接続
            else if (XRDevice.model.IndexOf("MR") >= 0
                || XRDevice.model.IndexOf("Mixed") >= 0)
            {
                tmpModelStr += "WindowsMR\n";
                tmpController = DataBridging.PlayerVRController.WindowsMR;

                #region コントローラーに"WindowsMR" の文字がなかったら XBOX
                //（リストなので一旦1行のテキストに変換）
                string[] tmpGetjoystickNames = Input.GetJoystickNames();
                string tmpJoystickNameMargeStr = "";
                for (int i = 0; i < tmpGetjoystickNames.Length; i++)
                { tmpJoystickNameMargeStr += tmpGetjoystickNames[i]; }

                //WindowsMRコンが刺さって"なかったら" XBOX
                if (tmpJoystickNameMargeStr.IndexOf("WINDOWSMR") == -1 
                    && tmpJoystickNameMargeStr.IndexOf("WindowsMR") == -1
                    && tmpJoystickNameMargeStr.IndexOf("Mixed") == -1)
                {
                    tmpModelStr += "Xboxコントローラー設定";
                    tmpController = DataBridging.PlayerVRController.Xbox;
                }
                #endregion
            }
        }

        #region //XBOXコントローラー判別（VR優先ということで一旦コメントアウト）（これは最後にXBOX判定しているのでXBOX優先）
        ////（リストなので一旦1行のテキストに変換）
        //string[] tmpGetjoystickNames = Input.GetJoystickNames();
        //string tmpJoystickNameMargeStr = "";
        //for (int i = 0; i < tmpGetjoystickNames.Length; i++)
        //{
        //    tmpJoystickNameMargeStr += tmpGetjoystickNames[i];
        //}

        ////XBOXコンが刺さってたら
        //if (tmpJoystickNameMargeStr.IndexOf("Xbox") >= 0)
        //{
        //    tmpModelStr += "Xboxコントローラー 接続";
        //    tmpController = DataBridging.PlayerVRController.Xbox;
        //}
        #endregion

        //設定
        ControllerSetting(tmpController);

        //DC.SubTitleVis(true, tmpModelStr);
    }

    //各コントローラー設定に切り替え
    public void ControllerSetting(DataBridging.PlayerVRController Controller = DataBridging.PlayerVRController.Xbox, bool isForceChange = false)
    {
        //既に同じコントローラーだったら何もしない
        if (DB.playerController == Controller && isForceChange == false) { return; }

        DB.inputDict.Clear();

        //キーボードマウスかXboxに設定(コントローラーとキーボードマウスどちらでも操作できるようにするため、デフォルトはXbox)
        if (Controller == DataBridging.PlayerVRController.Xbox)
        {
            DB.inputDict.Add("決定", "XboxA");
            DB.inputDict.Add("決定2", "_noData");
            DB.inputDict.Add("切り替え・バック", "XboxB");
            DB.inputDict.Add("切り替え・バック2", "_noData");
            DB.inputDict.Add("カメラ切り替え", "XboxY");
            DB.inputDict.Add("ノベルログ", "XboxX");
            DB.inputDict.Add("メニュー・ポーズ", "XboxSTART");
            if (DB.isUserPSControllerFix) { DB.inputDict.Add("メニュー・ポーズ2", "_noData"); }
            else { DB.inputDict.Add("メニュー・ポーズ2", "XboxRStickButton"); }//210515SteamVRで動かす用
            DB.inputDict.Add("セレクト", "XboxBACK");
            DB.inputDict.Add("右手", "XboxRB");
            DB.inputDict.Add("左手", "XboxLB");
            DB.inputDict.Add("移動ポイント前進デジタル", "XboxRB");
            DB.inputDict.Add("移動ポイント後退デジタル", "XboxLB");

            DB.inputDict.Add("移動ポイント前進後退アナログ", "XboxTriggerAxis");
            DB.inputDict.Add("選択左右", "XboxPadXAxis");
            //DB.inputDict.Add("選択上下", "XboxLStickYAxis");
            DB.inputDict.Add("選択上下", "XboxPadYAxis");
            if (DB.isUserPSControllerFix) { DB.inputDict.Add("選択上下2", "_noData"); }
            else { DB.inputDict.Add("選択上下2", "XboxRStickYAxis"); }//210515SteamVRで動かす用
            DB.inputDict.Add("カメラスティック回転X", "XboxLStickXAxis");
            DB.inputDict.Add("カメラスティック回転X2", "_noData");
            DB.inputDict.Add("カメラスティック回転Y", "XboxLStickYAxis");
            DB.inputDict.Add("カメラスティック回転Y2", "_noData");
            DB.inputDict.Add("移動・手・カーソルスティック移動X", "XboxRStickXAxis");
            DB.inputDict.Add("移動・手・カーソルスティック移動Y", "XboxRStickYAxis");
            DB.inputDict.Add("カメラリセット", "XboxLStickButton");
            DB.inputDict.Add("カメラリセット2", "_noData");

            //■エラー防ぎ
            //ViveのTrackPadで上下左右判定取ってからDown待ち仕様用
            DB.inputDict.Add("VivePadDown", "_noData");
            DB.inputDict.Add("VivePadDown2", "_noData");
            //トリガー決定エミュレート用
            DB.inputDict.Add("Axis決定", "_noData");
            DB.inputDict.Add("Axis決定2", "_noData");
            DB.inputDict.Add("Axis切り替え・バック", "_noData");
            DB.inputDict.Add("Axis切り替え・バック2", "_noData");
            DB.inputDict.Add("Axisメニュー・ポーズ", "_noData");
            DB.inputDict.Add("Axisメニュー・ポーズ2", "_noData");
            DB.inputDict.Add("AxisVivePadDown", "_noData");
            DB.inputDict.Add("AxisVivePadDown2", "_noData");

            //DB.inputDict["決定"] = "XboxA";
            //DB.inputDict["切り替え・バック"] = "XboxB";
            //DB.inputDict["カメラ切り替え"] = "XboxY";
            //DB.inputDict["ノベルログ"] = "XboxX";
            //DB.inputDict["メニュー・ポーズ"] = "XboxSTART";
            //DB.inputDict["セレクト"] = "XboxBACK";
            //DB.inputDict["右手"] = "XboxRB";
            //DB.inputDict["左手"] = "XboxLB";
            //DB.inputDict["移動ポイント前進デジタル"] = "XboxRB";
            //DB.inputDict["移動ポイント後退デジタル"] = "XboxLB";

            //DB.inputDict["移動ポイント前進後退アナログ"] = "XboxTriggerAxis";
            //DB.inputDict["選択左右"] = "XboxPadXAxis";
            //DB.inputDict["選択上下"] = "XboxPadYAxis";
            //DB.inputDict["カメラスティック回転X"] = "XboxLStickXAxis";
            //DB.inputDict["カメラスティック回転Y"] = "XboxLStickYAxis";
            //DB.inputDict["移動・手・カーソルスティック移動X"] = "XboxRStickXAxis";
            //DB.inputDict["移動・手・カーソルスティック移動Y"] = "XboxRStickYAxis";
            //DB.inputDict["カメラリセット"] = "XboxLStickButton";

            DB.playerController = DataBridging.PlayerVRController.Xbox;

        }
        else if (Controller == DataBridging.PlayerVRController.HTCVive)
        {
            DB.inputDict.Add("決定", "HTCViveLTriggerDown");
            DB.inputDict.Add("決定2", "HTCViveRTriggerDown");
            DB.inputDict.Add("切り替え・バック", "HTCViveLGripButton");
            DB.inputDict.Add("切り替え・バック2", "HTCViveRGripButton");
            DB.inputDict.Add("カメラ切り替え", "_noData");
            DB.inputDict.Add("ノベルログ", "_noData");
            DB.inputDict.Add("メニュー・ポーズ", "HTCViveLMenuButton");
            DB.inputDict.Add("メニュー・ポーズ2", "HTCViveRMenuButton");
            DB.inputDict.Add("移動ポイント前進デジタル", "_noData");
            DB.inputDict.Add("移動ポイント後退デジタル", "_noData");
            DB.inputDict.Add("セレクト", "_noData");
            DB.inputDict.Add("右手", "_noData");
            DB.inputDict.Add("左手", "_noData");

            DB.inputDict.Add("移動ポイント前進後退アナログ", "_noData");
            DB.inputDict.Add("選択左右", "_noData");
            DB.inputDict.Add("選択上下", "HTCViveLTrackPadYAxis");
            DB.inputDict.Add("選択上下2", "HTCViveRTrackPadYAxis");
            DB.inputDict.Add("カメラスティック回転X", "HTCViveLTrackPadXAxis");
            DB.inputDict.Add("カメラスティック回転X2", "HTCViveRTrackPadXAxis");
            DB.inputDict.Add("カメラスティック回転Y", "HTCViveLTrackPadYAxis");
            DB.inputDict.Add("カメラスティック回転Y2", "HTCViveRTrackPadYAxis");
            DB.inputDict.Add("移動・手・カーソルスティック移動X", "_noData");
            DB.inputDict.Add("移動・手・カーソルスティック移動Y", "_noData");
            DB.inputDict.Add("カメラリセット", "_noData");
            DB.inputDict.Add("カメラリセット2", "_noData");

            //ViveのTrackPadで上下左右判定取ってからDown待ち仕様用
            DB.inputDict.Add("VivePadDown", "HTCViveLTrackPadDown");
            DB.inputDict.Add("VivePadDown2", "HTCViveRTrackPadDown");

            //■エラー防ぎ
            //トリガー決定エミュレート用
            DB.inputDict.Add("Axis決定", "_noData");
            DB.inputDict.Add("Axis決定2", "_noData");
            DB.inputDict.Add("Axis切り替え・バック", "_noData");
            DB.inputDict.Add("Axis切り替え・バック2", "_noData");
            DB.inputDict.Add("Axisメニュー・ポーズ", "_noData");
            DB.inputDict.Add("Axisメニュー・ポーズ2", "_noData");
            DB.inputDict.Add("AxisVivePadDown", "_noData");
            DB.inputDict.Add("AxisVivePadDown2", "_noData");

            DB.playerController = DataBridging.PlayerVRController.HTCVive;

        }
        else if (Controller == DataBridging.PlayerVRController.OculusTouch)
        {
            DB.inputDict.Add("決定", "OculusTouchX");
            DB.inputDict.Add("決定2", "OculusTouchA");
            DB.inputDict.Add("切り替え・バック", "OculusTouchLGripButton");
            DB.inputDict.Add("切り替え・バック2", "OculusTouchRGripButton");
            DB.inputDict.Add("カメラ切り替え", "_noData");
            DB.inputDict.Add("ノベルログ", "_noData");
            DB.inputDict.Add("メニュー・ポーズ", "OculusTouchY");
            DB.inputDict.Add("メニュー・ポーズ2", "OculusTouchB");
            DB.inputDict.Add("セレクト", "_noData");
            DB.inputDict.Add("右手", "_noData");
            DB.inputDict.Add("左手", "_noData");
            DB.inputDict.Add("移動ポイント前進デジタル", "_noData");
            DB.inputDict.Add("移動ポイント後退デジタル", "_noData");

            DB.inputDict.Add("移動ポイント前進後退アナログ", "_noData");
            DB.inputDict.Add("選択左右", "_noData");
            DB.inputDict.Add("選択上下", "OculusTouchLStickYAxis");
            DB.inputDict.Add("選択上下2", "OculusTouchRStickYAxis");
            DB.inputDict.Add("カメラスティック回転X", "OculusTouchLStickXAxis");
            DB.inputDict.Add("カメラスティック回転X2", "OculusTouchRStickXAxis");
            DB.inputDict.Add("カメラスティック回転Y", "OculusTouchLStickYAxis");
            DB.inputDict.Add("カメラスティック回転Y2", "OculusTouchRStickYAxis");
            DB.inputDict.Add("移動・手・カーソルスティック移動X", "_noData");
            DB.inputDict.Add("移動・手・カーソルスティック移動Y", "_noData");
            DB.inputDict.Add("カメラリセット", "OculusTouchLStickButton");
            DB.inputDict.Add("カメラリセット2", "OculusTouchRStickButton");

            //■エラー防ぎ　ViveのTrackPadで上下左右判定取ってからDown待ち仕様用
            DB.inputDict.Add("VivePadDown", "_noData");
            DB.inputDict.Add("VivePadDown2", "_noData");

            //トリガー決定エミュレート用
            DB.inputDict.Add("Axis決定", "OculusTouchLTriggerAxis");
            DB.inputDict.Add("Axis決定2", "OculusTouchRTriggerAxis");

            //■エラー防ぎ
            DB.inputDict.Add("Axis切り替え・バック", "_noData");
            DB.inputDict.Add("Axis切り替え・バック2", "_noData");
            DB.inputDict.Add("Axisメニュー・ポーズ", "_noData");
            DB.inputDict.Add("Axisメニュー・ポーズ2", "_noData");
            DB.inputDict.Add("AxisVivePadDown", "_noData");
            DB.inputDict.Add("AxisVivePadDown2", "_noData");

            DB.playerController = DataBridging.PlayerVRController.OculusTouch;
        }
        else if (Controller == DataBridging.PlayerVRController.WindowsMR)
        {
            DB.inputDict.Add("決定", "MRTriggerLPress");
            DB.inputDict.Add("決定2", "MRTriggerRPress");
            DB.inputDict.Add("切り替え・バック", "MRGripLPress");
            DB.inputDict.Add("切り替え・バック2", "MRGripRPress");
            DB.inputDict.Add("カメラ切り替え", "_noData");
            DB.inputDict.Add("ノベルログ", "_noData");
            DB.inputDict.Add("メニュー・ポーズ", "MRMenuL_SteamVR");
            DB.inputDict.Add("メニュー・ポーズ2", "MRMenuR_SteamVR");
            DB.inputDict.Add("セレクト", "_noData");
            DB.inputDict.Add("右手", "_noData");
            DB.inputDict.Add("左手", "_noData");
            DB.inputDict.Add("移動ポイント前進デジタル", "_noData");
            DB.inputDict.Add("移動ポイント後退デジタル", "_noData");

            DB.inputDict.Add("移動ポイント前進後退アナログ", "_noData");
            DB.inputDict.Add("選択左右", "_noData");
            DB.inputDict.Add("選択上下", "MRStickLAxisY");
            DB.inputDict.Add("選択上下2", "MRStickRAxisY");
            DB.inputDict.Add("カメラスティック回転X", "MRStickLAxisX");
            DB.inputDict.Add("カメラスティック回転X2", "MRStickRAxisX");
            DB.inputDict.Add("カメラスティック回転Y", "MRStickLAxisY");
            DB.inputDict.Add("カメラスティック回転Y2", "MRStickRAxisY");
            DB.inputDict.Add("移動・手・カーソルスティック移動X", "_noData");
            DB.inputDict.Add("移動・手・カーソルスティック移動Y", "_noData");
            DB.inputDict.Add("カメラリセット", "MRPadLPress_SteamVR");
            DB.inputDict.Add("カメラリセット2", "MRPadRPress_SteamVR");

            //■エラー防ぎ　ViveのTrackPadで上下左右判定取ってからDown待ち仕様用
            DB.inputDict.Add("VivePadDown", "_noData");
            DB.inputDict.Add("VivePadDown2", "_noData");

            //トリガー決定エミュレート用
            DB.inputDict.Add("Axis決定", "_noData");
            DB.inputDict.Add("Axis決定2", "_noData");

            //■エラー防ぎ
            DB.inputDict.Add("Axis切り替え・バック", "_noData");
            DB.inputDict.Add("Axis切り替え・バック2", "_noData");
            DB.inputDict.Add("Axisメニュー・ポーズ", "_noData");
            DB.inputDict.Add("Axisメニュー・ポーズ2", "_noData");
            DB.inputDict.Add("AxisVivePadDown", "_noData");
            DB.inputDict.Add("AxisVivePadDown2", "_noData");

            DB.playerController = DataBridging.PlayerVRController.WindowsMR;
        }

        Debug.Log(Controller + "で認識");
    }

}
