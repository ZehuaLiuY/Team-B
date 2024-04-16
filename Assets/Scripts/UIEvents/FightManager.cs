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
    // private bool _gameOver = false;

    // Instantiate the player
    // spawn points
    public Transform cheeseSpawnPoints; // respawn points
    public Transform humanSpawnPoints;

    private List<Transform> _cheeseAvailablePoints;
    private List<Transform> _humanAvailablePoints;
    private CinemachineVirtualCamera _vc;
    public string playerName;

    private HashSet<int> _humanPlayerActorNumbers = new HashSet<int>();
    
    // skill balls
    public GameObject[] skillBallPrefabs;
    public Transform skillPointTf;
    public float refreshInterval = 60f;
    private GameObject skillBall;
    private HashSet<GameObject> skillBalls = new HashSet<GameObject>();
    // private Dictionary<int, Queue<GameObject>> skillBallPools = new Dictionary<int, Queue<GameObject>>();

    // UI
    private HumanFightUI fightUI;
    private CheeseFightUI fightUI1;

    // countdown timer and game end event
    public static float countdownTimer = 15f;
    public event Action<bool> OnGameEnd;
    private bool _isHumanWin = false;
    private int _remainingCheeseCount;

    // minimap
    public MiniMapController miniMapController;
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
        _remainingCheeseCount = PhotonNetwork.CurrentRoom.PlayerCount - 1;

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CountdownTimerCoroutine());
            StartCoroutine(SpawnSkillBallsPeriodically());
        }

        OnGameEnd += HandleGameEnd;

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

        _vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();

    }

    void OnDestroy()
    {
        OnGameEnd -= HandleGameEnd;
    }

    IEnumerator SpawnSkillBallsPeriodically()
    {
        while (true)
        {
            GenerateSkillBalls();
            yield return new WaitForSeconds(refreshInterval);
        }
    }

    IEnumerator CountdownTimerCoroutine()
    {
        while (countdownTimer > 0)
        {
            yield return new WaitForSeconds(1f);

            countdownTimer -= 1f;
            photonView.RPC("UpdateCountdownTimerRPC", RpcTarget.All, countdownTimer);

            if (countdownTimer <= 0)
            {
                // _gameOver = true;
                _isHumanWin = false;
                OnGameEnd?.Invoke(_isHumanWin);
                yield break;
            }
        }
    }

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
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void CheeseDied()
    {
        _remainingCheeseCount--;

        if (_remainingCheeseCount <= 0)
        {
            _isHumanWin = true;
            // _gameOver = true;

            // when all cheese die, the obeserved also show the lose UI
            Game.uiManager.CloseAllUI();
            // ShowEndGameUI(false);
            OnGameEnd?.Invoke(_isHumanWin);
        }
    }

    private void HandleGameEnd(bool isHumanWin)
    {
        photonView.RPC("EndGame", RpcTarget.All, isHumanWin);
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
