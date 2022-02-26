using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardRailCrash : MonoBehaviour {

    GameObject
        GuardRail,
        GuardRail_Koware;
    bool
        isKoware = false;


    void Start ()
    {
        GuardRail = transform.Find("GuardRail").gameObject;
        GuardRail_Koware = transform.Find("GuardRail_Koware").gameObject;
        Resets();
    }
	
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "ChieriCollider")
        {
            if(isKoware == false)
            {
                isKoware = true;
                Crash();
            }
        }
    }

    public void Resets()
    {
        GuardRail.SetActive(true);
        GuardRail_Koware.SetActive(false);
        isKoware = false;
    }
    void Crash()
    {
        GuardRail.SetActive(false);
        GuardRail_Koware.SetActive(true);
    }
}
