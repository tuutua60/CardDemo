using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 统一进行帧同步管理
/// </summary>
public class FrameSyncMgr : BaseManager<FrameSyncMgr>
{
    /// <summary>
    /// 逻辑帧更新监听
    /// </summary>
    private Action onLogicUpdate = null;
    /// <summary>
    /// 逻辑帧累计运行时间
    /// </summary>
    private float totalFrameRuntime;
    /// <summary>
    /// 下一个逻辑帧开始的时间
    /// </summary>
    private float nextFrameTime;
    /// <summary>
    /// 动画缓动
    /// </summary>
    public static float deltaTime;

    /// <summary>
    /// 提供给外部注册逻辑帧更新监听
    /// </summary>
    /// <param name="listener"></param>
    public void AddFrameUpdateListener(Action listener)
    {
        onLogicUpdate += listener;
    }

    /// <summary>
    /// 提供给外部移除逻辑帧更新监听
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveFrameUpdateListener(Action listener)
    {
        onLogicUpdate -= listener;
    }

    /// <summary>
    /// 逻辑帧更新
    /// </summary>
    private void OnFrameUpdate()
    {
        onLogicUpdate?.Invoke();
    }

    /// <summary>
    /// 交给GameManager的Update调用
    /// </summary>
    public void OnUpdate()
    {
#if CLIENT_LOGIC
        //逻辑帧运行时间累加
        totalFrameRuntime += Time.deltaTime;
        //当前逻辑帧累计时间，如果大于下一个逻辑帧开始的时间，就需要更新逻辑帧
        //while循环的另一个作用是控制帧数，保证所有设备的逻辑帧帧数一致性，并进行追帧操作。
        while (totalFrameRuntime > nextFrameTime)
        {
            //更新逻辑帧
            OnFrameUpdate();
            //计算下一个逻辑帧开始的时间
            nextFrameTime += FrameConfig.LogicFrameIntervalS;
            //逻辑帧id自增
            FrameConfig.CurrentFrameID++;
            //DebugMgr.Log("LogicFrameID :" + FrameConfig.CurrentFrameID);
        }
        deltaTime = (totalFrameRuntime + FrameConfig.LogicFrameIntervalS - nextFrameTime) / FrameConfig.LogicFrameIntervalS;
#else
        OnFrameUpdate();
#endif
    }

    public void Clear()
    {
        onLogicUpdate = null;
    }
}
