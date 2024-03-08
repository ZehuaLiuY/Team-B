using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    public TrailRenderer trail; // 引用 Trail 渲染器组件
    public float verticalSpeed = 0.1f; // 控制向上方飘散的速度

    private void Start()
    {
        
    }

    void Update()
    {
        // 获取当前 Trail 的位置
        Vector3 currentPosition = transform.position;

        // 计算垂直方向的位移
        float verticalOffset = verticalSpeed * Time.deltaTime;

        // 更新 Trail 的位置，使其在垂直方向上向上方飘散
        currentPosition.y += verticalOffset;

        // 将新位置应用到 Trail 上
        transform.position = currentPosition;
    }
}
