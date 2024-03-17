using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using CheeseController;

public class CheeseSmellController : MonoBehaviourPun
{
    public ParticleSystem smellParticlePrefab;

    private int _smellGenerateInterval = 30;

    public List<ParticleSystem> smellParticles = new List<ParticleSystem>();

    private bool _enable = true;

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
                photonView.RPC("GenerateSmell", RpcTarget.All);
            }
        }
        
       
    }

    public void setEnable(bool enable)
    {
        _enable = enable;
    }

    [PunRPC]
    void GenerateSmell()
    {
        // 在奶酪当前位置生成气味足迹
        ParticleSystem smellParticle = Instantiate(smellParticlePrefab, transform.position, Quaternion.identity);

        // 持续释放气味
        smellParticle.Play();

        //// 将生成的粒子系统加入列表
        //smellParticles.Add(smellParticle);

        //// 添加气味消散效果, 延迟1秒后关闭粒子系统
        //Invoke("StopParticle", 1f);

    }


    //void StopParticle()
    //{
    //    if (smellParticles.Count > 0)
    //    {
    //        ParticleSystem particleSystem = smellParticles[0];
    //        smellParticles.RemoveAt(0);
    //        particleSystem.Stop();
    //    }
    //}

    ////用于停止粒子系统
    //public void StopParticles()
    //{
    //    if(smellParticles.Count > 0)
    //    {
    //        foreach (ParticleSystem particleSystem in smellParticles)
    //        {
    //            particleSystem.Stop();
    //        }
    //    }
        
    //}


}
