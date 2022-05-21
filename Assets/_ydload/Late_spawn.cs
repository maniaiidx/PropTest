using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Late_spawn : MonoBehaviour
{
    //public float LateTime = 0f;
    [Header("子供がMesh,Particleの順にぶら下がっている前提")]
    public float LateTime_Mesh = 3f;
    public float LateTime_Particle = 1f;
    //public MeshRenderer Late_Mesh;
    //public ParticleSystem Late_Particle;
    //private GameObject ChildMesh;
    //private GameObject ChildParticle;
    // プレハブ格納用
    public GameObject MeshPrefab;
    public GameObject ParticlePrefab;
    // 生成位置
    //public Vector3 MeshSpawnPos;
    //public Vector3 ParticleSpawnPos;
    public Transform MeshSpawnTrs;
    public Transform ParticleSpawnTrs;
    // 生成回転
    //public Vector3 MeshSpawnRotation;
    //public Vector3 ParticleSpawnRotation;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(LateSpawn());
        StartCoroutine(LateSpawn_Mesh());
        StartCoroutine(LateSpawn_Particle());
        //子供取得。まずMesh、次にParticleの順にぶら下がっていること
        //ChildMesh = transform.GetChild(0).gameObject;
        //ChildParticle = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LateSpawn_Mesh()
    {
        Debug.Log("LateSpawn_Mesh");
        float time = LateTime_Mesh;
        while (time > 0)
        {
            time -= Time.deltaTime;

            yield return null;  // 必須
        }
        //Late_time経過後
        //Late_Mesh.enabled = true;
        //Late_Particle.Play();
        //ChildMesh.SetActive(true);
        // プレハブを指定位置に生成
        Instantiate(MeshPrefab, MeshSpawnTrs.position, MeshSpawnTrs.rotation, this.transform.parent);
    }

    private IEnumerator LateSpawn_Particle()
    {
        Debug.Log("LateSpawn_Mesh");
        float time = LateTime_Particle;
        while (time > 0)
        {
            time -= Time.deltaTime;

            yield return null;  // 必須
        }
        //Late_time経過後
        //Late_Mesh.enabled = true;
        //Late_Particle.Play();
        //ChildParticle.SetActive(true);
        // プレハブを指定位置に生成
        Instantiate(ParticlePrefab, ParticleSpawnTrs.position, ParticleSpawnTrs.rotation, this.transform.parent);
        //Quaternion.Euler(ParticleSpawnRotation)
    }
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
}
