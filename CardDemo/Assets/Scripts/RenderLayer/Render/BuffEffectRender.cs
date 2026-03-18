using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffectRender : BaseRenderObject
{
    private BuffLogic buffLogic;
    private HeroRender heroRender;
    public void Initialize(HeroRender heroRender,BuffLogic buffLogic)
    {
        this.buffLogic = buffLogic;
        this.heroRender = heroRender;
        this.transform.position = heroRender.transform.position;
        this.transform.SetParent(heroRender.transform,true);
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        EventCenter.Instance.AddEventListener<RemoveBuffEvent>(EventConfig.RemoveBuff + buffLogic.buffOwner.data.globalTeamType +
                                                               buffLogic.buffOwner.data.seatID+buffLogic.config.buffID,Clear);
        EventCenter.Instance.AddEventListener<BattleResultEvent>(EventConfig.OnBattleEnd, Clear);
    }

    private void UnRegisterEvent()
    {
        // 在注销事件前，增加空值检查
        if (heroRender == null)
        {
            return;
        }
        EventCenter.Instance.RemoveEventListener<RemoveBuffEvent>(EventConfig.RemoveBuff + buffLogic.buffOwner.data.globalTeamType + 
                                                               buffLogic.buffOwner.data.seatID + buffLogic.config.buffID, Clear);
        EventCenter.Instance.RemoveEventListener<BattleResultEvent>(EventConfig.OnBattleEnd, Clear);
    }

    public void Clear(RemoveBuffEvent args)
    {
        UnRegisterEvent();
        buffLogic = null;
        heroRender = null;
        PoolMgr.Instance.PushObject(this.gameObject.name,this.gameObject);
    }
    public void Clear(BattleResultEvent args)
    {
        UnRegisterEvent();
        buffLogic = null;
        heroRender = null;
        PoolMgr.Instance.PushObject(this.gameObject.name, this.gameObject);
    }
}
