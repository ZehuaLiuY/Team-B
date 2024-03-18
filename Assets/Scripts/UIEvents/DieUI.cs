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
        List<Player> cheesePlayers = allPlayers
            .Where(p => p != PhotonNetwork.LocalPlayer && p.TagObject is GameObject &&
                        ((GameObject)p.TagObject).CompareTag("Target")).ToList();

        if (cheesePlayers.Count > 0)
        {
            SwitchToRandomPlayer(cheesePlayers);
        }

        StartCoroutine(ObserveOtherPlayerOnClick(cheesePlayers));
    }

    private void SwitchToRandomPlayer(List<Player> players)
    {
        Player playerToObserve = players[Random.Range(0, players.Count)];
        GameObject playerGameObject = (GameObject)playerToObserve.TagObject;
        Transform followCameraTransform = playerGameObject.transform.Find("PlayerFollowCamera");

        if (followCameraTransform != null)
        {
            CinemachineVirtualCamera vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            if (vc != null)
            {
                vc.Follow = followCameraTransform;
            }
            else
            {
                Debug.Log("CinemachineVirtualCamera component not found on 'PlayerFollowCamera'.");
            }
        }
        else
        {
            Debug.Log("PlayerFollowCamera not found on 'Cheese(Clone)'.");
        }
    }

    private IEnumerator ObserveOtherPlayerOnClick(List<Player> players)
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SwitchToRandomPlayer(players);
            }
            yield return null;
        }
    }
}
