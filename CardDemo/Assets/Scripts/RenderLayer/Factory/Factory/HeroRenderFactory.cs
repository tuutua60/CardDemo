using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroRenderFactory : BaseFactory
{
    public override void CreateGameObject(CreateAssetEvent e)
    {
        CreateHeroRenderEvent args = e as CreateHeroRenderEvent;
        switch (args.createType)
        {
            case E_CreateAssetType.Pool:
                GameObject obj = LoadGameObjectFromPool(args.assetPath);
                HeroRender render = obj.GetComponent<HeroRender>();
                if (render == null) render = obj.AddComponent<HeroRender>();
                render.OnCreate(args.heroLogic);
                break;
            case E_CreateAssetType.PoolAsync:
                break;
            case E_CreateAssetType.Resources:
                break;
            case E_CreateAssetType.ResourcesAsync:
                break;
        }
    }
}
