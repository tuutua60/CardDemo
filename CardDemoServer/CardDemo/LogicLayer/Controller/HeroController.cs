using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



public class HeroController
{
    private List<HeroLogic> playerList = new List<HeroLogic>();
    private List<HeroLogic> enemyList = new List<HeroLogic>();
    private List<HeroLogic> allHeroList = new List<HeroLogic>();
    public List<HeroLogic> PlayerList => playerList;
    public List<HeroLogic> EnemyList => enemyList;
    public List<HeroLogic> AllHeroList => allHeroList;


    public void OnCreate(List<HeroData> playerList, List<HeroData> enemyList)
    {
        DebugMgr.Log("创建一个HeroController");
        CreateHeroLogicList(playerList, this.playerList,E_GlobalTeamType.Player);
        CreateHeroLogicList(enemyList, this.enemyList,E_GlobalTeamType.Enemy);
        RegisterEvents();
    }

    public void CreateHeroLogicList(List<HeroData> heroDataList, List<HeroLogic> heroLogicList,E_GlobalTeamType globalTeam)
    {
#if CLIENT_LOGIC
        List<Transform> posList = (globalTeam == E_GlobalTeamType.Player) ? BattleWorldNodes.Instance.playerPosList : BattleWorldNodes.Instance.enemyPosList;
        int count = 0;
        foreach (var heroData in heroDataList)
        {
            var heroLogic = new HeroLogic(heroData, posList[count++]);
            heroLogic.OnCreate();
            heroLogicList.Add(heroLogic);
            allHeroList.Add(heroLogic);
        }

#else
        foreach (var heroData in heroDataList)
        {
            var heroLogic = new HeroLogic(heroData);
            heroLogic.OnCreate();
            heroLogicList.Add(heroLogic);
            allHeroList.Add(heroLogic);
        }
#endif
    }

    public void OnDestroy()
    {
        UnregisterEvents();
        foreach (var player in playerList)
        {
            player.OnDestroy();
        }
        playerList.Clear();
        foreach (var enemy in enemyList)
        {
            enemy.OnDestroy();
        }
        enemyList.Clear();
        allHeroList.Clear();
    }
    
