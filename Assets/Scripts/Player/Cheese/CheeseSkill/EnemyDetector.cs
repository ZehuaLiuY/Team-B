using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyDetector : MonoBehaviourPunCallbacks
{
    public Material highlightMaterial; // using to highlight the enemy
    private Dictionary<Renderer, Material[]> _originalMaterials = new Dictionary<Renderer, Material[]>();
    public float detectionDuration = 5f; // detection duration
    private bool _skillUsed; // skill used flag
    private float _skillTimer; // skill timer to track skill duration

    private void Start()
    {
        _skillUsed = false;
    }

    void Update()
    {
        if (photonView.IsMine && !_skillUsed && Input.GetMouseButtonDown(1))
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