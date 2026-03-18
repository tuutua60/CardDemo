using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 工厂管理器，负责实例化各种游戏对象
/// </summary>
public class FactoryCenter : BaseManager<FactoryCenter>
{
    private Dictionary<Type, BaseFactory> factoryDic = new Dictionary<Type, BaseFactory>();

    public void Initialize()
    {
        InitFactory();
        RegisterEvent();
    }

    private void InitFactory()
    {
        AddFactory(typeof(CreateHeroRenderEvent),new HeroRenderFactory());
        AddFactory(typeof(CreateBuffEffectRenderEvent),new BuffEffectRenderFactory());
    }

    private void HandlePoolEvent(CreateAssetEvent args)
    {
        args.createType = E_CreateAssetType.Pool;
        CreateGameObject(args);

    }

    private void HandlePoolAsyncEvent(CreateAssetEvent args)
    {
        args.createType = E_CreateAssetType.PoolAsync;
        CreateGameObject(args);

    }

    private void HandleResourcesEvent(CreateAssetEvent args)
    {
        args.createType = E_CreateAssetType.Resources;
        CreateGameObject(args);

    }

    private void HandleResourcesAsyncEvent(CreateAssetEvent args)
    {
        args.createType = E_CreateAssetType.ResourcesAsync;
        CreateGameObject(args);
    }

    private void AddFactory<T>(Type key,T factory) where T :BaseFactory
    {
        if (!factoryDic.ContainsKey(key))
        {
            factoryDic.Add(key, factory as BaseFactory);
        }
    }

    private void CreateGameObject(CreateAssetEvent args)
    {
        Type key = args.GetType();
        if (factoryDic.ContainsKey(key))
        {
            factoryDic[key].CreateGameObject(args);
        }
    }
    private void RegisterEvent()
    {
        EventCenter.Instance.AddEventListener<CreateAssetEvent>(EventConfig.CreateAssetByPool, HandlePoolEvent);
        EventCenter.Instance.AddEventListener<CreateAssetEvent>(EventConfig.CreateAssetByPoolAsync, HandlePoolAsyncEvent);
        EventCenter.Instance.AddEventListener<CreateAssetEvent>(EventConfig.CreateAssetByResource, HandleResourcesEvent);
        EventCenter.Instance.AddEventListener<CreateAssetEvent>(EventConfig.CreateAssetByResourceAsync, HandleResourcesAsyncEvent);
    }

    private void UnRegisterEvent()
    {
        EventCenter.Instance.RemoveEventListener<CreateAssetEvent>(EventConfig.CreateAssetByPool, HandlePoolEvent);
        EventCenter.Instance.RemoveEventListener<CreateAssetEvent>(EventConfig.CreateAssetByPoolAsync, HandlePoolAsyncEvent);
        EventCenter.Instance.RemoveEventListener<CreateAssetEvent>(EventConfig.CreateAssetByResource, HandleResourcesEvent);
        EventCenter.Instance.RemoveEventListener<CreateAssetEvent>(EventConfig.CreateAssetByResourceAsync, HandleResourcesAsyncEvent);
    }

    public void Clear()
    {
        foreach (var factory in factoryDic.Values)
        {
            factory.Clear();
        }
        factoryDic.Clear();
        UnRegisterEvent();
    }
}
