using System.Collections;
using System.Collections.Generic;
using CheeseController;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;

public class Sprint_Skill : MonoBehaviourPun
{
    private bool _skillUsed = false;
    public float skillDuration = 5f;
    public GameObject trail;
    private float _skillDurationTimer;
    private CheeseThirdPerson _cheeseThirdPerson;
    // Start is called before the first frame update
    void Start()
    {
        _cheeseThirdPerson = GetComponent<CheeseThirdPerson>();
        if (_cheeseThirdPerson == null)
        {
            Debug.LogWarning("PlayerMovement component not found on the game object.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && !_skillUsed && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(ApplySprintSkill());
        }
    }

    IEnumerator ApplySprintSkill()
    {
        _skillUsed = true;
        photonView.RPC("SprintSkill", RpcTarget.All);

        _skillDurationTimer = skillDuration;
        UpdateIcon(1f);

        while (_skillDurationTimer > 0)
        {
            _skillDurationTimer -= Time.deltaTime;
            UpdateIcon(_skillDurationTimer / skillDuration);
            yield return null;
        }

        CheeseFightUI.Instance.UpdateSkill_Icon(1f);
        GetComponent<GetSkill>().DeactivateSkill("Sprint Skill");
        _skillUsed = false;
    }
    
    [PunRPC]
    public void SprintSkill()
    {
        if (_cheeseThirdPerson != null)
        {
            _cheeseThirdPerson.MoveSpeed = 250f;
            StartCoroutine(RestoreSpeed(5f));
            trail.gameObject.SetActive(true);
        }
    }

    private IEnumerator RestoreSpeed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_cheeseThirdPerson != null)
        {
            _cheeseThirdPerson.MoveSpeed = 100.0f;
        }
        trail.gameObject.SetActive(false);
    }
    
    private void UpdateIcon(float fillAmount)
    {
        CheeseFightUI.Instance.UpdateSkill_Icon(fillAmount);
    }
}
