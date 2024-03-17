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
using Photon.Voice.Unity;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using static UnityEngine.Rendering.DebugUI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class FightManager : MonoBehaviourPunCallbacks
{
    private bool _gameOver = false; // 游戏是否已经结束
    // private float captureDistance = 2f; // 抓住奶酪的距离阈值

    public Transform pointTf; // respawn points
    public Transform skillPointTf;
    private PhotonView _photonView;

    private HumanFightUI fightUI;
    private CheeseFightUI fightUI1;
    public static float countdownTimer = 180f;
    private bool _isHumanWin = false;
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
    
    void Start()
    {
        Game.uiManager.CloseAllUI();
        _remainingCheeseCount = PhotonNetwork.CurrentRoom.PlayerCount - 1; // 减去1是因为其中一个玩家是人类玩家

        generateSkillBall();
    }

    void DisplayUIBasedOnRole()
    {
        string playerType = (string)PhotonNetwork.LocalPlayer.CustomProperties["PlayerType"];
        
        switch (playerType)
        {
            case "Human":
                fightUI = Game.uiManager.ShowUI<HumanFightUI>("HumanFightUI");
                break;
            case "Cheese":
                fightUI1 = Game.uiManager.ShowUI<CheeseFightUI>("CheeseFightUI");
                fightUI1.InitializeUI(playerType);
                break;
            case "Cheese1":
                fightUI1 = Game.uiManager.ShowUI<CheeseFightUI>("CheeseFightUI");
                fightUI1.InitializeUI(playerType);
                break;
            case "Cheese2":
                fightUI1 = Game.uiManager.ShowUI<CheeseFightUI>("CheeseFightUI");
                fightUI1.InitializeUI(playerType);
                break;
            default:
                Debug.LogError("Unknown player type: " + playerType);
                break;
        }
    }
    
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
        var humanPlayer = players[humanIndex];

        Hashtable roomProps = new Hashtable();
        roomProps.Add("HumanPlayer", humanPlayer.ActorNumber);

        playerIndices.RemoveAt(humanIndex);

        // set the custom properties for human player
        Hashtable humanProps = new Hashtable { { "PlayerType", "Human" } };
        humanPlayer.SetCustomProperties(humanProps);

        // cheese players
        foreach (int index in playerIndices)
        {
            var player = players[index];
            Hashtable cheeseProps = new Hashtable { { "PlayerType", "Cheese" } };
            player.SetCustomProperties(cheeseProps);
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    }

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


        Transform humanSpawnPoint = availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)];
        Vector3 humanPos = humanSpawnPoint.position;
    
        availableSpawnPoints.Remove(humanSpawnPoint);
    
        Transform cheeseSpawnPoint = availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)];
        Vector3 pos = cheeseSpawnPoint.position;
    
        if (PhotonNetwork.LocalPlayer.ActorNumber == humanPlayerActorNumber)
        {
            GameObject human = PhotonNetwork.Instantiate("Human", humanPos, Quaternion.identity);

            // set the minimap icon for human
            int humanViewID = human.GetComponent<PhotonView>().ViewID;
            _miniMapPhotonView.RPC("AddPlayerIconRPC", RpcTarget.All, humanViewID);

            //camera follow the human
            CinemachineVirtualCamera vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            vc.Follow = human.transform.Find("PlayerRoot").transform;
            
            // set player name
            PlayerNameDisplay nameDisplay = human.GetComponentInChildren<PlayerNameDisplay>();
            if (nameDisplay != null)
            {
                nameDisplay.photonView.RPC("SetPlayerNameRPC", RpcTarget.AllBuffered, playerName);
            }

            //set voice channel interest group
            Recorder recorder = human.GetComponent<Recorder>();
            if (recorder != null)
            {
                recorder.InterestGroup = 1;
            }
        }
        else
        {

            GameObject cheese = PhotonNetwork.Instantiate("Cheese", pos, Quaternion.identity);

            // set the minimap icon for cheese
            int cheeseViewID = cheese.GetComponent<PhotonView>().ViewID;
            _miniMapPhotonView.RPC("AddPlayerIconRPC", RpcTarget.All, cheeseViewID);

            //camera follow the cheese
            CinemachineVirtualCamera cheeseVC = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            cheeseVC.Follow = cheese.transform.Find("PlayerRoot").transform;

            // set player name
            PlayerNameDisplay nameDisplay = cheese.GetComponentInChildren<PlayerNameDisplay>();
            if (nameDisplay != null)
            {
                nameDisplay.photonView.RPC("SetPlayerNameRPC", RpcTarget.AllBuffered, playerName);
            }

            // set voice channel interest group
            Recorder recorder = cheese.GetComponent<Recorder>();
            if (recorder != null)
            {
                recorder.InterestGroup = 2;
            }
        }
    }
    
    void generateSkillBall()
    {
        List<Transform> availableSpawnSkillPoints = new List<Transform>();
        for (int i = 0; i < skillPointTf.childCount; i++)
        {
            availableSpawnSkillPoints.Add(skillPointTf.GetChild(i));
        }
        for(int i = 0; i < skillPointTf.childCount; i++)
        {
            Transform skillSpawnPoint = availableSpawnSkillPoints[UnityEngine.Random.Range(0, availableSpawnSkillPoints.Count)];
            PhotonNetwork.Instantiate("Sphere", skillSpawnPoint.position, Quaternion.identity);
            availableSpawnSkillPoints.Remove(skillSpawnPoint);
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
                photonView.RPC("UpdateCountdownTimerRPC", RpcTarget.AllBuffered, newTimer);
            }
        }
        else if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("EndGame", RpcTarget.All, _isHumanWin);
            _gameOver = false;
        }
    }
    private float UpdateCountdownTimer()
    {
        if(countdownTimer > 0)
        {
            countdownTimer -= Time.deltaTime;
        }
        
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

            // when all cheese die, the obeserved also show the lose UI
            Game.uiManager.CloseAllUI();
            ShowEndGameUI(false);

        }
    }

    private void ShowEndGameUI(bool isHumanWin)
    {
        if (isHumanWin)
        {
            Game.uiManager.ShowUI<LossUI>("LossUI");
        }
        else
        {
            Game.uiManager.ShowUI<WinUI>("WinUI");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    [PunRPC]
    private void UpdateCountdownTimerRPC(float newTimer)
    {
        
        if (fightUI != null)
        {
            fightUI.SetCountdownTimer(newTimer);
        }

        if (fightUI1 != null)
        {
            fightUI1.SetCountdownTimer(newTimer);
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
            // Debug.Log("gameover: " + _gameOver);
        }
        
        
    }
    
   

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("HumanPlayer"))
        {
            humanPlayerActorNumber = (int)propertiesThatChanged["HumanPlayer"];
            SpawnPlayer(humanPlayerActorNumber);
            DisplayUIBasedOnRole();
        }
    }

    public void QuitToLoginScene()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        SceneManager.LoadScene("login");
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<LoginUI>("LoginUI");
    }
}
