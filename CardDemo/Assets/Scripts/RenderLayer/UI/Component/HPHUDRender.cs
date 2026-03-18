using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPHUDRender : BaseRenderObject
{

    //三个slider
    public Slider hpSlider;
    public Slider hpDamageAnimSlider;
    public Slider rageSlider;
    //Buff栏父对象
    public Transform buffParent;

    private Transform heroHUDParent;

    public void Initialize(HeroRender heroRender)
    {
        this.transform.SetParent(UIManager.Instance.GetPanel<HUDPanel>().transform, true);
        if(heroRender.hero.data.globalTeamType==E_GlobalTeamType.Player)
            heroHUDParent = heroRender.transform.Find("HUDParentPlayer");
        else heroHUDParent = heroRender.transform.Find("HUDParentEnemy");
        hpSlider.value = 1;
        hpDamageAnimSlider.value = 1;
        rageSlider.value = 0;
        for (int i = 0; i < buffParent.childCount; i++)
        {
            PoolMgr.Instance.PushObject(buffParent.GetChild(i).gameObject.name,buffParent.GetChild(i).gameObject);
        }
    }
    
    public void UpdateHPSlider(float value)
    {
        hpSlider.value = value;
        //设置缓动动画
        hpDamageAnimSlider.DOValue(value, 0.5f).SetDelay(0.4f).OnComplete(() =>
        {
            if (value == 0) Clear();
        });
    }
    public void UpdateRageSlider(float value)
    {
        rageSlider.DOValue(value, 0.5f).SetDelay(0.3f);
    }

    public void UpdateBuffIcon(SortedDictionary<int,BuffLogic> buffDic)
    {
        //todo 更新buff图标
        for (int i = buffParent.childCount-1; i >= 0; i--)
        {
            PoolMgr.Instance.PushObject(buffParent.GetChild(i).gameObject.name,buffParent.GetChild(i).gameObject);
        }
        foreach (var buff in buffDic.Values)
        {
            GameObject o = PoolMgr.Instance.GetObject(AssetDataPath.BuffEffectPath + "BuffIcon");
            o.GetComponent<Image>().sprite = buff.config.buffIcon;
            o.transform.localScale = Vector3.one;
            o.transform.position = buffParent.transform.position;
            o.transform.SetParent(buffParent, true);

        }
    }

    public void Update()
    {
        if (heroHUDParent == null) return;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(heroHUDParent.position);
        this.transform.position = screenPos;
    }

    public void Clear()
    {
        if (heroHUDParent == null) return;
        heroHUDParent = null;
        PoolMgr.Instance.PushObject(this.gameObject.name,this.gameObject);
    }
}
