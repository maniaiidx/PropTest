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
    private GameObject ChildMesh;
    private GameObject ChildParticle;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(LateSpawn());
        StartCoroutine(LateSpawn_Mesh());
        StartCoroutine(LateSpawn_Particle());
        //子供取得。まずMesh、次にParticleの順にぶら下がっていること
        ChildMesh = transform.GetChild(0).gameObject;
        ChildParticle = transform.GetChild(1).gameObject;
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
        ChildMesh.SetActive(true);
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
        ChildParticle.SetActive(true);
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
