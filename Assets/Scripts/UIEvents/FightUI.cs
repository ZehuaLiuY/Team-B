using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightUI : MonoBehaviour
{
    //private GameManager gameManager;
    private Text countdownText;

    private float previousTime;
    private bool iscount;
    public static float countdownTimer = 180f;
    private void Start()
    {
        iscount = true;
        countdownText = transform.Find("CountdownText").GetComponent<Text>();
        

    }


    //public AudioClip countSound;
    //public AudioClip timesupSound;
    //// Update is called once per frame
    void Update()
    {
        countdownTimer -= Time.deltaTime;
        //if (!gameManager.isGameOver)
        //{
        //    // 更新倒计时文本内容
        //    UpdateCountdownText();

        //}
        UpdateCountdownText();

    }

    void UpdateCountdownText()
    {
        // 获取 GameManager 中的倒计时时间
        //float countdownTime = gameManager.GetCountdownTime();

        // 将倒计时时间格式化为分钟:秒钟的形式
        string formattedTime = string.Format("{0:0}:{1:00}", Mathf.Floor(countdownTimer / 60), Mathf.Floor(countdownTimer % 60));



        // 更新 TextMeshProUGUI 文本内容
        if (countdownText != null && iscount)
        {
            // 判断是否小于等于10秒，如果是，将颜色设置为红色
            if (Mathf.Floor(countdownTimer) <= 10 && Mathf.Floor(countdownTimer) > 0f)
            {
                countdownText.color = Color.red;
                // 在10秒之后的每一秒播放倒计时音效
                //if(Mathf.Floor(countdownTime) != previousTime)
                //{
                //    this.GetComponent<AudioSource>().PlayOneShot(countSound);
                //    //Debug.Log("countdownTime:" + countdownTime);

                //}

                //this.GetComponent<AudioSource>().PlayOneShot(countSound);
            }
            else if (Mathf.Floor(countdownTimer) == 0f)
            {
                //this.GetComponent<AudioSource>().PlayOneShot(timesupSound);
                iscount = false;
            }
            else
            {
                // 如果不是，将颜色还原为之前的颜色
                countdownText.color = Color.black;
            }
            countdownText.text = "Time: " + formattedTime;
            // 更新上一次的整数部分时间
            previousTime = Mathf.Floor(countdownTimer);
        }
    }

    public float GetFightUITimer()
    {
        // 返回倒计时时间
        return countdownTimer;
    }
}
