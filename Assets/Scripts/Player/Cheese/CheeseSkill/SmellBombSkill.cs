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
        // 在Cheese位置实例化烟雾弹预制体
        PhotonNetwork.Instantiate("OdorParticle", transform.position, Quaternion.identity);

    }
}
