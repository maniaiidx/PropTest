using UnityEngine;
using System.Collections;

public class autoSortingOrderMinus1 : MonoBehaviour {

    public GameObject kaoObj = null;
    public GameObject mayugeObj = null;

    public MeshRenderer mayugeMR = null;
    public MeshRenderer kaminokeMR = null;
    public MeshRenderer kaoMR = null;
    

    void Start () {



    }



    void Update () {

        if((Camera.main.worldToCameraMatrix.MultiplyPoint(kaoObj.transform.position).z)
            > (Camera.main.worldToCameraMatrix.MultiplyPoint(mayugeObj.transform.position).z))
        {
            kaoMR.sortingOrder = 0;
        }
        else
        {
            kaoMR.sortingOrder = -2;
        }

        mayugeMR.sortingOrder = -1;
        kaminokeMR.sortingOrder = -2;

    }

}
