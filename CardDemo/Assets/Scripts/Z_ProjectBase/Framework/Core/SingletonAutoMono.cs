using UnityEngine;

/// <summary>
/// 健壮的MonoBehaviour单例基类。
/// 它能自动处理实例的查找、创建和生命周期管理。
/// </summary>
/// <typeparam name="T">需要实现单例的MonoBehaviour类</typeparam>
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object lockObject = new object();
    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            // 如果应用正在退出，则不再提供实例，防止创建幽灵对象
            if (applicationIsQuitting)
            {
                //Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again.");
                return null;
            }

            lock (lockObject)
            {
                if (instance == null)
                {
                    // 首先，尝试在场景中查找已存在的实例
                    instance = FindObjectOfType<T>();

                    // 如果场景中存在多个，发出警告
                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        //Debug.LogError($"[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                        return instance;
                    }

                    // 如果场景中没有实例，则动态创建一个
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"(Singleton) {typeof(T)}";

                        // 标记为在场景切换时不销毁
                        DontDestroyOnLoad(singletonObject);

                        //Debug.Log($"[Singleton] An instance of {typeof(T)} is needed in the scene, so '{singletonObject.name}' was created with DontDestroyOnLoad.");
                    }
                }

                return instance;
            }
        }
    }

    /// <summary>
    /// 当应用退出时，Unity会调用此方法。
    /// 我们在这里设置一个标志，以防止在对象销毁后再次创建单例。
    /// </summary>
    protected virtual void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
