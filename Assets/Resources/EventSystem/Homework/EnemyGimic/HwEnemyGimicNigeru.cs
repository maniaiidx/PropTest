using UnityEngine;
using System.Collections;

public class HwEnemyGimicNigeru : MonoBehaviour
{
    public DataCounter DC;

    //�M�~�b�N���Z�b�g�p�̃f�[�^��ǂݎ���Ă����āAHwEnemyObj�ċN���Ƀ��Z�b�g
    public GameObject HwEnemyObj;//�I�u�W�F�N�g�擾
    public bool hwEnemyActiveCheckBool;//HwEnemy�ċN���t���[���������Z�b�g�����s����p

    //���Z�b�g����ϐ�
    public Vector3 defPos;
    public bool hwGimicStageMoveBarTouchBool;

    void Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        HwEnemyObj = transform.GetChild(1).gameObject;
        defPos = transform.position;
    }

    void Update()
    {
        #region//HwEnemy�̃A�N�e�B�u��Ԃ�ON���t���[���������Z�b�g�����s�i�萻OnEnable�j
        //HwEnemy�A�N�e�B�u�Ȃ�true
        if (hwEnemyActiveCheckBool == false && HwEnemyObj.activeSelf == true)
        {
            hwEnemyActiveCheckBool = true;

            //���Z�b�g
            transform.position = defPos;
            hwGimicStageMoveBarTouchBool = false;
        }

        //��A�N�e�B�u�Ȃ�false
        if (hwEnemyActiveCheckBool == true && HwEnemyObj.activeSelf == false)
        {
            hwEnemyActiveCheckBool = false;
        }
        #endregion        
    }

    void OnTriggerStay(Collider col)
    {
        //�ǂ�������X�s�[�h���}�C�i�X�œ�����
        if (HwEnemyObj.activeSelf == true)
        { DC.HwEnemyGimicOikakeru(this.gameObject.transform, col, -0.05f, ref hwGimicStageMoveBarTouchBool); }
    }


}

