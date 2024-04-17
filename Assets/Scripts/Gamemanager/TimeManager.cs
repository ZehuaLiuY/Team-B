using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private TextMeshProUGUI _countdownText;

    private AudioSource _countdownMusic;
    private AudioClip _last10SecondsSound;
    private bool _last10SecondsSoundPlayed = false;

    private float _previousTime;
    private bool _iscount = true;

    private void Start()
    {
        _countdownMusic = transform.Find("countdownMusic").GetComponent<AudioSource>();
        _last10SecondsSound = Resources.Load<AudioClip>("10s");
        _countdownText = GetComponent<TextMeshProUGUI>();
       
    }

    public void setIsCount(bool isCount)
    {
        _iscount = isCount;
       
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
        else if(!_iscount)
        {
            _countdownText.text = "";
        }
    }
}
