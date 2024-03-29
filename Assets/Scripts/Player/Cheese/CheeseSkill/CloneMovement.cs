using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CloneMovement : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 100f;
    public float turnSpeed = 300f;
    public float backStepDistance = 1f; // 后退距离
    
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // 前进移动
            float move = moveSpeed * Time.deltaTime;
            transform.Translate(Vector3.forward * move);

            // 更新动画状态
            if (animator != null)
            {
                animator.SetBool("IsWalking", move > 0);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Target")
        {
            // 随机转向
            float randomDirection = Random.Range(-1f, 1f);
            transform.Rotate(0f, randomDirection * turnSpeed * Time.deltaTime, 0f);

            // 后退一小步
            transform.Translate(Vector3.back * backStepDistance);
        }
    }
}