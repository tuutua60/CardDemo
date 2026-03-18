using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// MonoBehaviour的管理者，提供全局的生命周期事件和协程服务。
/// 继承自SingletonAutoMono，会自动创建并持久化。
/// </summary>
public class MonoMgr : SingletonAutoMono<MonoMgr>
{
    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;
    private event UnityAction lateUpdateEvent;

    // Awake方法由基类SingletonAutoMono隐式管理，
    // DontDestroyOnLoad等逻辑已在基类中处理。

    private void Update()
    {
        updateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }

    #region Update 事件
    /// <summary>
    /// 添加帧更新事件监听
    /// </summary>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    /// <summary>
    /// 移除帧更新事件监听
    /// </summary>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }
    #endregion

    #region FixedUpdate 事件
    /// <summary>
    /// 添加物理更新事件监听
    /// </summary>
    public void AddFixedUpdateListener(UnityAction fun)
    {
        fixedUpdateEvent += fun;
    }

    /// <summary>
    /// 移除物理更新事件监听
    /// </summary>
    public void RemoveFixedUpdateListener(UnityAction fun)
    {
        fixedUpdateEvent -= fun;
    }
    #endregion

    #region LateUpdate 事件
    /// <summary>
    /// 添加延迟更新事件监听
    /// </summary>
    public void AddLateUpdateListener(UnityAction fun)
    {
        lateUpdateEvent += fun;
    }

    /// <summary>
    /// 移除延迟更新事件监听
    /// </summary>
    public void RemoveLateUpdateListener(UnityAction fun)
    {
        lateUpdateEvent -= fun;
    }
    #endregion
}
