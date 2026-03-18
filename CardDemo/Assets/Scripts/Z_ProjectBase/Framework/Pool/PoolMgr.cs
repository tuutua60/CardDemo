using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 缓存池模块
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    // 缓存池容器 (衣柜)
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    // 缓存池在场景中唯一的根对象
    private GameObject poolObj;

    /// <summary>
    /// 从缓存池异步获取对象。
    /// </summary>
    /// <param name="path">资源路径 (作为池的唯一标识)</param>
    /// <param name="callBack">获取到对象后的回调</param>
    public void GetObject(string path, UnityAction<GameObject> callBack)
    {
        // 尝试从池中获取
        if (poolDic.TryGetValue(path, out PoolData pool) && pool.poolStack.Count > 0)
        {
            callBack(pool.GetObj());
        }
        else
        {
            // 池中没有，异步加载新对象
            ResourcesMgr.Instance.LoadAsync<GameObject>(path, (o) =>
            {
                o.name = path;
                callBack(o);
            });
        }
    }

    /// <summary>
    /// 从缓存池同步获取对象。
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns>游戏对象</returns>
    public GameObject GetObject(string path)
    {
        // 尝试从池中获取
        if (poolDic.TryGetValue(path, out PoolData pool) && pool.poolStack.Count > 0)
        {
            return pool.GetObj();
        }
        else
        {
            // 池中没有，同步加载新对象
            GameObject obj = ResourcesMgr.Instance.Load<GameObject>(path);
            if (obj != null)
            {
                obj.name = path;
            }
            return obj;
        }
    }

    /// <summary>
    /// 将对象归还到缓存池。
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="obj">要归还的对象</param>
    public void PushObject(string path, GameObject obj)
    {
        // 增加健壮性，防止空对象入池
        if (obj == null)
        {
            Debug.LogError("试图将空对象放入对象池，路径: " + path);
            return;
        }
        // 确保根对象存在
        if (poolObj == null)
        {
            poolObj = new GameObject("Pool");
            GameObject.DontDestroyOnLoad(poolObj);
        }
        // 查找池，如果不存在则创建一个新的
        if (!poolDic.TryGetValue(path, out PoolData pool))
        {
            // 使用path作为池的名称
            pool = new PoolData(path, poolObj);
            poolDic.Add(path, pool);
        }
        // 将对象放入池中
        pool.PushObj(obj);
    }

    /// <summary>
    /// 清空所有缓存池中的失活对象。
    /// 此方法不会影响已经从池中取出的活跃对象。
    /// </summary>
    public void Clear()
    {
        foreach (PoolData pool in poolDic.Values)
        {
            // 让每个PoolData自己负责清理内部对象
            pool.Clear();
        }
        poolDic.Clear();
        // 销毁总父对象
        if (poolObj != null)
        {
            GameObject.Destroy(poolObj);
            poolObj = null;
        }
    }
}
