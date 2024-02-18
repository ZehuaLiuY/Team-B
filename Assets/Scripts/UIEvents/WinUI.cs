using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("resetBtn").GetComponent<Button>().onClick.AddListener(OnQuitBtn);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnQuitBtn()
    {
        // Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Loading...");
        // Game.uiManager.CloseAllUI();
        // // disconnect from server
        // PhotonNetwork.Disconnect();
        //
        // StartCoroutine(WaitForDisconnect());
        Application.Quit();
    }

    // IEnumerator WaitForDisconnect()
    // {
    //     Debug.Log("Started waiting for disconnect...");
    //     while (PhotonNetwork.IsConnected)
    //     {
    //         yield return null;
    //     }
    //     Debug.Log("Disconnected from Photon, loading login scene...");
    //     SceneManager.LoadScene("login");
    //
    //     // Ensure the scene has fully loaded
    //     yield return new WaitForEndOfFrame();
    //     Debug.Log("Login scene should be loaded now.");
    //     // Make sure there's an EventSystem in the scene for UI interactivity
    //     if (GameObject.FindObjectOfType<EventSystem>() == null)
    //     {   Debug.Log("No EventSystem found, creating a new one.");
    //         // Instantiate a new EventSystem if one does not exist
    //         new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    //     }
    //     else
    //     {
    //         Debug.Log("EventSystem found.");
    //     }
    //
    //     // Activate or instantiate the LoginUI GameObject
    //     Game.uiManager.ShowUI<LobbyUI>("LoginUI");
    //     Debug.Log("LoginUI should be shown now.");
    // }

    // Update is called once per frame
    void Update()
    {

    }
}
