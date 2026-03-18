
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class NetMgr:BaseManager<NetMgr>
{
    private Socket client;
    //客户端ID
    public int ClientID { get; private set; }
    private static int CLIENT_BEGIN_ID = 1;

    //缓存容器
    private byte[] cacheBytes = new byte[1024];
    private int cacheNum = 0;

    // 用于存储从网络线程接收到的消息，等待主线程处理
    private static Queue<KeyValuePair<int, byte[]>> msgQueue = new Queue<KeyValuePair<int, byte[]>>();
    private static readonly object msgLock = new object();

    public void StartNet(string ip,int port)
    {
        try
        {
            this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            this.ClientID = CLIENT_BEGIN_ID++;

            // 异步连接到服务器
            this.client.BeginConnect(iPEndPoint, ConnectCallback, this.client);
            DebugMgr.Log($"尝试连接到服务器 {ip}:{port}");
        }
        catch (Exception e)
        {
            DebugMgr.LogError($"连接服务器失败: {e.Message}");
        }
    }

    /// <summary>
    /// 在主线程中调用，用于处理网络消息
    /// </summary>
    public void Update()
    {
        lock (msgLock)
        {
            while (msgQueue.Count > 0)
            {
                KeyValuePair<int, byte[]> msg = msgQueue.Dequeue();
                MsgHandlerCenter.Instance.HandleMsg(msg.Key, msg.Value);
            }
        }
    }

    /// <summary>
    /// 连接成功后的回调
    /// </summary>
    private void ConnectCallback(IAsyncResult result)
    {
        try
        {
            if (this.client != null && this.client.Connected)
            {
                this.client.EndConnect(result);
                DebugMgr.Log("服务器连接成功");
                // 连接成功后，开始接收消息
                this.client.BeginReceive(cacheBytes, cacheNum, cacheBytes.Length - cacheNum, SocketFlags.None, ReceiveCallback, this.client);
            }
        }
        catch (Exception e)
        {
            DebugMgr.LogError($"连接回调出错: {e.Message}");
        }
    }


    /// <summary>
    /// 接收这个服务器发来的消息
    /// </summary>
    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            if (this.client != null && this.client.Connected)
            {
                //接收到的字节数
                int num = this.client.EndReceive(result);
                if (num == 0)
                {
                    // 如果接收到的字节数为0，通常表示连接已由对方关闭
                    DebugMgr.LogWarning("服务器主动断开连接");
                    Close();
                    return;
                }

                //处理分包黏包
                HandleReceiveMsg(num);

                //如果是连接状态，再继续收消息
                this.client.BeginReceive(cacheBytes, cacheNum, cacheBytes.Length - cacheNum, SocketFlags.None, ReceiveCallback, this.client);
            }
            else
            {
                DebugMgr.LogWarning("没有连接，不用再收消息了");
            }
        }
        catch (Exception e)
        {
            DebugMgr.LogError($"接收消息失败: {e.Message}");
            Close(); // 发生异常时关闭连接
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
            if (msgLength != -1 && cacheNum - currentIndex >= msgLength)
            {

                //将消息体字节数组复制出来，放入主线程处理队列
                byte[] msgBytes = new byte[msgLength];
                Array.Copy(cacheBytes, currentIndex, msgBytes, 0, msgLength);
                lock (msgLock)
                {
                    msgQueue.Enqueue(new KeyValuePair<int, byte[]>(msgID, msgBytes));
                }

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
            DebugMgr.LogError("网络未连接，无法发送消息");
        }
    }

    private void SendCallback(IAsyncResult result)
    {
        try
        {
            if (this.client != null && this.client.Connected)
            {
                int num = this.client.EndSend(result);
                // Console.WriteLine是线程安全的，但为了统一，也可以用DebugMgr
                // DebugMgr.Log($"发送了 {num} 字节");
            }
        }
        catch (Exception e)
        {
            DebugMgr.LogError($"发送消息失败: {e.Message}");
        }
    }

    /// <summary>
    /// 关闭客户端连接
    /// </summary>
    public void Close()
    {
        DebugMgr.Log("关闭网络链接");
        if (client != null)
        {
            // 先关闭Socket，再置为null
            if (client.Connected)
            {
                client.Shutdown(SocketShutdown.Both);
            }
            client.Close();
            client = null;
        }
    }
}