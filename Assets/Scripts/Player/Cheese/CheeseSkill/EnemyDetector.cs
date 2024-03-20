// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Photon.Pun;
//
// public class EnemyDetector : MonoBehaviourPunCallbacks
// {
//     public Material highlightMaterial; // 用于高亮显示敌人的材质
//     private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
//     public float cooldownTime = 30f;
//     private bool isDetecting = false; 
//     private bool isCooldown = false;
//     private float skillIconFill;
//     private float nextAvailableTime;
//     
//
//     void Update()
//     {
//         if (photonView.IsMine && Input.GetKeyDown(KeyCode.F) && !isDetecting && !isCooldown)
//         {
//             StartCoroutine(DetectEnemies());
//             StartCooldown();
//         }
//         if (isCooldown)
//         {
//             skillIconFill = (nextAvailableTime - Time.time) / cooldownTime;
//             skillIconFill = Mathf.Clamp(skillIconFill, 0f, 1f);
//             UpdateIcon();
//             
//             if (Time.time >= nextAvailableTime)
//             {
//                 isCooldown = false;
//                 skillIconFill = 0f;
//                 UpdateIcon(); 
//                 Debug.Log("Cooldown finished.");
//             }
//         }
//     }
//
//     IEnumerator DetectEnemies()
//     {
//         isDetecting = true;
//         Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius, LayerMask.GetMask("Human"));
//         foreach (var hitCollider in hitColliders)
//         {
//             SkinnedMeshRenderer[] renderers = hitCollider.GetComponentsInChildren<SkinnedMeshRenderer>();
//             foreach (var renderer in renderers)
//             {
//                 if (!originalMaterials.ContainsKey(renderer))
//                 {
//                     // 存储原始材质
//                     originalMaterials[renderer] = renderer.materials;
//                 }
//
//                 // 设置新材质
//                 Material[] newMaterials = new Material[renderer.materials.Length];
//                 for (int i = 0; i < newMaterials.Length; i++)
//                 {
//                     newMaterials[i] = highlightMaterial;
//                 }
//                 renderer.materials = newMaterials;
//             }
//         }
//
//         yield return new WaitForSeconds(5);
//
//         foreach (var renderer in originalMaterials.Keys)
//         {
//             StartCoroutine(ResetMaterialAfterDelay(renderer, 5)); // 只需传递渲染器和延迟
//         }
//
//         isDetecting = false;
//         Debug.Log("Detection period ended.");
//     }
//
//
//     IEnumerator ResetMaterialAfterDelay(Renderer renderer, float delay)
//     {
//         yield return new WaitForSeconds(delay);
//         if (renderer != null && renderer.gameObject.activeInHierarchy && renderer.enabled && originalMaterials.ContainsKey(renderer))
//         {
//             // 恢复原始材质
//             renderer.materials = originalMaterials[renderer];
//             Debug.Log("Materials reset for: " + renderer.gameObject.name);
//         }
//         else
//         {
//             Debug.Log("Renderer is null, inactive, or disabled for: " + (renderer != null ? renderer.gameObject.name : "Unknown"));
//         }
//     }
//     
//     void StartCooldown()
//     {
//         isCooldown = true;
//         nextAvailableTime = Time.time + cooldownTime;
//         skillIconFill = 1f;
//         UpdateIcon();
//         Debug.Log("Cooldown started.");
//     }
//     
//     private void UpdateIcon()
//     {
//         if (CheeseFightUI.Instance != null)
//         {
//             CheeseFightUI.Instance.UpdateSkill_Icon(skillIconFill);
//         } 
//     }
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyDetector : MonoBehaviourPunCallbacks
{
    public Material highlightMaterial; // 用于高亮显示敌人的材质
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    public float detectionDuration = 5f; // 检测敌人的持续时间
    private bool skillUsed = false; // 表示技能是否正在使用
    private float skillTimer; // 跟踪技能持续时间的计时器

    void Update()
    {
        if (photonView.IsMine && !skillUsed && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(DetectEnemies());
        }

        // 如果技能正在使用，则连续更新技能图标的填充量
        if (skillUsed)
        {
            UpdateIcon(skillTimer / detectionDuration);
        }
    }

    IEnumerator DetectEnemies()
    {
        skillUsed = true; // 标记技能为正在使用状态
        skillTimer = detectionDuration; // 重置技能计时器

        // 执行敌人检测逻辑
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius, LayerMask.GetMask("Human"));
        foreach (var hitCollider in hitColliders)
        {
            SkinnedMeshRenderer[] renderers = hitCollider.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in renderers)
            {
                if (!originalMaterials.ContainsKey(renderer))
                {
                    originalMaterials[renderer] = renderer.materials;
                }

                Material[] newMaterials = new Material[renderer.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = highlightMaterial;
                }
                renderer.materials = newMaterials;
            }
        }

        while (skillTimer > 0)
        {
            skillTimer -= Time.deltaTime;
            UpdateIcon(skillTimer / detectionDuration);
            yield return null;
        }
        
        CheeseFightUI.Instance.UpdateSkill_Icon(1f);
        GetComponentInParent<GetSkill>().DeactivateSkill("Detector Skill");
        skillUsed = false; // 重置技能使用状态

        // 恢复所有更改过的材质
        foreach (var renderer in originalMaterials.Keys)
        {
            if (renderer != null && originalMaterials.ContainsKey(renderer))
            {
                renderer.materials = originalMaterials[renderer];
            }
        }
        originalMaterials.Clear();
    }

    private void UpdateIcon(float fillAmount)
    {
        if (CheeseFightUI.Instance != null)
        {
            CheeseFightUI.Instance.UpdateSkill_Icon(fillAmount);
        }
    }
}
