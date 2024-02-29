using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class MiniMapController : MonoBehaviourPunCallbacks, IPunObservable
{
    public RectTransform minimapRect;
    public GameObject humanIconPrefab;
    public GameObject cheeseIconPrefab;
    private Dictionary<GameObject, RectTransform> playerIcons = new Dictionary<GameObject, RectTransform>();

    private float _mapScale;
    private string _localPlayerType;


    private void InitializeMiniMap()
    {
        float worldSize = 400f; // The size of the game world
        float mapSize = minimapRect.sizeDelta.y; // The height of the minimap UI
        Debug.Log(mapSize);
        _mapScale = mapSize / worldSize;
    }

    IEnumerator WaitForPlayerTypeAndInitialize()
    {
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerType"))
        {
            yield return null;
        }

        _localPlayerType = PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"] as string;
        Debug.Log("localPlayerType: " + _localPlayerType);
        InitializeMiniMap();
    }

    void Start()
    {
        StartCoroutine(WaitForPlayerTypeAndInitialize());
    }


    public void AddPlayerIcon(GameObject player)
    {

        string playerType = player.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] as string;
        Debug.Log("AddPlayerIcon called, playerType: " + playerType);
        Debug.Log("AddPlayerIcon called, localPlayerType: " + _localPlayerType);

        GameObject iconPrefab = playerType == "Human" ? humanIconPrefab : cheeseIconPrefab;
        RectTransform playerIcon = Instantiate(iconPrefab, minimapRect).GetComponent<RectTransform>();
        playerIcons[player] = playerIcon;
        Debug.Log("playerIcons[player]: " + playerIcons[player]);
        UpdatePlayerIcon(player.transform.position, playerIcon);
        Debug.Log("AddPlayerIcon");

    }

    void Update()
    {
        foreach (var kvp in playerIcons)
        {
            PhotonView playerPhotonView = kvp.Key.GetComponent<PhotonView>();

            if (playerPhotonView != null && (string)playerPhotonView.Owner.CustomProperties["PlayerType"] == _localPlayerType)
            {
                UpdatePlayerIcon(kvp.Key.transform.position, kvp.Value);
            }
        }
    }

    private void UpdatePlayerIcon(Vector3 playerWorldPosition, RectTransform playerIcon)
    {
        Vector2 minimapPosition = new Vector2(playerWorldPosition.x, playerWorldPosition.z) * _mapScale;
        playerIcon.anchoredPosition = minimapPosition;
        Debug.Log(playerIcon.anchoredPosition);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"] as string == _localPlayerType)
            {
                stream.SendNext(transform.position);
            }
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"] as string != _localPlayerType)
            {
                foreach (var playerIconKvp in playerIcons)
                {
                    PhotonView playerPhotonView = playerIconKvp.Key.GetComponent<PhotonView>();

                    if (playerPhotonView != null && (string)playerPhotonView.Owner.CustomProperties["PlayerType"] == _localPlayerType)
                    {
                        Vector3 remotePlayerPosition = (Vector3)stream.ReceiveNext();
                        UpdatePlayerIcon(remotePlayerPosition, playerIconKvp.Value);
                    }
                }
            }
        }
    }

}