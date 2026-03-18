using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSwapEvent
{
    public int fromIndex;
    public int toIndex;
}

public class HeroUnselectEvent
{
    /// <summary>
    /// 被移除的英雄所在的槽位索引
    /// </summary>
    public int fromIndex;
    /// <summary>
    /// 被移除的英雄ID
    /// </summary>
    public int heroID;
}

public class HeroMoveSlotEvent
{
    /// <summary>
    /// 目标槽位索引
    /// </summary>
    public int toIndex;
    /// <summary>
    /// 英雄ID
    /// </summary>
    public int heroID;

    public int fromIndex;
}