/// <summary>
/// 事件配置类
/// 统一管理项目中所有的事件名称，避免使用魔法字符串。
/// </summary>
public static class EventConfig
{
    //// --- 编写案例 ---
    ///// <summary>
    ///// 玩家数据加载完成
    ///// 参数: string (玩家名称)
    ///// </summary>
    //public const string PlayerDataLoaded = "PlayerDataLoaded";

    #region HeroController
    /// <summary>
    /// 从HeroController获取单个目标
    /// 参数: GetTargetEvent
    /// </summary>
    public const string GetSingleTarget = "GetSingleTarget";

    /// <summary>
    /// 从HeroController获取前排目标
    /// 参数: GetTargetEvent
    /// </summary>
    public const string GetFrontRowTarget = "GetFrontRowTarget";

    /// <summary>
    /// 从HeroController获取后排目标
    /// 参数: GetTargetEvent
    /// </summary>
    public const string GetBackRowTarget = "GetBackRowTarget";

    /// <summary>
    /// 从HeroController获取群体目标
    /// 参数: GetTargetEvent
    /// </summary>
    public const string GetGroupTarget = "GetGroupTarget";

    /// <summary>
    /// 从HeroController检查战斗是否结束
    /// </summary>
    public const string CheckIsBattleOver = "CheckIsBattleOver";
    #endregion

    #region ActionLogic
    /// <summary>
    /// 行动开启前触发
    /// </summary>
    public const string OnActionStart = "OnActionStart";
    /// <summary>
    /// 行动进行时触发
    /// </summary>
    public const string OnActionPerform = "OnActionPerform";
    /// <summary>
    /// 行动结束时触发
    /// </summary>
    public const string OnActionEnd = "OnActionEnd";
    /// <summary>
    /// 回合开始时触发
    /// </summary>
    public const string OnRoundStart = "OnRoundStart";
    #endregion

    #region HeroLogic
    /// <summary>
    /// 英雄移动到目标位置
    /// 参数 HeroMoveToTargetEvent
    /// </summary>
    public const string HeroMoveToTarget = "HeroMoveToTarget";
    /// <summary>
    /// 英雄死亡
    /// 参数 HeroDeadEvent
    /// </summary>
    public const string HeroDead = "HeroDead";
    /// <summary>
    /// 英雄播放动画事件
    /// 参数 string
    /// </summary>
    public const string HeroPlayAnim = "HeroPlayAnim";
    /// <summary>
    /// 英雄受伤
    /// </summary>
    public const string HeroWound = "HeroWound";
    /// <summary>
    /// 英雄攻击
    /// </summary>
    public const string HeroAttack = "HeroAttack";
    /// <summary>
    /// 英雄回血
    /// </summary>
    public const string HeroRestore = "HeroRestore";
    /// <summary>
    /// 英雄销毁
    /// </summary>
    public const string HeroDestroyEvent = "HeroDestroyEvent";
    #endregion

    #region FactoryMgr
    /// <summary>
    /// 通过对象池创建资源
    /// 参数 CreateAssetEvent
    /// </summary>
    public const string CreateAssetByPool = "CreateAssetByPool";

    public const string CreateAssetByPoolAsync = "CreateAssetByPoolAsync";

    /// <summary>
    /// 通过资源加载创建资源
    /// 参数 CreateAssetEvent
    /// </summary>
    public const string CreateAssetByResource = "CreateAssetByResource";

    public const string CreateAssetByResourceAsync = "CreateAssetByResourceAsync";
    #endregion

    #region HeroRender
    /// <summary>
    /// 获得英雄渲染对象
    /// 参数 GetRenderEvent
    /// </summary>
    public const string GetHeroRender = "GetHeroRender";
    #endregion

    #region Buff
    /// <summary>
    /// 移除Buff事件
    /// 蚕食 RemoveBuffEvent
    /// </summary>
    public const string RemoveBuff = "RemoveBuff";
    /// <summary>
    /// 更新Buff图标
    /// </summary>
    public const string UpdateBuffIcon = "UpdateBuffIcon";
    #endregion

    #region UI
    /// <summary>
    /// 选择英雄面板上英雄交换座位
    /// </summary>
    public const string  HeroSwap= "HeroSwap";
    /// <summary>
    /// 英雄取消选择，放回卡槽
    /// </summary>
    public const string HeroUnselectEvent = "HeroUnselectEvent";
    /// <summary>
    /// 单个英雄移动位置
    /// </summary>
    public const string HeroMoveSlotEvent = "HeroMoveSlotEvent";
    #endregion

    #region Game
    /// <summary>
    /// 战斗开始事件
    /// </summary>
    public const string OnBattleStart = "OnBattleStart";
    /// <summary>
    /// 战斗结束
    /// </summary>
    public const string OnBattleEnd = "OnBattleEnd";
    #endregion

    #region Net
    public const string TestHandler = "TestHandler";
    #endregion
}