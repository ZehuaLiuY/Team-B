using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    private Transform _canvasTf; // Canvas transform component

    private List<GameObject> _uiList; // Collection to store loaded interfaces

    public void Init()
    {
        // Find the canvas in the world
        _canvasTf = GameObject.Find("Canvas").transform;
        // Initialize the collection
        _uiList = new List<GameObject>();
    }

    private void FindOrCreateCanvas()
    {
        if (_canvasTf == null || _canvasTf.gameObject == null)
        {
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null) // Canvas does not exist, create a new one
            {
                canvasObj = new GameObject("Canvas");
                canvasObj.AddComponent<Canvas>();
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                canvasObj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }
            _canvasTf = canvasObj.transform;
        }
    }

    // Show
    public T ShowUI<T>(string uiName) where T : Component
    {
        FindOrCreateCanvas();
        T ui = Find<T>(uiName);
        if (ui == null)
        {
            // Not in the collection, need to load from Resources/UI folder
            GameObject obj = Object.Instantiate(Resources.Load("UI/" + uiName), _canvasTf) as GameObject;

            // Rename
            obj.name = uiName;

            // Add the required script
            ui = obj.AddComponent<T>();

            // Add to the collection for storage
            _uiList.Add(obj);
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
        for (int i = _uiList.Count - 1; i >= 0; i--)
        {
            Object.Destroy(_uiList[i].gameObject);
        }

        _uiList.Clear();// Clear the collection
    }

    // Close a specific interface
    public void CloseUI(string uiName)
    {
        GameObject ui = Find(uiName);
        if (ui != null)
        {
            _uiList.Remove(ui);
            Object.Destroy(ui.gameObject);
        }
    }

    // Find a specific interface
    public T Find<T>(string uiName) where T : Component
    {
        for (int i = 0; i < _uiList.Count; i++)
        {
            if (_uiList[i] != null && _uiList[i].name == uiName)
            {
                T component = _uiList[i].GetComponent<T>();
                if (component != null)
                    return component;
            }
        }
        return null;
    }

    public GameObject Find(string uiName)
    {
        for (int i = 0; i < _uiList.Count; i++)
        {
            if (_uiList[i].name == uiName)
            {
                return _uiList[i];
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
