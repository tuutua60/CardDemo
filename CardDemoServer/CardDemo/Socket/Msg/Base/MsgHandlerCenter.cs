public class MsgHandlerCenter : BaseManager<MsgHandlerCenter>
{
    //所有Handler的字典容器，key是消息ID，value是Handler对象
    private Dictionary<int,BaseHandler>handlerDic = new Dictionary<int, BaseHandler>();

    public MsgHandlerCenter()
    {
        handlerDic.Add(1001, new TestHandler());
        handlerDic.Add(2001, new BattleHandler());
        handlerDic.Add(2002, new SkipRequestHandler());
    }

    /// <summary>
    /// 提供给外部用于处理消息的接口
    /// </summary>
    /// <param name="msgID">消息ID</param>
    /// <param name="buffer">消息体</param>
    public void HandleMsg(int msgID,byte[] buffer,int clientID)
    {
        if(handlerDic.TryGetValue(msgID,out BaseHandler handler))
        {
            handler.HandleMsg(buffer, clientID);
        }
        else
        {
            Console.WriteLine("没有找到消息ID为{0}的处理器", msgID);
        }
    }
}