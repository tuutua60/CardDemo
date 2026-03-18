using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMgr : BaseManager<SkillMgr>
{

    private BuffConfigContainer BuffConfigContainer;
    private SkillConfigContainer SkillConfigContainer;

    /// <summary>
    /// 根据技能ID获取技能配置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SkillConfig GetSkillConfig(int heroID, bool isNormalAtk = true)
    {
        int skillID = isNormalAtk ? heroID * 10 : heroID * 10 + 1;
#if CLIENT_LOGIC
        SkillConfig config = ResourcesMgr.Instance.Load<SkillConfig>(AssetDataPath.SkillConfigPath + skillID);
#else
        if(SkillConfigContainer==null)
            SkillConfigContainer = BinaryDataMgr.Instance.GetTable<SkillConfigContainer>();
        SkillConfig config = SkillConfigContainer.dataDic[skillID];
#endif
        return config;
    }

    /// <summary>
    /// 根据BuffID获取BUff数据
    /// </summary>
    /// <param name="buffID"></param>
    /// <returns></returns>
    public BuffConfig GetBuffConfig(int buffID)
    {
#if CLIENT_LOGIC
        BuffConfig config = ResourcesMgr.Instance.Load<BuffConfig>(AssetDataPath.BuffConfigPath + buffID);
#else
        if(BuffConfigContainer==null)
            BuffConfigContainer = BinaryDataMgr.Instance.GetTable<BuffConfigContainer>();
        BuffConfig config = BuffConfigContainer.dataDic[buffID];
#endif
        return config;
    }

    public void CreateEffect(HeroLogic hero, string effectPath)
    {
#if CLIENT_LOGIC
        PoolMgr.Instance.GetObject(effectPath, (o) =>
        {
            o.AddComponent<SkillEffectRender>().OnCreate(hero);
        });
#endif
    }
}
