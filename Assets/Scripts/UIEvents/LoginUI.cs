using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LoginUI : MonoBehaviour, IConnectionCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("startBtn").GetComponent<Button>().onClick.AddListener(OnStartBtn);
        transform.Find("quitBtn").GetComponent<Button>().onClick.AddListener(OnQuitBtn);
    }

    public void OnStartBtn()
    {
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Loading...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnQuitBtn()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnConnected()
    {

    }

    public void OnConnectedToMaster()
    {
        Game.uiManager.CloseAllUI();
        // Debug.Log("Connected to master");
        Game.uiManager.ShowUI<LobbyUI>("LobbyUI");
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        Game.uiManager.CloseUI("MaskUI");
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {

    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {

    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {

    }
}
