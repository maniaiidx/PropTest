using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RFootCol : MonoBehaviour
{

    public DataCounter DC;
    public AudioSource audioSource;
    public List<AudioClip> seAsiotos = new List<AudioClip>();

    public float
        rFootColPosY,
        rFootColPosPrevY;
    public bool rFootColPosYZeroBool;

    // Use this for initialization
    void Start()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();
        audioSource = this.GetComponent<AudioSource>();
        seAsiotos.Add(Instantiate(DC.ResourceFiles.SE["足音0_bosu36"]));
        seAsiotos.Add(Instantiate(DC.ResourceFiles.SE["足音1_footsteps_shoe_grass_walk_03"]));
        seAsiotos.Add(Instantiate(DC.ResourceFiles.SE["足音2_footsteps_shoe_grass_walk_04"]));
        seAsiotos.Add(Instantiate(DC.ResourceFiles.SE["足音3_footsteps_shoe_grass_walk_05"]));
    }


    void OnCollisionEnter (Collision collision)
    {
        //// Torigger
        //Debug.Log(collision.relativeVelocity.magnitude);
        //Debug.Log("RFootCol");

        //audioSource.clip = seAsiotos[Random.Range(0, seAsiotos.Count)];
        //audioSource.Play();
    }
    
    // Update is called once per frame
    void Update()
    {
        rFootColPosPrevY = rFootColPosY;
        rFootColPosY = transform.position.y / DC.GameObjectsTrs.localScale.z;

        if (rFootColPosY < 0.02f &&
            rFootColPosYZeroBool == false)
        {
            Debug.Log(rFootColPosPrevY - rFootColPosY);
            rFootColPosYZeroBool = true;

            if(rFootColPosPrevY - rFootColPosY > 0.0001f)
            {
                audioSource.clip = seAsiotos[Random.Range(0, seAsiotos.Count)];
                audioSource.Play();
            }

        }

        if (rFootColPosY > 0.02f &&
            rFootColPosYZeroBool == true)
        {
            rFootColPosYZeroBool = false;
        }


    }
}