using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class h2 : MonoBehaviour
{

    public AudioMixerSnapshot general;
    public AudioMixerSnapshot hDos;



    private void OnCollisionEnter(Collision collision)
    {
        hDos.TransitionTo(0.5f);

    }

    private void OnCollisionExit(Collision collision)
    {
        general.TransitionTo(3);

    }
}
