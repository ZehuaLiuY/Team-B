using System;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FightUI1 : MonoBehaviour
{
    public static FightUI1 Instance { get; private set; }
    // public Image StaminaBar; // 确保在Unity编辑器中已经设置了这个引用
    //private GameManager gameManager;
    private Text countdownText;
    private Image Skill_Icon;
    private Transform tutorialPanel;

    private float previousTime;
    private bool iscount;
    //public static float countdownTimer = 180f;
    private void Start()
    {
        iscount = true;
        countdownText = transform.Find("CountdownText").GetComponent<Text>();
        tutorialPanel = transform.Find("TutorialPanel");
        Transform Invisible = transform.Find("Invisible Effect");
        Invisible.gameObject.SetActive(true);
        if (Invisible != null && Invisible.childCount > 0) {
            // 假设hp下只有一个子对象，直接获取第一个子对象
            Transform firstChild = Invisible.GetChild(0);
            Image image = firstChild.GetComponent<Image>();
            if (image != null) {
                // 成功找到了Image组件
                Skill_Icon = image;
            }
        }
        StartCoroutine(ShowTutorialPanel());
        
        //--------------------------
        // top left placeholder components
        // transform.Find("hp/fill").GetComponent<Image>().fillAmount =
        // transform.Find("hp/Text").GetComponent<Text>().text =
        //--------------------------

    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    public void UpdateSkill_Icon(float fillAmount)
    {
        if (Skill_Icon != null)
        {
            Skill_Icon.fillAmount = fillAmount;
        }
    }

    //public AudioClip countSound;
    //public AudioClip timesupSound;
    //// Update is called once per frame
    void Update()
    {

    }
    
    IEnumerator ShowTutorialPanel()
    {
        tutorialPanel.gameObject.SetActive(true); // 显示教程面板
        yield return new WaitForSeconds(10); // 等待5秒
        tutorialPanel.gameObject.SetActive(false); // 隐藏教程面板
    }

    public void SetCountdownTimer(float countdownTimer) 
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
}
