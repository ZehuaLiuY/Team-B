using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    Rigidbody rb;
    public float movementSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask ground;
    public Transform cameraTransform; // 摄像机的Transform

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 根据摄像机的朝向计算移动方向
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0; // 确保移动仅在水平面上
        cameraRight.y = 0;
        Vector3 direction = (cameraForward * vertical + cameraRight * horizontal).normalized;

        moveDirection = direction;

        // 跳跃逻辑
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // 在FixedUpdate中应用物理相关的移动
        rb.MovePosition(rb.position + moveDirection * movementSpeed * Time.fixedDeltaTime);
    }

    bool IsGrounded()
    {
        // 检测是否接触地面
        return Physics.CheckSphere(groundCheck.position, 0.1f, ground);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
        }
        else
        {
            rb.position = (Vector3)stream.ReceiveNext();
            rb.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
