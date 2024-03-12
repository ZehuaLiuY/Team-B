using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class DieUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        transform.Find("obBtn").GetComponent<Button>().onClick.AddListener(OnObBtn);
    }

    private void OnObBtn()
    {
        Debug.Log("Observe button clicked.");

        Player[] allPlayers = PhotonNetwork.PlayerList;
        List<Player> otherPlayers = allPlayers.ToList();
        otherPlayers.Remove(PhotonNetwork.LocalPlayer);

        if (otherPlayers.Count > 0)
        {
            Player playerToObserve = otherPlayers[Random.Range(0, otherPlayers.Count)];
            GameObject playerGameObject = FindPlayerGameObject(playerToObserve);

            if (playerGameObject != null)
            {
                Transform playerRoot = playerGameObject.transform.Find("PlayerRoot");

                if (playerRoot != null)
                {
                    CinemachineVirtualCamera vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
                    if (vc != null)
                    {
                        vc.Follow = playerRoot;
                        Debug.Log("Camera should now be following: " + playerRoot.name);
                    }
                    else
                    {
                        Debug.Log("CinemachineVirtualCamera component not found on 'PlayerFollowCamera'.");
                    }
                }

            }
        }
    }

    private GameObject FindPlayerGameObject(Player player)
    {
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            if (pv.Owner == player)
            {
                Debug.Log("Found player GameObject: " + pv.gameObject.name);
                return pv.gameObject;
            }
        }
        Debug.Log("Player GameObject not found.");
        return null;
    }
}
