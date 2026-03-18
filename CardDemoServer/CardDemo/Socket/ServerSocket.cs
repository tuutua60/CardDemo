using System.Net;
using System.Net.Sockets;

public class ServerSocket
{
    private static ServerSocket instance = new ServerSocket();
    public static ServerSocket Instance => instance;

    private Socket server;
    /// <summary>
    /// 当前服务器是否关闭的标识
    /// </summary>
    private bool isClose = true;

    /// <summary>
    /// key -- 客户端ID
    /// value -- 客户端连接对象
    /// </summary>
    private Dictionary<int,ClientSocket>clientDic = new Dictionary<int, ClientSocket>();

    /// <summary>
    /// 开启服务器端
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="listen"></param>
    public void Start(string ip, int port, int listen)
    {
        this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        try
        {
            this.server.Bind(ipPoint);
            this.server.Listen(listen);
            Console.WriteLine("服务器开启成功");
            this.server.BeginAccept(AcceptCallback, this.server);
        }
        catch
        {
            Console.WriteLine("服务器开启失败");
        }
    }

    /// <summary>
    /// 接受客户端连入
    /// </summary>
    private void AcceptCallback(IAsyncResult result)
    {
        try
        {
            //获取连入的客户端
            Socket socket = server.EndAccept(result);
            ClientSocket client = new ClientSocket(socket);
            //记录客户端
            clientDic.Add(client.ClientID, client);
            Console.WriteLine(client.ClientID + "客户端连入了");

            //继续让别的客户端可以连入
            this.server.BeginAccept(AcceptCallback, this.server);
        }
        catch (SocketException e)
        {
            Console.WriteLine("客户端连入出错 " + e.SocketErrorCode + e.Message);
        }
    }

    /// <summary>
    /// 向所有客户端广播消息
    /// </summary>
    public void BroadcastMsg(BaseMsg msg)
    {
        Console.WriteLine("服务器广播消息");
        foreach (var item in clientDic)
        {
            item.Value.SendMsg(msg);
        }
    }

    /// <summary>
    /// 发送指定消息给特定客户端
    /// </summary>
    public void SendMsg(int clientID, BaseMsg msg)
    {
        if (clientDic.TryGetValue(clientID, out ClientSocket client))
        {
            client.SendMsg(msg);
        }
    }

    /// <summary>
    /// 关闭客户端链接
    /// </summary>
    public void CloseClient(ClientSocket client)
    {
        lock (clientDic)
        {
            client.Close();
            if (clientDic.ContainsKey(client.ClientID))
            {
                clientDic.Remove(client.ClientID);
                Console.WriteLine("客户端{0}主动断开连接了", client.ClientID);
            }
        }
    }

    /// <summary>
    /// 关闭服务器
    /// </summary>
    public void Close()
    {
        foreach (var client in clientDic.Values)
        {
            client.Close();
        }
        clientDic.Clear();
        server.Shutdown(SocketShutdown.Both);
        server.Close();
        server = null;
        isClose = true;
    }
}
