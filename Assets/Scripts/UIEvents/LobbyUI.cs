using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviourPunCallbacks
{
    private TypedLobby _lobby;

    private Transform _contentTf;
    private GameObject _roomPrefab;
    private AudioClip _buttonClickSound;
    private AudioSource _audioSource;
    private bool _isButtonClicked;

    void Start()
    {
        _audioSource = transform.Find("audioSource").GetComponent<AudioSource>();
        _buttonClickSound = Resources.Load<AudioClip>("Button");
        
        Button closeBtn = transform.Find("content/title/closeBtn").GetComponent<Button>();
        closeBtn.onClick.AddListener(OnCloseBtn);
        closeBtn.onClick.AddListener(PlayButtonClickSound);

        Button createBtn = transform.Find("content/createBtn").GetComponent<Button>();
        createBtn.onClick.AddListener(OnCreateRoomBtn);
        createBtn.onClick.AddListener(PlayButtonClickSound);

        Button updateBtn = transform.Find("content/updateBtn").GetComponent<Button>();
        updateBtn.onClick.AddListener(OnUpdateRoomBtn);
        updateBtn.onClick.AddListener(PlayButtonClickSound);

        _contentTf = transform.Find("content/Scroll View/Viewport/Content");
        _roomPrefab = transform.Find("content/Scroll View/Viewport/item").gameObject;
        // join lobby
        _lobby = new TypedLobby("myLobby", LobbyType.SqlLobby);
        PhotonNetwork.JoinLobby(_lobby);
    }
    
    private void PlayButtonClickSound()
    {
        if (_audioSource != null && _buttonClickSound != null)
        {
            _audioSource.PlayOneShot(_buttonClickSound);
        }
    }

    public override void OnJoinedLobby()
    {

    }

    public void OnCloseBtn()
    {
        if (!_isButtonClicked)
        {
            PhotonNetwork.Disconnect();
            Game.uiManager.CloseUI(gameObject.name);
            Game.uiManager.ShowUI<LoginUI>("LoginUI");
        }
        else
        {

        }
    }

    public void OnCreateRoomBtn()
    {
        if (!_isButtonClicked)
        {
            Game.uiManager.ShowUI<CreateRoomUI>("CreateRoomUI");
        }
        else
        {

        }
    }

    public void OnUpdateRoomBtn()
    {
        if (!_isButtonClicked)
        {
            Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Updating room list...");
            PhotonNetwork.GetCustomRoomList(_lobby, "1");
        }
        else
        {

        }
    }

    // clear exist room item
    private void ClearRoomList()
    {
        while (_contentTf.childCount != 0)
        {
            DestroyImmediate(_contentTf.GetChild(0).gameObject);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Game.uiManager.CloseUI("MaskUI");

        ClearRoomList();

        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject obj = Instantiate(_roomPrefab, _contentTf);
            obj.SetActive(true);
            string roomName = roomList[i].Name;
            obj.transform.Find("roomName").GetComponent<Text>().text = roomName;
            obj.transform.Find("joinBtn").GetComponent<Button>().onClick.AddListener(delegate()
            {
                // join room
                Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Joining room...");
                PhotonNetwork.JoinRoom(roomName);
            });

        }
    }

    public override void OnJoinedRoom()
    {
        // join room successfully
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<RoomUI>("RoomUI");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //join room failed
        Game.uiManager.CloseUI("MaskUI");
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Join room failed: " + message);
        StartCoroutine(ReEnterLobbyAfterDelay(3f));
    }

    private IEnumerator ReEnterLobbyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Game.uiManager.CloseUI("MaskUI");
        Game.uiManager.ShowUI<LobbyUI>("LobbyUI");
    }
}
