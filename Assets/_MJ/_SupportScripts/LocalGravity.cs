using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGravity : MonoBehaviour
{
    public Vector3 localGravity = new Vector3(0, -9.81f, 0);
    Rigidbody rb;

    void Awake()
    { rb = this.GetComponent<Rigidbody>(); }

    void FixedUpdate()
    { rb.AddForce(localGravity, ForceMode.Acceleration); }

}
