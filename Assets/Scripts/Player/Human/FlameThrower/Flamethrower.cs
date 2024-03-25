using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using StarterAssets;

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
    // private bool updateStaminaBar = true;
    private StarterAssetsInputs _input;

    void Awake()
    {
        audioSource.clip = introClip;
        _input = GetComponent<StarterAssetsInputs>();
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
        Invoke("PlayLoopClip", introClip.length);
        photonView.RPC("ActivateShootingSystems", RpcTarget.All);
        ThirdPersonController thirdPersonController = GetComponentInParent<ThirdPersonController>();
        if (thirdPersonController != null)
        {
            thirdPersonController.EnableSprinting(false); // 重新启用冲刺
        }else
        {
            Debug.LogWarning("PlayerMovement script not found!");
        }
    }

    void StopShooting()
    {
        isShooting = false;
        CancelInvoke("PlayLoopClip"); // 防止introClip结束后切换到loopClip
        audioSource.Stop(); // 停止播放所有音乐
        photonView.RPC("DeactivateShootingSystems", RpcTarget.All);
        ThirdPersonController thirdPersonController = GetComponentInParent<ThirdPersonController>();
        if (thirdPersonController != null)
        {
            thirdPersonController.EnableSprinting(true); // 重新启用冲刺
        }else
        {
            Debug.LogWarning("PlayerMovement script not found!");
        }
    }

    void CloseShift()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return;
        }
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
