using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单个回合逻辑
/// </summary>
public class RoundLogic : IRoundBehaviour
{
    private ActionLogic currentAction;
    ///// <summary>
    ///// 当前回合状态
    ///// </summary>
    //private E_ProcessState roundState;
    /// <summary>
    /// 英雄出手列表
    /// </summary>
    private Queue<HeroLogic> turnOrderQu;

    private Action RoundEndCallback;
    public RoundLogic(List<HeroLogic> allHeroList,Action roundEndCallback)
    {
        //根据英雄速度 计算出手顺序
        List<HeroLogic> turnOrderList = new List<HeroLogic>(allHeroList);
        turnOrderList.Sort((a, b) => b.data.agl.CompareTo(a.data.agl));
        turnOrderQu = new Queue<HeroLogic>(turnOrderList);

        this.RoundEndCallback += roundEndCallback;
    }

    public void OnRoundStart()
    {
        //roundState = E_ProcessState.Start;
        // 回合开始时的逻辑

        //进入回合进行阶段
        OnRoundPerform();
    }

    public void OnRoundPerform()
    {
        //roundState = E_ProcessState.Perform;
        // 处理第一个角色的行动
        ProcessNextAction();
    }

    /// <summary>
    /// 处理下一个角色的行动
    /// </summary>
    private void ProcessNextAction()
    {
        // 如果还有角色未行动
        if (turnOrderQu.Count > 0)
        {
            var hero = turnOrderQu.Dequeue();
            //if (!hero.CanAct)
            //{
            //    OnCurrentActionComplete();
            //    return;
            //}
            // 创建并执行行动
            SkillConfig skill = SkillMgr.Instance.GetSkillConfig(hero.data.id,true);
            currentAction = new ActionLogic(hero, skill, OnCurrentActionComplete);
            //DelayCallMgr.Instance.DelayCall(1000, currentAction.Start);
            currentAction.Start();
        }
        else
        {
            // 所有角色行动完毕，结束回合
            OnRoundEnd();
        }
    }
    /// <summary>
    /// 当前行动完成后的回调
    /// </summary>
    private void OnCurrentActionComplete()
    {
        // 清理上一个行动
        if (currentAction != null)
        {
            currentAction.OnDestroy();
            currentAction = null;
        }
        //判断战斗是否结束
        CheckIsBattleOverEvent checkArgs = new CheckIsBattleOverEvent();
        EventCenter.Instance.EventTrigger<CheckIsBattleOverEvent>(EventConfig.CheckIsBattleOver, checkArgs);
        if (checkArgs.resultState == E_ResultState.Continue)
        {
            // 继续处理下一个行动
            ProcessNextAction();
        }
        else
        {
            //战斗结束

            OnRoundEnd();
        }
    }

    public void OnRoundEnd()
    {
        //roundState = E_ProcessState.End;
        // 回合结束时的逻辑
        RoundEndCallback?.Invoke();
        this.RoundEndCallback = null;
    }
}
