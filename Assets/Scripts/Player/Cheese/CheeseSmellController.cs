using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CheeseSmellController : MonoBehaviourPun
{
    public ParticleSystem smellParticlePrefab;

    public ParticleSystem smellEverywhere;

    private int _smellGenerateInterval = 25;

    public List<ParticleSystem> smellParticles = new List<ParticleSystem>();

    private Transform _targetFan = null;

    private bool _isFan = false;

    private bool _enable = true;

    private bool _disable = false;

    private void Start()
    {
        StartCoroutine(GenerateSmell());
    }

    IEnumerator GenerateSmell()
    {
        while (_enable)
        {
            yield return new WaitForSeconds(0.5f);
            photonView.RPC("GenerateSmell", RpcTarget.All, _disable);

            if (_disable)
            {
                break;
            }
        }
    }

    public void setEnable(bool enable)
    {
        _enable = enable;
    }

    public void setDisable(bool disable)
    {
        _disable = disable;
    }

    [PunRPC]
    void GenerateSmell(bool disable)
    {
        if (!disable && !_isFan)
        {
            // generate smell particle at the player position
            ParticleSystem smellParticle = Instantiate(smellParticlePrefab, transform.position, Quaternion.identity);

            // continue to play the particle
            smellParticle.Play();
        }
        else if(!disable && _isFan) 
        {
            GameObject smellParticle = Instantiate(smellParticlePrefab, transform.position, Quaternion.identity).gameObject;
            SetFanEffect(smellParticle);
        }
        else
        {
            ParticleSystem smellParticle = Instantiate(smellEverywhere, transform.position, Quaternion.identity);

            smellParticle.Play();
        }

    }

    public void setFan(bool isFan, Transform targetFan)
    {
        _isFan = isFan;
        _targetFan = targetFan;
    }

    void SetFanEffect(GameObject smellParticle)
    {
        Vector3 directionToExhaustFan = _targetFan.position - smellParticle.transform.position;

        ParticleSystem particleSystem = smellParticle.GetComponent<ParticleSystem>();

        var velocityOverLifetime = particleSystem.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        velocityOverLifetime.x = directionToExhaustFan.x;
        velocityOverLifetime.y = directionToExhaustFan.y;
        velocityOverLifetime.z = directionToExhaustFan.z;

        velocityOverLifetime.speedModifier = 0.5f;

        particleSystem.Play();

    }


}
