using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaraSE : MonoBehaviour
{
    public DataCounter DC;
    void Awake() { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }


    //デスクに置き
    void SE_CupPut() { DC.SEPlay("cup-put1", this.gameObject, 0.4f); }

}
