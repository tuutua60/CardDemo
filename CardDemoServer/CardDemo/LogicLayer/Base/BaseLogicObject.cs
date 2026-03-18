using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLogicObject : IFrameSyncBehaviour
{
    public virtual void OnCreate()
    {
        FrameSyncMgr.Instance.AddFrameUpdateListener(OnFrameUpdate);
    }

    public virtual void OnDestroy()
    {
        FrameSyncMgr.Instance.RemoveFrameUpdateListener(OnFrameUpdate);

    }

    public abstract void OnFrameUpdate();
}
