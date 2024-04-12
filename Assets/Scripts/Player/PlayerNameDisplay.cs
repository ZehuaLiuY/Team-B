using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerNameDisplay : MonoBehaviourPun {

    private TextMeshProUGUI _playerNameText;

    void Awake() {
        _playerNameText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start() {
        if(photonView.IsMine) {
            _playerNameText.enabled = false;
        }
    }

    [PunRPC]
    public void SetPlayerNameRPC(string name) {
        if (_playerNameText != null) {
            _playerNameText.text = name;
        } else {
            Debug.LogError("Text component not found on the GameObject.");
        }
    }
    
    void Update() {
        if(Camera.main != null) {
            var cameraPosition = Camera.main.transform.position;

            transform.LookAt(transform.position * 2 - cameraPosition);
        }
    }
}