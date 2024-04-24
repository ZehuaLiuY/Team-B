using System.Collections;
using System.Collections.Generic;
using CheeseController;
using UnityEngine;
using Photon.Pun;

public class SmellBombSkill : MonoBehaviourPun
{
    private bool _skillUsed;
    public float skillDuration = 10f;
    private float _skillDurationTimer;
    private CheeseThirdPerson _cheeseThirdPerson;

    void Start()
    {
        _skillUsed = false;
        _cheeseThirdPerson = GetComponent<CheeseThirdPerson>();
    }

    void Update()
    {
        if (photonView.IsMine && !_skillUsed && Input.GetMouseButtonDown(1))
        {
            StartCoroutine(ApplySmellBombSkill());
        }
    }

    IEnumerator ApplySmellBombSkill()
    {
        _skillUsed = true;

        DeploySmellBomb();
        _skillDurationTimer = skillDuration;
        UpdateIcon(1f);

        while (_skillDurationTimer > 0)
        {
            _skillDurationTimer -= Time.deltaTime;
            UpdateIcon(_skillDurationTimer / skillDuration);
            yield return null;
        }

        CheeseFightUI.Instance.UpdateSkill_Icon(1f);
        GetComponent<GetSkill>().DeactivateSkill("SmellBomb Skill");
        _skillUsed = false;
    }
    
    void DeploySmellBomb()
    {
        PhotonNetwork.Instantiate("SmellBomb Skill", transform.position, Quaternion.identity);
    }

    private void UpdateIcon(float fillAmount)
    {
        CheeseFightUI.Instance.UpdateSkill_Icon(fillAmount);
    }
}
