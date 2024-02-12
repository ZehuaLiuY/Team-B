using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static UIManager uiManager;
    public static bool isLoaded = false;

    private void Awake()
    {
        if (isLoaded == false)
        {
            isLoaded = true;
            DontDestroyOnLoad(gameObject);
            uiManager = new UIManager();
            uiManager.Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // display login ui
        uiManager.ShowUI<LoginUI>("LoginUI");
        
    }
}
