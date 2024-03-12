using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FlamethrowerAttackRadius : MonoBehaviourPun
{
    
    
    private void OnTriggerEnter(Collider other)
    {
        ProcessTriggerEvent(other);
    }

    private void OnTriggerStay(Collider other)
    {
        ProcessTriggerEvent(other);
    }

    private void ProcessTriggerEvent(Collider other)
    {
        // Debug.Log($"Triggered by {other.gameObject.name}");
        if (other.gameObject.CompareTag("Target"))
        {
            PhotonView targetPhotonView = other.gameObject.GetComponent<PhotonView>();
            if (targetPhotonView != null && photonView.IsMine)
            {
                targetPhotonView.RPC("ReduceSpeed", targetPhotonView.Owner, null);
            }
        }
    }
}