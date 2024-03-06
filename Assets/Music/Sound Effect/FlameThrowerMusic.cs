using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerMusic : MonoBehaviour
{
    public AudioSource audioSource; 
    public AudioClip introClip; 
    public AudioClip loopClip; 

    void Start()
    {
        audioSource.clip = introClip;
        audioSource.Play();
        Invoke("PlayLoopClip", introClip.length); 
    }

    void PlayLoopClip()
    {
        audioSource.clip = loopClip;
        audioSource.loop = true; 
        audioSource.Play();
    }
}
