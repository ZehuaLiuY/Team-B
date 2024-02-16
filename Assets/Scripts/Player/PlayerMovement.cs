using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// sync the player's position and rotation
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun, IPunObservable
{

    // components
    // public Animator ani;
    public Rigidbody body;
    public Transform cameraTf;
    public float moveSpeed = 3.5f;

    // movement and rotation
    public float H;
    public float V;
    public Vector3 dir;
    public float jumpForce = 5f;

    // camera offset
    public Vector3 offset;

    // mouse movement
    public float Mouse_X;
    public float Mouse_Y;
    public float Angle_X;
    public float Angle_Y;

    public Quaternion camRotation;

    public Transform groundCheck;
    public LayerMask ground;

    // the current position and rotation
    public Vector3 currentPos;
    public Quaternion currentRot;

    // Start is called before the first frame update
    void Start()
    {
        // ani = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        cameraTf = Camera.main.transform;


        Angle_X = transform.eulerAngles.x;
        Angle_Y = transform.eulerAngles.y;

        currentPos = transform.position;
        currentRot = transform.rotation;

        // update the UI

    }

    // private void LateUpdate()
    // {
    //     ani.SetFloat("Horizontal", H);
    //     ani.SetFloat("Vertical", V);
    // }

    // Update is called once per frame
    void Update()
    {
        // if the player is not mine, return
        // player only can be controlled by the local player
        if (photonView.IsMine)
        {
            UpdatePosition();
            UpdateRotation();
            InputCtl();
        }
        else
        {
            // update the other player's position and rotation
            UpdateLogic();
        }
    }

    // other player's position and rotation
    public void UpdateLogic()
    {
        transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime * moveSpeed * 10);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentRot, Time.deltaTime * 500);
    }

    // update the current position and rotation
    public void UpdatePosition()
    {
        H = Input.GetAxisRaw("Horizontal");
        V = Input.GetAxisRaw("Vertical");
        dir = cameraTf.forward * V + cameraTf.right * H;
        body.MovePosition(transform.position + dir.normalized * moveSpeed * Time.deltaTime);
    }

    public void UpdateRotation()
    {
        Mouse_X = Input.GetAxis("Mouse X");
        Mouse_Y = Input.GetAxis("Mouse Y");

        Angle_X = Angle_X + Mouse_X;
        Angle_Y = Angle_Y + Mouse_Y;

        // limit the angle
        Angle_X = ClampAngle(Angle_X, -60, 60);
        Angle_Y = ClampAngle(Angle_Y, -360, 360);

        camRotation = Quaternion.Euler(Angle_X, Angle_Y, 0);

        cameraTf.rotation = camRotation;

        cameraTf.position = transform.position + cameraTf.rotation * offset;

        transform.eulerAngles = new Vector3(0, cameraTf.eulerAngles.y, 0);
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }

    bool IsGrounded()
    {
        // 检测是否接触地面
        return Physics.CheckSphere(groundCheck.position, 0.1f, ground);
    }

    void InputCtl()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }


    // private void gameOver()
    // {
    //     // display mouse
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible = true;
    //
    //     Game.uiManager.ShowUI<LossUI>("LossUI");
    // }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // send the current position and rotation
            stream.SendNext(H);
            stream.SendNext(V);
            stream.SendNext(Angle_X);
            stream.SendNext(Angle_Y);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // receive the current position and rotation
            H = (float)stream.ReceiveNext();
            V = (float)stream.ReceiveNext();
            Angle_X = (float)stream.ReceiveNext();
            Angle_Y = (float)stream.ReceiveNext();
            currentPos = (Vector3)stream.ReceiveNext();
            currentRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
