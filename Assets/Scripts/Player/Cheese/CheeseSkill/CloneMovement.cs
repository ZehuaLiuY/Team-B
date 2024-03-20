using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CloneMovement : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 100f; // 克隆体的移动速度
    public float turnSpeed = 300f; // 遇到障碍时的转向速度
    
    private Animator animator; // Animator组件的引用

    private void Start()
    {
        // 获取Animator组件
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (photonView.IsMine) // 确保只有拥有者才更新动画状态
        {
            // 持续向前移动
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // 更新动画状态
            if (animator != null)
            {
                animator.SetBool("IsWalking", true); // 假设有一个名为 IsWalking 的布尔参数
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 遇到碰撞体时随机转向
        if (collision.gameObject.tag != "Target") // 确保不会因为玩家本身而转向
        {
            float randomDirection = Random.Range(-1f, 1f);
            transform.Rotate(0f, randomDirection * turnSpeed * Time.deltaTime, 0f);
        }
    }
}