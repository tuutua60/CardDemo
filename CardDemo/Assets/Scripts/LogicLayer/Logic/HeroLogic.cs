using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLogic : BaseLogicObject
{
    /// <summary>
    /// 关联的数据对象
    /// </summary>
    public HeroData data;
    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool IsDead = false;
    /// <summary>
    /// 是否能正常行动
    /// </summary>
    public bool CanAct = true;
    /// <summary>
    /// 逻辑位置
    /// </summary>
    public VInt3 LogicPosition;
    /// <summary>
    /// 出生位置
    /// </summary>
    public Transform BornPosition;
    /// <summary>
    /// Buff列表
    /// </summary>
    public SortedDictionary<int,BuffLogic> buffDic = new SortedDictionary<int, BuffLogic>();

    // -- todo 通过事件中心 将逻辑对象和渲染对象的关联解耦 (已完成)

#if CLIENT_LOGIC
    ///// <summary>
    ///// 关联的渲染对象
    ///// </summary>
    //public HeroRender render;
#endif

    public HeroLogic(HeroData heroData, Transform bornPos = default)
    {
        data = heroData;
        BornPosition = bornPos;
        if (bornPos != null)
            LogicPosition = new VInt3(bornPos.position);
        else
            LogicPosition = VInt3.zero;
    }

    /// <summary>
    /// 受伤
    /// </summary>
    /// <param name="damage"></param>
    public void Wound(VInt damage,bool isPlayAnim = true)
    {
        data.nowHp -= damage.RawInt;
        if(data.nowHp <= 0)
        {
            data.nowHp = 0;
            Dead();
        }
        else
        {
            // -- 受伤动画
            //DebugMgr.Log("英雄 " + data.name + " 受到了 " + damage.RawInt + " 点伤害,当前血量为 :" + data.nowHp);
            PlayAnim("OnHit");
        }
        EventCenter.Instance.EventTrigger<HeroWoundEvent>(EventConfig.HeroWound + data.globalTeamType + data.seatID, new HeroWoundEvent()
        {
            woundHero = this,
            damage = damage,
        });
    }

    /// <summary>
    /// 受到Buff伤害
    /// </summary>
    public void BuffWound(VInt damage)
    {
        DebugMgr.Log($"英雄 {data.name} 受到了 {damage.RawInt} 点Buff伤害,当前血量为 :" + data.nowHp);
        Wound(damage, false);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public void Dead()
    {
        IsDead = true;
        CanAct = false;
        DebugMgr.Log("英雄 " + data.name + " 死亡");
        PlayAnim("Death");
        EventCenter.Instance.EventTrigger(EventConfig.HeroDead + data.globalTeamType + data.seatID);
    }
    /// <summary>
    /// 回血
    /// </summary>
    /// <param name="restore"></param>
    public void Restore(VInt restore)
    {
        data.nowHp += restore.RawInt;
        data.nowHp = Mathf.Clamp(data.nowHp, 0, data.maxHp);
        //DebugMgr.Log($"英雄 {data.name} 恢复了 {restore.RawInt} 点生命，当前血量为 : {data.nowHp}");
        EventCenter.Instance.EventTrigger<HeroRestoreEvent>(EventConfig.HeroRestore + data.globalTeamType + data.seatID, new HeroRestoreEvent()
        {
            restoreHero = this,
            restore = restore,
        });
    }

    /// <summary>
    /// 被冻结
    /// </summary>
    public void Freeze()
    {
        CanAct = false;
        DebugMgr.Log($"英雄 {data.name} 被冰冻");
        PlayAnim("Stop");
    }

    public void UnFreeze()
    {
        CanAct = true;
        DebugMgr.Log($"英雄 {data.name} 解除冰冻");
        PlayAnim("Play");
    }

    /// <summary>
    /// 增加怒气值
    /// </summary>
    /// <param name="rage"></param>
    public void AddRage(int rage)
    {
        data.currentRage += rage;
        data.currentRage = Mathf.Clamp(data.currentRage,0, data.maxRage);
    }

    /// <summary>
    /// 移动到指定位置
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="deltaTime">（毫秒）</param>
    /// <param name="callback">移动完成后的回调</param>
    public void MoveToTarget(VInt3 targetPos,VInt deltaTime,Action callback)
    {
        //逻辑
        EventCenter.Instance.EventTrigger<HeroMoveToTargetEvent>(EventConfig.HeroMoveToTarget+data.globalTeamType + data.seatID, new HeroMoveToTargetEvent()
        {
            hero = this,
            targetPos = targetPos,
            deltaTime = deltaTime
        });
        LogicPosition = targetPos;
        DelayCallMgr.Instance.DelayCall(deltaTime, callback);
    }

    public void PlayAnim(string name)
    {
        //触发播放动画事件
        EventCenter.Instance.EventTrigger<string>(EventConfig.HeroPlayAnim + data.globalTeamType + data.seatID, name);
    }

    /// <summary>
    /// 添加Buff
    /// </summary>
    /// <param name="buffID"></param>
    public void AddBuff(int buffID)
    {
        BuffConfig config = SkillMgr.Instance.GetBuffConfig(buffID);
        // -- todo -- 判断是否成功附加上
        if (RandomMgr.Instance.GetRandomInt(0, 100) < config.percentage)
        {
            //如果不存在该Buff，则创建新的BuffLogic并添加到字典中
            if (!buffDic.ContainsKey(buffID))
            {
                BuffLogic newBuff = new BuffLogic(this, config, () =>
                {
                    buffDic.Remove(buffID);
                    EventCenter.Instance.EventTrigger<SortedDictionary<int, BuffLogic>>(EventConfig.UpdateBuffIcon + data.globalTeamType + data.seatID, buffDic);
                });
                buffDic.Add(buffID, newBuff);
                EventCenter.Instance.EventTrigger<SortedDictionary<int, BuffLogic>>(EventConfig.UpdateBuffIcon + data.globalTeamType + data.seatID, buffDic);
            }
            else
            {
                //加深Buff
                buffDic[buffID].AddOverlay(1);
            }
        }
    }

    public override void OnCreate()
    {
        base.OnCreate();
        //DebugMgr.Log("创建一个HeroLogic，ID为 :"+data.id+"  SeatID为 :"+data.seatID);
#if CLIENT_LOGIC
        ////实例化英雄渲染对象
        //GameObject obj = ResourcesMgr.Instance.Load<GameObject>(AssetDataPath.HeroRenderPath + data.id);
        //render = obj.AddComponent<HeroRender>();
        //render.OnCreate(this);
        EventCenter.Instance.EventTrigger<CreateAssetEvent>(EventConfig.CreateAssetByPool,new CreateHeroRenderEvent()
        {
            assetPath = AssetDataPath.HeroRenderPath + data.id,
            heroLogic = this,
        });
#endif
    }
    public override void OnFrameUpdate()
    {

    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        EventCenter.Instance.EventTrigger<HeroDestroyEvent>(EventConfig.HeroDestroyEvent + data.globalTeamType + data.seatID, new HeroDestroyEvent());
        buffDic.Clear();
    }
}


