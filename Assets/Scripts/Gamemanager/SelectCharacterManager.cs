using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectCharacterManager : MonoBehaviour
{
    // 这个方法将在选择角色后立即加载游戏场景，不再更新UI显示选定的角色
    public void SelectCharacter(string characterName)
    {
        // 保存选择的角色信息，这里我们使用PlayerPrefs
        PlayerPrefs.SetString("SelectedCharacter", characterName);

        // 加载游戏场景
        SceneManager.LoadScene("GameScene");
    }
}
