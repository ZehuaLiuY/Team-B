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

    //private Text _countdownText;
    private Image _skillIcon;
    private Transform _tutorialPanel;
    private int _remainingLife = 0;
    private TMP_Text _remainingLifeText;
    
    //private AudioSource _countdownMusic;
    //private AudioClip _last10SecondsSound;
    //private bool _last10SecondsSoundPlayed = false;

    //private float _previousTime;
    //private bool _iscount;

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
    
    private void Start()
    {
        //_countdownMusic = transform.Find("countdownMusic").GetComponent<AudioSource>();
        //_last10SecondsSound = Resources.Load<AudioClip>("10s");
        _remainingLifeText = transform.Find("RemainingLife").GetComponent<TMP_Text>();
        _remainingLifeText.text = _remainingLife + "";
        _tutorialPanel = transform.Find($"TutorialPanel");
        StartCoroutine(BeginStartSequence());
    }
    public void setRemainingLife(int remainingLife)
    {
        _remainingLife = remainingLife;
    }

    public void updateRemainingLife(int remainingLife)
    {
        _remainingLife = remainingLife;
        _remainingLifeText.text = _remainingLife + "";
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
}
