using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBCable02020_PareUSBCable_NoboriGoal : MonoBehaviour
{

    public DataCounter DC;
    void Awake()
    { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }

    //手で掴んだら
    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.parent == DC.AN_Hand_R_RootTrs
            && DC.AN_RGrapSignEnum == DataCounter.AN_GrapSignEnum.掴んでいる)
        {
            DC.isD2130Goal = true;
        }
        else if (collider.transform.parent == DC.AN_Hand_L_RootTrs
            && DC.AN_LGrapSignEnum == DataCounter.AN_GrapSignEnum.掴んでいる)
        {
            DC.isD2130Goal = true;
        }
    }

    //からだが触れても
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        { DC.isD2130Goal = true; }
    }

}
