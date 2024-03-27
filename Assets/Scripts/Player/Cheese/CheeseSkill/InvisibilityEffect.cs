using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

public class InvisibilityEffect : MonoBehaviourPunCallbacks
{
    public Material invisibleMaterial;
    public GameObject Smell;
    public float Skill_Duration = 3f;
    private Material[] originalMaterials;
    private SkinnedMeshRenderer[] childRenderers;
    private bool skillUsed = false;
    private float skillDurationTimer; 

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
        if (photonView.IsMine && !skillUsed && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(ApplyInvisibilityEffect());
        }
    }
    
    IEnumerator ApplyInvisibilityEffect()
    {
        skillUsed = true;
        
        photonView.RPC("SetInvisibility", RpcTarget.Others, true);
        
        if (photonView.IsMine)
        {
            foreach (var renderer in childRenderers)
            {
                renderer.material = invisibleMaterial;
            }
            Smell.SetActive(false); 
        }

        skillDurationTimer = Skill_Duration;
        UpdateIcon(skillDurationTimer);
    
        while(skillDurationTimer > 0)
        {
            skillDurationTimer -= Time.deltaTime;
            UpdateIcon(skillDurationTimer / Skill_Duration);
            yield return null;
        }

        // 恢复可见性
        if (photonView.IsMine)
        {
            for (int i = 0; i < childRenderers.Length; i++)
            {
                childRenderers[i].material = originalMaterials[i];
            }
            Smell.SetActive(true);
        }
        photonView.RPC("RestoreVisibility", RpcTarget.Others);

        CheeseFightUI.Instance.UpdateSkill_Icon(1f);
        GetComponent<GetSkill>().DeactivateSkill("Invisible Skill");
        skillUsed = false;
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
