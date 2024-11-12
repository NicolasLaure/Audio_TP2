using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class CuboInOut : MonoBehaviour
{
    public AudioMixerSnapshot CuboIn;
    public AudioMixerSnapshot general;


    private void OnTriggerEnter(Collider other)
    {
        GetComponent<AudioSource>().Play();
        CuboIn.TransitionTo(0.2f);

    }

    private void OnTriggerExit(Collider other)
    {
        GetComponent<AudioSource>().Play();
        general.TransitionTo(0.2f);



    }

}
