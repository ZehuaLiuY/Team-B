using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 确保导入了Input System命名空间
using UnityEngine.UI;
using StarterAssets;
using Photon.Pun;
using UnityEngine.VFX;

public class DoorInteraction : MonoBehaviour
{
    public GameObject door; // 门的引用
    public GameObject text; // 开门提示的UI元素引用
    public GameObject vfxSmell;

    private Animator doorAnimator;
    private bool isPlayerNear;
    private bool doorIsOpen;
    private PhotonView photonView;
    private GameObject currentVFXInstance; // 用于存储当前播放的 Visual Effect 实例

    private bool _cheeseInSide = false;
    private CheckCheeseInside _checkCheeseInside;

    void Awake()
    {
        doorAnimator = door.GetComponent<Animator>();
        text.SetActive(false); // 开始时禁用提示
        photonView = transform.parent.GetComponent<PhotonView>();
    }

    private void Start()
    {
        
        //_vfxSmell.SetActive(false);
        GameObject detector = GameObject.FindWithTag("Detector");
        _checkCheeseInside = detector.GetComponent<CheckCheeseInside>();
    }
    

    void Update()
    {
        checkCheese();
        if (isPlayerNear && Keyboard.current.eKey.wasPressedThisFrame)
        {
            photonView.RPC("ToggleDoor", RpcTarget.All); // Call the RPC method
            
            //Debug.Log("_cheeseInSide: " + _cheeseInSide);
            if (doorAnimator.GetBool("IsOpen") && _cheeseInSide)
            {
                photonView.RPC("PlayVFX", RpcTarget.All);
            }
            else
            {
                photonView.RPC("StopVFX", RpcTarget.All);
            }
        }
        else
        { 
            if (!_cheeseInSide && currentVFXInstance != null)
            {
                photonView.RPC("StopVFX", RpcTarget.All);
            }
        }
        
        
    }

    private void checkCheese()
    {
        _cheeseInSide = _checkCheeseInside.isCheeseInside;
    }

    // 当玩家进入触发区域
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Target") ) // 确保是玩家触发了这个区域
        {
            if (other.GetComponent<PhotonView>().IsMine)
            {
                isPlayerNear = true;
                text.SetActive(true); // 显示提示
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
                text.SetActive(false); // 隐藏提示
            }
        }
    }

    [PunRPC]
    void ToggleDoor() {

        doorAnimator.SetBool("IsOpen", !doorAnimator.GetBool("IsOpen")); // Toggle door state
    }

    [PunRPC]
    void PlayVFX()
    {
        // 实例化 Visual Effect 预制体并放置在门的位置
        currentVFXInstance = PhotonNetwork.Instantiate("VFXSmell", door.transform.position, Quaternion.identity);
        Debug.Log("generate smell");

        // 让 Visual Effect 开始播放
        //currentVFXInstance.GetComponent<VisualEffect>().Play();
    }

    [PunRPC]
    void StopVFX()
    {
        if (currentVFXInstance != null)
        {
            // 如果有正在播放的 Visual Effect，停止播放并销毁实例
            //currentVFXInstance.GetComponent<VisualEffect>().Stop();
            PhotonNetwork.Destroy(currentVFXInstance);
            currentVFXInstance = null;
        }
    }
}
