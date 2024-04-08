using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomItem : MonoBehaviour
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

        ChangeReady(isReady);

        nameInputField.interactable = !isReady;


        if (isReady)
        {
            string newName = nameInputField.text;
            Hashtable props = new Hashtable();
            props.Add("PlayerName", newName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
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
}