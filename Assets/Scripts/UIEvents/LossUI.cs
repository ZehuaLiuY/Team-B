using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LossUI : MonoBehaviourPunCallbacks
{
    private AudioClip _buttonClickSound;
    private AudioSource _audioSource;
    private bool _isButtonClicked;
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        _audioSource = transform.Find("audioSource").GetComponent<AudioSource>();
        _buttonClickSound = Resources.Load<AudioClip>("Button");

        _isButtonClicked = false;

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

    // quit to log-in scene
    private void OnQuitBtn()
    {
        if (!_isButtonClicked)
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
        else
        {

        }
    }

    // back to the room
    private void OnBackBtn()
    {
        if (!_isButtonClicked)
        {
            var player = PhotonNetwork.LocalPlayer;
            if (player != null && player.CustomProperties.ContainsKey("PlayerType"))
            {
                Hashtable props = new Hashtable
                {
                    { "PlayerType", null }
                };
                player.SetCustomProperties(props);
            }

            var voiceBridge = GameObject.Find("VoiceBridge");
            var voiceLogger = GameObject.Find("VoiceLogger");
            if (voiceBridge != null)
            {
                Destroy(voiceBridge);
            }

            if (voiceLogger != null)
            {
                Destroy(voiceLogger);
            }

            // Show a loading mask or any other indication to the player
            // Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Loading...");
            PhotonNetwork.LoadLevel("Login");
            Game.uiManager.CloseAllUI();
            Game.uiManager.ShowUI<RoomUI>("RoomUI");
        }
        else
        {

        }
    }
}

