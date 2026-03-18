using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 逻辑帧配置
/// </summary>
public static class FrameConfig
{
    /// <summary>
    /// 帧间隔（秒）
    /// </summary>
    public const float LogicFrameIntervalS = 0.066f;
    /// <summary>
    /// 帧间隔（毫秒）
    /// </summary>
    public const int LogicFrameIntervalMS = 66;
    /// <summary>
    /// 当前帧号
    /// </summary>
    public static int CurrentFrameID = 0;
}
