using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonAutoMono<GameManager>
{
    SelectHeroEvent args = new SelectHeroEvent();

    private void Awake()
    {
        Initialize();
    }
    private void Start()
    {
        //Time.timeScale = 10f;
        //测试
        //for (int i = 1; i <= 5; i++) playerList.Add(new HeroData(100+i,i,E_GlobalTeamType.Player));
        //for (int i = 1; i <= 5; i++) enemyList.Add(new HeroData(500+i,i,E_GlobalTeamType.Enemy));
        //WorldMgr.Instance.CreateWorld(playerList, enemyList);
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="args"></param>
    private void StartGame(SelectHeroEvent args)
    {
        this.args = args;
        // 使用当前时间的 Ticks 属性作为种子，并转换为 int
        this.args.seed = (int)DateTime.Now.Ticks;
        RecallGame();
    }

    public void RestartGame()
    {
        // 使用当前时间的 Ticks 属性作为种子，并转换为 int
        this.args.seed = (int)DateTime.Now.Ticks;
        RecallGame();
    }

    public void RecallGame()
    {
        //游戏内角色数据
        List<HeroData> playerList = new List<HeroData>();
        List<HeroData> enemyList = new List<HeroData>();
        //网络用消息
        BattleMsg battleMsg = new BattleMsg()
        {
            seed = args.seed,
            playerDataList = args.playerList,
            enemyDataList = args.enemyList
        };
        for (int i = 0; i < args.playerList.Count; i++)
        {
            playerList.Add(new HeroData(args.playerList[i], i + 1, E_GlobalTeamType.Player));
        }
        for (int i = 0; i < args.enemyList.Count; i++)
        {
            enemyList.Add(new HeroData(args.enemyList[i], i + 1, E_GlobalTeamType.Enemy));
        }

        //发送数据给服务器
        NetMgr.Instance.SendMsg(battleMsg);

        //测试固定阵容
        //for (int i = 1; i <= 5; i++)
        //{
        //    playerList.Add(new HeroData(100 + i, i, E_GlobalTeamType.Player));
        //}
        //for (int i = 1; i <= 5; i++)
        //{
        //    enemyList.Add(new HeroData(500 + i, i, E_GlobalTeamType.Enemy));
        //}

        UIManager.Instance.ShowPanel<RoundPanel>(E_UI_Layer.Middle, (o) =>
        {
            WorldMgr.Instance.CreateWorld(playerList, enemyList,args.seed);
        });
    }
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvent()
    {
        //游戏开始事件
        EventCenter.Instance.AddEventListener<SelectHeroEvent>(EventConfig.OnBattleStart,StartGame);
    }

    /// <summary>
    /// 初始化各个管理器
    /// </summary>
    private void Initialize()
    {
        Application.targetFrameRate = 60;
        BinaryDataMgr.Instance.Initialize();
        FactoryCenter.Instance.Initialize();
        UIManager.Instance.Initialize();
        MusicMgr.Instance.Initialize();
        WorldMgr.Instance.Initialize();
        RegisterEvent();

        NetMgr.Instance.StartNet("127.0.0.1",8080);
    }

    private void Update()
    {
        WorldMgr.Instance.Update();
        NetMgr.Instance.Update();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DelayCallMgr.Instance.Clear();
        FrameSyncMgr.Instance.Clear();
        PoolMgr.Instance.Clear();
        FactoryCenter.Instance.Clear();

        NetMgr.Instance.Close();
    }
}
