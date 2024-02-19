using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        float speed = 10f;
        bulletRigidbody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        // 检测碰撞对象是否为目标
        if (collision.gameObject.CompareTag("Target"))
        {
            // 假设目标对象有一个PhotonView和一个标识为"Target"的Tag
            PhotonView targetPhotonView = collision.gameObject.GetComponent<PhotonView>();
            if (targetPhotonView != null)
            {
                // 调用目标上的RPC方法来减少速度
                targetPhotonView.RPC("ReduceSpeed", RpcTarget.AllBuffered, null);
            }
            // 销毁子弹
            Destroy(gameObject);
        }
    }
}
