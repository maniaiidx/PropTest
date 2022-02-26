using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBCable02013_PareUSBCable_NoboriTrigger : MonoBehaviour
{
    public DataCounter DC;
    void Awake()
    { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }

    //手で掴んだら
    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.parent == DC.AN_Hand_R_RootTrs
            && DC.AN_RGrapSignEnum == DataCounter.AN_GrapSignEnum.掴んでいる)
        { goto 掴んだ; }

        else if (collider.transform.parent == DC.AN_Hand_L_RootTrs
            && DC.AN_LGrapSignEnum == DataCounter.AN_GrapSignEnum.掴んでいる)
        { goto 掴んだ; }

        return;


        掴んだ:
        DC.isD2130Kuruma = true;

    }
    //からだが触れても
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        { DC.isD2130Kuruma = true; }
    }

}
