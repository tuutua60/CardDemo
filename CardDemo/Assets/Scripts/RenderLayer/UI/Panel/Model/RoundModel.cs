using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundModel
{
    /// <summary>
    /// 当前帧号
    /// </summary>
    public int nowLogicFrame;
    /// <summary>
    /// 当前倍速
    /// </summary>
    public int nowSpeed;

    public void Initialize()
    {
        nowLogicFrame = 0;
        nowSpeed = 1;
    }
}

