using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

/// <summary>
/// UI层级
/// </summary>
public enum E_UI_Layer
{
    Bottom,
    Middle,
    Top,
    System,
}

/// <summary>
/// UI管理器
/// 1.管理所有显示的面板
/// 2.提供给外部 显示和隐藏等等接口
/// </summary>
public class UIManager : BaseManager<UIManager>
{
    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    private Transform bottom;
    private Transform middle;
    private Transform top;
    private Transform system;

    //记录我们UI的Canvas父对象 方便以后外部可能会使用它
    public RectTransform canvas;

    public UIManager()
    {
        //创建Canvas 让其过场景的时候 不被移除
        GameObject obj = ResourcesMgr.Instance.Load<GameObject>("UI/Canvas");
        canvas = obj.transform as RectTransform;
        GameObject.DontDestroyOnLoad(obj);

        //找到各层
        bottom = canvas.Find("Bottom");
        middle = canvas.Find("Middle");
        top = canvas.Find("Top");
        system = canvas.Find("System");

        //创建EventSystem 让其过场景的时候 不被移除
        obj = ResourcesMgr.Instance.Load<GameObject>("UI/EventSystem");
        GameObject.DontDestroyOnLoad(obj);
    }

    public void Initialize()
    {
        RegisterEvent();
        //预加载一些常用面板
        ShowPanel<HUDPanel>(E_UI_Layer.Top);

        UIManager.Instance.ShowPanel<SelectHeroPanel>();

    }

    private void OnBattleEnd(BattleResultEvent args)
    {
        HidePanel<RoundPanel>();
        ShowPanel<BattleResultPanel>(E_UI_Layer.Middle, (o) =>
        {
            o.Initialize(args.isPlayerWin);
        });
    }

    private void RegisterEvent()
    {
        EventCenter.Instance.AddEventListener<BattleResultEvent>(EventConfig.OnBattleEnd, OnBattleEnd);
    }

    private void UnregisterEvent()
    {

    }

    /// <summary>
    /// 通过层级枚举 得到对应层级的父对象
    /// </summary>
    /// <param name="layer">层级</param>
    /// <returns></returns>
    public Transform GetLayerFather(E_UI_Layer layer)
    {
        switch(layer)
        {
            case E_UI_Layer.Bottom:
                return this.bottom;
            case E_UI_Layer.Middle:
                return this.middle;
            case E_UI_Layer.Top:
                return this.top;
            case E_UI_Layer.System:
                return this.system;
        }
        return null;
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板脚本类型</typeparam>
    /// <param name="layer">显示在哪一层</param>
    /// <param name="callBack">当面板预设体创建成功后 你想做的事</param>
    public void ShowPanel<T>(E_UI_Layer layer = E_UI_Layer.Middle, UnityAction<T> callBack = null) where T:BasePanel
    {
        string panelName = typeof(T).Name;
        string resourcePath = "UI/" + panelName; // 资源路径作为键名
        // 添加面板重复显示保护
        if (panelDic.ContainsKey(panelName))
        {
            // 添加检查，防止面板被销毁但字典中还有引用
            if (panelDic[panelName] == null)
            {
                panelDic.Remove(panelName);
                // 继续执行加载逻辑
            }
            else
            {
                panelDic[panelName].ShowMe();
                callBack?.Invoke(panelDic[panelName] as T);
                return;
            }
        }
        #region 通过缓存池加载面板
        PoolMgr.Instance.GetObject(resourcePath, (obj) =>
        {
            //把他作为 Canvas的子对象
            //并且 要设置它的相对位置
            //找到父对象 你到底显示在哪一层
            Transform father = GetLayerFather(layer);
            //设置父对象  设置相对位置和大小
            obj.transform.SetParent(father);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;
            //得到预设体身上的面板脚本
            T panel = obj.GetComponent<T>();
            // 处理面板创建完成后的逻辑
            if (callBack != null)
                callBack(panel);

            panel.ShowMe();

            //把面板存起来
            panelDic.Add(panelName, panel);
        });
        #endregion
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    public void HidePanel<T>(bool immediate = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        string resourcePath = "UI/" + panelName; // 资源路径作为键名
        if (panelDic.ContainsKey(panelName) && panelDic[panelName] != null)
        {
            if (immediate)
            {
                // 立即隐藏并回收
                PoolMgr.Instance.PushObject(resourcePath, panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            }
            else
            {
                // 执行隐藏动画后再回收
                panelDic[panelName].HideMe(() =>
                {
                    PoolMgr.Instance.PushObject(resourcePath, panelDic[panelName].gameObject);
                    panelDic.Remove(panelName);
                });
            }
        }
    }

    /// <summary>
    /// 得到某一个已经显示的面板 方便外部使用
    /// </summary>
    public T GetPanel<T>() where T:BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        return null;
    }

    /// <summary>
    /// 给控件添加自定义事件监听
    /// </summary>
    /// <param name="control">控件对象</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBack">事件的响应函数</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }
}
