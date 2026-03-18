using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugMgr
{
    public static void Log(string message)
    {
#if CLIENT_LOGIC
        Debug.Log(message);
#else
        Console.WriteLine(message);
#endif
    }

    public static void LogWarning(string message)
    {
#if CLIENT_LOGIC
        Debug.LogWarning(message);
#else
        Console.WriteLine("*******警告*******  " + message);
#endif
    }

    public static void LogError(string message)
    {
#if CLIENT_LOGIC
        Debug.LogError(message);
#else
        Console.WriteLine("*******错误*******  " + message);
#endif
    }
}
