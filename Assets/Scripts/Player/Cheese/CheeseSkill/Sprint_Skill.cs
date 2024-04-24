using System.Collections;
using System.Collections.Generic;
using CheeseController;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;

public class Sprint_Skill : MonoBehaviourPun
{
    private bool _skillUsed;
    public float skillDuration = 5f;
    public GameObject trail;
    private float _skillDurationTimer;
    private CheeseThirdPerson _cheeseThirdPerson;
    
    void Start()
    {
        _skillUsed = false;
        _cheeseThirdPerson = GetComponent<CheeseThirdPerson>();
        if (_cheeseThirdPerson == null)
        {
            Debug.LogWarning("PlayerMovement component not found on the game object.");
        }
    }
    
    void Update()
    {
        if (photonView.IsMine && !_skillUsed && Input.GetMouseButtonDown(1))
        {
            StartCoroutine(ApplySprintSkill());
        }
    }
    
    IEnumerator ApplySprintSkill()
    {
        _skillUsed = true;
        photonView.RPC("SprintSkill", RpcTarget.All, true); // 启动 sprint，激活 trail

        _skillDurationTimer = skillDuration;
        UpdateIcon(1f);

        while (_skillDurationTimer > 0)
        {
            _skillDurationTimer -= Time.deltaTime;
            UpdateIcon(_skillDurationTimer / skillDuration);
            yield return null;
        }
        
        photonView.RPC("SprintSkill", RpcTarget.All, false); // 结束 sprint，关闭 trail
        CheeseFightUI.Instance.UpdateSkill_Icon(1f);
        GetComponent<GetSkill>().DeactivateSkill("Sprint Skill");
        _skillUsed = false;
    }

    
    [PunRPC]
    public void SprintSkill(bool activate)
    {
        if (_cheeseThirdPerson != null)
        {
            _cheeseThirdPerson.MoveSpeed = activate ? 250f : 100.0f;
            trail.gameObject.SetActive(activate);
            _cheeseThirdPerson.SetImmunityToSpeedReduction(activate);
            if (activate)
                _cheeseThirdPerson.CancelSpeedReduction();  
        }
      
    }
    
    
    private void UpdateIcon(float fillAmount)
    {
        CheeseFightUI.Instance.UpdateSkill_Icon(fillAmount);
    }
}