    /// <summary>
    /// 获取单体目标
    /// </summary>
    /// <param name="args"></param>
    private void GetSingleTarget(GetTargetEvent args)
    {        
        // 确定要搜索的友方和敌方列表
        List<HeroLogic> friendList = (args.owner.data.globalTeamType == E_GlobalTeamType.Player) ? playerList : enemyList;
        List<HeroLogic> opposingList = (args.owner.data.globalTeamType == E_GlobalTeamType.Player) ? enemyList : playerList;

        // 根据技能目标类型（友方或敌方）选择正确的列表进行搜索
        List<HeroLogic> searchList = (args.teamType == E_TeamType.Friend) ? friendList : opposingList;

        // 从选定的列表中找到第一个存活的目标
        if (searchList != null)
        {
            for (int i = 0; i < searchList.Count; i++)
            {
                if (!searchList[i].IsDead)
                {
                    args.targetList.Add(searchList[i]);
                    // 找到一个就够了
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 获得前排目标
    /// </summary>
    /// <param name="args"></param>
    private void GetFrontRowTarget(GetTargetEvent args)
    {
        // 确定要搜索的友方和敌方列表
        List<HeroLogic> friendList = (args.owner.data.globalTeamType == E_GlobalTeamType.Player) ? playerList : enemyList;
        List<HeroLogic> opposingList = (args.owner.data.globalTeamType == E_GlobalTeamType.Player) ? enemyList : playerList;

        // 根据技能目标类型（友方或敌方）选择正确的列表进行搜索
        List<HeroLogic> searchList = (args.teamType == E_TeamType.Friend) ? friendList : opposingList;

        if (searchList != null)
        {
            // 假设前排是索引0 1 2
            for (int i = 0; i < 3; i++)
            {
                if (!searchList[i].IsDead)
                {
                    args.targetList.Add(searchList[i]);
                }
            }
            if (args.targetList.Count == 0)
            {
                // 如果前排没有存活的目标，则选择后排
                for (int i = 3; i < searchList.Count; i++)
                {
                    if (!searchList[i].IsDead)
                    {
                        args.targetList.Add(searchList[i]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获得后排目标
    /// </summary>
    /// <param name="args"></param>
    private void GetBackRowTarget(GetTargetEvent args)
    {
        // 确定要搜索的友方和敌方列表
        List<HeroLogic> friendList = (args.owner.data.globalTeamType == E_GlobalTeamType.Player) ? playerList : enemyList;
        List<HeroLogic> opposingList = (args.owner.data.globalTeamType == E_GlobalTeamType.Player) ? enemyList : playerList;

        // 根据技能目标类型（友方或敌方）选择正确的列表进行搜索
        List<HeroLogic> searchList = (args.teamType == E_TeamType.Friend) ? friendList : opposingList;

        if (searchList != null)
        {
            // 假设后排是索引3 4
            for (int i = 3; i < searchList.Count; i++)
            {
                if (!searchList[i].IsDead)
                {
                    args.targetList.Add(searchList[i]);
                }
            }
            if (args.targetList.Count == 0)
            {
                // 如果后排没有存活的目标，则选择前排
                for (int i = 0; i < 3; i++)
                {
                    if (!searchList[i].IsDead)
                    {
                        args.targetList.Add(searchList[i]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获得群体目标
    /// </summary>
    /// <param name="args"></param>
    private void GetGroupTarget(GetTargetEvent args)
    {
        // 确定要搜索的友方和敌方列表
        List<HeroLogic> friendList = (args.owner.data.globalTeamType == E_GlobalTeamType.Player) ? playerList : enemyList;
        List<HeroLogic> opposingList = (args.owner.data.globalTeamType == E_GlobalTeamType.Player) ? enemyList : playerList;

        // 根据技能目标类型（友方或敌方）选择正确的列表进行搜索
        List<HeroLogic> searchList = (args.teamType == E_TeamType.Friend) ? friendList : opposingList;

        if (searchList != null)
        {
            for (int i = 0; i < searchList.Count; i++)
            {
                if (!searchList[i].IsDead)
                {
                    args.targetList.Add(searchList[i]);
                }
            }
        }
    }

    /// <summary>
    /// 判断战斗是否结束，并返回结果
    /// </summary>
    /// <returns>包含战斗结果的事件对象</returns>
    public void CheckIsBattleOver(CheckIsBattleOverEvent args)
    {
        args.resultState = E_ResultState.Continue;
        // 检查玩家队伍是否还有人存活
        bool isPlayerTeamAlive = playerList.Any(hero => !hero.IsDead);
        // 检查敌人队伍是否还有人存活
        bool isEnemyTeamAlive = enemyList.Any(hero => !hero.IsDead);
        if (!isPlayerTeamAlive)
        {
            args.resultState = E_ResultState.EnemyWin;
        }
        if (!isEnemyTeamAlive)
        {
            args.resultState = E_ResultState.PlayerWin;
        }
    }

    /// <summary>
    /// 战斗结束
    /// </summary>
    public void OnBattleResult()
    {
        StringBuilder temp = new StringBuilder();
        temp.AppendLine("玩家剩余状态：");
        foreach (var player in playerList)
        {
            temp.AppendLine($"{player.data.id}剩余血量{player.data.nowHp}  剩余怒气{player.data.currentRage}");
        }
        temp.AppendLine("\n敌人剩余状态：");
        foreach (var enemy in enemyList)
        {
            temp.AppendLine($"{enemy.data.id}剩余血量{enemy.data.nowHp}  剩余怒气{enemy.data.currentRage}");
        }
        DebugMgr.Log(temp.ToString());
    }

    public List<int> GetHeroNowHp(bool isPlayer)
    {
        List<int> nowHpList = new List<int>();
        List<HeroLogic> targetList = isPlayer ? playerList : enemyList;
        foreach (var hero in targetList)
        {
            nowHpList.Add(hero.data.nowHp);
        }
        return nowHpList;
    }
    public List<int> GetHeroNowRage(bool isPlayer)
    {
        List<int> currentRageList = new List<int>();
        List<HeroLogic> targetList = isPlayer ? playerList : enemyList;
        foreach (var hero in targetList)
        {
            currentRageList.Add(hero.data.currentRage);
        }
        return currentRageList;
    }

    private void RegisterEvents()
    {
        EventCenter.Instance.AddEventListener<GetTargetEvent>(EventConfig.GetSingleTarget,GetSingleTarget);
        EventCenter.Instance.AddEventListener<GetTargetEvent>(EventConfig.GetFrontRowTarget,GetFrontRowTarget);
        EventCenter.Instance.AddEventListener<GetTargetEvent>(EventConfig.GetBackRowTarget,GetBackRowTarget);
        EventCenter.Instance.AddEventListener<GetTargetEvent>(EventConfig.GetGroupTarget,GetGroupTarget);
        EventCenter.Instance.AddEventListener<CheckIsBattleOverEvent>(EventConfig.CheckIsBattleOver, CheckIsBattleOver);
    }
    private void UnregisterEvents()
    {
        EventCenter.Instance.RemoveEventListener<GetTargetEvent>(EventConfig.GetSingleTarget, GetSingleTarget);
        EventCenter.Instance.RemoveEventListener<GetTargetEvent>(EventConfig.GetFrontRowTarget, GetFrontRowTarget);
        EventCenter.Instance.RemoveEventListener<GetTargetEvent>(EventConfig.GetBackRowTarget, GetBackRowTarget);
        EventCenter.Instance.RemoveEventListener<GetTargetEvent>(EventConfig.GetGroupTarget, GetGroupTarget);
        EventCenter.Instance.RemoveEventListener<CheckIsBattleOverEvent>(EventConfig.CheckIsBattleOver, CheckIsBattleOver);

    }
}
