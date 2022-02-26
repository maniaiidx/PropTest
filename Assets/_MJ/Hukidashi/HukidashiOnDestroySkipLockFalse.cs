using UnityEngine;
using System.Collections;

public class HukidashiOnDestroySkipLockFalse : MonoBehaviour
{
    void OnDestroy() { GameObject.Find("Server").GetComponent<DataCounter>().isSkipLock = false; }
}
