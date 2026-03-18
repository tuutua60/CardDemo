using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 调试管理器
/// </summary>
public class DebugMgr : MonoBehaviour
{
    private static DebugMgr instance;
    public static DebugMgr Instance => instance;

    private Text logText;

    // 创建一个线程安全的队列来存储日志消息
    private static Queue<KeyValuePair<string, int>> logQueue = new Queue<KeyValuePair<string, int>>();
    // 用于锁住队列，确保线程安全
    private static readonly object logLock = new object();

    private void Awake()
    {
        instance = this;
        logText = GetComponentInChildren<Text>();
        if (logText != null)
        {
            logText.text = "";
        }
    }

    private void Update()
    {
        // 在主线程的Update中处理队列中的所有日志
        lock (logLock)
        {
            while (logQueue.Count > 0)
            {
                KeyValuePair<string, int> logEntry = logQueue.Dequeue();
                string message = logEntry.Key;
                int logType = logEntry.Value;

                if (logText != null)
                {
                    logText.text += "\n" + message;
                }

                switch (logType)
                {
                    case 0: // Log
                        Debug.Log(message);
                        break;
                    case 1: // LogError
                        Debug.LogError(message);
                        break;
                    case 2: // LogWarning
                        Debug.LogWarning(message);
                        break;
                }
            }
        }
    }

    // 将消息添加到队列，而不是直接打印
    private static void EnqueueLog(string str, int logType)
    {
        // 如果实例不存在（例如场景未加载），直接使用Unity的Debug打印，这在非主线程是安全的
        if (Instance == null)
        {
            switch (logType)
            {
                case 0: Debug.Log(str); break;
                case 1: Debug.LogError(str); break;
                case 2: Debug.LogWarning(str); break;
            }
            return;
        }

        lock (logLock)
        {
            logQueue.Enqueue(new KeyValuePair<string, int>(str, logType));
        }
    }

    public static void Log(string str)
    {
        EnqueueLog(str, 0);
    }

    public static void LogError(string str)
    {
        EnqueueLog(str, 1);
    }

    public static void LogWarning(string str)
    {
        EnqueueLog(str, 2);
    }
}
