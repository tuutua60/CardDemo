using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 逻辑帧接口
/// </summary>
public interface IFrameSyncBehaviour
{
    /// <summary>
    /// 每次逻辑帧更新调用
    /// </summary>
    void OnFrameUpdate();
   
}
