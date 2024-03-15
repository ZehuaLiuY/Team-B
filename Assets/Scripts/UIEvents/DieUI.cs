using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<CheeseFightUI>("Cheese_FightUI");

        Player[] allPlayers = PhotonNetwork.PlayerList;
        Debug.Log(allPlayers);
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
            if (pv.CompareTag("Target"))
            {
                return pv.gameObject;
            }
        }
        Debug.Log("Player GameObject not found.");
        return null;
    }
}
