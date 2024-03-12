using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomItem : MonoBehaviour
{
    public int ownerId;
    public bool IsReady = false;
    public InputField nameInputField;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
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
        ChangeReady(IsReady);
    }

    public void OnReadyBtn()
    {
        IsReady = !IsReady;

        Hashtable table = new Hashtable();
        table.Add("IsReady", IsReady);

        PhotonNetwork.LocalPlayer.SetCustomProperties(table);

        ChangeReady(IsReady);

        nameInputField.interactable = !IsReady;


        if (IsReady)
        {
            string newName = nameInputField.text;
            Hashtable props = new Hashtable();
            props.Add("PlayerName", newName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }


    public void ChangeReady(bool isReady)
    {
        transform.Find("Button/Text").GetComponent<Text>().text = isReady == true ? "Ready" : "Not Ready";

    }

    private void OnNameChanged(string newName)
    {
        // 只有本地玩家才能更改名字
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