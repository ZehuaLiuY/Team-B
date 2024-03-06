using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    public Transform cameraTransform; // 摄像机的Transform
    private bool canMove = true; // 控制角色是否可以移动

    void Update()
    {
        // 检测鼠标右键的按下和释放
        if (Input.GetMouseButtonDown(1)) // 鼠标右键按下
        {
            canMove = false;
            animator.SetBool("isRunning", false); // 停止播放跑动动画
        }
        if (Input.GetMouseButtonUp(1)) // 鼠标右键释放
        {
            canMove = true;
        }

        if (canMove)
        {
            MoveAndAnimate();
        }
    }

    private void MoveAndAnimate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("isRunning", true);
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, moveSpeed * 100 * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        // 更新动画参数
        animator.SetFloat("Speed", moveDirection.magnitude);
    }
}


