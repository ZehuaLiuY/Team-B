using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour
{

    Rigidbody rb;
    private Animator animator;
    // 用于控制物体是否可以移动
    private bool canMove = true;
    private Vector3 moveDirection = Vector3.zero;
    public float movementSpeed;
    public float crouchMovementSpeed;
    public bool pickup = false;

    // 方法供Animator事件调用，表示Pickup动作开始
    public void OnPickupStart()
    {
        // 禁用移动
        canMove = false;

        pickup = true;
    }

    // 方法供Animator事件调用，表示Pickup动作结束
    public void OnPickupEnd()
    {
        // 启用移动
        canMove = true;

        pickup = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");

        float vertical = Input.GetAxis("Vertical");


        // 转换输入为世界坐标系下的方向
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        inputDirection = Camera.main.transform.TransformDirection(inputDirection);
        inputDirection.y = 0; 

        moveDirection = inputDirection; 

        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("pickup");
        } 

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && canMove)
        {
            animator.SetBool("IsRun", false);
            animator.SetBool("IsCrouch", true);
            if (moveDirection != Vector3.zero)
            {
                animator.SetBool("IsCrouchWalk", true);
                rb.MovePosition(rb.position + moveDirection * crouchMovementSpeed * Time.fixedDeltaTime);
            }

        }
        else if (moveDirection != Vector3.zero && canMove)
        {
            animator.SetBool("IsCrouchWalk", false);
            animator.SetBool("IsCrouch", false);
           

            // run animation
            animator.SetBool("IsRun", true);

            rb.MovePosition(rb.position + moveDirection * movementSpeed * Time.fixedDeltaTime);



        }
        else
        {
            // idle animation
            animator.SetBool("IsRun", false);
            animator.SetBool("IsCrouch", false);
            animator.SetBool("IsCrouchWalk", false);
        }

        
       

    }
    
}
