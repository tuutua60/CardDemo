public class SkipHandler : BaseHandler
{
    public override void HandleMsg(byte[] buffer)
    {
        SkipMsg msg = new SkipMsg();
        msg.Reading(buffer);
        SkipEvent skipEvent = new SkipEvent()
        {
            isPlayerWin = msg.isPlayerWin,
            roundCount = msg.roundCount,
            playerNowHp = msg.playerNowHp,
            playerNowRage = msg.playerNowRage,
            enemyNowHp = msg.enemyNowHp,
            enemyNowRage = msg.enemyNowRage
        };
        DebugMgr.LogWarning("收到跳过战斗数据");
        EventCenter.Instance.EventTrigger<SkipEvent>(EventConfig.SkipBattle, skipEvent);
    }
}