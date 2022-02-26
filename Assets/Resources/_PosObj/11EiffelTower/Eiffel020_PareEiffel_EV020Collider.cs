using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eiffel020_PareEiffel_EV020Collider : MonoBehaviour
{

    public DataCounter DC;
    void Awake()
    { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }

    //void OnTriggerEnter(Collider collider)
    //{ }
    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.parent == DC.AN_Hand_R_RootTrs
            && DC.AN_RGrapSignEnum == DataCounter.AN_GrapSignEnum.掴んでいる)
        {
            DC.isB2090Goal = true;
        }
        else if (collider.transform.parent == DC.AN_Hand_L_RootTrs
            && DC.AN_LGrapSignEnum == DataCounter.AN_GrapSignEnum.掴んでいる)
        {
            DC.isB2090Goal = true;
        }
    }
    //void OnTriggerExit(Collider collider)
    //{ }

}
