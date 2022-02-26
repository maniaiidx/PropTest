using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eiffel010_PareEiffel_EV010Collider : MonoBehaviour
{

    public DataCounter DC;
    void Awake()
    { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            DC.isB2090Noboreteru = true;
        }
    }
    void OnTriggerStay(Collider collider)
    { }
    void OnTriggerExit(Collider collider)
    { }

}
