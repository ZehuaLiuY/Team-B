using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoomUI : MonoBehaviourPunCallbacks
{

    InputField roomNameInput;
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("bg/okBtn").GetComponent<Button>().onClick.AddListener(OnCreateBtn);
        transform.Find("bg/title/closeBtn").GetComponent<Button>().onClick.AddListener(OnCloseBtn);
        roomNameInput = transform.Find("bg/InputField").GetComponent<InputField>();

        roomNameInput.text = "Room_" + Random.Range(0, 1000);
    }


    public void OnCreateBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Creating room...");
        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomNameInput.text, room);
    }

    public void OnCloseBtn()
    {
        Game.uiManager.CloseUI(gameObject.name);
        Game.uiManager.ShowUI<LobbyUI>("LobbyUI");
    }

    public override void OnCreatedRoom()
    {
        Game.uiManager.CloseAllUI();
        // Game.uiManager.CloseUI(gameObject.name);
        Game.uiManager.ShowUI<RoomUI>("RoomUI");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Game.uiManager.CloseUI("MaskUI");
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Create room failed: " + message);
    }
}
