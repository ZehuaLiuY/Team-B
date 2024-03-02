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
    public static float countdownTimer = 180f;
    private bool _isHumanWin;
    private int humanPlayerActorNumber;
    private int _remainingCheeseCount; // 剩余活着的 cheese 数量
    private bool _allCheeseDie = false;

    public MiniMapController miniMapController;

    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            AssignRoles();
        }
    }

    void Start()
    {
        Game.uiManager.CloseAllUI();
        fightUI = Game.uiManager.ShowUI<FightUI>("FightUI");

        Game.uiManager.ShowUI<FightUI>("FightUI");
        _remainingCheeseCount = PhotonNetwork.CurrentRoom.PlayerCount - 1; // 减去1是因为其中一个玩家是人类玩家
    }

    void AssignRoles()
    {
        var players = PhotonNetwork.PlayerList;
        List<int> playerIndices = new List<int>();
        for (int i = 0; i < players.Length; i++)
        {
            playerIndices.Add(i);
        }

        // random select a human player
        int humanIndex = playerIndices[Random.Range(0, playerIndices.Count)];
        playerIndices.RemoveAt(humanIndex);

        List<int> cheesePlayers = new List<int>();
        // set human player
        var customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["HumanPlayer"] = players[humanIndex].ActorNumber;
        foreach(int i in playerIndices)
        {
            cheesePlayers.Add(players[i].ActorNumber);
            
        }
       
        int[] cheesePlayersArray = cheesePlayers.ToArray();
        customProperties["CheesePlayers"] = cheesePlayersArray;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);

    }

    void SpawnPlayer(int humanPlayerActorNumber)
    {
        List<Transform> availableSpawnPoints = new List<Transform>();
        for (int i = 0; i < pointTf.childCount; i++)
        {
            availableSpawnPoints.Add(pointTf.GetChild(i));
        }

        Transform humanSpawnPoint = availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)];
        Vector3 humanPos = humanSpawnPoint.position;

        availableSpawnPoints.Remove(humanSpawnPoint);

        Transform cheeseSpawnPoint = availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)];
        Vector3 pos = cheeseSpawnPoint.position;

        if (PhotonNetwork.LocalPlayer.ActorNumber == humanPlayerActorNumber)
        {
            GameObject human = PhotonNetwork.Instantiate("Human", humanPos, Quaternion.identity);
            human.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] = "Human";
            miniMapController.AddPlayerIcon(human);
            CinemachineVirtualCamera vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            vc.Follow = human.transform.Find("PlayerRoot").transform;
        }
        else
        {
            // GameObject cheese = PhotonNetwork.Instantiate("Cheese", pos, Quaternion.identity);
            // cheese.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] = "Cheese";
            
            GameObject cheese = PhotonNetwork.Instantiate("Cheese1", pos, Quaternion.identity);
            cheese.GetComponent<PhotonView>().Owner.CustomProperties["PlayerType"] = "Cheese1";
            
            miniMapController.AddPlayerIcon(cheese);
            CinemachineVirtualCamera cheeseVC = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            cheeseVC.Follow = cheese.transform.Find("PlayerRoot").transform;
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
                fightUI.SetCountdownTimer(newTimer);
                photonView.RPC("UpdateCountdownTimerRPC", RpcTarget.Others, newTimer);
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

    [PunRPC]
    private void UpdateCountdownTimerRPC(float newTimer)
    {
        // 在所有客户端上同步倒计时
        fightUI.SetCountdownTimer(newTimer);
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
        }
        
    }
}
