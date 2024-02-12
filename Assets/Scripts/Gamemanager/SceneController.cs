using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public GameObject human;  // Assign the human character in the inspector
    public GameObject cheese; // Assign the cheese character in the inspector
    public GameObject gameoverCanvas;

    void Start()
    {
        // 获取保存的选择的角色名称
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter");

        // 根据选择激活相应的角色并禁用其他角色
        switch (selectedCharacter)
        {
            case "Human":
                human.SetActive(true);
                cheese.SetActive(false);
                break;
            case "Cheese":
                human.SetActive(false);
                cheese.SetActive(true);
                break;
            default:
                Debug.LogError("Selected character not recognized.");
                break;
        }
    }
    public void SwitchToGameoverUI(bool isHumanWin)
    {
        gameoverCanvas.SetActive(true);
        if (isHumanWin)
        {
            TextMeshProUGUI humanWinText = gameoverCanvas.transform.Find("Human_Win").GetComponent<TextMeshProUGUI>();
            humanWinText.gameObject.SetActive(true);
        }
        else
        {
            TextMeshProUGUI cheeseWinText = gameoverCanvas.transform.Find("Cheese_Win").GetComponent<TextMeshProUGUI>();
            cheeseWinText.gameObject.SetActive(true);
        }

        human.SetActive(false);
        cheese.SetActive(false);
    }

}
