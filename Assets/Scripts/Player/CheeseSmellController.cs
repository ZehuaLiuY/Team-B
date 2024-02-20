using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CheeseSmellController : MonoBehaviourPun
{
    public ParticleSystem smellParticlePrefab;

    private int _smellGenerateInterval = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 在 Update 方法中控制气味生成的频率
        if (Time.frameCount % _smellGenerateInterval == 0)
        {
            photonView.RPC("GenerateSmell", RpcTarget.All);
            
        }
    }

    [PunRPC]
    void GenerateSmell()
    {
        // 在奶酪当前位置生成气味足迹
        ParticleSystem smellParticle = Instantiate(smellParticlePrefab, transform.position, Quaternion.identity);

        // 持续释放气味
        smellParticle.Play();

        // 添加气味消散效果
        StartCoroutine(DisappearAfter(smellParticle, 1f)); // 1秒后消散


    }

    IEnumerator DisappearAfter(ParticleSystem particleSystem, float duration)
    {
        yield return new WaitForSeconds(duration);
        particleSystem.Stop();
    }
}
