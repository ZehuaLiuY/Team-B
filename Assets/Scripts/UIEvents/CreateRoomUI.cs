using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoomUI : MonoBehaviourPunCallbacks
{
    private AudioClip _buttonClickSound;
    private AudioSource _audioSource; 
    private InputField _roomNameInput;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = transform.Find("audioSource").GetComponent<AudioSource>();
        _buttonClickSound = Resources.Load<AudioClip>("Button");
       
        Button okBtn = transform.Find("bg/okBtn").GetComponent<Button>();
        okBtn.onClick.AddListener(OnCreateBtn);
        okBtn.onClick.AddListener(PlayButtonClickSound);  

        Button closeBtn = transform.Find("bg/title/closeBtn").GetComponent<Button>();
        closeBtn.onClick.AddListener(OnCloseBtn);
        closeBtn.onClick.AddListener(PlayButtonClickSound); 
        _roomNameInput = transform.Find("bg/InputField").GetComponent<InputField>();

        _roomNameInput.text = "Room_" + Random.Range(0, 1000);
    }
    
    private void PlayButtonClickSound()
    {
        if (_audioSource != null && _buttonClickSound != null)
        {
            _audioSource.PlayOneShot(_buttonClickSound);
        }
    }



    public void OnCreateBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Creating room...");
        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(_roomNameInput.text, room);
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
