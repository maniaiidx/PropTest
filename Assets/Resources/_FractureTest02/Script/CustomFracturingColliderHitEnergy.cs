using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFracturingColliderHitEnergy : MonoBehaviour
{

    #region UTL拡張で爆発設定をする用変数
    public bool //これがtrueならその設定でHit時爆発おこす
        isUTLImpactSetting = false;
    public GameObject 
        posObj;
    public float
        impactForce,
        impactRadius;
    public bool
        bAlsoImpactFreeChunks;

    #endregion

    void OnTriggerEnter(Collider collider)
    {
        //破片スクリプトがあれば解除命令（Impactを利用）
        if (collider.GetComponent<FracturedChunk>() != null)
        {
            FracturedChunk tempChunk = collider.GetComponent<FracturedChunk>();

            //UTLのImpact設定ONなら
            if (isUTLImpactSetting)
            {
                tempChunk.Impact(
                    posObj.transform.position //爆発地点
                    , impactForce //力
                    , impactRadius //範囲
                    , bAlsoImpactFreeChunks);
            }
            else//なしなら今まで通り 無力Impact
            {
                tempChunk.Impact(
                    collider.transform.position //爆発地点
                    , 0f //力
                    , 0f //範囲
                    , false);
            }
        }


        if (collider.tag == "FracturedObj" && isUTLImpactSetting == false)//UTL設定オフ前提
        {
            //破片化（Detach）されていれば、進行方向へエネルギー送る
            if (collider.GetComponent<FracturedChunk>().IsDetachedChunk)
            {
                collider.attachedRigidbody.AddForce(direction * (energy / 1500), ForceMode.Force);
            }
        }

    }


    //void OnTriggerStay(Collider collider)
    //{
    //    //埋まり対策で、埋まってたら本体と同じだけ移動させる。
    //    if (collider.tag == "FracturedObj")
    //    {
    //        //破片化（Detach）されていれば
    //        if (collider.GetComponent<FracturedChunk>().IsDetachedChunk)
    //        { collider.transform.position += direction; }
    //    }

    //}

    #region 質量x速度で運動量を出す
    Vector3
        nowPos,
        prevPos,
        direction;

    public float
        mass = 50,
        distance = 0,
        speed = 0,
        energy = 0;

    private void Start()
    { nowPos = prevPos = transform.position; }

    private void FixedUpdate()
    {
        //位置取得
        nowPos = transform.position;

        //動いてたら計算 else動いてなければゼロ
        if (nowPos != prevPos)
        {
            //1フレーム前と今の位置
            distance
                = Vector3.Distance(nowPos, prevPos);

            //速度（速さ）＝ 距離 ÷ 時間
            speed =
                distance / (1 * Time.deltaTime);

            //運動量 = 質量 * 速度
            energy =
                mass * speed;

            //求めたい方向 ＝ 求めたい方向の座標 – 現在の座標
            direction =
                nowPos - prevPos;
        }
        else
        {
            if (energy != 0)//ゼロじゃなければゼロ
            { distance = speed = energy = 0; direction = Vector3.zero; }
        }

        //位置取得
        prevPos = transform.position;
    }




    #endregion


}
