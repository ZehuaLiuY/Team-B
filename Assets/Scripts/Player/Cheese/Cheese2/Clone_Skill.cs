using System;
using System.Collections;
using System.Collections.Generic;
using CheeseController;
using Photon.Pun;
using UnityEngine;

public class Clone_Skill : MonoBehaviourPunCallbacks
{
    public float cooldown = 30f;
    private bool isCooldown = false;

    void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.C) && !isCooldown)
        {
            Clone();
            StartCoroutine(StartCooldown());
        }
    }

    void Clone()
    {
        Debug.Log("Attempting to clone character.");
        // GameObject clone = PhotonNetwork.Instantiate(this.gameObject.name.Replace("(Clone)",""), transform.position, transform.rotation);
        GameObject clone = PhotonNetwork.Instantiate("Cheese1", transform.position, transform.rotation);
        var cloneMovement = clone.AddComponent<CloneMovement>();
        cloneMovement.moveSpeed = 3f; 
        cloneMovement.turnSpeed = 300f; 

        StartCoroutine(DestroyNetworkObject(clone, 10f));
    }
    IEnumerator DestroyNetworkObject(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(target);
    }

    IEnumerator StartCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isCooldown = false;
    }
}
