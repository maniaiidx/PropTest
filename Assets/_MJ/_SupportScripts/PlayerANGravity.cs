using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;//FuncやAction、Mathを使うために必要

public class PlayerANGravity : MonoBehaviour
{
    public DataCounter DC;
    public DataBridging DB;
    Rigidbody rb;

    //■オリジナル重力と空気抵抗の調整値(DCから読み出すが、再生後具合を確かめられるようにこちらに数値)
    public float dragAdjust = 2.5f;
    public float gravityAdjust = 5;

    void Awake()
    {

    }
    void Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        DB = GameObject.Find("DataBridging").GetComponent<DataBridging>();
        rb = this.GetComponent<Rigidbody>();
        //値読み込み
        GameObject PlayerRigidbodyColliderObj = Resources.Load("EventSystem/Asinobori/Prefab/PlayerRigidbodyColliderObj") as GameObject;
        Rigidbody tmpPlayerRigidBody
            = PlayerRigidbodyColliderObj.GetComponent<Rigidbody>();
        //値渡し（もっと一発でやる方法はないものか）
        rb.mass = tmpPlayerRigidBody.mass;
        rb.drag = tmpPlayerRigidBody.drag;
        rb.angularDrag = tmpPlayerRigidBody.angularDrag;
        rb.useGravity = tmpPlayerRigidBody.useGravity;
        rb.isKinematic = tmpPlayerRigidBody.isKinematic;
        rb.interpolation = tmpPlayerRigidBody.interpolation;
        rb.collisionDetectionMode = tmpPlayerRigidBody.collisionDetectionMode;
        rb.constraints = tmpPlayerRigidBody.constraints;

        dragAdjust = DC.dragAdjust;
        gravityAdjust = DC.gravityAdjust;

        //■体重計算してRigidBodyに
        if (DC.nowPlayerTaizyuuFloat != DB.taizyuuFloat * (float)Math.Pow(DC.nowPlayerLocalScale.x, 3))
        {
            DC.nowPlayerTaizyuuFloat = DB.taizyuuFloat * (float)Math.Pow(DC.nowPlayerLocalScale.x, 3);
            rb.mass = DC.nowPlayerTaizyuuFloat;
        }
    }
    float AN_Drag;
    Vector3 AN_Gravity;
    

    void FixedUpdate()
    {
        //■体重計算してRigidBodyに
        if (DC.nowPlayerTaizyuuFloat != DB.taizyuuFloat * (float)Math.Pow(DC.nowPlayerLocalScale.x, 3))
        {
            DC.nowPlayerTaizyuuFloat = DB.taizyuuFloat * (float)Math.Pow(DC.nowPlayerLocalScale.x, 3);
            rb.mass = DC.nowPlayerTaizyuuFloat;
        }
        //■オリジナル重力と空気抵抗を大きさから計算（tmpで直接数値を調整）
        if (AN_Drag != (dragAdjust * (100f / DC.GameObjectsTrs.localScale.x)) * (float)Math.Pow(DC.nowPlayerLocalScale.x, 3))
        { AN_Drag = (dragAdjust * (100f / DC.GameObjectsTrs.localScale.x)) * (float)Math.Pow(DC.nowPlayerLocalScale.x, 3); }
        if (AN_Gravity.y != (gravityAdjust * -9.81f) * DC.nowPlayerLocalScale.x)
        { AN_Gravity = new Vector3(0, (gravityAdjust * -9.81f) * DC.nowPlayerLocalScale.x, 0); }

        // 空気抵抗を与える
        rb.AddForce((-AN_Drag * DC.GameObjectsTrs.localScale.x) * rb.velocity);
        // 重力加速度を与える
        rb.AddForce(AN_Gravity * DC.GameObjectsTrs.localScale.x, ForceMode.Acceleration);
    }

}
