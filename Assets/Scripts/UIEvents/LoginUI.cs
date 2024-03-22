using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LoginUI : MonoBehaviour, IConnectionCallbacks
{
    private bool isTryingToConnect = false;
    private float connectionTimeout = 10.0f; // 10 seconds for timeout
    private float connectionStartTime;

    // Start is called before the first frame update
    void Start()
    {
        transform.Find("startBtn").GetComponent<Button>().onClick.AddListener(OnStartBtn);
        transform.Find("quitBtn").GetComponent<Button>().onClick.AddListener(OnQuitBtn);
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnStartBtn()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("No internet connection available. Please connect to the internet and try again.");
            return; // Exit if no internet
        }

        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Loading...");

        isTryingToConnect = true;
        connectionStartTime = Time.time;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (isTryingToConnect && (Time.time - connectionStartTime > connectionTimeout))
        {
            // Timeout logic
            isTryingToConnect = false;
            PhotonNetwork.Disconnect();
            Game.uiManager.CloseUI("MaskUI");
            Debug.Log("Connection timed out. Please check your network and try again.");
        }
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
        isTryingToConnect = false;
        Game.uiManager.CloseAllUI();
        // Debug.Log("Connected to master");
        Game.uiManager.ShowUI<LobbyUI>("LobbyUI");
        PhotonNetwork.NetworkStatisticsEnabled = true;
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        isTryingToConnect = false;
        Game.uiManager.CloseUI("MaskUI");
        Debug.Log($"Disconnected: {cause}");
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
