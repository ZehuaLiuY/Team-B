using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private Transform canvasTf; // Canvas transform component

    private List<GameObject> uiList; // Collection to store loaded interfaces

    public void Init()
    {
        // Find the canvas in the world
        canvasTf = GameObject.Find("Canvas").transform;
        // Initialize the collection
        uiList = new List<GameObject>();
    }

    // Show
    public T ShowUI<T>(string uiName) where T : Component
    {
        T ui = Find<T>(uiName);
        if (ui == null)
        {
            // Not in the collection, need to load from Resources/UI folder
            GameObject obj = Object.Instantiate(Resources.Load("UI/" + uiName), canvasTf) as GameObject;

            // Rename
            obj.name = uiName;

            // Add the required script
            ui = obj.AddComponent<T>();

            // Add to the collection for storage
            uiList.Add(obj);
        }
        else
        {
            // Show
            ui.gameObject.SetActive(true);
        }

        return ui;
    }

    // Hide
    public void HideUI(string uiName)
    {
        GameObject ui = Find(uiName);
        if (ui != null)
        {
            ui.SetActive(true);
        }
    }

    // Close all interfaces
    public void CloseAllUI()
    {
        for (int i = uiList.Count - 1; i >= 0; i--)
        {
            Object.Destroy(uiList[i].gameObject);
        }

        uiList.Clear();// Clear the collection
    }

    // Close a specific interface
    public void CloseUI(string uiName)
    {
        GameObject ui = Find(uiName);
        if (ui != null)
        {
            uiList.Remove(ui);
            Object.Destroy(ui.gameObject);
        }
    }

    // Find a specific interface
    public T Find<T>(string uiName) where T : Component
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            if (uiList[i].name == uiName)
            {
                return uiList[i].GetComponent<T>();
            }
        }
        return null;
    }

    public GameObject Find(string uiName)
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            if (uiList[i].name == uiName)
            {
                return uiList[i];
            }
        }
        return null;
    }

    // Get the required component from the interface
    public T GetUI<T>(string uiName) where T : Component
    {
        GameObject ui = Find(uiName);
        if (ui != null)
        {
            return ui.GetComponent<T>();
        }
        return null;
    }
}
