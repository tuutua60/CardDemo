using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMoveToTargetEvent
{
    /// <summary>
    /// 要移动的英雄
    /// </summary>
    public HeroLogic hero;
    /// <summary>
    /// 目标位置
    /// </summary>
    public VInt3 targetPos;
    /// <summary>
    /// 时间（毫秒）
    /// </summary>
    public VInt deltaTime;
}
