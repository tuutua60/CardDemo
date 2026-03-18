using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionLogic
{
    /// <summary>
    /// 行动完成时触发
    /// </summary>
    private Action OnActionComplete;
    /// <summary>
    /// 行动拥有者
    /// </summary>
    private HeroLogic actionOwner;
    /// <summary>
    /// 目标英雄
    /// </summary>
    private List<HeroLogic> targetList;
    /// <summary>
    /// 目标位置
    /// </summary>
    private VInt3 targetPos;
    /// <summary>
    /// 行动使用的技能配置
    /// </summary>
    private SkillConfig config;

    public ActionLogic(HeroLogic heroOwner,SkillConfig skillConfig,Action callback)
    {
        actionOwner = heroOwner;
        this.config = skillConfig;
        this.OnActionComplete += callback;

    }
    /// <summary>
    /// 提供给外部调用，开始行动
    /// </summary>
    public void Start()
    {
        //查找技能目标
        FindTargets();
        //获取目标位置
        if(targetList.Count>0) targetPos = targetList[0].LogicPosition;
        //前摇
        OnActionBefore();
    }

    /// <summary>
    /// 技能前摇
    /// </summary>
    private void OnActionBefore()
    {
        //通知行动前
        EventCenter.Instance.EventTrigger<OnActionStartEvent>(EventConfig.OnActionStart + actionOwner.data.globalTeamType + actionOwner.data.seatID, new OnActionStartEvent());
        if (actionOwner.IsDead)
        {
            EndAction();
            return;
        }
        if (!actionOwner.CanAct)
        {
            DebugMgr.Log($"角色 {actionOwner.data.name} 无法行动，行动被取消");
            DelayCallMgr.Instance.DelayCall(config.shakeBeforeTimeMS+config.shakeAfterTimeMS, EndAction);
            return;
        }
        //播放动画
        PlayAnim(config.animName);
        //根据技能类型处理前摇逻辑
        switch (config.skillType)
        {
            case E_SkillType.MoveSkill:
                actionOwner.MoveToTarget(targetPos,config.shakeBeforeTimeMS,StartAction);
                break;
            case E_SkillType.Chant:
                DelayCallMgr.Instance.DelayCall(config.shakeBeforeTimeMS,StartAction);
                break;
            case E_SkillType.Ballistic:
                DelayCallMgr.Instance.DelayCall(config.shakeBeforeTimeMS,StartAction);
                break;
        }
    }

    /// <summary>
    /// 开始行动
    /// </summary>
    public void StartAction()
    {
        //通知行动开始
        EventCenter.Instance.EventTrigger<OnActionPerformEvent>(EventConfig.OnActionPerform + actionOwner.data.globalTeamType + actionOwner.data.seatID, new OnActionPerformEvent());
        //DebugMgr.Log($"等待了 {config.shakeBeforeTimeMS} 毫秒的前摇");
        DebugMgr.Log($"角色 {actionOwner.data.name} 开始行动。使用ID为 {config.skillID} 的技能");
        //if (!config.isNormalAttack)
        //{
        //    DebugMgr.LogError($"技能ID: {config.skillID} ");
        //}

        //行动逻辑
        if (targetList.Count > 0)
        {
            foreach (var target in targetList)
            {
                //根据技能伤害类型处理伤害逻辑
                switch (config.skillDamageType)
                {
                    case E_SkillDamageType.PhysicalDamage:
                        PhysicalDamageSkill(target);
                        break;
                    case E_SkillDamageType.RealDamage:
                        RealDamageSkill(target);
                        break;
                    case E_SkillDamageType.RestoreHealth:
                        RestoreHealthSkill(target);
                        break;
                    case E_SkillDamageType.NoneDamage:
                        break;
                }
                // -- todo 播放特效等逻辑
                if (config.atkEffectName != "") PlayEffect(actionOwner, AssetDataPath.SkillEffectPath + config.atkEffectName);
                if (config.hitEffectName != "") PlayEffect(target, AssetDataPath.SkillEffectPath + config.hitEffectName);
                //处理附加Buff逻辑
                foreach (var buffID in config.buffArr)
                {
                    for(int i = 0; i < config.damageCount; i++)
                    {
                        target.AddBuff(buffID);
                    }
                }
                //for (int i = 0;i < config.buffArr.Length; i++)
                //{
                //    target.AddBuff(config.buffArr[i]);
                //}
                EventCenter.Instance.EventTrigger<HeroAttackEvent>(EventConfig.HeroAttack + actionOwner.data.globalTeamType + actionOwner.data.seatID, new HeroAttackEvent()
                {
                    atker = actionOwner,
                    target = target,
                });
            }
        }
        else DebugMgr.LogWarning("没有找到技能目标");

        //后摇
        DelayCallMgr.Instance.DelayCall(config.atkDurationMS, OnActionAfter);        
    }

    /// <summary>
    /// 技能后摇
    /// </summary>
    private void OnActionAfter()
    {
        //根据技能类型处理后摇逻辑
        switch (config.skillType)
        {
            case E_SkillType.MoveSkill:
#if CLIENT_LOGIC
                actionOwner.MoveToTarget(new VInt3(actionOwner.BornPosition.position), config.shakeAfterTimeMS, EndAction);
#else
                actionOwner.MoveToTarget(new VInt3(), config.shakeAfterTimeMS, EndAction);
#endif
                break;
            case E_SkillType.Chant:
                DelayCallMgr.Instance.DelayCall(config.shakeAfterTimeMS,EndAction);
                break;
            case E_SkillType.Ballistic:
                DelayCallMgr.Instance.DelayCall(config.shakeAfterTimeMS,EndAction);
                break;
        }  
    }


    /// <summary>
    /// 完成行动并通知外部
    /// </summary>
    private void EndAction()
    {
        //DebugMgr.Log($"等待了 {config.shakeAfterTimeMS} 毫秒的后摇");
        EventCenter.Instance.EventTrigger<OnActionEndEvent>(EventConfig.OnActionEnd, new OnActionEndEvent());
        EventCenter.Instance.EventTrigger<OnActionEndEvent>(EventConfig.OnActionEnd + actionOwner.data.globalTeamType + actionOwner.data.seatID, new OnActionEndEvent());
        //DebugMgr.Log($"角色 {actionOwner.data.name} 行动完成。");
        /* --测试能否正常释放技能-- */
        if (actionOwner.CanAct&& actionOwner.data.currentRage >= actionOwner.data.maxRage)
        {
            actionOwner.AddRage(-actionOwner.data.maxRage);
            SkillConfig skill = SkillMgr.Instance.GetSkillConfig(actionOwner.data.id, false);
            this.config = skill;
            Start();
        }
        else
        {
            // 触发完成事件
            OnActionComplete?.Invoke();
        }
        /* ---- */
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    private void PlayAnim(string name)
    {
        //DebugMgr.Log(actionOwner.data.name+"  播放动画");
        //技能前摇阶段播放动画
        actionOwner.PlayAnim(name);
    }

    /// <summary>
    /// 创建特效
    /// </summary>
    /// <param name="hero"></param>
    /// <param name="effectPath"></param>
    private void PlayEffect(HeroLogic hero, string effectPath)
    {
        SkillMgr.Instance.CreateEffect(hero, effectPath);
    }

    /// <summary>
    /// 物理伤害技能
    /// </summary>
    /// <param name="target"></param>
    private void PhysicalDamageSkill(HeroLogic target)
    {
        //根据技能伤害次数逐次计算伤害
        VInt damage = BattleFormula.CalculateFinalPhysicalDamage(actionOwner, target, config);
        for (int i = 0; i < config.damageCount; i++)
        {
            //如果目标已死亡则跳出循环
            if (target.IsDead) break;
            DebugMgr.Log(actionOwner.data.name + "  对  " + target.data.name + " 造成了 " + damage + " 点伤害," + "当前生命值为" + target.data.nowHp);
            //受伤
            target.Wound(damage);
        }

        //增加怒气
        if (config.isNormalAttack) actionOwner.AddRage(actionOwner.data.atkRage);
        if (damage > 0) target.AddRage(target.data.takeDamageRage);
    }

    /// <summary>
    /// 真实伤害技能
    /// </summary>
    /// <param name="target"></param>
    private void RealDamageSkill(HeroLogic target)
    {
        // -- todo 真实伤害技能逻辑
        VInt damage = BattleFormula.CalculateFinalRealDamage(actionOwner, target, config);
        for (int i = 0; i < config.damageCount; i++)
        {
            //如果目标已死亡则跳出循环
            if (target.IsDead) break;
            DebugMgr.Log(actionOwner.data.name + "  对  " + target.data.name + " 造成了 " + damage + " 点伤害," + "当前生命值为" + target.data.nowHp);
            //受伤
            target.Wound(damage);
        }

        //增加怒气
        if (config.isNormalAttack) actionOwner.AddRage(actionOwner.data.atkRage);
        if (damage > 0) target.AddRage(target.data.takeDamageRage);
    }

    /// <summary>
    /// 恢复生命技能
    /// </summary>
    /// <param name="target"></param>
    private void RestoreHealthSkill(HeroLogic target)
    {
        // -- todo 治疗技能逻辑
        VInt restore = BattleFormula.CalculateFinalRestoreHealth(actionOwner, target, config);
        for (int i = 0; i < config.damageCount; i++)
        {
            DebugMgr.Log(actionOwner.data.name + "  为  " + target.data.name + " 恢复了 " + restore + " 点生命," + "当前生命值为" + target.data.nowHp);
            //回血
            target.Restore(restore);

        }
    }

    /// <summary>
    /// 根据技能配置查找目标
    /// </summary>
    private void FindTargets()
    {
        //初始化事件参数
        GetTargetEvent args = new GetTargetEvent
        {
            owner = actionOwner,
            teamType = config.teamType,
            targetType = config.targetType,
            //这个targetList是用来接收返回值的
            targetList = new List<HeroLogic>()
        };
        switch (config.targetType)
        {
            //单体目标
            case E_TargetType.SingleTarget:
                //触发事件
                EventCenter.Instance.EventTrigger<GetTargetEvent>(EventConfig.GetSingleTarget, args);
                break;
            //前排目标
            case E_TargetType.FrontRowTarget:
                EventCenter.Instance.EventTrigger<GetTargetEvent>(EventConfig.GetFrontRowTarget, args);
                break;
            //后排目标
            case E_TargetType.BackRowTarget:
                EventCenter.Instance.EventTrigger<GetTargetEvent>(EventConfig.GetBackRowTarget, args);
                break;
            //群体目标
            case E_TargetType.GroupTarget:
                EventCenter.Instance.EventTrigger<GetTargetEvent>(EventConfig.GetGroupTarget, args);
                break;
        }
        this.targetList = args.targetList;
    }

    public void OnDestroy()
    {
        OnActionComplete = null;
    }
}
