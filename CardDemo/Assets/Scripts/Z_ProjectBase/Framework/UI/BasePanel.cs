using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 面板基类
/// 提供面板共有的显示、隐藏动画等功能。
/// </summary>
public class BasePanel : MonoBehaviour
{
    // CanvasGroup组件，用于控制UI的整体透明度和交互性
    private CanvasGroup canvasGroup;
    private Tweener fadeTweener; // 用于管理淡入淡出动画

    protected float showDeltaTime = 0.3f; // 动画持续时间
    protected float hideDeltaTime = 0.3f; // 动画持续时间

    /// <summary>
    /// 子类可以重写此方法来初始化控件引用和事件监听。
    /// </summary>
    protected virtual void Awake()
    {
        // 确保面板上有CanvasGroup组件
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// 显示面板（淡入动画）
    /// </summary>
    public virtual void ShowMe()
    {
        // 如果当前有动画在播放，先杀死它
        fadeTweener?.Kill();
        // 设置为可交互状态
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        // 使用DOTween执行淡入动画
        canvasGroup.alpha = 0;
        fadeTweener = canvasGroup.DOFade(1, showDeltaTime).SetUpdate(true);
    }

    /// <summary>
    /// 隐藏面板（淡出动画）
    /// </summary>
    /// <param name="callback">隐藏动画完成后的回调函数</param>
    public virtual void HideMe(UnityAction callback)
    {
        // 如果当前有动画在播放，先杀死它
        fadeTweener?.Kill();
        // 设置为不可交互状态
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        // 使用DOTween执行淡出动画，并在完成后执行回调
        canvasGroup.alpha = 1;
        fadeTweener = canvasGroup.DOFade(0, hideDeltaTime).SetUpdate(true).OnComplete(() =>
        {
            callback?.Invoke();
        });
    }
    /// <summary>
    /// 在对象销毁时，子类可以重写此方法来清理事件监听。
    /// </summary>
    protected virtual void OnDestroy()
    {
        // 停止所有正在播放的动画，防止在对象销毁后继续执行
        fadeTweener?.Kill();
    }
}
