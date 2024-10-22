using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.PUN;

public class WinUI : MonoBehaviour
{
    private AudioClip _buttonClickSound;
    private AudioSource _audioSource; 
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        _audioSource = transform.Find("audioSource").GetComponent<AudioSource>();
        _buttonClickSound = Resources.Load<AudioClip>("Button");
        
        Button resetButton = transform.Find("resetBtn").GetComponent<Button>();
        Button roomButton = transform.Find("roomBtn").GetComponent<Button>();
        resetButton.onClick.AddListener(OnQuitBtn);
        roomButton.onClick.AddListener(OnBackBtn);
        resetButton.onClick.AddListener(PlayButtonClickSound);
        roomButton.onClick.AddListener(PlayButtonClickSound);
    }
    
    private void PlayButtonClickSound()
    {
        if (_audioSource != null && _buttonClickSound != null)
        {
            _audioSource.PlayOneShot(_buttonClickSound);
        }
    }

    private void OnQuitBtn()
    {
        // Show a loading mask or any other indication to the player
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Loading...");

        GameObject fightManagerObject = GameObject.Find("fight");
        if (fightManagerObject != null)
        {
            FightManager fightManager = fightManagerObject.GetComponent<FightManager>();
            fightManager.QuitToLoginScene();
        }
    }

    // back to the room
    private void OnBackBtn()
    {
        var voiceBridge = GameObject.Find("VoiceBridge");
        if (voiceBridge != null)
        {
            Destroy(voiceBridge);
        }

        // Show a loading mask or any other indication to the player
        // Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Loading...");
        PhotonNetwork.LoadLevel("Login");
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<RoomUI>("RoomUI");

    }
}
