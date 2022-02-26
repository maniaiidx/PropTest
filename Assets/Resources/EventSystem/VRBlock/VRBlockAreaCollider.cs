using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class VRBlockAreaCollider : MonoBehaviour
{
    public DataCounter DC;
    void Start()
    { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }

    void OnTriggerEnter(Collider collider)
    { DC.OnCollisionEnter_VRBlockAreaColliderSphere(collider); }
    void OnTriggerExit(Collider collider)
    { DC.OnCollisionExit_VRBlockAreaColliderSphere(collider); }

}
