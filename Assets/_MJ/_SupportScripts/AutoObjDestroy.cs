using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoObjDestroy : MonoBehaviour
{
    public float destroyTime = 3;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(destroyTime);

        DestroyImmediate(this.gameObject);

        yield break;
    }

}
