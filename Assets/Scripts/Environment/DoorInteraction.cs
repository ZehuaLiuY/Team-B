using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 确保导入了Input System命名空间
using StarterAssets;

public class DoorInteraction : MonoBehaviour
{
    public GameObject door; // 门的引用
    public GameObject test; // 开门提示的UI元素引用

    private Animator doorAnimator;
    private bool isPlayerNear;
    private bool doorIsOpen;

    void Awake()
    {
        doorAnimator = door.GetComponent<Animator>();
        test.SetActive(false); // 开始时禁用提示
    }

    void Update()
    {
        if (isPlayerNear && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.LogError("E is Pressed");
            doorAnimator.SetBool("IsOpen", !doorAnimator.GetBool("IsOpen")); // 切换门的开闭状态
        }
    }

    // 当玩家进入触发区域
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 确保是玩家触发了这个区域
        {
            isPlayerNear = true;
            test.SetActive(true); // 显示提示
        }
    }

    // 当玩家离开触发区域
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            test.SetActive(false); // 隐藏提示
        }
    }
}
