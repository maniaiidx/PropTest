using UnityEngine;
using System.Collections;

public class HwEnemyGimicRhythmUp : MonoBehaviour
{
    public DataCounter DC;


    //�������Ŏ��̏�����HwEnemy��

    //�M�~�b�N���Z�b�g�p�̃f�[�^��ǂݎ���Ă����āAHwEnemyObj�ċN���Ƀ��Z�b�g
    public GameObject HwEnemyObj;//�I�u�W�F�N�g�擾

    //�u���b�N�������[�u�p�ϐ�
    float
        moveX = 6f, moveY = 6f;
    bool
        isHwPointStay;

    void Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        HwEnemyObj = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        //HwEnemy�A�N�e�B�u�Ȃ� //�������Ă���Ƃ��͓����Ȃ�
        if (HwEnemyObj.activeSelf == true &&
            isHwPointStay == false)
        {
            transform.Translate(moveX * Time.deltaTime, moveY * Time.deltaTime, 0);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "ShopStgBar" || col.name == "MainStgBar")
        { moveY = -moveY; }
        if (col.name == "NextStgBar" || col.name == "PreStgBar")
        { moveX = -moveX; }

        Debug.Log(col.name);

        if (col.name.IndexOf("HwPoint") >= 0)
        { isHwPointStay = true; }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.name.IndexOf("HwPoint") >= 0)
        { isHwPointStay = false; }

    }

}

