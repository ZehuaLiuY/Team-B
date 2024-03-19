using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using CheeseController;

public class CheeseSmellController : MonoBehaviourPun
{
    public ParticleSystem smellParticlePrefab;

    public ParticleSystem smellEverywhere;

    private int _smellGenerateInterval = 30;

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
        // 在 Update 方法中控制气味生成的频率
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
            // 在奶酪当前位置生成气味足迹
            ParticleSystem smellParticle = Instantiate(smellParticlePrefab, transform.position, Quaternion.identity);

            // 持续释放气味
            smellParticle.Play();
        }
        else
        {
            ParticleSystem smellParticle = Instantiate(smellEverywhere, transform.position, Quaternion.identity);

            smellParticle.Play();
        }

    }




}
