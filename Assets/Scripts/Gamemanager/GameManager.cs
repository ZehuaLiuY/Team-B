using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject humanPlayer;
    public GameObject cheesePlayer;

    public PlayerControl playerControl;

    public SceneController sceneController;

    private bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        if(cheesePlayer != null && humanPlayer != null)
        {
            // 获取人类和芝士之间的距离
            float distance = Vector3.Distance(GetHumanPosition(), GetCheesePosition());

            // 检查距离是否小于等于 2 个单位
            if (distance <= 2f && playerControl.pickup)
            {
                isGameOver = true;

                sceneController.SwitchToGameoverUI(true);
            }
        }
        
    }


    Vector3 GetHumanPosition()
    {
        // 实现获取人类位置的方法，可以是 PlayerController 中的方法

        return humanPlayer.transform.position;// 这里简单地返回 Vector3.zero，请替换成你的实际实现
    }

    Vector3 GetCheesePosition()
    {
        // 实现获取芝士位置的方法
        return cheesePlayer.transform.position; // 这里简单地返回 Vector3.one，请替换成你的实际实现
    }
}
