using System;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CheeseFightUI : MonoBehaviour
{
    public static CheeseFightUI Instance { get; private set; }

    //private GameManager gameManager;
    private Text _countdownText;
    private Image _skillIcon;
    private Transform _tutorialPanel;

    private float _previousTime;
    private bool _iscount;

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    public void InitializeUI(string playerType)
    {
        // Debug.Log($"Initializing UI for playerType: {playerType}");
        _iscount = true;
        _countdownText = transform.Find("CountdownText").GetComponent<Text>();
        _tutorialPanel = transform.Find($"TutorialPanel_{playerType}");
        StartCoroutine(BeginStartSequence());
    }

  
    public void ShowSkillUI(bool show, string skills)
    {
        Transform skillTransform = transform.Find(skills);
        if (skillTransform != null)
        {
            skillTransform.gameObject.SetActive(show); // render the skill UI or not
            if (show)
            {
                _skillIcon = skillTransform.GetComponent<Image>();
                Transform currentChild = skillTransform.GetChild(0);
                StartCoroutine(ShowSkillsTutorial(currentChild));
            }
        }
    }
    
    IEnumerator ShowSkillsTutorial(Transform skillTutorial)
    {
        skillTutorial.gameObject.SetActive(true); // activate the skill tutorial panel
        yield return new WaitForSeconds(5f); // wait for 5 seconds
        skillTutorial.gameObject.SetActive(false); // deactivate the skill tutorial panel
    }

    public void UpdateSkill_Icon(float fillAmount)
    {
        if (_skillIcon != null)
        {
            _skillIcon.fillAmount = fillAmount;
        }
    }
    
    IEnumerator BeginStartSequence()
    {
        yield return new WaitForSeconds(2); // wait for 2 seconds
        StartCoroutine(ShowTutorialPanel()); // show the tutorial panel
    }
    
    IEnumerator ShowTutorialPanel()
    {
        for (int i = 0; i < _tutorialPanel.childCount; i++)
        {
            Transform currentChild = _tutorialPanel.GetChild(i);
            currentChild.gameObject.SetActive(true);
            if (i == _tutorialPanel.childCount - 1)
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
        // get the countdown time from the game manager
        //float countdownTime = gameManager.GetCountdownTime();

        // standard time format
        string formattedTime = string.Format("{0:0}:{1:00}", Mathf.Floor(countdownTimer / 60), Mathf.Floor(countdownTimer % 60));



        // update the countdown text
        if (_countdownText != null && _iscount)
        {
            // if the countdown time is less than 10 seconds, change the color to red
            if (Mathf.Floor(countdownTimer) <= 10 && Mathf.Floor(countdownTimer) > 0f)
            {
                _countdownText.color = Color.red;
                // play the countdown sound
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
                _iscount = false;
            }
            else
            {
                // if the countdown time is more than 10 seconds, change the color to black
                _countdownText.color = Color.black;
            }
            _countdownText.text = "Time: " + formattedTime;
            // update the previous time
            _previousTime = Mathf.Floor(countdownTimer);
        }
    }
}
