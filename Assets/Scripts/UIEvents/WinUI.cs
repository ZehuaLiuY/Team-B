using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Assuming the button is called "resetBtn" in your scene
        transform.Find("resetBtn").GetComponent<Button>().onClick.AddListener(OnQuitBtn);
    }

    public void OnQuitBtn()
    {
        // Show a loading mask or any other indication to the player
        Game.uiManager.ShowUI<MaskUI>("MaskUI").ShowMask("Loading...");

        GameObject fightManagerObject = GameObject.Find("fight");
        if (fightManagerObject != null)
        {
            FightManager fightManager = fightManagerObject.GetComponent<FightManager>();
            fightManager.QuitToLoginScene();
        }
    }
}
