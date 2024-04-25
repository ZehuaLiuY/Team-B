using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomUI : MonoBehaviour, IInRoomCallbacks
{
    Transform startTf;
    Transform contentTf;
    GameObject roomPrefab;
    public List<RoomItem> roomList;
    private AudioClip _buttonClickSound;
    private AudioSource _audioSource;
    private bool _isButtonClicked = false;

    private void Awake()
    {
        _audioSource = transform.Find("audioSource").GetComponent<AudioSource>();
        _buttonClickSound = Resources.Load<AudioClip>("Button");
        
        roomList = new List<RoomItem>();
        contentTf = transform.Find("bg/Content");
        roomPrefab = transform.Find("bg/roomItem").gameObject;
        
        startTf = transform.Find("bg/startBtn");
        startTf.GetComponent<Button>().onClick.AddListener(OnStartBtn);
        startTf.GetComponent<Button>().onClick.AddListener(PlayButtonClickSound);

        Button closeBtn = transform.Find("bg/title/closeBtn").GetComponent<Button>();
        closeBtn.onClick.AddListener(OncloseBtn);
        closeBtn.onClick.AddListener(PlayButtonClickSound);

        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    private void PlayButtonClickSound()
    {
        if (_audioSource != null && _buttonClickSound != null)
        {
            _audioSource.PlayOneShot(_buttonClickSound);
        }
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

    private void CreateRoomItem(Player p)
    {
        GameObject obj = Instantiate(roomPrefab, contentTf);
        obj.SetActive(true);
        RoomItem item = obj.AddComponent<RoomItem>();
        item.ownerId = p.ActorNumber;
        roomList.Add(item);

        // Set initial values for readiness and player name.
        if (p.CustomProperties.TryGetValue("IsReady", out object isReadyVal))
        {
            item.isReady = (bool)isReadyVal;
            item.ChangeReady(item.isReady);
        }
        if (p.CustomProperties.TryGetValue("PlayerName", out object playerNameVal))
        {
            item.SetPlayerName((string)playerNameVal);
        }
    }

    private void DeleteRoomItem(Player p)
    {
        RoomItem item = roomList.Find((RoomItem _item) => {return p.ActorNumber == _item.ownerId;});

        if (item != null)
        {
            roomList.Remove(item);
            Destroy(item.gameObject);
        }
    }

    private void OncloseBtn()
    {
        PhotonNetwork.Disconnect();
        Game.uiManager.CloseUI(gameObject.name);
        Game.uiManager.ShowUI<LoginUI>("LoginUI");
    }

    private void OnStartBtn()
    {
        if (!_isButtonClicked)
        {
            _isButtonClicked = true;

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            StartCoroutine(WaitAndLoadScene());
        }

    }

    private void ResetAllPlayersReadyState()
    {
        var playerList = PhotonNetwork.PlayerList;
        foreach (var player in playerList)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable props = new Hashtable {{"IsReady", false}};
                player.SetCustomProperties(props);
            }
        }
    }

    IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.LoadLevel("GameScene");
        ResetAllPlayersReadyState();
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
                item.isReady = (bool)changedProps["IsReady"];
                item.ChangeReady(item.isReady);
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
                if (!roomItem.isReady)
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
