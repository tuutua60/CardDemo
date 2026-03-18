using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreateAssetEvent
{
    public E_CreateAssetType createType;
    public string assetPath;
}

public class CreateHeroRenderEvent : CreateAssetEvent
{
    public HeroLogic heroLogic;
}

public class CreateBuffEffectRenderEvent : CreateAssetEvent
{
    public BuffLogic buffLogic;
    public HeroRender heroRender;
}
