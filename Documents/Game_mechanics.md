This script is the core script that handles the logic of the game, all the mechanics about the game are in this script [FightManager](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/UIEvents/FightManager.cs). 

This script is mounted on the **fight** empty game object on the GameScene. This script has logic executed only by the **MasterClient** and many remote calls (RPC).

## Initialisation of this script
This script contains several variables, starting with the spawn point for Human and Cheese camp and the SkillBalls. They are a series of empty game objects that have been set up in the GameScene, with only their **Transform** component referenced to instantiate these game characters.

In **Awake** function, the miniMap's PhotonView and a HashSet, HumanPlayerActorNumber has been prepared for the game start. Also, the winning conditions have been reset. After that,  only the **MasterClient** executes the `AssignRoles` function.

### AssignRoles
In this function, `PhotonNetwork.PlayerList` to get the number of players and determine the number of Human players. Then randomly determine which players in the room will be the Human players and the remaining players will be play as Cheese and synchronise their properties with the `CustiomProperties` settings. Once the properties have been set, Photon's `OnRoomPropertiesUpdate` callback is triggered to spawn these players.

### SpawnPlayers
This function will be called when the `OnRoomPropertiesUpdate` callback is triggered.

``` C#
GameObject playerObject;
Transform spawnPoint;
Vector3 spawnPos;
string prefabName;
byte interestGroup;
```

This is an attribute that every character has, and to simplify the code.
1. First, determine if they are a human player by `_humanPlayerActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber)`. 

2. Then to get their respective spawnPoint, and get their Transform component by `spawnPos = spawnPoint.position`;. 

3. Finally, the character instantiates by `playerObject = PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity); `

4. Remaining to carry out their mini-map setup has Cinimachine the virtual camera to follow, assigning different voice channels as well as player names to display.

``` C#
        // minimap icon display
        _miniMapPhotonView.RPC("AddPlayerIconRPC", RpcTarget.All, playerObject.GetComponent<PhotonView>().ViewID);

        if (_humanPlayerActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
        {
            SetupHumanCamera(playerObject);
        }

        // camera follow
        _vc.Follow = playerObject.transform.Find("PlayerRoot").transform;

        // display the player name
        PlayerNameDisplay nameDisplay = playerObject.GetComponentInChildren<PlayerNameDisplay>();
        if (nameDisplay != null)
        {
            nameDisplay.photonView.RPC("SetPlayerNameRPC", RpcTarget.AllBuffered, playerName);
        }

        // voice channel interest group
        Recorder recorder = playerObject.GetComponent<Recorder>();
        if (recorder != null)
        {
            recorder.InterestGroup = interestGroup;
        }
```
This is how the character is instantiated.

