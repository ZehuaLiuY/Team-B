using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LossUI : MonoBehaviourPunCallbacks
{
    private AudioClip _buttonClickSound;
    private AudioSource _audioSource; 
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        _audioSource = transform.Find("audioSource").GetComponent<AudioSource>();
        _buttonClickSound = Resources.Load<AudioClip>("Button");

        Button resetButton = transform.Find("resetBtn").GetComponent<Button>();
        resetButton.onClick.AddListener(OnQuitBtn);
        resetButton.onClick.AddListener(PlayButtonClickSound);
    }
    
    private void PlayButtonClickSound()
    {
        if (_audioSource != null && _buttonClickSound != null)
        {
            _audioSource.PlayOneShot(_buttonClickSound);
        }
    }

    public void OnQuitBtn()
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
}

