using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroRender : BaseRenderObject
{
    public HeroLogic hero;

    private Animator animator;

    private HPHUDRender hPHUDRender;

    public void OnCreate(HeroLogic hero)
    {
        hPHUDRender = null;
        this.hero = hero;
        CreateHUDRender();
        Initialize();
    }

    private void Initialize()
    {
        //初始化位置
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        this.transform.SetParent(hero.BornPosition,false);
        //找到对应组件
        animator = this.transform.GetChild(0).GetChild(0).GetComponent<Animator>();

        // 解决方案：在初始化时，强制重置 Animator 状态机
        // 这会使 Animator 跳转到默认状态（通常是 Idle），并清除所有触发器
        if (animator != null)
        {
            animator.Play(0, -1, 0f);
        }
        RegisterEvent();
    }

    private void GetRender(GetRenderEvent args)
    {
        args.callback?.Invoke(this);
    }

    /// <summary>
    /// 创建血条HUD
    /// </summary>
    private void CreateHUDRender()
    {
        if (hero.data.globalTeamType == E_GlobalTeamType.Player)
        {
            hPHUDRender = PoolMgr.Instance.GetObject(AssetDataPath.HUDPath + "HPObjectPlayer").GetComponent<HPHUDRender>();
            //DebugMgr.LogError(hero.data.id+"血条生成");
            hPHUDRender.Initialize(this);
        }
        else if(hero.data.globalTeamType == E_GlobalTeamType.Enemy)
        {
            hPHUDRender = PoolMgr.Instance.GetObject(AssetDataPath.HUDPath + "HPObjectEnemy").GetComponent<HPHUDRender>();
            hPHUDRender.Initialize(this);
        }
    }

    /// <summary>
    /// 移动到目标位置
    /// </summary>
    /// <param name="args"></param>
    private void MoveToTarget(HeroMoveToTargetEvent args)
    {
        //DebugMgr.Log($"英雄 {hero.data.name} 移动到目标位置 {args.targetPos} ，耗时 {args.deltaTime.RawInt} 毫秒");
        this.transform.DOMove(args.targetPos.vec3,args.deltaTime.RawInt/1000f);
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="name"></param>
    private void PlayAnim(string name)
    {
        if (name == "Stop") animator.speed = 0;
        else if (name == "Play") animator.speed = 1;
        else animator.SetTrigger(name);
    }

    /// <summary>
    /// 英雄受伤表现
    /// </summary>
    /// <param name="args"></param>
    private void OnWound(HeroWoundEvent args)
    {
        //更新血条
        hPHUDRender.UpdateHPSlider((float)args.woundHero.data.nowHp/args.woundHero.data.maxHp);
        //更新怒气
        int tempRage = hero.data.currentRage + hero.data.takeDamageRage;
        tempRage = Mathf.Clamp(tempRage, 0, hero.data.maxRage);
        hPHUDRender.UpdateRageSlider((float)tempRage/args.woundHero.data.maxRage);
        //显示伤害数字
        UIManager.Instance.GetPanel<HUDPanel>().OnHeroWound(args);
    }
    /// <summary>
    /// 英雄回血表现
    /// </summary>
    /// <param name="args"></param>
    private void OnRestore(HeroRestoreEvent args)
    {
        //更新血条
        hPHUDRender.UpdateHPSlider((float)args.restoreHero.data.nowHp / args.restoreHero.data.maxHp);

        //显示回血数字
        UIManager.Instance.GetPanel<HUDPanel>().OnHeroRestore(args);
    }

    /// <summary>
    /// 英雄攻击表现
    /// </summary>
    /// <param name="args"></param>
    private void OnAttack(HeroAttackEvent args)
    {
        hPHUDRender.UpdateRageSlider((float)args.atker.data.currentRage/args.atker.data.maxRage);
    }

    /// <summary>
    /// 英雄更新Buff图标
    /// </summary>
    private void OnUpdateBuff(SortedDictionary<int,BuffLogic>buffDic)
    {
        hPHUDRender.UpdateBuffIcon(buffDic);
    }

    private void RegisterEvent()
    {
        //获得渲染对象
        EventCenter.Instance.AddEventListener<GetRenderEvent>(EventConfig.GetHeroRender + hero.data.globalTeamType + hero.data.seatID,GetRender);
        //移动到目标位置
        EventCenter.Instance.AddEventListener<HeroMoveToTargetEvent>(EventConfig.HeroMoveToTarget + hero.data.globalTeamType +hero.data.seatID, MoveToTarget);
        //播放动画
        EventCenter.Instance.AddEventListener<string>(EventConfig.HeroPlayAnim + hero.data.globalTeamType + hero.data.seatID, PlayAnim);
        //受伤
        EventCenter.Instance.AddEventListener<HeroWoundEvent>(EventConfig.HeroWound + hero.data.globalTeamType + hero.data.seatID, OnWound);
        //攻击
        EventCenter.Instance.AddEventListener<HeroAttackEvent>(EventConfig.HeroAttack + hero.data.globalTeamType + hero.data.seatID, OnAttack);
        //回血
        EventCenter.Instance.AddEventListener<HeroRestoreEvent>(EventConfig.HeroRestore + hero.data.globalTeamType + hero.data.seatID, OnRestore);
        //更新Buff图标
        EventCenter.Instance.AddEventListener<SortedDictionary<int,BuffLogic>>(EventConfig.UpdateBuffIcon + hero.data.globalTeamType + hero.data.seatID, OnUpdateBuff);
        //英雄销毁
        EventCenter.Instance.AddEventListener<HeroDestroyEvent>(EventConfig.HeroDestroyEvent + hero.data.globalTeamType + hero.data.seatID, Clear);
    }

    private void UnregisterEvent()
    {
        EventCenter.Instance.RemoveEventListener<GetRenderEvent>(EventConfig.GetHeroRender + hero.data.globalTeamType + hero.data.seatID, GetRender);
        EventCenter.Instance.RemoveEventListener<HeroMoveToTargetEvent>(EventConfig.HeroMoveToTarget + hero.data.globalTeamType + hero.data.seatID, MoveToTarget);
        EventCenter.Instance.RemoveEventListener<string>(EventConfig.HeroPlayAnim + hero.data.globalTeamType + hero.data.seatID, PlayAnim);
        EventCenter.Instance.RemoveEventListener<HeroWoundEvent>(EventConfig.HeroWound + hero.data.globalTeamType + hero.data.seatID, OnWound);
        EventCenter.Instance.RemoveEventListener<HeroAttackEvent>(EventConfig.HeroAttack + hero.data.globalTeamType + hero.data.seatID, OnAttack);
        EventCenter.Instance.RemoveEventListener<HeroRestoreEvent>(EventConfig.HeroRestore + hero.data.globalTeamType + hero.data.seatID, OnRestore);
        EventCenter.Instance.RemoveEventListener<SortedDictionary<int, BuffLogic>>(EventConfig.UpdateBuffIcon + hero.data.globalTeamType + hero.data.seatID, OnUpdateBuff);
        EventCenter.Instance.RemoveEventListener<HeroDestroyEvent>(EventConfig.HeroDestroyEvent + hero.data.globalTeamType + hero.data.seatID, Clear);

    }

    public void Clear(HeroDestroyEvent args)
    {
        if (hPHUDRender != null) hPHUDRender.Clear();
        UnregisterEvent();
        PoolMgr.Instance.PushObject(this.gameObject.name, this.gameObject);
    }
}
