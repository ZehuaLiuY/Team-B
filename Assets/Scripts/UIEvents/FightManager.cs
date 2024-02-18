using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using UnityEditor.Rendering;
using Photon.Pun.UtilityScripts;
using System.Runtime.CompilerServices;


public class FightManager : MonoBehaviourPunCallbacks
{
    private bool gameOver = false; // 游戏是否已经结束
    // private float captureDistance = 2f; // 抓住奶酪的距离阈值

    public Transform pointTf; // respawn points
    private PhotonView _photonView;

    private FightUI fightUI;
    public static float countdownTimer = 180f;

    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        Game.uiManager.CloseAllUI();
        fightUI = Game.uiManager.ShowUI<FightUI>("FightUI");
        
        if (PhotonNetwork.IsMasterClient)
        {
            AssignRoles();
        }

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

        // set human player
        var customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["HumanPlayer"] = players[humanIndex].ActorNumber;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    void SpawnPlayer(int humanPlayerActorNumber)
    {
        Transform spawnPoint = pointTf.GetChild(UnityEngine.Random.Range(0, pointTf.childCount));
        Vector3 pos = spawnPoint.position;

        if (PhotonNetwork.LocalPlayer.ActorNumber == humanPlayerActorNumber)
        {
            GameObject human = PhotonNetwork.Instantiate("Human", pos, Quaternion.identity);
            CinemachineVirtualCamera vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            vc.Follow = human.transform.Find("PlayerRoot").transform;
        }
        else
        {
            GameObject cheese = PhotonNetwork.Instantiate("Cheese", pos, Quaternion.identity);
            CinemachineVirtualCamera cheeseVC = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            cheeseVC.Follow = cheese.transform.Find("PlayerRoot").transform;
        }
    }

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

    void CheckGameResult()
    {

        // 如果倒计时结束
        if (countdownTimer <= 0f)
        {
            gameOver = true; // 设置游戏结束标志为 true
            Debug.Log("gameover: " + gameOver);
            //CheckGameResult(); // 检查游戏结果
            Game.uiManager.ShowUI<WinUI>("WinUI");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 获取场景中所有带有 "Player" 标签的对象
        //GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        //PlayerControl human = GetComponent<PlayerControl>();

        //// 遍历所有的玩家对象
        //foreach (GameObject playerObject in playerObjects)
        //{
        //    // 如果对象不是人类
        //    if (playerObject != human.gameObject)
        //    {
        //        // 计算人类和奶酪的距离
        //        float distance = Vector3.Distance(human.gameObject.transform.position, playerObject.transform.position);
        //        // 如果距离小于抓住距离阈值
        //        if (distance < captureDistance)
        //        {
        //            // 游戏结束，人类胜利
        //            gameOver = true;
        //            Debug.Log("Human Win!");
        //            // 设置奶酪的画面为 "You Die"
        //            //playerObject.GetComponentInChildren<TextMesh>().text = "You Die";
        //        }
        //    }
        //}
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("HumanPlayer"))
        {
            int humanPlayerActorNumber = (int)propertiesThatChanged["HumanPlayer"];
            SpawnPlayer(humanPlayerActorNumber);
        }
    }
}
