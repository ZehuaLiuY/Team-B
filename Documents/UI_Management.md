For UI management, the first step was to create what the various UIs would look like in Canavs and save them as prefabs. The UI is stored in a folder called Resource. Unified use of [UIManager](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/UIEvents/UIManager.cs) for UIs management. 

### The UIManager Script
This script is mounted and instantiated on the Game script, which can be accessed via `Game.uiManager.` It automatically find the Canvas component when you start the game by the `GameObject.Find("Canvas").transform;` and initialise the uiList. 

This script provides the following function to help you manage your UIs:
1. `ShowUI<T>(string uiName)`

This function allows you to show your exist UI where locate in the **Resources** folder, you can simply call by `Game.uiManager.ShowUI<Your UIName>(Your UIName)`. Each time you call this function, the Canvas component is looked for or created again to prevent null pointers by `FindOrCreateCanvas` function.

2. `HideUI(string uiName)`

This function allows you to hide any UI already shown in the Canvas, you can simply call by `Game.uiManager.HideUI(Your UIName)`

3. `CloseAllUI()`

This function allows you to close all UIs already shown in the Canvas, you can simply call by `Game.uiManager.CloseAllUI()`. This function is commonly used when the game end or switch the scene in this project.

These three functions can help you to manage your UIs. It can not only manage UI but also find UI through `Find`, and `GetUI` functions. However, we only recommend using the above three methods to manage your UIs

