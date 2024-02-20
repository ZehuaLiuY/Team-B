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
using static UnityEngine.Rendering.DebugUI;


public class FightManager : MonoBehaviourPunCallbacks
{
    private bool gameOver = false; // 游戏是否已经结束
    // private float captureDistance = 2f; // 抓住奶酪的距离阈值

    public Transform pointTf; // respawn points
    private PhotonView _photonView;

    private FightUI fightUI;
    public static float countdownTimer = 100f;
    private bool isHumanWin;
    private int humanPlayerActorNumber;
    //private List<int> _cheesePlayersActorNumber = new List<int>();
    //private int _cheeseIndex = 0;
    GameObject human;
    //Dictionary<GameObject, int> cheeses = new Dictionary<GameObject, int>();

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
            human = PhotonNetwork.Instantiate("Human", humanPos, Quaternion.identity);
            CinemachineVirtualCamera vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            vc.Follow = human.transform.Find("PlayerRoot").transform;
            
        }
        else
        {
            GameObject cheese = PhotonNetwork.Instantiate("Cheese", pos, Quaternion.identity);
            CinemachineVirtualCamera cheeseVC = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            cheeseVC.Follow = cheese.transform.Find("PlayerRoot").transform;
            //Debug.Log(cheese.name);
            //Debug.Log("PhotonNetwork.LocalPlayer.ActorNumber:" + PhotonNetwork.LocalPlayer.ActorNumber);
            //cheeses.Add(cheese, _cheesePlayersActorNumber[_cheeseIndex]);
            //_cheeseIndex++;

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
        if (!gameOver)
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
            photonView.RPC("EndGame", RpcTarget.All, isHumanWin);
        }
    }
    private float UpdateCountdownTimer()
    {

        countdownTimer -= Time.deltaTime;
        return countdownTimer;
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

        // 获取当前客户端的玩家对象
        Player localPlayer = PhotonNetwork.LocalPlayer;

        if (localPlayer.ActorNumber == humanPlayerActorNumber)
        {
            if (isHumanWin)
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
            if (isHumanWin)
            {
                Game.uiManager.ShowUI<LossUI>("LossUI");
            }
            else
            {
                Game.uiManager.ShowUI<WinUI>("WinUI");
            }
        }

       
    }

 

    void CheckGameResult()
    {

        // 如果倒计时结束
        if (countdownTimer <= 0f)
        {
            gameOver = true; // 设置游戏结束标志为 true
            isHumanWin = false;
            Debug.Log("gameover: " + gameOver);

        }
        //else
        //{
        //    // 获取当前客户端的玩家对象
        //    Player localPlayer = PhotonNetwork.LocalPlayer;

        //    if (localPlayer.ActorNumber == humanPlayerActorNumber)
        //    {
        //        Vector3 humanPosition = human.transform.position;
        //        if (cheeses != null)
        //        {
        //            Debug.Log("cheeses is not null");
        //            Debug.Log("cheese number: " + cheeses.Count);
        //            foreach (GameObject cheese in cheeses.Keys)
        //            {
        //                float distance = Vector3.Distance(humanPosition, cheese.transform.position);
        //                Debug.Log("distance: " + distance);

        //                // 如果距离小于2f且按下了R键
        //                if (distance <= 20f)
        //                {
        //                    Debug.Log("capture");
        //                    // 使用 RPC 通知奶酪对象显示 "DieUI"
        //                    cheese.GetComponent<PhotonView>().RPC("ShowDieUI", RpcTarget.All, cheeses[cheese]);

        //                }
        //            }
        //        }
        //    }
        //}

        


    }

    //[PunRPC]
    //void ShowDieUI(int cheeseNumber)
    //{
    //    // 获取当前客户端的玩家对象
    //    Player localPlayer = PhotonNetwork.LocalPlayer;

    //    if (localPlayer.ActorNumber == cheeseNumber)
    //    {
    //        Game.uiManager.ShowUI<DieUI>("DieUI");
    //    }
    //}

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

       
        if (propertiesThatChanged.ContainsKey("HumanPlayer"))
        {
            humanPlayerActorNumber = (int)propertiesThatChanged["HumanPlayer"];
            SpawnPlayer(humanPlayerActorNumber);
        }
        //if (propertiesThatChanged.ContainsKey("CheesePlayers"))
        //{

        //    int[] cheesePlayers = (int[])propertiesThatChanged["CheesePlayers"];
        //    foreach (int cheeseNumber in cheesePlayers)
        //    {
        //        _cheesePlayersActorNumber.Add(cheeseNumber);
        //    }

        //}
    }
}
