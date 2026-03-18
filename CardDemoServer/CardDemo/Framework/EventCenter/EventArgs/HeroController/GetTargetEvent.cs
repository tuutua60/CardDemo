using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 获取目标事件参数
/// </summary>
public class GetTargetEvent
{
    /// <summary>
    /// 拥有者
    /// </summary>
    public HeroLogic owner;
    /// <summary>
    /// 队伍类型
    /// </summary>
    public E_TeamType teamType;
    /// <summary>
    /// 目标类型
    /// </summary>
    public E_TargetType targetType;

    //返回值
    public List<HeroLogic> targetList;
}
