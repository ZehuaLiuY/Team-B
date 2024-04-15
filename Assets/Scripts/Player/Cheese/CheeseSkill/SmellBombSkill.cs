using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmellBombSkill : MonoBehaviourPun
{
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                DeploySmellBomb();
            }
        }
    }

    void DeploySmellBomb()
    {
        // generate a smell bomb at the player's position
        PhotonNetwork.Instantiate("SmellBomb Skill", transform.position, Quaternion.identity);
    }
}
