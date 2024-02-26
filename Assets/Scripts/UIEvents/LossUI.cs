using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LossUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("resetBtn").GetComponent<Button>().onClick.AddListener(OnQuitBtn);

        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }
    public void OnQuitBtn()
    {
        
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
