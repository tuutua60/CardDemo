public class TestHandler : BaseHandler
{

    public override void HandleMsg(byte[] buffer,int clientID)
    {
        TestMsg msg = new TestMsg();
        msg.Reading(buffer);
        Console.WriteLine("处理测试消息");
        Console.WriteLine(msg.name);
    }
}