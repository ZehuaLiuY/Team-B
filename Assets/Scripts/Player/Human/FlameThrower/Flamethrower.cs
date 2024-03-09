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
    [SerializeField] private AudioSource audioSource;
    public AudioClip introClip; 
    public AudioClip loopClip;

    private bool isShooting = false;

    void Awake()
    {
        audioSource.clip = introClip;
    }

    private void Start()
    {
        // 确保在开始时，喷火器和攻击半径是关闭的
        ShootingSystem.gameObject.SetActive(false);
        AttackRadius.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            HandleShooting();
        }
    }

    void HandleShooting()
    {
        if (Mouse.current.leftButton.isPressed && !isShooting)
        {
            StartShooting();
        }
        else if (!Mouse.current.leftButton.isPressed && isShooting)
        {
            StopShooting();
        }
    }

    void StartShooting()
    {
        isShooting = true;
        audioSource.Play(); // 播放 introClip
        Invoke("PlayLoopClip", introClip.length); // 在 introClip 结束后播放 loopClip
        photonView.RPC("ActivateShootingSystems", RpcTarget.All);
    }

    void StopShooting()
    {
        isShooting = false;
        CancelInvoke("PlayLoopClip"); // 防止introClip结束后切换到loopClip
        audioSource.Stop(); // 停止播放所有音乐
        photonView.RPC("DeactivateShootingSystems", RpcTarget.All);
    }

    void PlayLoopClip()
    {
        audioSource.loop = true;
        audioSource.clip = loopClip;
        audioSource.Play();
    }

    [PunRPC]
    void ActivateShootingSystems()
    {
        ShootingSystem.gameObject.SetActive(true);
        AttackRadius.gameObject.SetActive(true);
    }

    [PunRPC]
    void DeactivateShootingSystems()
    {
        ShootingSystem.gameObject.SetActive(false);
        AttackRadius.gameObject.SetActive(false);
    }
}
