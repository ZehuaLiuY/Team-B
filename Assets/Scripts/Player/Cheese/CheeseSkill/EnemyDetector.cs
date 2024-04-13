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
    public Material highlightMaterial; // using to highlight the enemy
    private Dictionary<Renderer, Material[]> _originalMaterials = new Dictionary<Renderer, Material[]>();
    public float detectionDuration = 5f; // detection duration
    private bool _skillUsed = false; // skill used flag
    private float _skillTimer; // skill timer to track skill duration

    void Update()
    {
        if (photonView.IsMine && !_skillUsed && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(DetectEnemies());
        }

        // if the skill has been used, continuously update the skill icon
        if (_skillUsed)
        {
            UpdateIcon(_skillTimer / detectionDuration);
        }
    }

    IEnumerator DetectEnemies()
    {
        _skillUsed = true; // mark the skill as used
        _skillTimer = detectionDuration; // reset the skill timer
        Camera mainCamera = Camera.main;
        bool originalOcclusionSetting = false;
        
        if (mainCamera != null)
        {
            // 保存原始的遮挡剔除设置
            originalOcclusionSetting = mainCamera.useOcclusionCulling;
            // 禁用遮挡剔除
            mainCamera.useOcclusionCulling = false;
        }
        // highlight all enemies in the detection range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius, LayerMask.GetMask("Human"));
        foreach (var hitCollider in hitColliders)
        {
            SkinnedMeshRenderer[] renderers = hitCollider.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in renderers)
            {
                if (!_originalMaterials.ContainsKey(renderer))
                {
                    _originalMaterials[renderer] = renderer.materials;
                }

                Material[] newMaterials = new Material[renderer.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = highlightMaterial;
                }
                renderer.materials = newMaterials;
            }
        }

        while (_skillTimer > 0)
        {
            _skillTimer -= Time.deltaTime;
            UpdateIcon(_skillTimer / detectionDuration);
            yield return null;
        }
        
        if (mainCamera != null)
        {
            mainCamera.useOcclusionCulling = originalOcclusionSetting;
        }
        
        CheeseFightUI.Instance.UpdateSkill_Icon(1f);
        GetComponentInParent<GetSkill>().DeactivateSkill("Detector Skill");
        _skillUsed = false; // reset the skill used flag

        // reset the materials of all highlighted enemies
        foreach (var renderer in _originalMaterials.Keys)
        {
            if (renderer != null && _originalMaterials.ContainsKey(renderer))
            {
                renderer.materials = _originalMaterials[renderer];
            }
        }
        _originalMaterials.Clear();
    }

    private void UpdateIcon(float fillAmount)
    {
        if (CheeseFightUI.Instance != null)
        {
            CheeseFightUI.Instance.UpdateSkill_Icon(fillAmount);
        }
    }
}