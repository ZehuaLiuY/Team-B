using System;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class HumanFightUI : MonoBehaviour
{
    // public Image StaminaBar; // make sure to assign this in the inspector
    public static HumanFightUI Instance { get; private set; }
    //private GameManager gameManager;
    private Text _countdownText;
    private Transform _tutorialPanel;
    private Image _staminaBar;
    private TMP_Text _cheeseCaughtText;
    private TMP_Text _catchText;
    private AudioSource _countdownMusic;
    private AudioClip _last10SecondsSound;
    private bool _last10SecondsSoundPlayed = false;


    private float _previousTime;
    private bool _iscount;
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
        _iscount = true;
        _countdownText = transform.Find("CountdownText").GetComponent<Text>();
        _tutorialPanel = transform.Find("TutorialPanel");
        _cheeseCaughtText = transform.Find("CheeseCaughtText").GetComponent<TMP_Text>();
        _catchText = transform.Find("CatchText").GetComponent<TMP_Text>();
        _countdownMusic = transform.Find("countdownMusic").GetComponent<AudioSource>();
        _last10SecondsSound = Resources.Load<AudioClip>("10s");
     
        Transform hpTransform = transform.Find("hp");
        if (hpTransform != null && hpTransform.childCount > 0) {
            // suppose the first child is the fill image
            Transform firstChild = hpTransform.GetChild(0);
            Image image = firstChild.GetComponent<Image>();
            if (image != null) {
                // get the stamina bar image
                _staminaBar = image;
            }
        }

        StartCoroutine(BeginStartSequence());
        //--------------------------
        // top left placeholder components
        // transform.Find("hp/fill").GetComponent<Image>().fillAmount =
        // transform.Find("hp/Text").GetComponent<Text>().text =
        //--------------------------

    }

    public void showCheeseCaught()
    {
        _cheeseCaughtText.gameObject.SetActive(true);
        StartCoroutine(ResetCheeseCaught());
    }

    IEnumerator ResetCheeseCaught()
    { 
        yield return new WaitForSeconds(2f);
        _cheeseCaughtText.gameObject.SetActive(false);

    }
    
    public void showCatchText()
    {
        _catchText.gameObject.SetActive(true);
    }
    
    public void stopCatchText()
    {
        _catchText.gameObject.SetActive(false);
    }

    public void UpdateStaminaBar(float fillAmount)
    {
        if (_staminaBar != null)
        {
            _staminaBar.fillAmount = fillAmount;
        }
    }
    IEnumerator BeginStartSequence()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(ShowTutorialPanel());
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

        // format the time to minutes and seconds
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
                if (!_last10SecondsSoundPlayed)
                {
                    _countdownMusic.PlayOneShot(_last10SecondsSound);
                    _last10SecondsSoundPlayed = true; 
                }
            }
            else if (Mathf.Floor(countdownTimer) == 0f)
            {
                //this.GetComponent<AudioSource>().PlayOneShot(timesupSound);
                _iscount = false;
            }
            else
            {
                // if the countdown time is more than 10 seconds, change the color to black
                _countdownText.color = Color.white;
            }
            _countdownText.text = "Time: " + formattedTime;
            // update the previous time
            _previousTime = Mathf.Floor(countdownTimer);
        }
    }
}
