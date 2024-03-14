using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 


public class InvisibilityEffect : MonoBehaviourPunCallbacks
{
    public Material invisibleMaterial; 
    public float Skill_Cooldown;
    public float Skill_Duration;
    private Material[] originalMaterials;
    private SkinnedMeshRenderer[] childRenderers;
    private bool isInvisible = false;
    private bool isCooldown = false;
    private float skillIconFill;

    void Start()
    {
        childRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        originalMaterials = new Material[childRenderers.Length];

        for (int i = 0; i < childRenderers.Length; i++)
        {
            originalMaterials[i] = childRenderers[i].material;
        }
    }

    void Update()
    {
        if (photonView.IsMine && !isInvisible && !isCooldown && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(BecomeInvisible());
        }
    }

    IEnumerator BecomeInvisible()
    {
        photonView.RPC("SetInvisibility", RpcTarget.All, true); // 开始隐身效果
        isInvisible = true;
        isCooldown = true;

        StartCoroutine(EndInvisibilityAfterDuration());

        float cooldownEndTime = Time.time + Skill_Cooldown;

        skillIconFill = 1f;
        UpdateIcon();

        while (Time.time < cooldownEndTime)
        {
            skillIconFill = (cooldownEndTime - Time.time) / Skill_Cooldown;
            UpdateIcon();
            yield return null;
        }

        skillIconFill = 0f;
        UpdateIcon();
        isCooldown = false; 
    }

    IEnumerator EndInvisibilityAfterDuration()
    {
        yield return new WaitForSeconds(Skill_Duration);

        if (isInvisible)
        {
            photonView.RPC("RestoreVisibility", RpcTarget.All);
            isInvisible = false;
        }
    }
    
    private void UpdateIcon()
    {
        if (CheeseFightUI.Instance != null)
        {
            CheeseFightUI.Instance.UpdateSkill_Icon(skillIconFill);
        } 
    }
    
    [PunRPC]
    void SetInvisibility(bool state)
    {
        if (photonView.IsMine)
        {
            Material mat = state ? invisibleMaterial : originalMaterials[0];
            foreach (var renderer in childRenderers)
            {
                renderer.material = mat;
            }
        }
        else
        {
            foreach (var renderer in childRenderers)
            {
                renderer.gameObject.SetActive(false);
            }
        }
    }

    [PunRPC]
    void RestoreVisibility()
    {
        if (photonView.IsMine)
        {
            for (int i = 0; i < childRenderers.Length; i++)
            {
                childRenderers[i].material = originalMaterials[i];
            }
        }
        else
        {
            foreach (var renderer in childRenderers)
            {
                renderer.gameObject.SetActive(true);
            }
        }
    }
}