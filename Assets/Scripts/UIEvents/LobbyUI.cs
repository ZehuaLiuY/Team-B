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

    void Start()
    {
        transform.Find("content/title/closeBtn").GetComponent<Button>().onClick.AddListener(OnCloseBtn);
        transform.Find("content/createBtn").GetComponent<Button>().onClick.AddListener(OnCreateRoomBtn);
        transform.Find("content/updateBtn").GetComponent<Button>().onClick.AddListener(OnUpdateRoomBtn);

        _contentTf = transform.Find("content/Scroll View/Viewport/Content");
        _roomPrefab = transform.Find("content/Scroll View/Viewport/item").gameObject;
        // join lobby
        _lobby = new TypedLobby("myLobby", LobbyType.SqlLobby);
        PhotonNetwork.JoinLobby(_lobby);
    }

    public override void OnJoinedLobby()
    {

    }

    public void OnCloseBtn()
    {
        PhotonNetwork.Disconnect();
        Game.uiManager.CloseUI(gameObject.name);
        Game.uiManager.ShowUI<LoginUI>("LoginUI");
    }

    public void OnCreateRoomBtn()
    {
        Game.uiManager.ShowUI<CreateRoomUI>("CreateRoomUI");
    }

    public void OnUpdateRoomBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Updating room list...");
        PhotonNetwork.GetCustomRoomList(_lobby, "1");
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
