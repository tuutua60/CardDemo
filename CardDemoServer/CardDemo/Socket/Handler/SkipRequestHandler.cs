public class SkipRequestHandler : BaseHandler
{
    public override void HandleMsg(byte[] buffer,int clientID)
    {
        SkipMsg msg = WorldMgr.Instance.GetSkipMsg();
        ServerSocket.Instance.SendMsg(clientID,msg);
    }
}