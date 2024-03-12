using System;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightUI3 : MonoBehaviour
{
    // public Image StaminaBar; // 确保在Unity编辑器中已经设置了这个引用
    public static FightUI3 Instance { get; private set; }
    //private GameManager gameManager;
    private Text countdownText;
    private Image Skill_Icon;
    private Transform tutorialPanel;

    private float previousTime;
    private bool iscount;
    //public static float countdownTimer = 180f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        iscount = true;
        countdownText = transform.Find("CountdownText").GetComponent<Text>();
        tutorialPanel = transform.Find("TutorialPanel_Cheese3");
        Transform Clone = transform.Find("Human Detector");
        Clone.gameObject.SetActive(true);
        if (Clone != null && Clone.childCount > 0) {
            // 假设hp下只有一个子对象，直接获取第一个子对象
            Transform firstChild = Clone.GetChild(0);
            Image image = firstChild.GetComponent<Image>();
            if (image != null) {
                // 成功找到了Image组件
                Skill_Icon = image;
            }
        }
        StartCoroutine(BeginStartSequence());
        //--------------------------
        // top left placeholder components
        // transform.Find("hp/fill").GetComponent<Image>().fillAmount =
        // transform.Find("hp/Text").GetComponent<Text>().text =
        //--------------------------

    }
    
    public void UpdateSkill_Icon(float fillAmount)
    {
        if (Skill_Icon != null)
        {
            Skill_Icon.fillAmount = fillAmount;
        }
    }
    
    IEnumerator BeginStartSequence()
    {
        yield return new WaitForSeconds(2); // 首先等待2秒
        StartCoroutine(ShowTutorialPanel()); // 然后显示教程面板
    }
    
    IEnumerator ShowTutorialPanel()
    {
        for (int i = 0; i < tutorialPanel.childCount; i++)
        {
            Transform currentChild = tutorialPanel.GetChild(i);
            currentChild.gameObject.SetActive(true);
            if (i == tutorialPanel.childCount - 1)
            {
                yield return new WaitForSeconds(10);
            }
            else
            {
                yield return new WaitForSeconds(5);
            }
            currentChild.gameObject.SetActive(false);
        }
    }

    //public AudioClip countSound;
    //public AudioClip timesupSound;
    //// Update is called once per frame
    void Update()
    {

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
