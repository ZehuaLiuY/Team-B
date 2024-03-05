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
        isInvisible = true;
        photonView.RPC("SetInvisibility", RpcTarget.All, true);
        
        yield return new WaitForSeconds(Skill_Duration);

        photonView.RPC("RestoreVisibility", RpcTarget.All);

        isInvisible = false;
        isCooldown = true;
        yield return new WaitForSeconds(Skill_Cooldown);
        isCooldown = false;
    }
    
    [PunRPC]
    void SetInvisibility(bool state)
    {
        if (photonView.IsMine)
        {
            // 如果是本地玩家，设置为隐身材质，以便玩家可以看到自己
            Material mat = state ? invisibleMaterial : originalMaterials[0];
            foreach (var renderer in childRenderers)
            {
                renderer.material = mat;
            }
        }
        else
        {
            // 对于其他玩家，如果状态是要变为隐身，则禁用渲染器，否则启用
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
            // 为本地玩家恢复原始材质
            for (int i = 0; i < childRenderers.Length; i++)
            {
                childRenderers[i].material = originalMaterials[i];
            }
        }
        else
        {
            // 重新启用其他玩家的渲染器
            foreach (var renderer in childRenderers)
            {
                renderer.gameObject.SetActive(true);
            }
        }
    }
}