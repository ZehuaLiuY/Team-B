using CheeseController;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RespawnUI : MonoBehaviour
{

    private int _deadCheeseViewID;
    
    void Start()
    {
        StartCoroutine(RespawnCountdown(5));
        Cursor.visible = false;
    }

    public void setDeadCheese(int deadCheeseViewID)
    {
        Debug.Log("Enter setDeadCheese");
        _deadCheeseViewID = deadCheeseViewID;
    }

    IEnumerator RespawnCountdown(int seconds)
    {
        Debug.Log("Enter countDown");
        while (seconds > 0)
        {
            Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("You will be respawn in " + seconds + " sec...");
            yield return new WaitForSeconds(1);
            seconds--;
        }

        GameObject fightManagerObject = GameObject.Find("fight");
        if (fightManagerObject != null)
        {
            FightManager fightManager = fightManagerObject.GetComponent<FightManager>();
            fightManager.respawnCheese(_deadCheeseViewID);
        }
        //GameObject fightManagerObject = GameObject.Find("fight");
        //if (fightManagerObject != null)
        //{
        //    FightManager fightManager = fightManagerObject.GetComponent<FightManager>();
        //    fightManager.RespawnCheese();
        //}

        //PhotonView.RPC("RespawnCheese", )
    }
}
