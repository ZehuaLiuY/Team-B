using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 确保导入了Input System命名空间
using StarterAssets;
using Photon.Pun;

public class DoorInteraction : MonoBehaviour
{
    public GameObject door; // 门的引用
    public GameObject test; // 开门提示的UI元素引用

    private Animator doorAnimator;
    private bool isPlayerNear;
    private bool doorIsOpen;
    private PhotonView photonView;

    void Awake()
    {
        doorAnimator = door.GetComponent<Animator>();
        test.SetActive(false); // 开始时禁用提示
        photonView = transform.parent.GetComponent<PhotonView>();
    }

    void Update()
    {
        if (isPlayerNear && Keyboard.current.eKey.wasPressedThisFrame)
        {
            photonView.RPC("ToggleDoor", RpcTarget.All); // Call the RPC method
        }
    }

    // 当玩家进入触发区域
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Target") ) // 确保是玩家触发了这个区域
        {
            if (other.GetComponent<PhotonView>().IsMine)
            {
                isPlayerNear = true;
                test.SetActive(true); // 显示提示
            }

        }
    }

    // 当玩家离开触发区域
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Target"))
        {
            if (other.GetComponent<PhotonView>().IsMine)
            {
                isPlayerNear = false;
                test.SetActive(false); // 隐藏提示
            }
        }
    }

    [PunRPC]
    void ToggleDoor() {
        doorAnimator.SetBool("IsOpen", !doorAnimator.GetBool("IsOpen")); // Toggle door state
    }
}
