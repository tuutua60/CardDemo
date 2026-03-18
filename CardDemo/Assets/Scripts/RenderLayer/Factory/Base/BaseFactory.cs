using System;
using UnityEngine;

/// <summary>
/// 所有具体工厂的基类，提供通用的功能，例如从对象池加载资源。
/// </summary>
public abstract class BaseFactory
{
    /// <summary>
    /// 一般一个工厂只用一种生成方式
    /// </summary>
    public abstract void CreateGameObject(CreateAssetEvent e);

    protected GameObject LoadGameObjectFromPool(string assetPath)
    {
        return PoolMgr.Instance.GetObject(assetPath);
    }
    protected GameObject LoadGameObjectFromResources(string assetPath)
    {
        return ResourcesMgr.Instance.Load<GameObject>(assetPath);
    }

    /// <summary>
    /// 从对象池异步加载游戏对象。
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    /// <param name="callback">加载完成后的回调</param>
    protected void LoadGameObjectFromPoolAsync(string assetPath, Action<GameObject> callback)
    {
        PoolMgr.Instance.GetObject(assetPath, (obj) =>
        {
            callback?.Invoke(obj);
        });
    }

    /// <summary>
    /// 从 ResourcesMgr异步加载游戏对象。
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    /// <param name="callback">加载完成后的回调</param>
    protected void LoadGameObjectFromResourceAsync(string assetPath, Action<GameObject> callback)
    {
        ResourcesMgr.Instance.LoadAsync<GameObject>(assetPath, (obj) =>
        {
            callback?.Invoke(obj);
        });
    }

    public virtual void Clear()
    {

    }
}

public enum E_CreateAssetType
{
    Pool,
    PoolAsync,
    Resources,
    ResourcesAsync
}