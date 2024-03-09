using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OdorDiffusion : MonoBehaviour
{
    public float diffusionCoefficient = 0.1f; // 扩散系数
    public float odorReleaseRate = 1.0f; // 气味释放速率
    public float odorDecayRate = 0.01f; // 气味衰减速率

    private float[,] odorGrid; // 二维数组存储气味浓度
    private int gridSizeX = 5; // 网格尺寸X
    private int gridSizeZ = 5; // 网格尺寸Z

    //public GameObject cheesePlayer; // 奶酪玩家

    void Start()
    {
        // 初始化气味网格
        odorGrid = new float[gridSizeX, gridSizeZ];

        // 设置初始气味浓度
        InitializeOdorGrid();

        // 启动气味扩散
        InvokeRepeating("DiffuseOdor", 1f, 1f);
    }

    void InitializeOdorGrid()
    {
        // 将玩家位置周围的网格节点的气味浓度设置为初始值
        Vector3 position = transform.position;
        int gridX = Mathf.RoundToInt(position.x);
        int gridZ = Mathf.RoundToInt(position.z);

        // 确保坐标在有效范围内
        gridX = Mathf.Clamp(gridX, 0, gridSizeX - 1);
        gridZ = Mathf.Clamp(gridZ, 0, gridSizeZ - 1);

        odorGrid[gridX, gridZ] = 1f; // 初始浓度为1，表示气味源的位置
    }

    void DiffuseOdor()
    {
        // 在每个节点上应用扩散方程来更新气味浓度
        for (int x = 1; x < gridSizeX - 1; x++)
        {
            for (int z = 1; z < gridSizeZ - 1; z++)
            {
                // 计算当前节点周围四个方向上的气味浓度变化
                float deltaOdor = (odorGrid[x - 1, z] + odorGrid[x + 1, z] +
                                   odorGrid[x, z - 1] + odorGrid[x, z + 1]) * 0.25f - odorGrid[x, z];

                // 根据释放速率和扩散变化更新气味浓度
                odorGrid[x, z] += odorReleaseRate * deltaOdor - odorDecayRate * odorGrid[x, z];
            }
        }

        // 更新气味粒子的位置和属性
        UpdateOdorParticles();
    }

    void UpdateOdorParticles()
    {
        // 获取粒子系统组件
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();

        // 获取粒子系统的主模块
        ParticleSystem.MainModule mainModule = particleSystem.main;

        // 获取粒子系统的粒子数组
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
        int particleCount = particleSystem.GetParticles(particles);

        // 遍历每个粒子
        for (int i = 0; i < particleCount; i++)
        {
            // 获取当前粒子的位置
            Vector3 particlePosition = particles[i].position;

            // 根据当前粒子的位置获取对应的气味浓度
            float odorConcentration = GetOdorConcentration(particlePosition);

            // 根据气味浓度调整粒子的大小
            particles[i].startSize *= odorConcentration * diffusionCoefficient;

            // 根据气味浓度调整粒子的颜色
            particles[i].startColor = Color.Lerp(Color.white, Color.yellow, odorConcentration);
        }

        // 将更新后的粒子数组应用到粒子系统中
        particleSystem.SetParticles(particles, particleCount);
    }

    float GetOdorConcentration(Vector3 position)
    {
        // 将世界坐标转换为网格坐标
        int x = Mathf.RoundToInt(position.x);
        int z = Mathf.RoundToInt(position.z);

        // 确保坐标在有效范围内
        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        z = Mathf.Clamp(z, 0, gridSizeZ - 1);

        // 返回对应网格节点的气味浓度
        return odorGrid[x, z];
    }
}
