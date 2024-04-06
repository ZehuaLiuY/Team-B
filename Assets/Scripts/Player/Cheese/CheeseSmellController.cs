using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using CheeseController;

public class CheeseSmellController : MonoBehaviourPun
{
    public ParticleSystem smellParticlePrefab;

    public ParticleSystem smellEverywhere;

    private int _smellGenerateInterval = 25;

    public List<ParticleSystem> smellParticles = new List<ParticleSystem>();

    private bool _enable = true;

    private bool _disable = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // control the smell generation on the Update function
        if (_enable)
        {
            if (Time.frameCount % _smellGenerateInterval == 0)
            {
                photonView.RPC("GenerateSmell", RpcTarget.All, _disable);
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
        if (!disable)
        {
            // generate smell particle at the player position
            ParticleSystem smellParticle = Instantiate(smellParticlePrefab, transform.position, Quaternion.identity);

            // continue to play the particle
            smellParticle.Play();
        }
        else
        {
            ParticleSystem smellParticle = Instantiate(smellEverywhere, transform.position, Quaternion.identity);

            smellParticle.Play();
        }

    }
}
