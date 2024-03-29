using System.Collections.Generic;
using System.Linq;
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
        // destroy all existing player items
        foreach (Transform child in _contentTf)
        {
            Destroy(child.gameObject);
        }

        // get all players who are humans and have a cheese count
        var playerInfoList = PhotonNetwork.PlayerList
            .Where(player => player.CustomProperties.TryGetValue("PlayerType", out object type) && type.ToString() == "Human")
            .Select(player => new {
                Player = player,
                CheeseCount = player.CustomProperties.TryGetValue("CheeseCount", out object cheeseCount) ? (int)cheeseCount : 0
            })
            .OrderByDescending(info => info.CheeseCount)
            .ToList();

        // create UI elements for each player
        foreach (var info in playerInfoList)
        {
            GameObject itemObj = Instantiate(_roomPrefab, _contentTf);
            itemObj.SetActive(true);

            // set player name
            Text playerNameText = itemObj.transform.Find("playerName").GetComponent<Text>();
            if (playerNameText != null)
            {
                string playerName = "Player" + photonView.Owner.ActorNumber; // default name
                if (info.Player.CustomProperties.TryGetValue("PlayerName", out object name))
                {
                    playerName = (string)name;
                }
                playerNameText.text = playerName;
            }

            // set cheese count
            Text cheeseCountText = itemObj.transform.Find("Counter").GetComponent<Text>();
            if (cheeseCountText != null)
            {
                cheeseCountText.text = $"Cheese Caught: {info.CheeseCount}";
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if (changedProps.ContainsKey("CheeseCount")) {
            UpdateLeaderboard();
        }
    }
}
