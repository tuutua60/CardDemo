using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池数据，用于存储一类对象
/// </summary>
public class PoolData
{
    // 池中对象的父节点，用于在Hierarchy中进行组织
    public GameObject fatherObj;
    // 使用栈来存储对象
    public Stack<GameObject> poolStack;
    /// <summary>
    /// PoolData的构造函数
    /// </summary>
    /// <param name="objName">对象池的名称（通常是资源路径）</param>
    /// <param name="poolRoot">总对象池的根节点</param>
    public PoolData(string objName, GameObject poolRoot)
    {
        // 为这类对象创建一个父物体，方便管理
        fatherObj = new GameObject(objName);
        fatherObj.transform.SetParent(poolRoot.transform);
        poolStack = new Stack<GameObject>();
    }

    /// <summary>
    /// 将对象存入池中
    /// </summary>
    public void PushObj(GameObject obj)
    {
        // 失活并设置父节点
        obj.SetActive(false);
        obj.transform.SetParent(fatherObj.transform);
        poolStack.Push(obj);
    }

    /// <summary>
    /// 从池中获取对象
    /// </summary>
    public GameObject GetObj()
    {
        GameObject obj = poolStack.Pop();
        // 激活并移除父节点
        obj.SetActive(true);
        obj.transform.SetParent(null);
        return obj;
    }

    /// <summary>
    /// 清空池中所有缓存的对象
    /// </summary>
    public void Clear()
    {
        // 销毁所有在池中的对象
        while (poolStack.Count > 0)
        {
            GameObject.Destroy(poolStack.Pop());
        }
        // 销毁这个池的父对象
        GameObject.Destroy(fatherObj);
    }
}