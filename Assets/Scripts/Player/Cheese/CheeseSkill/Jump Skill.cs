using System.Collections;
using System.Collections.Generic;
using CheeseController;
using UnityEngine;
using Photon.Pun; 

public class Jump_Skill : MonoBehaviourPun
{
    private bool skillUsed = false;
    public float Skill_Duration = 5f;
    private float skillDurationTimer; 
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
        if (photonView.IsMine && !skillUsed && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(ApplyJumpSkill());
        }
    }

    IEnumerator ApplyJumpSkill()
    {
        skillUsed = true;
        photonView.RPC("JumpSkill", RpcTarget.All);

        skillDurationTimer = Skill_Duration;
        UpdateIcon(1f);

        while (skillDurationTimer > 0)
        {
            skillDurationTimer -= Time.deltaTime;
            UpdateIcon(skillDurationTimer / Skill_Duration);
            yield return null;
        }

        CheeseFightUI.Instance.UpdateSkill_Icon(1f);
        GetComponent<GetSkill>().DeactivateSkill("Jump Skill");
        skillUsed = false;
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