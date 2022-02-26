using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEnter_Player : MonoBehaviour
{
    public DataCounter DC;
    void Awake()
    { DC = GameObject.Find("Server").GetComponent<DataCounter>(); }

    void OnCollisionEnter(Collision collision)
    { StartCoroutine(DC.OnCollisionEnter_Player(collision)); }
    void OnCollisionStay(Collision collision)
    { DC.OnCollisionStay_Player(collision); }
    void OnCollisionExit(Collision collision)
    { StartCoroutine(DC.OnCollisionExit_Player(collision)); }

    void OnTriggerEnter(Collider collider)
    { DC.OnTriggerEnter_Player(collider); }
    void OnTriggerStay(Collider collider)
    { DC.OnTriggerStay_Player(collider); }
    void OnTriggerExit(Collider collider)
    { DC.OnTriggerExit_Player(collider); }

}
