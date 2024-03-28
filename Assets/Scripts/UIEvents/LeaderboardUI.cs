using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LeaderboardUI : MonoBehaviour
{
    private Transform _contentTf;
    private GameObject _roomPrefab;
    private List<PlayerItem> _playerItems;

    private void Awake()
    {
        _contentTf = transform.Find("content/Scroll View/Viewport/Content");
        _roomPrefab = transform.Find("content/Scroll View/Viewport/item").gameObject;
        _playerItems = new List<PlayerItem>();
    }

    void Start()
    {
        UpdateLeaderboard();
    }

    public void UpdateLeaderboard()
{
    // Remove old player items from the leaderboard
    // foreach (PlayerItem item in _playerItems)
    // {
    //     Destroy(item.gameObject);
    // }
    // _playerItems.Clear();

    // Get the list of human players from the custom room properties
    int[] humanPlayerActorNumbers;
    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("HumanPlayerActorNumbers", out object humanActorNumbers))
    {
        if(humanActorNumbers is int[])
        {
            humanPlayerActorNumbers = (int[])humanActorNumbers;
        }
        else
        {
            Debug.LogError("HumanPlayerActorNumbers is not an int[] type.");
            return;
        }
    }
    else
    {
        // If there's no list, assume all players are human
        humanPlayerActorNumbers = new int[PhotonNetwork.PlayerList.Length];
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            humanPlayerActorNumbers[i] = PhotonNetwork.PlayerList[i].ActorNumber;
        }
    }

    // Create a leaderboard item for each human player
    foreach (Player player in PhotonNetwork.PlayerList)
    {
        if (System.Array.IndexOf(humanPlayerActorNumbers, player.ActorNumber) > -1)
        {
            string playerName = player.NickName; // Default to NickName if no custom name is set
            if (player.CustomProperties.TryGetValue("PlayerName", out object name))
            {
                playerName = (string)name;
            }

            // Instantiate the item prefab and set the name
            GameObject itemObj = Instantiate(_roomPrefab, _contentTf);
            itemObj.SetActive(true);

            // Access the Text component and set the playerName
            Text playerNameText = itemObj.transform.Find("playerName").GetComponent<Text>();
            if (playerNameText != null)
            {
                playerNameText.text = playerName;
            }
        }
    }
}

}

// Make sure you have a PlayerItem script attached to your prefab that has a method 'SetPlayerName'.
public class PlayerItem : MonoBehaviour
{
    public Text playerNameText;

    public void SetPlayerName(string name)
    {
        playerNameText.text = name;
    }
}
