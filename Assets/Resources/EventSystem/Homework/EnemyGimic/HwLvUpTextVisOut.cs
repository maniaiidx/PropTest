using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;//DOTween

public class HwLvUpTextVisOut : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
        transform.DOLocalMove(new Vector3(0, 1, 0), 2)
            .OnComplete(() => 
            {
                Destroy(transform.parent.gameObject);
            });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
