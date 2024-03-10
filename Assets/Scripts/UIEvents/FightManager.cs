using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using UnityEditor.Rendering;
using Photon.Pun.UtilityScripts;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine.Serialization;
using static UnityEngine.Rendering.DebugUI;


public class FightManager : MonoBehaviourPunCallbacks
{
    private bool _gameOver = false; // 游戏是否已经结束
    // private float captureDistance = 2f; // 抓住奶酪的距离阈值

    public Transform pointTf; // respawn points
    private PhotonView _photonView;

    private FightUI fightUI;
    private FightUI1 fightUI1;
    private FightUI2 fightUI2;
    private FightUI3 fightUI3;
    public static float countdownTimer = 180f;
    private bool _isHumanWin;
    private int humanPlayerActorNumber;
    private int _remainingCheeseCount; // 剩余活着的 cheese 数量
    private bool _allCheeseDie = false;

    public MiniMapController miniMapController;

    public string playerName;

    private PhotonView _miniMapPhotonView;


    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _miniMapPhotonView = miniMapController.GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            AssignRoles();
        }
    }

    // void Start()
    // {
    //     Game.uiManager.CloseAllUI();
    //     fightUI = Game.uiManager.ShowUI<FightUI>("FightUI");
    //
    //     Game.uiManager.ShowUI<FightUI>("FightUI");
    //     _remainingCheeseCount = PhotonNetwork.CurrentRoom.PlayerCount - 1; // 减去1是因为其中一个玩家是人类玩家
    // }
    
    void Start()
    {
        Game.uiManager.CloseAllUI();
        _remainingCheeseCount = PhotonNetwork.CurrentRoom.PlayerCount - 1; // 减去1是因为其中一个玩家是人类玩家
    }
    
    // void DisplayUIBasedOnRole()
    // {
    //     string playerType = (string)PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"];
    //
    //     if (playerType == "Human")
    //     {
    //         fightUI = Game.uiManager.ShowUI<FightUI>("Human_FightUI");
    //     }
    //     else if (playerType == "Cheese")
    //     {
    //         fightUI1 = Game.uiManager.ShowUI<FightUI1>("Cheese_FightUI");
    //     }
    // }
    void DisplayUIBasedOnRole()
    {
        string playerType = (string)PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"];
        
        switch (playerType)
        {
            case "Human":
                fightUI = Game.uiManager.ShowUI<FightUI>("Human_FightUI");
                break;
            case "Cheese":
                fightUI1 = Game.uiManager.ShowUI<FightUI1>("Cheese_FightUI");
                break;
            case "Cheese1":
                fightUI2 = Game.uiManager.ShowUI<FightUI2>("Cheese_FightUI");
                break;
            case "Cheese2":
                fightUI3 = Game.uiManager.ShowUI<FightUI3>("Cheese_FightUI");
                break;
            default:
                Debug.LogError("Unknown player type: " + playerType);
                break;
        }
    }

    // void AssignRoles()
    // {
    //     var players = PhotonNetwork.PlayerList;
    //     List<int> playerIndices = new List<int>();
    //     for (int i = 0; i < players.Length; i++)
    //     {
    //         playerIndices.Add(i);
    //     }
    //
    //     // random select a human player
    //     int humanIndex = playerIndices[Random.Range(0, playerIndices.Count)];
    //     playerIndices.RemoveAt(humanIndex);
    //
    //     List<int> cheesePlayers = new List<int>();
    //     // set human player
    //     var customProperties = new ExitGames.Client.Photon.Hashtable();
    //     customProperties["HumanPlayer"] = players[humanIndex].ActorNumber;
    //     foreach(int i in playerIndices)
    //     {
    //         cheesePlayers.Add(players[i].ActorNumber);
    //         
    //     }
    //    
    //     int[] cheesePlayersArray = cheesePlayers.ToArray();
    //     customProperties["CheesePlayers"] = cheesePlayersArray;
    //     PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    // }
    
    void AssignRoles()
    {
        var players = PhotonNetwork.PlayerList;
        List<int> playerIndices = new List<int>();
        for (int i = 0; i < players.Length; i++)
        {
            playerIndices.Add(i);
        }

        // 随机选择一个人类玩家
        int humanIndex = UnityEngine.Random.Range(0, playerIndices.Count);
        playerIndices.RemoveAt(humanIndex);

        // 分配Cheese类型
        string[] cheeseTypes = { "Cheese", "Cheese1", "Cheese2" };
        List<string> availableCheeseTypes = new List<string>(cheeseTypes);
    
        var customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["HumanPlayer"] = players[humanIndex].ActorNumber;
    
        // 随机分配Cheese类型给其他玩家
        foreach (int i in playerIndices)
        {
            int cheeseIndex = UnityEngine.Random.Range(0, availableCheeseTypes.Count);
            string cheeseType = availableCheeseTypes[cheeseIndex];
            availableCheeseTypes.RemoveAt(cheeseIndex); // 移除已分配的类型
        
            customProperties["PlayerType_" + players[i].ActorNumber] = cheeseType;
        }
    
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    // void SpawnPlayer(int humanPlayerActorNumber)
    // {
    //     List<Transform> availableSpawnPoints = new List<Transform>();
    //     for (int i = 0; i < pointTf.childCount; i++)
    //     {
    //         availableSpawnPoints.Add(pointTf.GetChild(i));
    //     }
    //
    //     Transform humanSpawnPoint = availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)];
    //     Vector3 humanPos = humanSpawnPoint.position;
    //
    //     availableSpawnPoints.Remove(humanSpawnPoint);
    //
    //     Transform cheeseSpawnPoint = availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)];
    //     Vector3 pos = cheeseSpawnPoint.position;
    //
    //     if (PhotonNetwork.LocalPlayer.ActorNumber == humanPlayerActorNumber)
    //     {
    //         GameObject human = PhotonNetwork.Instantiate("Human", humanPos, Quaternion.identity);
    //         human.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] = "Human";
    //         miniMapController.AddPlayerIcon(human);
    //         CinemachineVirtualCamera vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
    //         vc.Follow = human.transform.Find("PlayerRoot").transform;
    //     }
    //     else
    //     {
    //         GameObject cheese = PhotonNetwork.Instantiate("Cheese", pos, Quaternion.identity);
    //         cheese.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] = "Cheese";
    //         
    //         // GameObject cheese = PhotonNetwork.Instantiate("Cheese1", pos, Quaternion.identity);
    //         // cheese.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] = "Cheese1";
    //         
    //         miniMapController.AddPlayerIcon(cheese);
    //         CinemachineVirtualCamera cheeseVC = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
    //         cheeseVC.Follow = cheese.transform.Find("PlayerRoot").transform;
    //     }
    // }
    
    void SpawnPlayer(int humanPlayerActorNumber)
    {
        List<Transform> availableSpawnPoints = new List<Transform>();
        for (int i = 0; i < pointTf.childCount; i++)
        {
            availableSpawnPoints.Add(pointTf.GetChild(i));
        }
        
        if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PlayerName", out object name))
        {
            playerName = (string)name;
        }
        else
        {
            playerName = "Player" + PhotonNetwork.LocalPlayer.ActorNumber;
        }
        Debug.Log(playerName);
    
        Transform humanSpawnPoint = availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)];
        Vector3 humanPos = humanSpawnPoint.position;
    
        availableSpawnPoints.Remove(humanSpawnPoint);
    
        Transform cheeseSpawnPoint = availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)];
        Vector3 pos = cheeseSpawnPoint.position;
    
        if (PhotonNetwork.LocalPlayer.ActorNumber == humanPlayerActorNumber)
        {
            GameObject human = PhotonNetwork.Instantiate("Human", humanPos, Quaternion.identity);
            human.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] = "Human";
            // miniMapController.AddPlayerIcon(human);
            int humanViewID = human.GetComponent<PhotonView>().ViewID;
            _miniMapPhotonView.RPC("AddPlayerIconRPC", RpcTarget.All, humanViewID);
            CinemachineVirtualCamera vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            vc.Follow = human.transform.Find("PlayerRoot").transform;
            
            // set player name
            PlayerNameDisplay nameDisplay = human.GetComponentInChildren<PlayerNameDisplay>();
            if (nameDisplay != null)
            {
                nameDisplay.photonView.RPC("SetPlayerNameRPC", RpcTarget.AllBuffered, playerName);
            }
        }
        else
        {
            // 根据房间属性中的类型实例化Cheese角色
            string playerType = (string)PhotonNetwork.CurrentRoom.CustomProperties["PlayerType_" + PhotonNetwork.LocalPlayer.ActorNumber];
            GameObject cheese = PhotonNetwork.Instantiate(playerType, pos, Quaternion.identity);
            cheese.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] = playerType;
            
            // GameObject cheese = PhotonNetwork.Instantiate("Cheese1", pos, Quaternion.identity);
            // cheese.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] = "Cheese1";
            
            // miniMapController.AddPlayerIcon(cheese);
            int cheeseViewID = cheese.GetComponent<PhotonView>().ViewID;
            _miniMapPhotonView.RPC("AddPlayerIconRPC", RpcTarget.All, cheeseViewID);
            CinemachineVirtualCamera cheeseVC = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            cheeseVC.Follow = cheese.transform.Find("PlayerRoot").transform;
            PlayerNameDisplay nameDisplay = cheese.GetComponentInChildren<PlayerNameDisplay>();
            if (nameDisplay != null)
            {
                nameDisplay.photonView.RPC("SetPlayerNameRPC", RpcTarget.AllBuffered, playerName);
            }
        }
    }
    

    // void getInstantiate(Vector3 humanPosition, Transform pointTf
    // {
    //     List<KeyValuePair<Transform, float>> spawnPointsAndDistances = new List<KeyValuePair<Transform, float>>();
    //     foreach (Transform spawnPoint in pointTf) {
    //         float distance = Vector3.Distance(spawnPoint.position, humanPosition);
    //         spawnPointsAndDistances.Add(new KeyValuePair<Transform, float>(spawnPoint, distance));
    //     }
    //
    //     spawnPointsAndDistances.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
    //
    //     Vector3 cheesePosition = spawnPointsAndDistances[0].Key.position;
    //
    // }

    // Update is called once per frame
    void Update()
    {
        if (!_gameOver)
        {
            // 检查游戏结果
            CheckGameResult();
            // 检查当前客户端是否是房主
            if (PhotonNetwork.IsMasterClient)
            {
                // 如果是房主，更新倒计时并发送 RPC
                float newTimer = UpdateCountdownTimer();
                // fightUI.SetCountdownTimer(newTimer);
                photonView.RPC("UpdateCountdownTimerRPC", RpcTarget.All, newTimer);
            }
        }
        else
        {
            photonView.RPC("EndGame", RpcTarget.All, _isHumanWin);
        }
    }
    private float UpdateCountdownTimer()
    {

        countdownTimer -= Time.deltaTime;
        return countdownTimer;
    }

    public void CheeseDied()
    {
        _remainingCheeseCount--;

        // 检查是否所有 cheese 都死亡了
        if (_remainingCheeseCount <= 0)
        {
            _isHumanWin = true;
            _gameOver = true;
            _allCheeseDie = true;
           
            Debug.Log("human win!");
            
        }
    }

    // [PunRPC]
    // private void UpdateCountdownTimerRPC(float newTimer)
    // {
    //     // 在所有客户端上同步倒计时
    //     fightUI.SetCountdownTimer(newTimer);
    // }

    [PunRPC]
    private void UpdateCountdownTimerRPC(float newTimer)
    {
        
        if (fightUI != null)
        {
            fightUI.SetCountdownTimer(newTimer);
        }
        else
        {
            Debug.Log("fightUI is null when trying to set timer.");
        }

        if (fightUI1 != null)
        {
            fightUI1.SetCountdownTimer(newTimer);
        }
        else
        {
            Debug.Log("fightUI1 is null when trying to set timer.");
        }
        if (fightUI2 != null)
        {
            fightUI2.SetCountdownTimer(newTimer);
        }
        else
        {
            Debug.Log("fightUI2 is null when trying to set timer.");
        }
        if (fightUI3 != null)
        {
            fightUI3.SetCountdownTimer(newTimer);
        }
        else
        {
            Debug.Log("fightUI3 is null when trying to set timer.");
        }
    }

    [PunRPC]
    void EndGame(bool isHumanWin)
    {

        Game.uiManager.CloseUI("DieUI");
        
        if (isHumanWin)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == humanPlayerActorNumber)
            {
                Game.uiManager.ShowUI<WinUI>("WinUI");
                //Debug.Log("showHumanWinUI");
            }
            else
            {
                Game.uiManager.ShowUI<LossUI>("LossUI");
                //Debug.Log("showHumanLossUI");
            }
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != humanPlayerActorNumber)
            {
                
                Game.uiManager.ShowUI<WinUI>("WinUI");
                //Debug.Log("showCheeseWinUI");
            }
            else
            {
                Game.uiManager.ShowUI<LossUI>("LossUI");
                //Debug.Log("showCheeseLossUI");
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

 

    void CheckGameResult()
    {
       
        // 如果倒计时结束
        if (countdownTimer <= 0f)
        {
            _gameOver = true; // 设置游戏结束标志为 true
            _isHumanWin = false;
            Debug.Log("gameover: " + _gameOver);
        }
        
        
    }
    
   

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

       
        if (propertiesThatChanged.ContainsKey("HumanPlayer"))
        {
            humanPlayerActorNumber = (int)propertiesThatChanged["HumanPlayer"];
            SpawnPlayer(humanPlayerActorNumber);
            DisplayUIBasedOnRole();
        }
        
    }
}
