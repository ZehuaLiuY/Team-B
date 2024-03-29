using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LeaderboardUI : MonoBehaviourPunCallbacks
{
    private Transform _contentTf;
    private GameObject _roomPrefab;

    private void Awake()
    {
        _contentTf = transform.Find("content/Scroll View/Viewport/Content");
        _roomPrefab = transform.Find("content/Scroll View/Viewport/item").gameObject;
    }

    void Start()
    {
        UpdateLeaderboard();
    }

    private void UpdateLeaderboard()
    {
        // Remove old player items from the leaderboard
        // foreach (PlayerItem item in _playerItems)
        // {
        //     Destroy(item.gameObject);
        // }
        // _playerItems.Clear();

        // Get the list of human players from the custom room properties
        int[] humanPlayerActorNumbers;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("HumanPlayerActorNumbers", out object humanActorNumbers) && humanActorNumbers is int[])
        {
            humanPlayerActorNumbers = (int[])humanActorNumbers;
        }
        else
        {
            humanPlayerActorNumbers = new int[PhotonNetwork.PlayerList.Length];
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                humanPlayerActorNumbers[i] = PhotonNetwork.PlayerList[i].ActorNumber;
            }
        }

        // Add new player items to the leaderboard
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (System.Array.IndexOf(humanPlayerActorNumbers, player.ActorNumber) >= 0)
            {
                if (player.CustomProperties.TryGetValue("CheeseCount", out object cheeseCountObj))
                {
                    // Create a new player item
                    GameObject itemObj = Instantiate(_roomPrefab, _contentTf);
                    itemObj.SetActive(true);

                    // set player name
                    Text playerNameText = itemObj.transform.Find("playerName").GetComponent<Text>();
                    if (playerNameText != null) {
                        playerNameText.text = player.NickName;
                    }

                    // set cheese count
                    Text cheeseCountText = itemObj.transform.Find("Counter").GetComponent<Text>();
                    if (cheeseCountText != null) {
                        cheeseCountText.text = $"Cheese Caught:  {cheeseCountObj}";
                    }
                }
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if (changedProps.ContainsKey("CheeseCount")) {
            UpdateLeaderboard();
        }
    }

}
