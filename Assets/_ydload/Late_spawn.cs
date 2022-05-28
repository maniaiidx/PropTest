using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Late_spawn : MonoBehaviour
{
    //public float LateTime = 0f;
    //[Header("子供がMesh,Particleの順にぶら下がっている前提")]
    //public float LateTime_Mesh = 3f;
    //public float LateTime_Particle = 1f;
    //public MeshRenderer Late_Mesh;
    //public ParticleSystem Late_Particle;
    //private GameObject ChildMesh;
    //private GameObject ChildParticle;
    //[Header("瓦礫パラメータ")]
    //// プレハブ格納用
    //public GameObject MeshPrefab;
    //// 生成位置
    //public Vector3 MeshSpawnPos;
    //public Vector3 ParticleSpawnPos;
    //public Transform MeshSpawnTrs;
    //public Transform ParticleSpawnTrs;
    //[Header("煙パラメータ")]
    //public GameObject ParticlePrefab;

    //// 生成回転
    //public Vector3 MeshSpawnRotation;
    //public Vector3 ParticleSpawnRotation;
    //Inspectorに表示される
    [SerializeField]
    public SpawnStructArray[] SpawnObject = new SpawnStructArray[0];
    //プロパティStruct
    [System.Serializable]
    public class SpawnStructArray
    {
        public GameObject
            SpawnPrefab;
        public float
            LateTime;
        public Vector3
            Position;
        public Vector3
            Rotation;
        public Vector3
            Scale;
        public SpawnStructArray()
        {
            SpawnPrefab = null;
            LateTime = 0;
            Position = new Vector3(0, 0, 0);
            Rotation = new Vector3(0, 0, 0);
            Scale = new Vector3(1, 1, 1);
        }
    }
   

    // Start is called before the first frame update
    void Start()
    {
        ////一つ以上指定あれば（構造体）
        //if (1 <= SpawnObject.Length)
        //{
        //    for (int mv = 0; mv < SpawnObject.Length; mv++)
        //    {
        //        if (SpawnObject[mv].SpawnPrefab != null)
        //        {
        //            StartCoroutine(LateSpawn(mv));
        //        }
        //    }
        //}

        //StartCoroutine(LateSpawn_Particle());
        //子供取得。まずMesh、次にParticleの順にぶら下がっていること
        //ChildMesh = transform.GetChild(0).gameObject;
        //ChildParticle = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LateSpawn(int mv)
    {
        Debug.Log("LateSpawn_CountStart");
        float time = SpawnObject[mv].LateTime;
        while (time > 0)
        {
            time -= Time.deltaTime;

            yield return null;  // 必須
        }
        //Late_time経過後
        // プレハブを指定位置に生成
        GameObject obj = Instantiate(SpawnObject[mv].SpawnPrefab, this.transform);
        //GameObject obj = Instantiate(SpawnObject[mv].SpawnPrefab, SpawnObject[mv].Position, Quaternion.Euler(SpawnObject[mv].Rotation), this.transform);
        obj.transform.localScale = SpawnObject[mv].Scale;
        obj.transform.localPosition = SpawnObject[mv].Position;
        obj.transform.localRotation = Quaternion.Euler(SpawnObject[mv].Rotation);
    }

    //private IEnumerator LateSpawn_Particle()
    //{
    //    Debug.Log("LateSpawn_Mesh");
    //    float time = LateTime_Particle;
    //    while (time > 0)
    //    {
    //        time -= Time.deltaTime;

    //        yield return null;  // 必須
    //    }
    //    //Late_time経過後
    //    //Late_Mesh.enabled = true;
    //    //Late_Particle.Play();
    //    //ChildParticle.SetActive(true);
    //    // プレハブを指定位置に生成
    //    Instantiate(ParticlePrefab, ParticleSpawnTrs.position, ParticleSpawnTrs.rotation, this.transform.parent);
    //    //Quaternion.Euler(ParticleSpawnRotation)
    //}
    //private IEnumerator LateSpawn()
    //{
    //    Debug.Log("LateSpawn");
    //    float time = LateTime;
    //    while (time > 0)
    //    {
    //        time -= Time.deltaTime;

    //        yield return null;  // 必須
    //    }
    //    //Late_time経過後
    //    gameObject.SetActive(true);
    //    //Late_Particle.Play();
    //}
    public void LateStart()
    {
        //一つ以上指定あれば（構造体）
        if (1 <= SpawnObject.Length)
        {
            for (int mv = 0; mv < SpawnObject.Length; mv++)
            {
                if (SpawnObject[mv].SpawnPrefab != null)
                {
                    StartCoroutine(LateSpawn(mv));
                }
            }
        }
    }
}