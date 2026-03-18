using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能配置
/// </summary>
#if CLIENT_LOGIC
[CreateAssetMenu(fileName ="SkillConfig",menuName ="Config/SkillConfig",order =0)]
public class SkillConfig : ScriptableObject
#else
public class SkillConfig
#endif
{
    /// <summary>
    /// 技能图标
    /// </summary>
    public Sprite skillIcon;
    /// <summary>
    /// 技能ID
    /// </summary>
    public int skillID;
    /// <summary>
    /// 技能名
    /// </summary>
    public string skillName;
    /// <summary>
    /// 技能描述
    /// </summary>
    public string skillDes;
    /// <summary>
    /// 技能前摇（毫秒）
    /// </summary>
    public int shakeBeforeTimeMS = 500;
    /// <summary>
    /// 技能持续世界（毫秒）
    /// </summary>
    public int atkDurationMS = 500;
    /// <summary>
    /// 技能后摇（毫秒）
    /// </summary>
    public int shakeAfterTimeMS = 500;
    /// <summary>
    /// 是否是普通攻击
    /// </summary>
    public bool isNormalAttack = true;
    /// <summary>
    /// 技能伤害次数
    /// </summary>
    public int damageCount = 1;
    /// <summary>
    /// 所需怒气值
    /// </summary>
    public int needRageValue = 100;
    /// <summary>
    /// 技能类型
    /// </summary>
    public E_SkillType skillType;
    /// <summary>
    /// 技能目标类型
    /// </summary>
    public E_TargetType targetType;
    /// <summary>
    /// 技能作用队伍类型
    /// </summary>
    public E_TeamType teamType;
    /// <summary>
    /// 技能伤害类型
    /// </summary>
    public E_SkillDamageType skillDamageType;
    /// <summary>
    /// 技能倍率百分比
    /// </summary>
    public int skillPercentage = 100;
    /// <summary>
    /// 子弹资源路径（如果需要的话）
    /// </summary>
    public string bulletPath;
    /// <summary>
    /// 动画名
    /// </summary>
    public string animName;
    /// <summary>
    /// 音效名
    /// </summary>
    public string audioName;
    /// <summary>
    /// 技能特效
    /// </summary>
    public string atkEffectName;
    /// <summary>
    /// 受到该技能的目标特效
    /// </summary>
    public string hitEffectName;
    /// <summary>
    /// 技能附加Buff
    /// </summary>
    public int[] buffArr;
}
