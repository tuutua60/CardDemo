using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责创建新回合 (RoundLogic)，监听回合结束事件，
/// 并开启下一回合，形成战斗循环
/// </summary>
public class RoundController
{
    private HeroController heroController;
    private RoundLogic currentRound;
    private List<RoundLogic> roundList = new List<RoundLogic>();
    private int roundCount = 0;
    /// <summary>
    /// 开始一个新的回合
    /// </summary>
    private void StartNewRound()
    {
        roundCount++;
        DebugMgr.Log("第 " + roundCount + " 回合开始");
        //得到所有英雄列表
        List<HeroLogic> allHeroList = heroController.AllHeroList;
        //创建新的回合逻辑对象
        currentRound = new RoundLogic(allHeroList, OnRoundEnd);
        //触发回合开始事件
        EventCenter.Instance.EventTrigger<RoundStartEvent>(EventConfig.OnRoundStart, new RoundStartEvent()
        {
            round = roundList.Count+1
        });
        roundList.Add(currentRound);

        currentRound.OnRoundStart();
    }

    private void OnRoundEnd()
    {
        DebugMgr.Log("第 " + roundCount + " 回合结束");
        currentRound = null;
        //检测战斗是否结束
        CheckIsBattleOverEvent checkArgs = new CheckIsBattleOverEvent();
        EventCenter.Instance.EventTrigger<CheckIsBattleOverEvent>(EventConfig.CheckIsBattleOver, checkArgs);
        if (checkArgs.resultState == E_ResultState.Continue)
        {
            //开始新回合
            StartNewRound();
        }
        else
        {
            //战斗结束
            DebugMgr.Log("战斗结束");
            BattleResultEvent args = new BattleResultEvent();
            if (checkArgs.resultState == E_ResultState.PlayerWin) args.isPlayerWin = true;
            else args.isPlayerWin = false;
            WorldMgr.Instance.IsPlayerWin = args.isPlayerWin;
            EventCenter.Instance.EventTrigger<BattleResultEvent>(EventConfig.OnBattleEnd, args);
        }
    }

    public void SkipBattle(SkipEvent args)
    {
        this.roundCount = args.roundCount;
    }

    public void OnCreate(HeroController heroController)
    {
        DebugMgr.Log("创建一个RoundController");
        this.heroController = heroController;
        StartNewRound();
    }

    public void OnDestroy()
    {
        if(heroController!=null) heroController = null;
        if(currentRound!=null)currentRound = null;
        roundList.Clear();
    }
}
