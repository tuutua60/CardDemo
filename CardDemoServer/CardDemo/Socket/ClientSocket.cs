
using System.Net.Sockets;

public class ClientSocket
{
    private Socket client;
    //客户端ID
    public int ClientID { get; private set; }
    private static int CLIENT_BEGIN_ID = 1;

    //缓存容器
    private byte[] cacheBytes = new byte[1024];
    private int cacheNum = 0;

    public ClientSocket(Socket socket)
    {
        this.client = socket;
        this.ClientID = CLIENT_BEGIN_ID++;

        //开始接收消息
        this.client.BeginReceive(cacheBytes, cacheNum, cacheBytes.Length, SocketFlags.None, ReceiveCallback, this.client);
    }

    /// <summary>
    /// 接收这个客户端发来的消息
    /// </summary>
    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            if (this.client != null && this.client.Connected)
            {
                //接收到的字节数
                int num = this.client.EndReceive(result);
                //当接收到的字节数为0时，表示客户端已主动断开连接
                if (num == 0)
                {
                    ServerSocket.Instance.CloseClient(this);
                    return;
                }
                //处理分包黏包
                HandleReceiveMsg(num);

                //如果是连接状态，再继续收消息
                this.client.BeginReceive(cacheBytes, cacheNum, cacheBytes.Length - cacheNum, SocketFlags.None, ReceiveCallback, this.client);
            }
            else
            {
                ServerSocket.Instance.CloseClient(this);
                Console.WriteLine("没有连接，不用再收消息了");
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e.Message);
            ServerSocket.Instance.CloseClient(this);
        }
    }

    /// <summary>
    /// 处理接收到的消息，处理分包和黏包问题
    /// </summary>
    /// <param name="num">接收到的字节数</param>
    private void HandleReceiveMsg(int num)
    {
        int msgID = 0;
        int msgLength = 0;
        int currentIndex = 0;

        cacheNum += num;

        while (true)
        {
            //初始化msgLength为-1，表示还没有解析消息头
            msgLength = -1;
            //第一步.如果能解析出来完整消息头（消息ID和消息长度），就解析出消息头
            if (cacheNum - currentIndex >= 8)
            {
                //解析消息ID
                msgID = BitConverter.ToInt32(cacheBytes, currentIndex);
                currentIndex += 4;
                //解析消息长度
                msgLength = BitConverter.ToInt32(cacheBytes, currentIndex);
                currentIndex += 4;
            }
            //第二步.如果解析过消息头, 而且能解析出完整消息体，就解析出消息体
            if(msgLength != -1 && cacheNum - currentIndex >= msgLength)
            {

                //将消息体字节数组复制出来，交给MsgHandlerCenter处理
                byte[] msgBytes = new byte[msgLength];
                Array.Copy(cacheBytes, currentIndex, msgBytes, 0, msgLength);
                MsgHandlerCenter.Instance.HandleMsg(msgID, msgBytes, this.ClientID);

                currentIndex += msgLength;
                if (currentIndex == cacheNum)
                {
                    cacheNum = 0;
                    break;
                }
            }
            //如果没进到第二步,说明消息头不完整或者消息体不完整,继续接收消息
            else
            {
                //msgLength != -1 代表进行了消息头的解析，但是没有解析消息体
                //如果消息头解析了,但是消息体不完整,则currentIndex回退8个字节,等下一次接收消息时再重新解析消息头
                if (msgLength != -1)
                {
                    currentIndex -= 8;
                }
                //移动不完整的数据到缓存数组开头
                Array.Copy(cacheBytes, currentIndex, cacheBytes, 0, cacheNum - currentIndex);
                cacheNum -= currentIndex;
                break;
            }
        }
    }

    /// <summary>
    /// 给这个客户端发送消息
    /// </summary>
    public void SendMsg(BaseMsg msg)
    {
        if (client != null && this.client.Connected)
        {
            byte[] bytes = msg.Writing();
            this.client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, SendCallback, this.client);
        }
        else
        {
            ServerSocket.Instance.CloseClient(this);
        }
    }

    private void SendCallback(IAsyncResult result)
    {
        try
        {
            if (this.client != null && this.client.Connected)
            {
                int num = this.client.EndSend(result);
                Console.WriteLine($"发送了 {num} 字节");
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e.Message);
            ServerSocket.Instance.CloseClient(this);
        }
    }

    /// <summary>
    /// 关闭客户端连接
    /// </summary>
    public void Close()
    {
        Console.WriteLine("关闭客户端连接 ：" + this.ClientID);
        if (client != null)
        {
            client.Close();
            client = null;
        }
    }
}