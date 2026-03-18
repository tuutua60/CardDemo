using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffLogic
{
    public BuffConfig config;

    public HeroLogic buffOwner;

    private Action onRemove;

    /// <summary>
    /// 当前叠加层数
    /// </summary>
    private int currentOverlayCount;
    /// <summary>
    /// 还需的持续回合数
    /// </summary>
    private int continueRoundCount;
    /// <summary>
    /// 是否是Buff创建的第一回合
    /// </summary>
    private bool isFirstRound;

    /// <summary>
    /// 由于Buff导致英雄变化的数值大小
    /// </summary>
    private VInt result;

    public BuffLogic(HeroLogic hero,BuffConfig config,Action onRemove)
    {
        this.buffOwner = hero;
        this.config = config;
        this.onRemove = onRemove;
        this.isFirstRound = true;
        currentOverlayCount = 1;
        continueRoundCount = config.durationRoundCount;
        CreateEffect();
        AddListener();
    }

    private void ChangeIsFirstRound(OnActionEndEvent args)
    {
        isFirstRound = false;
    }

    /// <summary>
    /// 创建特效
    /// </summary>
    private void CreateEffect()
    {
#if CLIENT_LOGIC
        if (config.buffEffectName == "") return;
        EventCenter.Instance.EventTrigger<GetRenderEvent>(EventConfig.GetHeroRender + buffOwner.data.globalTeamType + buffOwner.data.seatID, new GetRenderEvent()
        {
            callback = (baseRender) =>
            {
                //PoolMgr.Instance.GetObject(AssetDataPath.BuffEffectPath + config.buffEffectName, (o) =>
                //{
                //    effect = o.GetComponent<BuffEffectRender>();
                //    effect.Initialize(baseRender as HeroRender);
                //});
                EventCenter.Instance.EventTrigger<CreateAssetEvent>(EventConfig.CreateAssetByPoolAsync, new CreateBuffEffectRenderEvent()
                {
                    assetPath = AssetDataPath.BuffEffectPath + config.buffEffectName,
                    buffLogic = this,
                    heroRender = baseRender as HeroRender,
                });
            }
        });
#endif
    }

    /// <summary>
    /// 根据Buff触发类型，监听不同的事件
    /// </summary>
    private void AddListener()
    {
        switch (config.triggerType)
        {
            case E_BuffTriggerType.Now:
                TriggerBuff();
                break;
            case E_BuffTriggerType.OnActionStart:
                EventCenter.Instance.AddEventListener<OnActionStartEvent>(EventConfig.OnActionStart + buffOwner.data.globalTeamType +buffOwner.data.seatID, TriggerBuff);
                break;
            case E_BuffTriggerType.OnActionPerform:
                EventCenter.Instance.AddEventListener<OnActionPerformEvent>(EventConfig.OnActionPerform + buffOwner.data.globalTeamType + buffOwner.data.seatID, TriggerBuff);
                break;
            case E_BuffTriggerType.OnActionEnd:
                EventCenter.Instance.AddEventListener<OnActionEndEvent>(EventConfig.OnActionEnd + buffOwner.data.globalTeamType + buffOwner.data.seatID, TriggerBuff);
                break;
        }
        EventCenter.Instance.AddOneTimeEventListener<OnActionEndEvent>(EventConfig.OnActionEnd, ChangeIsFirstRound);
        EventCenter.Instance.AddEventListener<OnActionEndEvent>(EventConfig.OnActionEnd + buffOwner.data.globalTeamType + buffOwner.data.seatID, ReduceRound);
        //EventCenter.Instance.AddEventListener(EventConfig.HeroDead + buffOwner.data.globalTeamType + buffOwner.data.seatID, RemoveBuff);
        EventCenter.Instance.AddEventListener<HeroDestroyEvent>(EventConfig.HeroDestroyEvent + buffOwner.data.globalTeamType + buffOwner.data.seatID, RemoveBuff);

    }

    private void RemoveListener()
    {
        switch (config.triggerType)
        {
            case E_BuffTriggerType.OnActionStart:
                EventCenter.Instance.RemoveEventListener<OnActionStartEvent>(EventConfig.OnActionStart + buffOwner.data.globalTeamType + buffOwner.data.seatID, TriggerBuff);
                break;
            case E_BuffTriggerType.OnActionPerform:
                EventCenter.Instance.RemoveEventListener<OnActionPerformEvent>(EventConfig.OnActionPerform + buffOwner.data.globalTeamType + buffOwner.data.seatID, TriggerBuff);
                break;
            case E_BuffTriggerType.OnActionEnd:
                EventCenter.Instance.RemoveEventListener<OnActionEndEvent>(EventConfig.OnActionEnd + buffOwner.data.globalTeamType + buffOwner.data.seatID, TriggerBuff);
                break;
        }
        EventCenter.Instance.RemoveEventListener<OnActionEndEvent>(EventConfig.OnActionEnd + buffOwner.data.globalTeamType + buffOwner.data.seatID, ReduceRound);
        //EventCenter.Instance.RemoveEventListener(EventConfig.HeroDead + buffOwner.data.globalTeamType + buffOwner.data.seatID, RemoveBuff);
        EventCenter.Instance.RemoveEventListener<HeroDestroyEvent>(EventConfig.HeroDestroyEvent + buffOwner.data.globalTeamType + buffOwner.data.seatID, RemoveBuff);

    }

    /// <summary>
    /// 尝试加深Buff
    /// </summary>
    /// <param name="count"></param>
    public void AddOverlay(int count = 1)
    {
        //加深Buff
        currentOverlayCount += count;
        currentOverlayCount = Mathf.Clamp(currentOverlayCount, 0, config.maxOverlayCount);
        //刷新持续回合数
        continueRoundCount = config.durationRoundCount;
        isFirstRound = true;
    }

    /// <summary>
    /// 减少持续回合数
    /// </summary>
    public void ReduceRound(OnActionEndEvent args)
    {
        if (isFirstRound)
        {
            isFirstRound = false;
            return;
        }
         continueRoundCount--;
        if (continueRoundCount <= 0)
        {
            // Buff效果结束
            //移除Buff
            RemoveBuff(null);
        }
    }

    #region 触发相关
    /// <summary>
    /// 触发Buff效果
    /// </summary>
    private void TriggerBuff()
    {
        TriggerBuff(null);
    }

    /// <summary>
    /// 触发Buff效果
    /// </summary>
    private void TriggerBuff(ActionStateEvent args)
    {
        //触发Buff
        //DebugMgr.Log(buffOwner.data.name + " 触发Buff，ID为  " + config.buffID);
        switch (config.buffType)
        {
            case E_BuffType.Buff:
                HandleTriggerBuff();
                break;
            case E_BuffType.DEBuff:
                HandleTriggerDEBuff();
                break;
            case E_BuffType.Damage:
                HandleTriggerDamageBuff();
                break;
            case E_BuffType.Control:
                HandleTriggerControlBuff();
                break;
        }
    }

    private void HandleTriggerBuff()
    {
        switch (config.buffDetailType)
        {
            //增加攻击力
            case E_BuffDetailType.UpAtk:
                // 先恢复之前的数值，再计算新的增益
                buffOwner.data.atk -= result.RawInt;
                result = buffOwner.data.atk * new VInt(config.rate/100f * currentOverlayCount);
                buffOwner.data.atk += result.RawInt;
                DebugMgr.Log(buffOwner.data.name + "被提升了" + (config.rate * currentOverlayCount) + "%攻击力" + ",当前攻击力为" + buffOwner.data.atk);
                break;
            //减少受到的伤害
            case E_BuffDetailType.LowDamage:
                DebugMgr.Log(buffOwner.data.name + "被减少了" + (config.rate * currentOverlayCount) + "%受到的伤害");
                // 先恢复之前的数值，再计算新的增益
                buffOwner.data.changeDamagePercent -= result.RawInt;
                result = new VInt(config.rate * currentOverlayCount);
                buffOwner.data.changeDamagePercent += result.RawInt;
                break;
        }
    }

    private void HandleTriggerDEBuff()
    {
        switch (config.dEBuffDetailType)
        {
            //降低速度
            case E_DEBuffDetailType.LowSpeed:
                // 先恢复之前的数值，再计算新的减益
                buffOwner.data.agl += result.RawInt;
                result = buffOwner.data.agl * new VInt(config.rate/100f * currentOverlayCount);
                buffOwner.data.agl -= result.RawInt;
                DebugMgr.Log(buffOwner.data.name + "被减速了 " + (config.rate * currentOverlayCount) + "%"+"  当前敏捷为 " + buffOwner.data.agl);
                break;
        }
    }

    private void HandleTriggerDamageBuff()
    {
        switch (config.damageBuffDetailType)
        {
            //燃烧状态
            case E_DamageBuffDetailType.Fire:
                result = BattleFormula.CalcBuffDamage(buffOwner, config,currentOverlayCount);
                DebugMgr.Log(buffOwner.data.name + " 执行燃烧伤害，层数为 "+ currentOverlayCount);
                buffOwner.BuffWound(result);
                break;
        }
    }

    private void HandleTriggerControlBuff()
    {
        switch (config.controlBuffDetailType)
        {
            //冻结状态
            case E_ControlBuffDetailType.Freeze:
                buffOwner.Freeze();
                break;
        }
    }
    #endregion

    #region 移除相关
    /// <summary>
    /// 直接移除Buff
    /// </summary>
    public void RemoveBuff(HeroDestroyEvent args)
    {
        switch (config.buffType)
        {
            case E_BuffType.Buff:
                HandleRemoveBuff();
                break;
            case E_BuffType.DEBuff:
                HandleRemoveDEBuff();
                break;
            case E_BuffType.Damage:
                HandleRemoveDamageBuff();
                break;
            case E_BuffType.Control:
                HandleRemoveControlBuff();
                break;
        }
        EventCenter.Instance.EventTrigger<RemoveBuffEvent>(EventConfig.RemoveBuff+buffOwner.data.globalTeamType+
                                                           buffOwner.data.seatID+config.buffID, new RemoveBuffEvent());
        OnRemoveBuff();
    }
    private void HandleRemoveBuff()
    {
        switch (config.buffDetailType)
        {
            case E_BuffDetailType.None:
                break;
            case E_BuffDetailType.UpAtk:
                DebugMgr.Log(buffOwner.data.name + "的攻击力增加效果结束");
                buffOwner.data.atk -= result.RawInt;
                break;
            case E_BuffDetailType.LowDamage:
                DebugMgr.Log(buffOwner.data.name + "的减伤效果结束");
                buffOwner.data.changeDamagePercent -= result.RawInt;
                break;
        }
    }
    private void HandleRemoveDEBuff()
    {
        switch (config.dEBuffDetailType)
        {
            case E_DEBuffDetailType.LowSpeed:
                DebugMgr.Log(buffOwner.data.name + "的减速效果结束");
                buffOwner.data.agl += result.RawInt;
                result = 0;
                break;
        }
    }
    private void HandleRemoveDamageBuff()
    {
        switch (config.damageBuffDetailType)
        {
            case E_DamageBuffDetailType.Fire:
                DebugMgr.Log(buffOwner.data.name + "的燃烧效果结束");
                result = 0;
                break;
        }
    }
    private void HandleRemoveControlBuff()
    {
        switch (config.controlBuffDetailType)
        {
            //冻结结束
            case E_ControlBuffDetailType.Freeze:
                buffOwner.UnFreeze();
                result = 0;
                break;
        }
    }
    #endregion

    /// <summary>
    /// 移除Buff时调用
    /// </summary>
    private void OnRemoveBuff()
    {
        onRemove?.Invoke();
        RemoveListener();
    }
}


