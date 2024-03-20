using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 


// public class InvisibilityEffect : MonoBehaviourPunCallbacks
// {
//     public Material invisibleMaterial; 
//     public float Skill_Cooldown = 30f;
//     public float Skill_Duration = 3f;
//     private Material[] originalMaterials;
//     private SkinnedMeshRenderer[] childRenderers;
//     private bool isInvisible = false;
//     private bool isCooldown = false;
//     private float skillIconFill;
//
//     void Start()
//     {
//         childRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
//         originalMaterials = new Material[childRenderers.Length];
//
//         for (int i = 0; i < childRenderers.Length; i++)
//         {
//             originalMaterials[i] = childRenderers[i].material;
//         }
//     }
//
//     void Update()
//     {
//         if (photonView.IsMine && !isInvisible && !isCooldown && Input.GetKeyDown(KeyCode.F))
//         {
//             StartCoroutine(BecomeInvisible());
//         }
//     }
//
//     IEnumerator BecomeInvisible()
//     {
//         photonView.RPC("SetInvisibility", RpcTarget.All, true); // 开始隐身效果
//         isInvisible = true;
//         isCooldown = true;
//
//         StartCoroutine(EndInvisibilityAfterDuration());
//
//         float cooldownEndTime = Time.time + Skill_Cooldown;
//
//         skillIconFill = 1f;
//         UpdateIcon();
//
//         while (Time.time < cooldownEndTime)
//         {
//             skillIconFill = (cooldownEndTime - Time.time) / Skill_Cooldown;
//             UpdateIcon();
//             yield return null;
//         }
//
//         skillIconFill = 0f;
//         UpdateIcon();
//         isCooldown = false; 
//     }
//
//     IEnumerator EndInvisibilityAfterDuration()
//     {
//         yield return new WaitForSeconds(Skill_Duration);
//
//         if (isInvisible)
//         {
//             photonView.RPC("RestoreVisibility", RpcTarget.All);
//             isInvisible = false;
//         }
//     }
//     
//     private void UpdateIcon()
//     {
//         if (CheeseFightUI.Instance != null)
//         {
//             CheeseFightUI.Instance.UpdateSkill_Icon(skillIconFill);
//         } 
//     }
//     
//     [PunRPC]
//     void SetInvisibility(bool state)
//     {
//         if (photonView.IsMine)
//         {
//             Material mat = state ? invisibleMaterial : originalMaterials[0];
//             foreach (var renderer in childRenderers)
//             {
//                 renderer.material = mat;
//             }
//         }
//         else
//         {
//             foreach (var renderer in childRenderers)
//             {
//                 renderer.gameObject.SetActive(false);
//             }
//         }
//     }
//
//     [PunRPC]
//     void RestoreVisibility()
//     {
//         if (photonView.IsMine)
//         {
//             for (int i = 0; i < childRenderers.Length; i++)
//             {
//                 childRenderers[i].material = originalMaterials[i];
//             }
//         }
//         else
//         {
//             foreach (var renderer in childRenderers)
//             {
//                 renderer.gameObject.SetActive(true);
//             }
//         }
//     }
// }


public class InvisibilityEffect : MonoBehaviourPunCallbacks
{
    public Material invisibleMaterial;
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
        skillUsed = true; // 标记技能已被使用

        photonView.RPC("SetInvisibility", RpcTarget.All, true);

        skillDurationTimer = Skill_Duration;
        UpdateIcon(skillDurationTimer);

        // 当技能持续时间还没结束时逐渐减少填充量
        while(skillDurationTimer > 0)
        {
            skillDurationTimer -= Time.deltaTime;
            UpdateIcon(skillDurationTimer / Skill_Duration); // 更新UI的填充量
            yield return null;
        }

        photonView.RPC("RestoreVisibility", RpcTarget.All);

        // 技能结束后禁用UI并重置技能图标的填充量
        CheeseFightUI.Instance.UpdateSkill_Icon(1f); // 重置填充量
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
        foreach (var renderer in childRenderers)
        {
            renderer.material = state ? invisibleMaterial : originalMaterials[0];
        }
    }

    [PunRPC]
    void RestoreVisibility()
    {
        for (int i = 0; i < childRenderers.Length; i++)
        {
            childRenderers[i].material = originalMaterials[i];
        }
    }
}
