using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFlicker : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public float targetAlpha = 0f;

    public float deltaTime = 1f;

    public float delayTime = 0f;

    public int loopCount = -1;

    public LoopType loopType = LoopType.Yoyo;
    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        TweenerCore<float, float, FloatOptions> core = canvasGroup.DOFade(targetAlpha, deltaTime);
        if(delayTime>0) core.SetDelay(delayTime);
        if(loopCount!=1&&loopCount!=0) core.SetLoops(loopCount, loopType);
    }
}
