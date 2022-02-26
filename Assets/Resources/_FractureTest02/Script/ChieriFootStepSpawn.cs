using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChieriFootStepSpawn : MonoBehaviour
{

    [System.Serializable]
    public struct footSpawnObjs
    {
        public GameObject Obj;
        //public Vector3 offsetLocalPos;
        public bool 
            isPosYWorldZero, isRotYOnly;
        public float autoDelTime;
    }

    public footSpawnObjs[]
        rFootSpawnObjs,
        lFootSpawnObjs;

    public bool
        isFootStepObj = true,
        isFootStepDecal = true;

}
