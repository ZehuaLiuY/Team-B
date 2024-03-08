using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Enter triger");
        if (other.CompareTag("Target"))
        {
            Debug.Log("Player caught the cheese!");

            PhotonView targetPhotonView = other.gameObject.GetComponent<PhotonView>();

            if (targetPhotonView != null && Input.GetKeyDown(KeyCode.R))
            {
                // 调用目标上的RPC方法来显示DeiUI
                targetPhotonView.RPC("showDeiUI", targetPhotonView.Owner, null);
            }
        }
    }
}
