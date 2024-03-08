using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class MiniMapController : MonoBehaviourPunCallbacks
{
    public RectTransform minimapRect;
    public GameObject humanIconPrefab;
    public GameObject cheeseIconPrefab;
    private RectTransform _playerIcon;
    private Dictionary<GameObject, RectTransform> _playerIcons = new Dictionary<GameObject, RectTransform>();

    private float _mapScale;
    private string _localPlayerType;
    private Vector2 _minimapPosition;

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
        _playerIcon = Instantiate(iconPrefab, minimapRect).GetComponent<RectTransform>();
        _playerIcons[player] = _playerIcon;
        Debug.Log("playerIcons[player]: " + _playerIcons[player]);
        // UpdatePlayerIcon(player.transform.position, _playerIcon);
        Debug.Log("AddPlayerIcon");

    }

    void Update()
    {
        // foreach (var kvp in _playerIcons)
        // {
        //     UpdatePlayerIcon(kvp.Key.transform.position, kvp.Value);
        // }
    }

    // private void UpdatePlayerIcon(Vector3 playerWorldPosition, RectTransform playerIcon)
    // {
    //     _minimapPosition = new Vector2(playerWorldPosition.x, playerWorldPosition.z) * _mapScale;
    //     playerIcon.anchoredPosition = _minimapPosition;
    //     // Debug.Log(playerIcon.anchoredPosition == _minimapPosition);
    // }

    public void UpdatePlayerIcon(GameObject player, Vector3 newPosition)
    {
        if (_playerIcons.ContainsKey(player))
        {
            RectTransform iconTransform = _playerIcons[player];
            Vector2 minimapPosition = new Vector2(newPosition.x, newPosition.z) * _mapScale;
            iconTransform.anchoredPosition = minimapPosition;
        }
        else
        {
            AddPlayerIcon(player);
        }
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if (stream.IsWriting)
    //     {
    //         if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"] as string == _localPlayerType)
    //         {
    //             stream.SendNext(_playerIcon);
    //             stream.SendNext(_minimapPosition);
    //         }
    //     }
    //     else
    //     {
    //         if (PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"] as string == _localPlayerType)
    //         {
    //             _playerIcon = (RectTransform)stream.ReceiveNext();
    //             _minimapPosition = (Vector2)stream.ReceiveNext();
    //         }
    //     }
    // }
}