using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 事件中心 - 明确区分带参和不带参事件
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    private readonly Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    #region 带参数的事件方法
    /// <summary>
    /// 添加带参数的事件监听
    /// </summary>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("事件名不能为空");
            return;
        }

        if (eventDic.TryGetValue(name, out IEventInfo eventInfo))
        {
            if (eventInfo is EventInfo<T> typedEventInfo)
            {
                // 使用Linq简化重复检查
                if (typedEventInfo.actions != null && typedEventInfo.actions.GetInvocationList().Contains(action))
                {
                    Debug.LogWarning($"带参数事件 {name} 已添加相同监听器，跳过重复添加");
                    return;
                }
                typedEventInfo.actions += action;
            }
            else
            {
                Debug.LogError($"事件 {name} 类型冲突！现有类型: {eventInfo.GetEventType()}，尝试添加: EventInfo<{typeof(T).Name}>");
            }
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    /// <summary>
    /// 移除带参数的事件监听
    /// </summary>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.TryGetValue(name, out IEventInfo eventInfo) && eventInfo is EventInfo<T> typedEventInfo)
        {
            typedEventInfo.actions -= action;
            // 如果委托为空，则从字典中移除该事件
            if (typedEventInfo.actions == null)
            {
                eventDic.Remove(name);
            }
        }
    }

    /// <summary>
    /// 触发带参数的事件
    /// </summary>
    public void EventTrigger<T>(string name, T info)
    {
        if (eventDic.TryGetValue(name, out IEventInfo eventInfo))
        {
            if (eventInfo is EventInfo<T> typedEventInfo)
            {
                typedEventInfo.actions?.Invoke(info);
            }
            else
            {
                Debug.LogError($"触发带参数事件失败：{name} 不是带参数事件，实际类型: {eventInfo.GetEventType()}");
            }
        }
    }
    #endregion

    #region 不带参数的事件方法
    /// <summary>
    /// 添加不带参数的事件监听
    /// </summary>
    public void AddEventListener(string name, UnityAction action)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("事件名不能为空");
            return;
        }

        if (eventDic.TryGetValue(name, out IEventInfo eventInfo))
        {
            if (eventInfo is EventInfo typedEventInfo)
            {
                if (typedEventInfo.actions != null && typedEventInfo.actions.GetInvocationList().Contains(action))
                {
                    Debug.LogWarning($"不带参数事件 {name} 已添加相同监听器，跳过重复添加");
                    return;
                }
                typedEventInfo.actions += action;
            }
            else
            {
                Debug.LogError($"事件 {name} 类型冲突！现有类型: {eventInfo.GetEventType()}，尝试添加: EventInfo");
            }
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }

    /// <summary>
    /// 移除不带参数的事件监听
    /// </summary>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.TryGetValue(name, out IEventInfo eventInfo) && eventInfo is EventInfo typedEventInfo)
        {
            typedEventInfo.actions -= action;
            if (typedEventInfo.actions == null)
            {
                eventDic.Remove(name);
            }
        }
    }

    /// <summary>
    /// 触发不带参数的事件
    /// </summary>
    public void EventTrigger(string name)
    {
        if (eventDic.TryGetValue(name, out IEventInfo eventInfo))
        {
            if (eventInfo is EventInfo typedEventInfo)
            {
                typedEventInfo.actions?.Invoke();
            }
            else
            {
                Debug.LogError($"触发不带参数事件失败：{name} 不是不带参数事件，实际类型: {eventInfo.GetEventType()}");
            }
        }
    }
    #endregion

    #region 一次性事件监听
    /// <summary>
    /// 添加一次性带参数事件监听
    /// </summary>
    public void AddOneTimeEventListener<T>(string name, UnityAction<T> action)
    {
        UnityAction<T> wrappedAction = null;
        wrappedAction = (data) =>
        {
            action(data);
            RemoveEventListener(name, wrappedAction);
        };
        AddEventListener(name, wrappedAction);
    }

    /// <summary>
    /// 添加一次性不带参数事件监听
    /// </summary>
    public void AddOneTimeEventListener(string name, UnityAction action)
    {
        UnityAction wrappedAction = null;
        wrappedAction = () =>
        {
            action();
            RemoveEventListener(name, wrappedAction);
        };
        AddEventListener(name, wrappedAction);
    }
    #endregion

    #region 其他方法
    /// <summary>
    /// 清空事件中心
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
    /// <summary>
    /// 获取指定事件的监听器数量
    /// </summary>
    public int GetEventListenerCount(string name)
    {
        return eventDic.TryGetValue(name, out IEventInfo eventInfo) ? eventInfo.GetListenerCount() : 0;
    }
    /// <summary>
    /// 打印所有事件及其信息（用于调试）
    /// </summary>
    public void PrintAllEvents()
    {
        if (eventDic.Count == 0)
        {
            Debug.Log("事件中心当前没有注册任何事件");
            return;
        }

        foreach (var pair in eventDic)
        {
            Debug.Log($"事件: {pair.Key}, 类型: {pair.Value.GetEventType()}, 监听器数量: {pair.Value.GetListenerCount()}");
        }
    }
    #endregion
}


public interface IEventInfo
{
    int GetListenerCount();
    string GetEventType();
}

/// <summary>
/// 带参数的事件信息
/// </summary>
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }

    public int GetListenerCount()
    {
        return actions != null ? actions.GetInvocationList().Length : 0;
    }

    public string GetEventType()
    {
        return $"EventInfo<{typeof(T).Name}>";
    }
}

/// <summary>
/// 不带参数的事件信息
/// </summary>
public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }

    public int GetListenerCount()
    {
        return actions != null ? actions.GetInvocationList().Length : 0;
    }

    public string GetEventType()
    {
        return "EventInfo";
    }
}
