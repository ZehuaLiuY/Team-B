using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;


public class FightManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool gameOver = false; // 游戏是否已经结束
    // private float captureDistance = 2f; // 抓住奶酪的距离阈值

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

        Transform pointTf = GameObject.Find("Point").transform;
        // Vector3 pos = pointTf.GetChild(UnityEngine.Random.Range(0, pointTf.childCount)).position;
        // GameObject cheese1 = PhotonNetwork.Instantiate("Cheese", pos, Quaternion.identity);
        //
        // CinemachineFreeLook vc = GameObject.Find("FreeLook Camera").GetComponent<CinemachineFreeLook>();
        // vc.Follow = cheese1.transform;
        // vc.LookAt = cheese1.transform.Find("eye").transform;
        // GameObject human1 = PhotonNetwork.Instantiate("Human", pos, Quaternion.identity);
        //
        // CinemachineVirtualCamera vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        // vc.Follow = human1.transform.Find("PlayerRoot").transform;


        int humanIndex = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length);
        
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Vector3 pos = pointTf.GetChild(UnityEngine.Random.Range(0, pointTf.childCount)).position;
        
            if (i == humanIndex)
            {
                // 为选定的玩家实例化Human
                GameObject human = PhotonNetwork.Instantiate("Human", pos, Quaternion.identity);
                // 设置相机跟随
                CinemachineVirtualCamera vc = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
                vc.Follow = human.transform.Find("PlayerRoot").transform;
            }
            else
            {
                // 为其他玩家实例化Cheese
                // Old Version
                // GameObject cheese = PhotonNetwork.Instantiate("Cheese", pos, Quaternion.identity);
                // CinemachineFreeLook vc = GameObject.Find("FreeLook Camera").GetComponent<CinemachineFreeLook>();
                // vc.Follow = cheese.transform;
                // vc.LookAt = cheese.transform.Find("eye").transform;
                
                // New Version
                GameObject cheese = PhotonNetwork.Instantiate("Cheese", pos, Quaternion.identity);
                CinemachineFreeLook vc = GameObject.Find("Cheese VC").GetComponent<CinemachineFreeLook>();
                vc.Follow = cheese.transform.Find("PlayerRoot").transform;
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
