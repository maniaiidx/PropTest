using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LFootCol : MonoBehaviour
{

    public DataCounter DC;
    public AudioSource audioSource;
    public List<AudioClip> seAsiotos = new List<AudioClip>();

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


    void OnTriggerEnter(Collider collision)
    {
        // Torigger
        Debug.Log("LFoot!");
        audioSource.clip = seAsiotos[Random.Range(0, seAsiotos.Count)];
        audioSource.Play();
    }





    // Update is called once per frame
    void Update()
    {
    }
}