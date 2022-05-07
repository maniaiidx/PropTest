using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayFire
{
    public class demolish : MonoBehaviour
    {
        public float LateTime_Mesh = 0f;
        public float LateTime_Particle = 0f;
        public RayfireRigid Next_RFRigid;
        public RayfireRigid Next_RFRigid2;
        public MeshRenderer Late_Mesh;
        public ParticleSystem Late_Particle;
        private bool onece;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "OnTest")
            {
                Debug.Log("Ontrigger");
                Debug.Log(other.gameObject.name);
                if (onece == false)
                {
                    if (Next_RFRigid != null)
                    {
                        Next_RFRigid.Initialize();
                        Next_RFRigid.Demolish();
                    }
                    if (Next_RFRigid2 != null)
                    {
                        Next_RFRigid2.Initialize();
                        Next_RFRigid2.Demolish();
                    }
                    StartCoroutine(LateSpawn_Mesh());
                    StartCoroutine(LateSpawn_Particle());
                    onece = true;
                }
            }
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
            Late_Mesh.enabled = true;
            //Late_Particle.Play();
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
            Late_Particle.Play();
        }
    }
}