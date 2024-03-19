using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomUI : MonoBehaviour, IInRoomCallbacks
{
    Transform startTf;
    Transform contentTf;
    GameObject roomPrefab;
    public List<RoomItem> roomList;

    private void Awake()
    {
        roomList = new List<RoomItem>();
        contentTf = transform.Find("bg/Content");
        roomPrefab = transform.Find("bg/roomItem").gameObject;
        transform.Find("bg/title/closeBtn").GetComponent<Button>().onClick.AddListener(OncloseBtn);
        startTf = transform.Find("bg/startBtn");
        startTf.GetComponent<Button>().onClick.AddListener(OnStartBtn);

        PhotonNetwork.AutomaticallySyncScene = true;

    }

    // Start is called before the first frame update
    void Start()
    {
        // generate the player who in the room
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player p = PhotonNetwork.PlayerList[i];
            CreateRoomItem(p);
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void CreateRoomItem(Player p)
    {
        GameObject obj = Instantiate(roomPrefab, contentTf);
        obj.SetActive(true);
        RoomItem item = obj.AddComponent<RoomItem>();
        item.ownerId = p.ActorNumber;
        roomList.Add(item);

        // Set initial values for readiness and player name.
        object isReadyVal;
        if (p.CustomProperties.TryGetValue("IsReady", out isReadyVal))
        {
            item.IsReady = (bool)isReadyVal;
            item.ChangeReady(item.IsReady);
        }

        object playerNameVal;
        if (p.CustomProperties.TryGetValue("PlayerName", out playerNameVal))
        {
            item.SetPlayerName((string)playerNameVal);
        }
    }

    public void DeleteRoomItem(Player p)
    {
        RoomItem item = roomList.Find((RoomItem _item) => {return p.ActorNumber == _item.ownerId;});

        if (item != null)
        {
            roomList.Remove(item);
            Destroy(item.gameObject);
        }
    }

    void OncloseBtn()
    {
        PhotonNetwork.Disconnect();
        Game.uiManager.CloseUI(gameObject.name);
        Game.uiManager.ShowUI<LoginUI>("LoginUI");
    }

    void OnStartBtn()
    {
        // Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Loading...");
        // Debug.Log(PhotonNetwork.IsMasterClient);
        PhotonNetwork.LoadLevel("GameScene");
        Debug.Log("Start button clicked, loading game scene.");
    }

    // new player enter the room
    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreateRoomItem(newPlayer);
    }

    // player leave the room
    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        DeleteRoomItem(otherPlayer);
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {

    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        RoomItem item = roomList.Find(_item => _item.ownerId == targetPlayer.ActorNumber);

        if (item != null)
        {
            // Update readiness if it has changed.
            if (changedProps.ContainsKey("IsReady"))
            {
                item.IsReady = (bool)changedProps["IsReady"];
                item.ChangeReady(item.IsReady);
            }

            // Update player name if it has changed.
            if (changedProps.ContainsKey("PlayerName"))
            {
                string newName = (string)changedProps["PlayerName"];
                item.SetPlayerName(newName);
            }
        }

        // If you are the Master Client, check if all players are ready to start the game.
        if (PhotonNetwork.IsMasterClient)
        {
            bool isAllReady = true;
            foreach (var roomItem in roomList)
            {
                if (!roomItem.IsReady)
                {
                    isAllReady = false;
                    break;
                }
            }
            startTf.gameObject.SetActive(isAllReady);
        }
    }

    public void OnNameChanged(string newName)
    {
        Hashtable playerProperties = new Hashtable();
        playerProperties.Add("PlayerName", newName);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {

    }
}
