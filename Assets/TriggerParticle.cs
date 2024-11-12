using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class TriggerParticle : MonoBehaviour
{
    public AudioMixerSnapshot CuboIn;
    public AudioMixerSnapshot hDos;


    private void OnTriggerEnter(Collider other)
    {
        CuboIn.TransitionTo(1);
    }

    private void OnTriggerExit(Collider other)
    {
        hDos.TransitionTo(1);
    }
   

}
