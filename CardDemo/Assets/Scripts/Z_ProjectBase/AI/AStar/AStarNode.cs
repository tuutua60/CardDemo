using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Node_Type
{
    //可以走的地方
    Walkable,
    //不能走的阻挡
    Obstacle,
}

/// <summary>
/// A* 寻路算法的节点类
/// </summary>
public class AStarNode
{
    // 节点在地图中的坐标
    public int x;
    public int y;

    // 寻路消耗
    public float f; // 总消耗 (g + h)
    public float g; // 距离起点的消耗
    public float h; // 距离终点的估算消耗 (启发函数)
    
    // 寻路过程中的父节点
    public AStarNode father;
    // 节点类型（是否为障碍物）
    public E_Node_Type type;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="x">节点的x坐标</param>
    /// <param name="y">节点的y坐标</param>
    /// <param name="type">节点的类型</param>
    public AStarNode(int x, int y, E_Node_Type type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }
}