## End Game Conditions
### Implementation Details
The game ending conditions are also managed by the `FightManager` script, monitoring the game state and determining when the game should end.
#### Countdown Timer:
* The game has a countdown timer set to 180 seconds (3 minutes).
* The timer decreases by 1 second every second until it reaches 0.
* When the countdown timer reaches 0, the game ends.
#### Target Score:
* Each time the Cheese player dies, the target score decreases by 1.
* Each time the target score decreases, the current score increases by 1.
* When the target score reaches 0, indicating that all Cheese players have been defeated, the game ends.
#### Handle Game End:
* The `HandleGameEnd` method is called when either the countdown timer reaches 0 or the target score reaches 0.
* This method takes a boolean parameter `isHumanWin` to determine the outcome of the game. If `isHumanWin` is true, it means the Human players have won; otherwise, the Cheese players have won. After that, clear all players' `customPerporties` for the next round game.
* The method closes all UI elements, displays the appropriate win or loss UI based on the outcome, and ends the game for the local player by invoking the `endGame` method on the appropriate player controller.
* Finally, the method sets the cursor to be visible and unlocks its movement.
### Sample Code
``` C#
// Countdown timer coroutine
IEnumerator CountdownTimerCoroutine()
{
    while (countdownTimer > 0 && !_gameOver)
    {
        yield return new WaitForSeconds(1f);
        countdownTimer -= 1f;
        photonView.RPC("UpdateCountdownTimerRPC", RpcTarget.All, countdownTimer);

        if (countdownTimer <= 0)
        {
            _isHumanWin = false;
            HandleGameEnd(false);
            yield break;
        }
    }
}

// Method to handle game end
private void HandleGameEnd(bool isHumanWin)
{
    if (_gameOver) return;

    _gameOver = true;
    _isHumanWin = isHumanWin;

    if (PhotonNetwork.IsMasterClient)
    {
        var players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            Hashtable propsToRemove = new Hashtable
            {
                {"PlayerType", null}
            };
            player.SetCustomProperties(propsToRemove);
        }
    }

    photonView.RPC("EndGame", RpcTarget.All, isHumanWin);
}

// RPC method to end the game
[PunRPC]
private void EndGame(bool isHumanWin)
{
    Game.uiManager.CloseAllUI();

    // Display appropriate win/loss UI based on the outcome
    if (isHumanWin)
    {
        if (_humanPlayerActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
        {
            Game.uiManager.ShowUI<WinUI>("WinUI");
        }
        else
        {
            Game.uiManager.ShowUI<LossUI>("LossUI");
        }
    }
    else
    {
        if (_humanPlayerActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
        {
            Game.uiManager.ShowUI<LossUI>("LossUI");
        }
        else
        {
            Game.uiManager.ShowUI<WinUI>("WinUI");
        }
    }

    // End the game for the local player
    if (_localPlayer != null)
    {
        if (_localPlayer.CompareTag("Player"))
        {
            ThirdPersonController humanController = _localPlayer.GetComponent<ThirdPersonController>();
            if (humanController != null) humanController.endGame();
        }
        else if (_localPlayer.CompareTag("Target"))
        {
            CheeseThirdPerson cheeseController = _localPlayer.GetComponent<CheeseThirdPerson>();
            if (cheeseController != null) cheeseController.endGame();
        }
    }

    // Set cursor to visible and unlock its movement
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}
```
### Considerations
When implementing end game conditions, ensure that the game provides clear feedback to players about the outcome and gracefully transitions to the appropriate UI state. Additionally, consider balancing factors such as timer duration and target score to create engaging gameplay experiences.

## SkillBalls generation
### Overview
In the game, SkillBalls are critical items that provide various skills to players. These items are periodically generated and strategically visible only to specific player types—namely, the Cheese players. 
### Periodic Generation of SkillBalls
SkillBalls are spawned at regular intervals throughout the game to ensure a dynamic and engaging environment. The generation process is controlled by a coroutine that refreshes the availability of SkillBalls every 60 seconds.
```
IEnumerator SpawnSkillBallsPeriodically()
{
    while (true)
    {
        GenerateSkillBalls();
        yield return new WaitForSeconds(60);
    }
}
```
### Dynamic SkillBall Creation
During each cycle, the GenerateSkillBalls function is triggered, which manages the instantiation of new SkillBalls at predetermined spawn points across the game map. Each spawn involves selecting a SkillBall type at random from a predefined list of prefabs.
```
void GenerateSkillBalls()
{
    if (PhotonNetwork.IsMasterClient)
    {
        DeleteExistingSkillBalls();
        foreach (Transform spawnPoint in skillPointTf)
        {
            int skillType = Random.Range(0, skillBallPrefabs.Length);
            var skillBall = PhotonNetwork.Instantiate(skillBallPrefabs[skillType].name, spawnPoint.position, Quaternion.identity);
            skillBalls.Add(skillBall);
        }
    }
}
```
### Only Visibility for Cheese Players
A key feature of this system is that SkillBalls are made visible only to Cheese players.  This is achieved through the SetupHumanCamera function, which adjusts the main camera's culling mask for Human players to prevent them from seeing the SkillBalls.
```
void SetupHumanCamera(GameObject playerObject)
{
    if (_vc != null)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            int skillBallsLayer = LayerMask.NameToLayer("Skillballs");
            mainCamera.cullingMask &= ~(1 << skillBallsLayer);
        }
    }
}
```
## RPCs
There are several RPCs in the FightManager, mainly used to control the countdown and display UI.

`private void UpdateCountdownTimerRPC(float newTimer)` used for setting countdownTimer for all player.

`private void EndGame(bool isHumanWin)` used for showing the Ui when the game end.

