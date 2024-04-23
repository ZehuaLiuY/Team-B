using System.Collections;
using System.Collections.Generic;
using CheeseController;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;

public class Jump_Skill : MonoBehaviourPun
{
    private bool _skillUsed;
    public float skillDuration = 5f;
    private float _skillDurationTimer;
    private CheeseThirdPerson _cheeseThirdPerson;
    // Start is called before the first frame update
    void Start()
    {
        _cheeseThirdPerson = GetComponent<CheeseThirdPerson>();
        _skillUsed = false;
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
            StartCoroutine(ApplyJumpSkill());
        }
    }

    IEnumerator ApplyJumpSkill()
    {
        _skillUsed = true;
        photonView.RPC("JumpSkill", RpcTarget.All);

        _skillDurationTimer = skillDuration;
        UpdateIcon(1f);

        while (_skillDurationTimer > 0)
        {
            _skillDurationTimer -= Time.deltaTime;
            UpdateIcon(_skillDurationTimer / skillDuration);
            yield return null;
        }

        CheeseFightUI.Instance.UpdateSkill_Icon(1f);
        GetComponent<GetSkill>().DeactivateSkill("Jump Skill");
        _skillUsed = false;
    }
    
    [PunRPC]
    public void JumpSkill()
    {
        if (_cheeseThirdPerson != null)
        {
            _cheeseThirdPerson.JumpHeight = 50f;
            _cheeseThirdPerson.Gravity = -28f;
            StartCoroutine(RestoreJump(5f));
        }
    }

    private IEnumerator RestoreJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_cheeseThirdPerson != null)
        {
            _cheeseThirdPerson.JumpHeight = 10f;
            _cheeseThirdPerson.Gravity = -140f;
        }
    }
    
    private void UpdateIcon(float fillAmount)
    {
        CheeseFightUI.Instance.UpdateSkill_Icon(fillAmount);
    }
}