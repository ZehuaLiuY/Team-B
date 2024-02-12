using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowMask(string msg)
    {
        transform.Find("msg/bg/Text").GetComponent<UnityEngine.UI.Text>().text = msg;
        gameObject.SetActive(true);
    }
}
