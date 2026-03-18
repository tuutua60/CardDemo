using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffectRenderFactory : BaseFactory
{
    public override void CreateGameObject(CreateAssetEvent e)
    {
        CreateBuffEffectRenderEvent args = e as CreateBuffEffectRenderEvent;
        switch (args.createType)
        {
            case E_CreateAssetType.Pool:
                break;
            case E_CreateAssetType.PoolAsync:
                LoadGameObjectFromPoolAsync(args.assetPath, (o) =>
                {
                    o.GetComponent<BuffEffectRender>().Initialize(args.heroRender,args.buffLogic);
                });
                break;
            case E_CreateAssetType.Resources:
                break;
            case E_CreateAssetType.ResourcesAsync:
                break;
        }
    }
}
