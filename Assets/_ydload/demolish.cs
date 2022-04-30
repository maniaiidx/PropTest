using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayFire
{
    public class demolish : MonoBehaviour
    {
        public float LateTime = 0f;
        public RayfireRigid Next_RFRigid;
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
                    StartCoroutine(LateSpawn());
                    onece = true;
                }
            }
        }

        private IEnumerator LateSpawn()
        {
            Debug.Log("LateSpawn");
            Next_RFRigid.Initialize();
            Next_RFRigid.Demolish();
            float time = LateTime;
            while (time > 0)
            {
                time -= Time.deltaTime;

                yield return null;  // 必須
            }
            //Late_time経過後
            Late_Mesh.enabled = true;
            Late_Particle.Play();
        }
    }
}