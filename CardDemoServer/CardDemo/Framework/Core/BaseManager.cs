using System;

/// <summary>
/// 线程安全的泛型单例模式基类 (非Mono)。
/// 使用Lazy<T>实现延迟初始化。
/// </summary>
/// <typeparam name="T">需要实现单例的类</typeparam>
public class BaseManager<T> where T : new()
{
    // 使用Lazy<T>来确保实例的延迟创建和线程安全
    private static readonly Lazy<T> lazyInstance = new Lazy<T>(() => new T());

    public static T Instance
    {
        get
        {
            // 第一次访问时，lazyInstance会调用工厂方法创建实例
            // 后续访问则直接返回已创建的实例
            return lazyInstance.Value;
        }
    }
}

