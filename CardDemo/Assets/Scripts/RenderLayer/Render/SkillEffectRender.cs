using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectRender : BaseRenderObject
{

    private ParticleSystem particle;

    public void OnCreate(HeroLogic hero)
    {
        //获取组件
        particle = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        //初始化位置
        this.transform.position = hero.LogicPosition.vec3;
        //初始化朝向
        int dir = hero.data.globalTeamType==E_GlobalTeamType.Player?0:180;
        this.transform.rotation = Quaternion.Euler(0, dir, 0);
        //重新播放特效
        particle.Play(true);
        //DelayCallMgr.Instance.DelayCall(2000, Clear);
        Invoke("Clear",2f);
    }

    public void Clear()
    {
        PoolMgr.Instance.PushObject(this.gameObject.name, this.gameObject);
    }
}
