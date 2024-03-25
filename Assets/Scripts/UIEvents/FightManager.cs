using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using Photon.Voice.Unity;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;


public class FightManager : MonoBehaviourPunCallbacks
{
    private bool _gameOver = false; // 游戏是否已经结束
    // private float captureDistance = 2f; // 抓住奶酪的距离阈值

    // points
    public Transform cheeseSpawnPoints; // respawn points
    public GameObject skillBallSpawner;
    public Transform humanSpawnPoints;

    private HumanFightUI fightUI;
    private CheeseFightUI fightUI1;
    public static float countdownTimer = 180f;
    private bool _isHumanWin = false;
    private List<int> _humanPlayerActorNumbers = new List<int>();
    private int _remainingCheeseCount; // 剩余活着的 cheese 数量


    public MiniMapController miniMapController;

    public string playerName;

    private PhotonView _miniMapPhotonView;


    void Awake()
    {
        _miniMapPhotonView = miniMapController.GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            AssignRoles();
        }
    }
    
    void Start()
    {
        Game.uiManager.CloseAllUI();
        _remainingCheeseCount = PhotonNetwork.CurrentRoom.PlayerCount - 1; // TODO: change this to the actual number of human players
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
            default:
                Debug.LogError("Unknown player type: " + playerType);
                break;
        }
    }
    
    void AssignRoles()
    {
        var players = PhotonNetwork.PlayerList;
        List<int> humanIndices = new List<int>();

        int numberOfHumans = Math.Max(1, players.Length / 4);

        for (int i = 0; i < numberOfHumans; i++)
        {
            int humanIndex = Random.Range(0, players.Length);
            while (humanIndices.Contains(humanIndex))
            {
                humanIndex = Random.Range(0, players.Length);
            }
            humanIndices.Add(humanIndex);
        }

        foreach (int index in humanIndices)
        {
            _humanPlayerActorNumbers.Add(players[index].ActorNumber);
        }

        for (int i = 0; i < players.Length; i++)
        {
            Hashtable props = new Hashtable();
            if (humanIndices.Contains(i))
            {
                props.Add("PlayerType", "Human");
            }
            else
            {
                props.Add("PlayerType", "Cheese");
            }
            players[i].SetCustomProperties(props);
        }

        Hashtable roomProps = new Hashtable();
        roomProps.Add("HumanPlayerActorNumbers", _humanPlayerActorNumbers.ToArray());
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    }


    void SpawnPlayers()
    {
        // get the player name
        if(!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PlayerName", out object name))
        {
            playerName = "Player" + PhotonNetwork.LocalPlayer.ActorNumber;
        }
        else
        {
            playerName = (string)name;
        }

        // ---- init the player ---- //
        GameObject playerObject;
        Transform spawnPoint;
        Vector3 spawnPos;
        string prefabName;
        byte interestGroup;

        // check the player type
        if (_humanPlayerActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
        {
            // human available points
            List<Transform> humanAvailablePoints = new List<Transform>();
            for (int i = 0; i < humanSpawnPoints.childCount; i++)
            {
                humanAvailablePoints.Add(humanSpawnPoints.GetChild(i));
            }

            spawnPoint = humanAvailablePoints[Random.Range(0, humanAvailablePoints.Count)];
            spawnPos = spawnPoint.position;
            prefabName = "Human";
            interestGroup = 1;
        }
        else
        {
            // cheese available points
            List<Transform> cheeseAvailablePoints = new List<Transform>();
            for (int i = 0; i < cheeseSpawnPoints.childCount; i++)
            {
                cheeseAvailablePoints.Add(cheeseSpawnPoints.GetChild(i));
            }

            spawnPoint = cheeseAvailablePoints[Random.Range(0, cheeseAvailablePoints.Count)];
            spawnPos = spawnPoint.position;
            prefabName = "Cheese";
            interestGroup = 2;
        }

        // spawn the player
        playerObject = PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity);

        // minimap icon display
        _miniMapPhotonView.RPC("AddPlayerIconRPC", RpcTarget.All, playerObject.GetComponent<PhotonView>().ViewID);

        // camera follow
        CinemachineVirtualCamera vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        vc.Follow = playerObject.transform.Find("PlayerRoot").transform;

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
    }
    
    private void UpdateSkillBallSpawnerActivity()
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.LocalPlayer.CustomProperties == null)
        {
            return;
        }

        object playerType;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PlayerType", out playerType))
        {
            skillBallSpawner.SetActive(playerType.ToString() == "Cheese");
        }
    }
    

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
            if (_humanPlayerActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
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
            if (_humanPlayerActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
            {
                Game.uiManager.ShowUI<LossUI>("LossUI");
                //Debug.Log("showCheeseWinUI");
            }
            else
            {
                Game.uiManager.ShowUI<WinUI>("WinUI");
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
        if (propertiesThatChanged.ContainsKey("HumanPlayerActorNumbers"))
        {
            var actorNumbers = propertiesThatChanged["HumanPlayerActorNumbers"] as int[];
            if (actorNumbers != null)
            {
                _humanPlayerActorNumbers.Clear(); 
                _humanPlayerActorNumbers.AddRange(actorNumbers);
                SpawnPlayers();
                DisplayUIBasedOnRole();
                UpdateSkillBallSpawnerActivity();
            }
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
