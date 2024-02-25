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

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                photonView.RPC("Shoot", RpcTarget.All);
            }
            else
            {
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

