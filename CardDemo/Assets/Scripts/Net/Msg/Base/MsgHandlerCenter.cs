using System;
using System.Collections.Generic;
public class MsgHandlerCenter : BaseManager<MsgHandlerCenter>
{
    //所有Handler的字典容器，key是消息ID，value是Handler对象
    private Dictionary<int,BaseHandler>handlerDic = new Dictionary<int, BaseHandler>();

    public MsgHandlerCenter()
    {
        handlerDic.Add(2003,new SkipHandler());
    }

    /// <summary>
    /// 提供给外部用于处理消息的接口
    /// </summary>
    /// <param name="msgID">消息ID</param>
    /// <param name="buffer">消息体</param>
    public void HandleMsg(int msgID,byte[] buffer)
    {
        if(handlerDic.TryGetValue(msgID,out BaseHandler handler))
        {
            handler.HandleMsg(buffer);
        }
        else
        {
            Console.WriteLine("没有找到消息ID为{0}的处理器", msgID);
        }
    }
}