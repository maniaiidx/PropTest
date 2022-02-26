using UnityEngine;
using System.Collections;

public class PlayerTargetCollider : MonoBehaviour
{

    public DataCounter DC;

    IEnumerator Start()
    {
        //衝突フラグを送る為に取得
        while (GameObject.Find("Server") == null) { yield return new WaitForSeconds(0.5f); }
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        yield break;
    }

    // ぶつかったら
    void OnTriggerEnter(Collider coll)
    { StartCoroutine(DC.PlayerTargetOnTriggerEnter(coll)); }

    void OnTriggerExit(Collider coll)
    { StartCoroutine(DC.PlayerTargetOnTriggerExit(coll)); }

    void OnTriggerStay(Collider coll)
    { DC.PlayerTargetOnTriggerStay(coll); }

}
