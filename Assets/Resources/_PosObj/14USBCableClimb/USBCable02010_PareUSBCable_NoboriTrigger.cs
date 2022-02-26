using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBCable02010_PareUSBCable_NoboriTrigger : MonoBehaviour
{
    public DataCounter DC;
    void Awake()
    { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        { DC.isD2130Omoi = true; }
    }
    //void OnTriggerStay(Collider collider)
    //{ }
    //void OnTriggerExit(Collider collider)
    //{ }

}
