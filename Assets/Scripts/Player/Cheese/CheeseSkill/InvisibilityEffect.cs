using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;

public class InvisibilityEffect : MonoBehaviourPunCallbacks
{
    public Material invisibleMaterial;
    public float skillDuration = 3f;
    private Material[] _originalMaterials;
    private SkinnedMeshRenderer[] _childRenderers;
    private bool _skillUsed = false;
    private float _skillDurationTimer;

    void Start()
    {
        _childRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        _originalMaterials = new Material[_childRenderers.Length];
        

        for (int i = 0; i < _childRenderers.Length; i++)
        {
            _originalMaterials[i] = _childRenderers[i].material;
        }
    }

    void Update()
    {
        if (photonView.IsMine && !_skillUsed && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(ApplyInvisibilityEffect());
        }
    }
    
    IEnumerator ApplyInvisibilityEffect()
    {
        _skillUsed = true;
        
        photonView.RPC("SetInvisibility", RpcTarget.Others, true);
        
        if (photonView.IsMine)
        {
            foreach (var renderer in _childRenderers)
            {
                renderer.material = invisibleMaterial;
            }
        }

        _skillDurationTimer = skillDuration;
        UpdateIcon(_skillDurationTimer);
    
        while(_skillDurationTimer > 0)
        {
            _skillDurationTimer -= Time.deltaTime;
            UpdateIcon(_skillDurationTimer / skillDuration);
            yield return null;
        }


        if (photonView.IsMine)
        {
            for (int i = 0; i < _childRenderers.Length; i++)
            {
                _childRenderers[i].material = _originalMaterials[i];
            }
        }
        photonView.RPC("RestoreVisibility", RpcTarget.Others);

        CheeseFightUI.Instance.UpdateSkill_Icon(1f);
        GetComponent<GetSkill>().DeactivateSkill("Invisible Skill");
        _skillUsed = false;
    }
    
    private void UpdateIcon(float fillAmount)
    {
        CheeseFightUI.Instance.UpdateSkill_Icon(fillAmount);
    }

    [PunRPC]
    void SetInvisibility(bool state)
    {
        gameObject.SetActive(false);
    }

    [PunRPC]
    void RestoreVisibility()
    {
        gameObject.SetActive(true);
    }
}
