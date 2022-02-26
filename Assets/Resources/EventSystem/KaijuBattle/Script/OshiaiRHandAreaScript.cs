using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OshiaiRHandAreaScript : MonoBehaviour
{

    public DataCounter DC;

    void Start()
    { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }
    
    void OnCollisionEnter(Collision collision)
    {
        DC.KB_RHandAreaCollisionEnter(collision);
    }
    void OnCollisionExit(Collision coll)
    {
    }

}
