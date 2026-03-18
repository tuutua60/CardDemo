using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDPanel : BasePanel
{

    public SkillHUD skillHUD;

    public override void ShowMe()
    {
        base.ShowMe();
        CloseSkillHUD();
    }

    public void OnHeroWound(HeroWoundEvent args)
    {
        PoolMgr.Instance.GetObject(AssetDataPath.HUDPath+ "DamageText", (o) =>
        {
            o.GetComponent<DamageTextRender>().Initialize(args.woundHero, args.damage);
        });
    }

    public void OnHeroRestore(HeroRestoreEvent args)
    {
        PoolMgr.Instance.GetObject(AssetDataPath.HUDPath + "RestoreHPText", (o) =>
        {
            o.GetComponent<DamageTextRender>().Initialize(args.restoreHero, args.restore);
        });
    }

    public void OpenSkillHUD(int id,string name)
    {
        Sprite heroSprite = ResourcesMgr.Instance.Load<Sprite>("Texture/" + id);
        this.skillHUD.Open(heroSprite,name);
        DelayCallMgr.Instance.DelayCall(1500, CloseSkillHUD);
    }

    public void CloseSkillHUD()
    {
        this.skillHUD.Close();
    }
}
