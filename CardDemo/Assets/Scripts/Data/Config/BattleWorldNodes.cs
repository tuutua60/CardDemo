using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWorldNodes : MonoBehaviour
{
    private static BattleWorldNodes instance;
    public static BattleWorldNodes Instance => instance;
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 玩家节点
    /// </summary>
    public List<Transform> playerPosList;
    /// <summary>
    /// 敌人节点
    /// </summary>
    public List<Transform> enemyPosList;
    /// <summary>
    /// 世界中心点
    /// </summary>
    public Transform worldCenter;
    /// <summary>
    /// 玩家中心点
    /// </summary>
    public Transform playerCenter;
    /// <summary>
    /// 敌人中心点
    /// </summary>
    public Transform enemyCenter;
}
