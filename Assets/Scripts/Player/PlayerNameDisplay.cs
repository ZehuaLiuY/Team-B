using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerNameDisplay : MonoBehaviourPunCallbacks
{
    private Text _nameTag;
    private void UpdateNameDisplay()
    {
        if (photonView.IsMine)
        {
            // Don't show the name tag for the local player to themselves
            _nameTag.enabled = false;
        }
        else
        {
            // Set the text of the name tag to the player's custom property
            if (photonView.Owner.CustomProperties.TryGetValue("PlayerName", out object playerName))
            {
                _nameTag.text = playerName.ToString();
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // If the target player is this player and the PlayerName property has changed
        if (targetPlayer == photonView.Owner && changedProps.ContainsKey("PlayerName"))
        {
            UpdateNameDisplay();
        }
    }

    // Call this method to initialize the name display
    public void InitializeNameDisplay()
    {
        UpdateNameDisplay();
    }

    public void SetName(string name)
    {
        _nameTag.text = name;
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
