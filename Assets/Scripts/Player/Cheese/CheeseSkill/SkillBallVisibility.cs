using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SkillBallVisibility : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CheckVisibility());
    }

    IEnumerator CheckVisibility()
    {
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerType"))
        {
            yield return null;
        }
        
        if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"].ToString() != "Cheese")
        {
            this.gameObject.SetActive(false);
        }
    }
}

