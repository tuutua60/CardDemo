using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗世界管理器
/// </summary>
public class WorldMgr : BaseManager<WorldMgr>
{
    private HeroController heroController;
    private RoundController roundController;

    private bool isBattle = false;
    public bool IsPlayerWin { get; set; }
    public void Initialize()
    {
        DelayCallMgr.Instance.Initialize();
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        EventCenter.Instance.AddEventListener<BattleResultEvent>(EventConfig.OnBattleEnd, OnBattleEnd);
    }

    private void UnRegisterEvent()
    {
        EventCenter.Instance.RemoveEventListener<BattleResultEvent>(EventConfig.OnBattleEnd, OnBattleEnd);
    }
    private void OnBattleEnd(BattleResultEvent args)
    {
        //DestroyWorld();
        heroController.OnBattleResult();
    }

    /// <summary>
    /// 生成一个战斗世界
    /// </summary>
    /// <param name="playerList">玩家列表</param>
    /// <param name="enemyList">敌人列表</param>
    public void CreateWorld(List<HeroData> playerList, List<HeroData> enemyList, int seed = 0)
    {
        DestroyWorld();
        DebugMgr.Log("创建一个战斗世界");
        RandomMgr.Instance.Initialize(seed);
        //测试
        //RandomMgr.Instance.Initialize(12345678);

        heroController = new HeroController();
        roundController = new RoundController();
        //1.创建所有英雄逻辑对象
        heroController.OnCreate(playerList, enemyList);
        //重置逻辑帧号
        FrameConfig.CurrentFrameID = 0;
        //设置是否战斗标识
        isBattle = true;
        DelayCallMgr.Instance.DelayCall(3500, () =>
        {

            //2.将英雄控制器交给回合控制器 ，并开始战斗循环
            roundController.OnCreate(heroController);
        });
    }

    public SkipMsg GetSkipMsg()
    {
        SkipMsg msg = new SkipMsg();
        msg.isPlayerWin = IsPlayerWin;
        msg.playerNowHp = heroController.GetHeroNowHp(true);
        msg.enemyNowHp = heroController.GetHeroNowHp(false);
        msg.playerNowRage = heroController.GetHeroNowRage(true);
        msg.enemyNowRage = heroController.GetHeroNowRage(false);
        msg.roundCount = roundController.GetCurrentRoundID();

       return msg;
    }

    public void Update()
    {
        if (isBattle) FrameSyncMgr.Instance.OnUpdate();
    }

    public void DestroyWorld()
    {
        isBattle = false;
        //heroController.OnBattleResult();
        heroController?.OnDestroy();
        roundController?.OnDestroy();
        heroController = null;
        roundController = null;
#if CLIENT_LOGIC
        PoolMgr.Instance.Clear();
#endif
        //DebugMgr.Log("帧号 : " + FrameConfig.CurrentFrameID);
    }

    public void Clear()
    {
        UnRegisterEvent();
        DelayCallMgr.Instance.Clear();
        RandomMgr.Instance.Clear();
    }
}
