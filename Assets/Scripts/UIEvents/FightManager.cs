using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class FightManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool gameOver = false; // 游戏是否已经结束
    private float captureDistance = 2f; // 抓住奶酪的距离阈值

    public Transform pointTf; // respawn points
    private PhotonView _photonView;

    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<FightUI>("FightUI");

        #if UNITY_EDITOR
        Transform pointTf = GameObject.Find("Point").transform;

        Vector3 pos = pointTf.GetChild(UnityEngine.Random.Range(0, pointTf.childCount)).position;

        PhotonNetwork.Instantiate("Human", pos, Quaternion.identity);
        #else

        if (PhotonNetwork.IsMasterClient)
        {
            // not sure if check the player in the room is necessary
            AssignCharactersAndSpawnPoints();
        };

        #endif
    }

    [PunRPC]
    void SpawnCharacter(string characterType, Vector3 position, int actorNumber)
    {
        // if the local player is the one to spawn the character
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            PhotonNetwork.Instantiate(characterType, position, Quaternion.identity);
        }
    }

    void AssignCharactersAndSpawnPoints()
    {

        List<Transform> availablePoints = new List<Transform>();
        for (int i = 0; i < pointTf.childCount; i++)
        {
            availablePoints.Add(pointTf.GetChild(i));
        }

        // Vector3 pos = pointTf.GetChild(UnityEngine.Random.Range(0, pointTf.childCount)).position;

        // PhotonNetwork.Instantiate("Player", pos, Quaternion.identity);
        // TODO: optimise the logic to assign characters and spawn points, also for the RPC calls

        // random select one Human player
        int humanIndex = Random.Range(0, PhotonNetwork.PlayerList.Length);
        Player humanPlayer = PhotonNetwork.PlayerList[humanIndex];

        int humanSpawnIndex = Random.Range(0, availablePoints.Count);
        Vector3 humanPos = availablePoints[humanSpawnIndex].position;
        availablePoints.RemoveAt(humanSpawnIndex); // remove the used spawn point

        // notify all clients to spawn the Human player
        _photonView.RPC("SpawnCharacter", RpcTarget.All, "Human", humanPos, humanPlayer.ActorNumber);

        // spawn the Cheese players
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player != humanPlayer) // if the player is not the human player
            {
                int cheeseSpawnIndex = Random.Range(0, availablePoints.Count);
                Vector3 cheesePos = availablePoints[cheeseSpawnIndex].position;
                availablePoints.RemoveAt(cheeseSpawnIndex); // remove the used spawn point

                // notify all clients to spawn the Cheese player
                _photonView.RPC("SpawnCharacter", RpcTarget.All, "Cheese", cheesePos, player.ActorNumber);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            // 检查游戏结果
            CheckGameResult();

        }
    }

    void CheckGameResult()
    {

        // 如果倒计时结束
        if (FightUI.countdownTimer <= 0f)
        {
            gameOver = true; // 设置游戏结束标志为 true
            Debug.Log("gameover: " + gameOver);
            //CheckGameResult(); // 检查游戏结果
            Game.uiManager.ShowUI<WinUI>("WinUI");
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
}
