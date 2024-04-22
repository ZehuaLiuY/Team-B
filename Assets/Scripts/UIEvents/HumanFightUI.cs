using System;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using System.Runtime.InteropServices;

public class HumanFightUI : MonoBehaviour
{
    // public Image StaminaBar; // make sure to assign this in the inspector
    public static HumanFightUI Instance { get; private set; }
    //private GameManager gameManager;
    //private Text _countdownText;
    private Transform _tutorialPanel;
    private Image _staminaBar;
    private TMP_Text _cheeseCaughtText;
    private TMP_Text _catchText;
    private int _targetScore = 0;
    private int _currentScore = 0;
    private TMP_Text _targetScoreText;
    private TMP_Text _currentScoreText;
    
    //private AudioSource _countdownMusic;
    //private AudioClip _last10SecondsSound;
    //private bool _last10SecondsSoundPlayed = false;


    //private float _previousTime;
    //private bool _iscount;
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
        //_iscount = true;
        //_countdownText = transform.Find("CountdownText").GetComponent<Text>();
        _tutorialPanel = transform.Find("TutorialPanel");
        _cheeseCaughtText = transform.Find("CheeseCaughtText").GetComponent<TMP_Text>();
        _catchText = transform.Find("CatchText").GetComponent<TMP_Text>();
        _targetScoreText = transform.Find("TargetScore").GetComponent<TMP_Text>();
        _currentScoreText = transform.Find("CurrentScore").GetComponent<TMP_Text>();
        //_countdownMusic = transform.Find("countdownMusic").GetComponent<AudioSource>();
        //_last10SecondsSound = Resources.Load<AudioClip>("10s");
        _targetScoreText.text = _targetScore + "";
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

    public void setTargetScore(int targetScore)
    {

        _targetScore = targetScore;
        

    }

    public void updateCurrentScore(int currentScore)
    {
        _currentScore = currentScore;
        _currentScoreText.text = _currentScore + "";
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
}
