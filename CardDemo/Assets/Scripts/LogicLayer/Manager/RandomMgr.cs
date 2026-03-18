using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 管理随机数的管理器
/// </summary>
public class RandomMgr : BaseManager<RandomMgr>
{
    //随机种子
    private int seed;
    //用System.Random来生成随机数
    private Random random;

    public void Initialize(int seed)
    {
        this.seed = seed;
        random = new Random(seed);
    }

    public int GetRandomInt(int min, int max)
    {
        return random.Next(min, max);
    }

    public void Clear()
    {
        seed = 0;
        random = null;
    }
}
