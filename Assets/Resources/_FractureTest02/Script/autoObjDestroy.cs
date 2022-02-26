using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoObjDestroy : MonoBehaviour {

    public float autoDelTime = 1;
	
	IEnumerator Start ()
    {
        yield return new WaitForSeconds(autoDelTime);
        if (gameObject) { Destroy(gameObject); }
	}
}
