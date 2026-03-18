using CardDemo;

public class BattleHandler : BaseHandler
{
    public override void HandleMsg(byte[] buffer, int clientID)
    {
        
        BattleMsg msg = new BattleMsg();
        msg.Reading(buffer);
        Console.WriteLine($"收到战斗角色数据,种子 {msg.seed}");
        GameManager.CreateWorld(msg);
    }
}