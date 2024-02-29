using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class MiniMapController : MonoBehaviour
{
    public RectTransform minimapRect;
    public GameObject humanIconPrefab;
    public GameObject cheeseIconPrefab;
    private Dictionary<GameObject, RectTransform> playerIcons = new Dictionary<GameObject, RectTransform>();

    private float mapScale;
    private string localPlayerType;

    void Start()
    {
        float worldSize = 1000000f; // The size of the game world
        float mapSize = minimapRect.sizeDelta.y; // The height of the minimap UI
        mapScale = mapSize / worldSize;

        // Determine the local player type
        localPlayerType = PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"] as string;

        Debug.Log("localPlayerType: " + localPlayerType);


    }

    public void AddPlayerIcon(GameObject player)
    {
        Debug.Log("AddPlayerIcon");
        string playerType = player.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] as string;

        // Only add the icon if the player is of the same type as the local player
        if (playerType == localPlayerType)
        {
            GameObject iconPrefab = playerType == "Human" ? humanIconPrefab : cheeseIconPrefab;
            RectTransform playerIcon = Instantiate(iconPrefab, minimapRect).GetComponent<RectTransform>();
            playerIcons[player] = playerIcon;
            UpdatePlayerIcon(player.transform.position, playerIcon);

        }
    }

    void Update()
    {
        foreach (var kvp in playerIcons)
        {
            UpdatePlayerIcon(kvp.Key.transform.position, kvp.Value);
        }
    }

    private void UpdatePlayerIcon(Vector3 playerWorldPosition, RectTransform playerIcon)
    {
        Vector2 minimapPosition = new Vector2(playerWorldPosition.x, playerWorldPosition.z) * mapScale;
        playerIcon.anchoredPosition = minimapPosition;
        Debug.Log("UpdatePlayerIcon");
    }
}