using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CloneMovement : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 100f;
    public float turnSpeed = 300f;
    public float backStepDistance = 1f; // 后退距离
    
    public float lifetime = 10f;
    private Animator _animator;

    private void Start()
    {
        StartCoroutine(DestroyAfterTime(lifetime));
        _animator = GetComponent<Animator>();
    }
    
    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(gameObject);
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // moving forward
            float move = moveSpeed * Time.deltaTime;
            transform.Translate(Vector3.forward * move);

            // update animation
            if (_animator != null)
            {
                _animator.SetBool("IsWalking", move > 0);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Target")
        {
            // random direction
            float randomDirection = Random.Range(-1f, 1f);
            transform.Rotate(0f, randomDirection * turnSpeed * Time.deltaTime, 0f);

            // back step
            transform.Translate(Vector3.back * backStepDistance);
        }
    }
}