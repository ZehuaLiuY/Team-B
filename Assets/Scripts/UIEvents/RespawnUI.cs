using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RespawnUI : MonoBehaviour
{


    void Start()
    {
        RespawnCountdown(5);
        Cursor.visible = false;
    }

    IEnumerator RespawnCountdown(int seconds)
    {
        while (seconds > 0)
        {
            Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("You will be respawn in " + seconds + " sec...");
            yield return new WaitForSeconds(1);
            seconds--;
        }



        //GameObject fightManagerObject = GameObject.Find("fight");
        //if (fightManagerObject != null)
        //{
        //    FightManager fightManager = fightManagerObject.GetComponent<FightManager>();
        //    fightManager.RespawnCheese();
        //}
    }
}
