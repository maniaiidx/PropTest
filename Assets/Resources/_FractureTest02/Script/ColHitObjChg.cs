using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColHitObjChg : MonoBehaviour
{

    public GameObject
        ChgPrefab;

    bool
        isChg;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "ChieriCollider" && isChg == false)
        { Chg(); isChg = true; }
    }

    void Chg()
    {
        GameObject chgObj = Instantiate(ChgPrefab, transform.parent);
        chgObj.transform.position = transform.position;
        chgObj.transform.rotation = transform.rotation;
        chgObj.transform.localScale = transform.localScale;
        Destroy(gameObject);
        Debug.Log("aa");
    }
}
