using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayCallMgr : BaseManager<DelayCallMgr>,IFrameSyncBehaviour
{
    private List<DelayCaller> delayCallerList = new List<DelayCaller>();

    public void Initialize()
    {
        //DebugMgr.Log("创建DelayCallMgr");
        FrameSyncMgr.Instance.AddFrameUpdateListener(OnFrameUpdate);
    }

    public void DelayCall(VInt delayTime, Action callback, int loop = 1)
    {
#if CLIENT_LOGIC
        DelayCaller delayCaller = new DelayCaller(delayTime, callback, loop);
        delayCallerList.Add(delayCaller);
#else
        //服务端立即触发回调，无需延迟
        for(int i = 0; i < loop; i++)
        {
            callback?.Invoke();
        }
#endif
    }
    public void OnFrameUpdate()
    {
        for (int i = 0; i < delayCallerList.Count; i++)
        {
            delayCallerList[i].OnFrameUpdate();
        }
        //检测是否有完成工作的计时器
        for (int i = delayCallerList.Count - 1; i >= 0; i--)
        {
            if (delayCallerList[i].isWorkFinish) delayCallerList.Remove(delayCallerList[i]);
        }
    }
    public void Clear()
    {
        delayCallerList.Clear();
        FrameSyncMgr.Instance.RemoveFrameUpdateListener(OnFrameUpdate);
    }
    public void ClearList()
    {
        delayCallerList.Clear();
    }
}

public class DelayCaller
{
    private VInt delayTime;
    private Action onTimeComplete;
    private int loopCount;
    //工作是否完成
    public bool isWorkFinish;
    //当前累计时间
    private VInt currentTime;
    public DelayCaller(VInt delayTime, Action callback, int loop = 1)
    {
        isWorkFinish = false;
        this.delayTime = delayTime;
        this.onTimeComplete = callback;
        this.loopCount = loop;
    }
    public void OnFrameUpdate()
    {
        currentTime += (VInt)FrameConfig.LogicFrameIntervalMS;
        if (currentTime >= delayTime)
        {
            onTimeComplete?.Invoke();
            currentTime = 0;
            loopCount--;
            if (loopCount == 0) isWorkFinish = true;
        }
    }
}
