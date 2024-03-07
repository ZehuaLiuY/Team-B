using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using Photon.Pun;

[DisallowMultipleComponent]
public class Flamethrower : MonoBehaviourPun
{
    [SerializeField] private ParticleSystem ShootingSystem;
    [SerializeField] private ParticleSystem OnFireSystemPrefab;
    [SerializeField] private FlamethrowerAttackRadius AttackRadius;
    private AudioSource audioSource;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                photonView.RPC("Shoot", RpcTarget.All);
            }
            else
            {
                audioSource.Stop();
                photonView.RPC("StopShooting", RpcTarget.All);
            }
        }
    }


    [PunRPC]
    void Shoot()
    {
        ShootingSystem.gameObject.SetActive(true);
        AttackRadius.gameObject.SetActive(true);
    }

    [PunRPC]
    void StopShooting()
    {
        ShootingSystem.gameObject.SetActive(false);
        AttackRadius.gameObject.SetActive(false);
    }
}