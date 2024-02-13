using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class FightManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool gameOver = false; // 游戏是否已经结束
    private float captureDistance = 2f; // 抓住奶酪的距离阈值
    void Start()
    {
        Game.uiManager.CloseAllUI();
        Game.uiManager.ShowUI<FightUI>("FightUI");

        Transform pointTf = GameObject.Find("Point").transform;

        // get the respawn points
        List<Transform> availablePoints = new List<Transform>();
        for (int i = 0; i < pointTf.childCount; i++)
        {
            availablePoints.Add(pointTf.GetChild(i));
        }

        // random one point to Human
        int humanIndex = UnityEngine.Random.Range(0, availablePoints.Count);
        Vector3 humanPos = availablePoints[humanIndex].position;
        PhotonNetwork.Instantiate("Human", humanPos, Quaternion.identity);

        // delete the point
        availablePoints.RemoveAt(humanIndex);

        // random one point to Cheese
        int cheeseIndex = UnityEngine.Random.Range(0, availablePoints.Count);
        Vector3 cheesePos = availablePoints[cheeseIndex].position;
        PhotonNetwork.Instantiate("Cheese", cheesePos, Quaternion.identity);
        
        
        
       

        

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
