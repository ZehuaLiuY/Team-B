using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerNameDisplay : MonoBehaviourPunCallbacks
{
    private Text _nameTag;

    private void Awake()
    {
        _nameTag = GetComponentInChildren<Text>();
    }

    private void UpdateNameDisplay(string name)
    {
        if (photonView.IsMine)
        {
            // Don't show the name tag for the local player to themselves
            _nameTag.enabled = false;
        }
        else
        {
            _nameTag.text = name;
            Debug.Log("PlayerNameDisplay: " + _nameTag.text);
        }
    }

    // Call this method to initialize the name display
    public void InitializeNameDisplay(string name)
    {
        UpdateNameDisplay(name);
    }

    void Update()
    {
        var cam = Camera.main;
        if(cam != null)
        {
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
    }


}
