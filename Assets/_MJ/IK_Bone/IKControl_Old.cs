using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl_Old : MonoBehaviour
{
    public DataCounter DC;

    protected Animator animator;

    public AnimationCurve curve;

    public bool eyeIKActive, bodyIKActive, headIKActive, rHandIKActive, lHandIKActive;
    public Transform IKLookAtTarget, IKRHandTarget, IKLHandTarget;
    public Transform IKLookAtTargetLeapTarget, IKRHandTargetLeapTarget, IKLHandTargetLeapTarget;

    public float eyeStartWeight, eyeEndWeight, eyeNowWeight;
    public float bodyStartWeight, bodyEndWeight, bodyNowWeight;
    public float headStartWeight, headEndWeight, headNowWeight;
    public float rHandStartWeight, rHandEndWeight, rHandNowWeight;
    public float lHandStartWeight, lHandEndWeight, lHandNowWeight;
    public float weightTime;//IK ONOFFのウェイトタイム
    public float lookAtWeightTime, rHandPosWeightTime, rHandRotWeightTime,//少ない方が遅い
                 lHandPosWeightTime, lHandRotWeightTime;
    float eyeStartTime, bodyStartTime, headStartTime, rHandStartTime, lHandStartTime;

    bool eyeIKOnece, bodyIKOnece, headIKOnece, rHandIKOnece, lHandIKOnece;

    public Toggle IKEyeToggle, IKBodyToggle, IKHeadToggle, IKRHandToggle, IKLHandToggle;




    #region//目だけ独立してLookAt（IKを切っている場合オンリー）
    /*
    //まず目の子にデフォルトLookAtで動くCube（CubeEye）を置く
    //更にその子に智恵理の目と同じだけずれたCube（ChieriCubeEye）を置く
    //その間に、ChieriCubeEyeがデフォルトの動きでちゃんと目を向ける用にGameObject（aida）を置いて、調整する。
    //CubeEyeをデフォルトのLookAtで制御して、ChieriCubeEyeの回転を本元の智恵理の目の回転に代入する

    public GameObject CubeEyeL, CubeEyeR, ChieriCubeEyeL, ChieriCubeEyeR,
                      joint_L_eye00, joint_R_eye00,
                      CubeEyeLookAtTargetObj, CubeEyeLookAtTargetLeapTargetObj;
    public Vector3 defaultChieriEyeLeulerAngles, defaultChieriEyeReulerAngles;

    public bool eyeLookAtBool, eyeLookAtSystemBool;
    public float cubeEyeLookAtWeightTime;
    */
    #endregion

    void Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        animator = GetComponent<Animator>();
        IKLookAtTarget = GameObject.Find("IKLookAtTarget_Old").transform;
        IKRHandTarget = GameObject.Find("IKRHandTarget_Old").transform;
        IKLHandTarget = GameObject.Find("IKLHandTarget_Old").transform;

        IKLookAtTargetLeapTarget = GameObject.Find("IKLookAtTargetLeapTarget_Old").transform;
        IKRHandTargetLeapTarget = GameObject.Find("IKRHandTargetLeapTarget_Old").transform;
        IKLHandTargetLeapTarget = GameObject.Find("IKLHandTargetLeapTarget_Old").transform;

        IKEyeToggle = GameObject.Find("IKEyeToggle").GetComponent<Toggle>();
        IKBodyToggle = GameObject.Find("IKBodyToggle").GetComponent<Toggle>();
        IKHeadToggle = GameObject.Find("IKHeadToggle").GetComponent<Toggle>();
        IKRHandToggle = GameObject.Find("IKRHandToggle").GetComponent<Toggle>();
        IKLHandToggle = GameObject.Find("IKLHandToggle").GetComponent<Toggle>();


        //public系float 初期値入力
        eyeStartWeight = 0; eyeEndWeight = 0.7f;
        bodyStartWeight = 0; bodyEndWeight = 0.7f;
        headStartWeight = 0; headEndWeight = 0.7f;
        rHandStartWeight = 0; rHandEndWeight = 0.7f;
        lHandStartWeight = 0; lHandEndWeight = 0.7f;
        weightTime = 2;//IK ONOFFのウェイト
        lookAtWeightTime = 3; rHandPosWeightTime = 3; rHandRotWeightTime = 3;//少ない方が遅い
        lHandPosWeightTime = 3; lHandRotWeightTime = 3;



        #region//目だけ独立してLookAt（IKを切っている場合オンリー）
        /*
        CubeEyeLookAtTargetObj = GameObject.Find("CubeEyeLookAtTarget").gameObject;
        CubeEyeLookAtTargetLeapTargetObj = GameObject.Find("CubeEyeLookAtTargetLeapTarget").gameObject;


        CubeEyeL = GameObject.Find("CubeEyeL").gameObject;
        CubeEyeR = GameObject.Find("CubeEyeR").gameObject;
        ChieriCubeEyeL = CubeEyeL.transform.FindChild("aida/ChieriCubeEyeL").gameObject;
        ChieriCubeEyeR = CubeEyeR.transform.FindChild("aida/ChieriCubeEyeR").gameObject;
        


        joint_L_eye00 = DC.girl.transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/joint_L_eye00").gameObject;
        joint_R_eye00 = DC.girl.transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/joint_R_eye00").gameObject;
        #region//CubeEyeL初期ローカル座標を保持し、ペアレント後代入
        Vector3 tempPosV3CubeEyeL = CubeEyeL.transform.localPosition;
        Vector3 tempRotV3CubeEyeL = CubeEyeL.transform.localEulerAngles;
        Vector3 tempSclV3CubeEyeL = CubeEyeL.transform.localScale;
        CubeEyeL.transform.parent = joint_L_eye00.transform;
        CubeEyeL.transform.localPosition = tempPosV3CubeEyeL;
        CubeEyeL.transform.localEulerAngles = tempRotV3CubeEyeL;
        CubeEyeL.transform.localScale = tempSclV3CubeEyeL;
        #endregion
        #region//CubeEyeR初期ローカル座標を保持し、ペアレント後代入
        Vector3 tempPosV3CubeEyeR = CubeEyeR.transform.localPosition;
        Vector3 tempRotV3CubeEyeR = CubeEyeR.transform.localEulerAngles;
        Vector3 tempSclV3CubeEyeR = CubeEyeR.transform.localScale;
        CubeEyeR.transform.parent = joint_R_eye00.transform;
        CubeEyeR.transform.localPosition = tempPosV3CubeEyeR;
        CubeEyeR.transform.localEulerAngles = tempRotV3CubeEyeR;
        CubeEyeR.transform.localScale = tempSclV3CubeEyeR;
        #endregion

        defaultChieriEyeLeulerAngles = joint_L_eye00.transform.localEulerAngles;
        defaultChieriEyeReulerAngles = joint_R_eye00.transform.localEulerAngles;

        cubeEyeLookAtWeightTime = 10f;//ウェイトタイム初期値
        */
        #endregion


    }

    void Update()
    {
        #region//デバッグ用IKCheckBox

        if (IKEyeToggle.isOn == false)
        {
            eyeIKActive = false;
        }
        if (IKEyeToggle.isOn == true)
        {
            eyeIKActive = true;
        }

        if (IKBodyToggle.isOn == false)
        {
            bodyIKActive = false;
        }
        if (IKBodyToggle.isOn == true)
        {
            bodyIKActive = true;
        }

        if (IKHeadToggle.isOn == false)
        {
            headIKActive = false;
        }
        if (IKHeadToggle.isOn == true)
        {
            headIKActive = true;
        }

        if (IKRHandToggle.isOn == false)
        {
            rHandIKActive = false;
        }
        if (IKRHandToggle.isOn == true)
        {
            rHandIKActive = true;
        }

        if (IKLHandToggle.isOn == false)
        {
            lHandIKActive = false;
        }
        if (IKLHandToggle.isOn == true)
        {
            lHandIKActive = true;
        }
        #endregion

        //Leap移動させるためにTargetLeapTarget
        IKLookAtTarget.position = Vector3.Lerp(IKLookAtTarget.position, IKLookAtTargetLeapTarget.position,
            lookAtWeightTime * Time.deltaTime);

        IKRHandTarget.position = Vector3.Lerp(IKRHandTarget.position, IKRHandTargetLeapTarget.position,
            rHandPosWeightTime * Time.deltaTime);
        IKLHandTarget.position = Vector3.Lerp(IKLHandTarget.position, IKLHandTargetLeapTarget.position,
            lHandPosWeightTime * Time.deltaTime);




        //回転はLerpにするとグシャグシャになるので一旦切り。数値の入力をLerpにするしかないかも
        //IKRHandTarget.eulerAngles = Vector3.Lerp(IKRHandTarget.eulerAngles, IKRHandTargetLeapTarget.eulerAngles, rHandRotWeightTime * Time.deltaTime);
        //IKLHandTarget.eulerAngles = Vector3.Lerp(IKLHandTarget.eulerAngles, IKLHandTargetLeapTarget.eulerAngles, lHandRotWeightTime * Time.deltaTime);

        //QuaternionでSlerpならいけたっぽい
        IKRHandTarget.rotation = Quaternion.Slerp(IKRHandTarget.rotation, IKRHandTargetLeapTarget.rotation, rHandRotWeightTime * Time.deltaTime);
        IKLHandTarget.rotation = Quaternion.Slerp(IKLHandTarget.rotation, IKLHandTargetLeapTarget.rotation, lHandRotWeightTime * Time.deltaTime);



        #region//目だけ独立してLookAt（IKを切っている場合オンリー）
        /*
        if (eyeLookAtSystemBool == true)//挙動が満足いかないので一旦全切りBoolでとめておく
        {
            //Leap移動
            CubeEyeLookAtTargetObj.transform.position = Vector3.Slerp(
                CubeEyeLookAtTargetObj.transform.position,
                CubeEyeLookAtTargetLeapTargetObj.transform.position,
                cubeEyeLookAtWeightTime * Time.deltaTime);


            if (eyeLookAtBool == true)
            {
                CubeEyeL.transform.LookAt(CubeEyeLookAtTargetObj.transform);
                CubeEyeR.transform.LookAt(CubeEyeLookAtTargetObj.transform);

                joint_L_eye00.transform.eulerAngles = ChieriCubeEyeL.transform.eulerAngles;
                joint_R_eye00.transform.eulerAngles = ChieriCubeEyeR.transform.eulerAngles;
            }
            else
            {
                //Leapでデフォルトに戻る
                joint_L_eye00.transform.localEulerAngles = Vector3.Slerp(
                    defaultChieriEyeLeulerAngles,
                    joint_L_eye00.transform.localEulerAngles,
                    cubeEyeLookAtWeightTime * Time.deltaTime);

                joint_R_eye00.transform.localEulerAngles = Vector3.Slerp(
                    defaultChieriEyeReulerAngles,
                    joint_R_eye00.transform.localEulerAngles,
                    cubeEyeLookAtWeightTime * Time.deltaTime);
            }


        }
        */
        #endregion

    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        //■■■■■■■IKのSetLookAtPositionはひとつしかない（目・頭・体を別々に指定できない）
        //LookAtスクリプトで別々にしようと思ったが、モーションに加えるという形でLookAtスクリプトは使えないと思われる
        //(モーションの現在の値と向かせたい方向の値を計算させるとかすればいけるかも知れないが複雑そう)
        animator.SetLookAtPosition(IKLookAtTarget.position);

        //SetLookAtWeightのclampWeightがちゃんと動いてない気がする。
        //1以上にしても動く。でも0よりは制限される

        #region//eye
        //ONになった瞬間の時間取得後、そこから過ぎた時間（diff）を指定した時間（time）で割って、start（0）からend（1）までの数値（rate）をだす
        //rateを変数（eyeNowWeight）に代入することで、time分時間かけてweightを1にすることができた。そこにアニメーションカーブも取り入れた。
        if (eyeIKActive == true)
        {
            if (eyeIKOnece == false)
            {
                eyeStartTime = Time.timeSinceLevelLoad;
                eyeIKOnece = true;
            }
            var diff = Time.timeSinceLevelLoad - eyeStartTime;
            var rate = diff / weightTime;
            var curvePos = curve.Evaluate(rate);

            eyeNowWeight = Mathf.Lerp(eyeStartWeight, eyeEndWeight, curvePos);

            animator.SetLookAtWeight(1, bodyNowWeight, headNowWeight, eyeNowWeight, clampWeight: 0.9f);
        }

        //OFFになったら↑の逆をする
        if (eyeIKActive == false)
        {
            if (eyeIKOnece == true)
            {
                eyeStartTime = Time.timeSinceLevelLoad;
                eyeIKOnece = false;
            }

            if (eyeNowWeight != 0)
            {
                var diff = Time.timeSinceLevelLoad - eyeStartTime;
                var rate = diff / weightTime;
                var curvePos = curve.Evaluate(rate);

                eyeNowWeight = Mathf.Lerp(eyeEndWeight, eyeStartWeight, curvePos);

                animator.SetLookAtWeight(1, bodyNowWeight, headNowWeight, eyeNowWeight, clampWeight: 0.9f);
            }

        }
        #endregion

        #region//body
        //ONになった瞬間の時間取得後、そこから過ぎた時間（diff）を指定した時間（time）で割って、start（0）からend（1）までの数値（rate）をだす
        //rateを変数（headNowWeight）に代入することで、time分時間かけてweightを1にすることができた。そこにアニメーションカーブも取り入れた。
        if (bodyIKActive == true)
        {
            if (bodyIKOnece == false)
            {
                bodyStartTime = Time.timeSinceLevelLoad;
                bodyIKOnece = true;
            }
            var diff = Time.timeSinceLevelLoad - bodyStartTime;
            var rate = diff / weightTime;
            var curvePos = curve.Evaluate(rate);

            bodyNowWeight = Mathf.Lerp(bodyStartWeight, bodyEndWeight, curvePos);

            animator.SetLookAtWeight(1, bodyNowWeight, headNowWeight, eyeNowWeight, clampWeight: 0.9f);
        }

        //OFFになったら↑の逆をする
        if (bodyIKActive == false)
        {
            if (bodyIKOnece == true)
            {
                bodyStartTime = Time.timeSinceLevelLoad;
                bodyIKOnece = false;
            }

            if (bodyNowWeight != 0)
            {
                var diff = Time.timeSinceLevelLoad - bodyStartTime;
                var rate = diff / weightTime;
                var curvePos = curve.Evaluate(rate);

                bodyNowWeight = Mathf.Lerp(bodyEndWeight, bodyStartWeight, curvePos);

                animator.SetLookAtWeight(1, bodyNowWeight, headNowWeight, eyeNowWeight, clampWeight: 0.9f);
            }

        }
        #endregion

        #region//head
        //ONになった瞬間の時間取得後、そこから過ぎた時間（diff）を指定した時間（time）で割って、start（0）からend（1）までの数値（rate）をだす
        //rateを変数（headNowWeight）に代入することで、time分時間かけてweightを1にすることができた。そこにアニメーションカーブも取り入れた。
        if (headIKActive == true)
        {
            if (headIKOnece == false)
            {
                headStartTime = Time.timeSinceLevelLoad;
                headIKOnece = true;
            }
            var diff = Time.timeSinceLevelLoad - headStartTime;
            var rate = diff / weightTime;
            var curvePos = curve.Evaluate(rate);

            headNowWeight = Mathf.Lerp(headStartWeight, headEndWeight, curvePos);

            animator.SetLookAtWeight(1, bodyNowWeight, headNowWeight, eyeNowWeight, clampWeight: 0.9f);
        }

        //OFFになったら↑の逆をする
        if (headIKActive == false)
        {
            if (headIKOnece == true)
            {
                headStartTime = Time.timeSinceLevelLoad;
                headIKOnece = false;
            }

            if (headNowWeight != 0)
            {
                var diff = Time.timeSinceLevelLoad - headStartTime;
                var rate = diff / weightTime;
                var curvePos = curve.Evaluate(rate);

                headNowWeight = Mathf.Lerp(headEndWeight, headStartWeight, curvePos);

                animator.SetLookAtWeight(1, bodyNowWeight, headNowWeight, eyeNowWeight, clampWeight: 0.9f);
            }

        }
        #endregion

        #region//rHand
        if (rHandIKActive == true)
        {
            if (rHandIKOnece == false)
            {
                rHandStartTime = Time.timeSinceLevelLoad;
                rHandIKOnece = true;
            }
            var diff = Time.timeSinceLevelLoad - rHandStartTime;
            var rate = diff / weightTime;
            var curvePos = curve.Evaluate(rate);

            rHandNowWeight = Mathf.Lerp(rHandStartWeight, rHandEndWeight, curvePos);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rHandNowWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rHandNowWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, IKRHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, IKRHandTarget.rotation);
        }

        if (rHandIKActive == false)
        {
            if (rHandIKOnece == true)
            {
                rHandStartTime = Time.timeSinceLevelLoad;
                rHandIKOnece = false;
            }
            if (rHandNowWeight != 0)
            {
                var diff = Time.timeSinceLevelLoad - rHandStartTime;
                var rate = diff / weightTime;
                var curvePos = curve.Evaluate(rate);

                rHandNowWeight = Mathf.Lerp(rHandEndWeight, rHandStartWeight, curvePos);

                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rHandNowWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rHandNowWeight);
                animator.SetIKPosition(AvatarIKGoal.RightHand, IKRHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, IKRHandTarget.rotation);
            }
        }
        #endregion

        #region//lHand
        if (lHandIKActive == true)
        {
            if (lHandIKOnece == false)
            {
                lHandStartTime = Time.timeSinceLevelLoad;
                lHandIKOnece = true;
            }
            var diff = Time.timeSinceLevelLoad - lHandStartTime;
            var rate = diff / weightTime;
            var curvePos = curve.Evaluate(rate);

            lHandNowWeight = Mathf.Lerp(lHandStartWeight, lHandEndWeight, curvePos);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lHandNowWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, lHandNowWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, IKLHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, IKLHandTarget.rotation);
        }

        if (lHandIKActive == false)
        {
            if (lHandIKOnece == true)
            {
                lHandStartTime = Time.timeSinceLevelLoad;
                lHandIKOnece = false;
            }
            if (lHandNowWeight != 0)
            {
                var diff = Time.timeSinceLevelLoad - lHandStartTime;
                var rate = diff / weightTime;
                var curvePos = curve.Evaluate(rate);

                lHandNowWeight = Mathf.Lerp(lHandEndWeight, lHandStartWeight, curvePos);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lHandNowWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, lHandNowWeight);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, IKLHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, IKLHandTarget.rotation);
            }
        }
        #endregion

    }
}
