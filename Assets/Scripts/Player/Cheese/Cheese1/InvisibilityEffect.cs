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
        Material mat = state ? invisibleMaterial : originalMaterials[0]; 
        foreach (var renderer in childRenderers)
        {
            renderer.material = mat;
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