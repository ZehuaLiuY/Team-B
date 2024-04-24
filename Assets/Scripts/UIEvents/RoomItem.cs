using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomItem : MonoBehaviourPunCallbacks
{
    public int ownerId;
    public bool isReady;
    public InputField nameInputField;

    void Awake()
    {
        isReady = false;
    }

    void Start()
    {
        nameInputField = transform.Find("InputField").GetComponent<InputField>();
        nameInputField.characterLimit = 8;

        if (ownerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnReadyBtn);
            nameInputField.interactable = true;
            transform.Find("Button").GetComponent<Image>().color = new Color(0.506f, 0.035f, 0.890f, 1.0f);
        }
        else
        {
            transform.Find("Button").GetComponent<Image>().color = Color.black;
            nameInputField.interactable = false;
        }

        ChangeReady(isReady);
    }

    public void OnReadyBtn()
    {
        isReady = !isReady;

        Hashtable table = new Hashtable();
        table.Add("IsReady", isReady);

        PhotonNetwork.LocalPlayer.SetCustomProperties(table);

        Button button = transform.Find("Button").GetComponent<Button>();
        Image buttonImage = button.GetComponent<Image>();
        Debug.Log("Current Button Color: " + buttonImage.color);

        if (isReady)
        {
            buttonImage.color = Color.green;
            string newName = nameInputField.text;
            Hashtable props = new Hashtable();
            props.Add("PlayerName", newName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        else
        {
            buttonImage.color = new Color(0.506f, 0.035f, 0.890f, 1.0f);
        }

        ChangeReady(isReady);

        nameInputField.interactable = !isReady;

    }


    public void ChangeReady(bool isReady)
    {
        transform.Find("Button/Text").GetComponent<Text>().text = isReady == true ? "Ready!" : "Ready?";
    }

    private void OnNameChanged(string newName)
    {
        // only the local player can change the name
        if (ownerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Hashtable props = new Hashtable();
            props.Add("PlayerName", newName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            // photonView.RPC("RpcSyncNames", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, newName);
        }
    }

    public void SetPlayerName(string name)
    {
        if (nameInputField != null)
        {
            nameInputField.text = name;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer.ActorNumber == ownerId && changedProps.ContainsKey("IsReady"))
        {
            isReady = (bool)changedProps["IsReady"];
            UpdateButtonColor();
        }
    }
    
    private void UpdateButtonColor()
    {
        Image buttonImage = transform.Find("Button").GetComponent<Image>();
        if (isReady)
        {
            buttonImage.color = Color.green;
        }
        else
        {
            buttonImage.color = Color.black;
        }

        ChangeReady(isReady);
    }
}