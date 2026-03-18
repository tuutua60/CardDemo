using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能类型
/// </summary>
public enum E_SkillType
{
    /// <summary>
    /// 移动型技能
    /// </summary>
    MoveSkill,
    /// <summary>
    /// 吟唱型技能
    /// </summary>
    Chant,
    /// <summary>
    /// 弹道型技能
    /// </summary>
    Ballistic,
}

/// <summary>
/// 技能伤害类型
/// </summary>
public enum E_SkillDamageType
{
    /// <summary>
    /// 物理伤害
    /// </summary>
    PhysicalDamage,
    ///// <summary>
    ///// 法术伤害
    ///// </summary>
    //MagicalDamage,
    /// <summary>
    /// 真实伤害
    /// </summary>
    RealDamage,
    /// <summary>
    /// 恢复生命
    /// </summary>
    RestoreHealth,
    /// <summary>
    /// 无伤害
    /// </summary>
    NoneDamage,
}
/// <summary>
/// 目标类型（通用）
/// </summary>
public enum E_TargetType
{
    /// <summary>
    /// 单体目标
    /// </summary>
    SingleTarget,
    /// <summary>
    /// 前排目标
    /// </summary>
    FrontRowTarget,
    /// <summary>
    /// 后排目标
    /// </summary>
    BackRowTarget,
    /// <summary>
    /// 群体目标
    /// </summary>
    GroupTarget,
}

/// <summary>
/// 作用队伍类型（通用）
/// </summary>
public enum E_TeamType
{
    /// <summary>
    /// 敌方
    /// </summary>
    Enemy,
    /// <summary>
    /// 友方
    /// </summary>
    Friend,
}

/// <summary>
/// Buff类型(泛)
/// </summary>
public enum E_BuffType
{
    //增益
    Buff,
    //减益
    DEBuff,
    //伤害型
    Damage,
    //控制型
    Control,
}

/// <summary>
/// Buff触发时机
/// </summary>
public enum E_BuffTriggerType
{
    Now,
    OnActionStart,
    OnActionPerform,
    OnActionEnd,
}

public enum E_BuffDetailType
{
    None,
    /// <summary>
    /// 增加攻击力
    /// </summary>
    UpAtk,
    /// <summary>
    /// 伤害减免
    /// </summary>
    LowDamage,
}

public enum E_DEBuffDetailType
{
    None,
    /// <summary>
    /// 降低速度
    /// </summary>
    LowSpeed,
}

public enum E_DamageBuffDetailType
{
    None,
    /// <summary>
    /// 燃烧
    /// </summary>
    Fire,
}

public enum E_ControlBuffDetailType
{
    None,
    /// <summary>
    /// 冻结
    /// </summary>
    Freeze,
}