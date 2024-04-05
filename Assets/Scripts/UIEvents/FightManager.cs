using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // spawn points
    public Transform cheeseSpawnPoints; // respawn points
    public Transform humanSpawnPoints;

    private List<Transform> _cheeseAvailablePoints;
    private List<Transform> _humanAvailablePoints;
    
    // skill balls
    public GameObject[] skillBallPrefabs; // 球体预制体数组
    public Transform skillPointTf;        // 技能球生成点
    public float refreshInterval = 60f;   // 刷新间隔

    // UI
    private HumanFightUI fightUI;
    private CheeseFightUI fightUI1;
    public static float countdownTimer = 180f;
    private bool _isHumanWin = false;

    private HashSet<int> _humanPlayerActorNumbers = new HashSet<int>();
    private int _remainingCheeseCount; // 剩余活着的 cheese 数量

    private GameObject skillBall;
    private HashSet<GameObject> skillBalls = new HashSet<GameObject>();


    public MiniMapController miniMapController;
    private PhotonView _miniMapPhotonView;

    public string playerName;


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
        _remainingCheeseCount = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        StartCoroutine(SpawnSkillBallsPeriodically());

        _humanAvailablePoints = new List<Transform>();
        for (int i = 0; i < humanSpawnPoints.childCount; i++)
        {
            _humanAvailablePoints.Add(humanSpawnPoints.GetChild(i));
        }

        _cheeseAvailablePoints = new List<Transform>();
        for (int i = 0; i < cheeseSpawnPoints.childCount; i++)
        {
            _cheeseAvailablePoints.Add(cheeseSpawnPoints.GetChild(i));
        }

    }

    IEnumerator SpawnSkillBallsPeriodically()
    {
        while (true)
        {
            GenerateSkillBalls();
            yield return new WaitForSeconds(refreshInterval);
        }
    }

    void GenerateSkillBalls()
    {
        photonView.RPC("UpdateSkillBallsVisibilityRPC", RpcTarget.All);
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
        photonView.RPC("UpdateSkillBallsVisibilityRPC", RpcTarget.All);
    }

    void DeleteExistingSkillBalls()
    {
        foreach (var skillBall in skillBalls)
        {
            if (skillBall != null)
            {
                PhotonNetwork.Destroy(skillBall);
            }
        }
        skillBalls.Clear();
    }


    [PunRPC]
    void UpdateSkillBallsVisibilityRPC()
    {
        UpdateSkillBallsVisibility();
    }

    void UpdateSkillBallsVisibility()
    {
        string[] skillTags = { "Jump Skill", "Sprint Skill", "Invisible Skill", "Detector Skill","Clone Skill" };
        bool shouldShow = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PlayerType", out object playerType) && playerType.ToString() == "Cheese";

        foreach (string tag in skillTags)
        {
            GameObject[] skillBalls = GameObject.FindGameObjectsWithTag(tag);
            foreach (var skillBall in skillBalls)
            {
                skillBall.SetActive(shouldShow);
            }
        }
    }

    public override void OnJoinedRoom()
    {
        UpdateSkillBallsVisibility();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateSkillBallsVisibility();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateSkillBallsVisibility();
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
        HashSet<int> humanIndices = new HashSet<int>();

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
            spawnPoint = _humanAvailablePoints[Random.Range(0, _humanAvailablePoints.Count)];
            spawnPos = spawnPoint.position;
            prefabName = "Human";
            interestGroup = 1;
            Destroy(spawnPoint.gameObject);
        }
        else
        {
            spawnPoint = _cheeseAvailablePoints[Random.Range(0, _cheeseAvailablePoints.Count)];
            spawnPos = spawnPoint.position;
            prefabName = "Cheese";
            interestGroup = 2;
            Destroy(spawnPoint.gameObject);
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
                _humanPlayerActorNumbers.UnionWith(actorNumbers);
                SpawnPlayers();
                DisplayUIBasedOnRole();
            }
        }
    }


    public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("PlayerType"))
        {
            UpdateSkillBallsVisibility();
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
