using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("resetBtn").GetComponent<Button>().onClick.AddListener(OnQuitBtn);
    }

    public void OnQuitBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Loading...");
        Game.uiManager.CloseAllUI();
        // disconnect from server
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("Login");
        // Game.uiManager.ShowUI<LobbyUI>("LoginUI");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
