## Display player's custom name

[PlayerNameDisplay](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/Player/PlayerNameDisplay.cs)used for display player's custom name by RPC call. 

`SetPlayerNameRPC(string name)`

Additionally, keep the display component always face to camera.


## Mini-Map Manager
[Mini-Map Manager](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/Gamemanager/MiniMapController.cs) used to create player mini-map icons and hidden icons. It is also implemented by the RPC call. When a player is instantiated, the icon type is determined by detecting local or networked players.

``` C#
public void AddPlayerIcon(GameObject player)
{
    string playerType = player.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] as string;
    PhotonView playerPv = player.GetComponent<PhotonView>();
    if (playerType == _localPlayerType)
    {
        GameObject iconPrefab = playerPv.IsMine ? localPlayerIconPrefab : networkPlayerIconPrefab;
        _playerIcon = Instantiate(iconPrefab, minimapRect).GetComponent<RectTransform>();
        _playerIcons[player] = _playerIcon;
    }
}

```

## Time Manager
[TimeManager](https://github.com/Jordan-Teaching-Technologist/Team-B/blob/main/Assets/Scripts/Gamemanager/TimeManager.cs) is responsible for managing the countdown timer and displaying it in the UI. It handles the countdown timer functionality within the game. It updates the countdown timer text in the UI and plays audio cues when the timer reaches certain thresholds.

### Methods
* `Start`: Initializes the script by retrieving references to the required components.
* `setIsCount(bool isCount)`: Sets the _iscount variable to enable or disable the countdown timer display.
* `SetCountdownTimer(float countdownTimer)`: Updates the countdown timer text based on the provided countdown time. It also plays audio cues when the countdown timer reaches specific thresholds, such as the last 10 seconds.
