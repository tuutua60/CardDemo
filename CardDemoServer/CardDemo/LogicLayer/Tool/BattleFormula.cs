using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗公式中心：负责处理所有伤害、治疗、暴击等计算
/// </summary>
public static class BattleFormula
{

    /// <summary>
    /// 计算最终物理伤害
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static VInt CalculateFinalPhysicalDamage(HeroLogic attacker,HeroLogic target,SkillConfig config)
    {
        VInt baseDamage = CalculateBasePhysicalDamage(attacker, target, config);
        //根据其他加成计算最终伤害
        //可拓展
        VInt result = baseDamage * config.skillPercentage / 100;
        //计算增伤减伤
        result += result * (attacker.data.changeAtkPercent - target.data.changeDamagePercent ) / 100;
        return result;
    }

    /// <summary>
    /// 计算最终真实伤害
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static VInt CalculateFinalRealDamage(HeroLogic attacker, HeroLogic target, SkillConfig config)
    {
        //真实伤害计算逻辑
        VInt baseDamage = CalculateBaseRealDamage(attacker, target, config);
        //根据其他加成计算最终伤害
        VInt result = baseDamage * config.skillPercentage / 100;
        //可拓展
        //计算增伤减伤
        result += result * (attacker.data.changeAtkPercent - target.data.changeDamagePercent) / 100;
        return result;
    }

    /// <summary>
    /// 计算最终治疗量
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static VInt CalculateFinalRestoreHealth(HeroLogic attacker, HeroLogic target, SkillConfig config)
    {
        //治疗量计算逻辑
        VInt baseRestore = CalculateBaseRestoreHealth(attacker, target, config);
        //根据其他加成计算最终治疗量
        VInt result = baseRestore * config.skillPercentage / 100;
        //可拓展

        return result;
    }

    /// <summary>
    /// 计算Buff伤害
    /// </summary>
    /// <param name="buffOwner"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static VInt CalcBuffDamage(HeroLogic buffOwner,BuffConfig config,int overlayCount)
    {
        VInt result = buffOwner.data.maxHp * config.rate / 100 * overlayCount;
        return result;
    }

    /// <summary>
    /// 计算基础物理伤害
    /// </summary>
    /// <param name="attacker">攻击者</param>
    /// <param name="target">目标者</param>
    /// <param name="config">使用的技能</param>
    /// <returns></returns>
    private static VInt CalculateBasePhysicalDamage(HeroLogic attacker, HeroLogic target, SkillConfig config)
    {
        VInt result = attacker.data.atk * (1 - (target.data.def / (400 + target.data.def)));
        return result;
    }

    /// <summary>
    /// 计算基础真实伤害
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    private static VInt CalculateBaseRealDamage(HeroLogic attacker, HeroLogic target, SkillConfig config)
    {
        VInt result = attacker.data.atk;
        return result;
    }

    /// <summary>
    /// 计算基础治疗量
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    private static VInt CalculateBaseRestoreHealth(HeroLogic attacker, HeroLogic target, SkillConfig config)
    {
        VInt result = attacker.data.atk;
        return result;
    }


}
