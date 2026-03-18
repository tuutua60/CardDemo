using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if CLIENT_LOGIC
[CreateAssetMenu(fileName = "BuffConfig", menuName = "Config/BuffConfig",order =1)]
public class BuffConfig : ScriptableObject
#else
public class BuffConfig
#endif
{
    /// <summary>
    /// Buff图标
    /// </summary>
    public Sprite buffIcon;
    /// <summary>
    /// BuffID
    /// </summary>
    public int buffID;
    /// <summary>
    /// Buff名称
    /// </summary>
    public string buffName;
    /// <summary>
    /// Buff描述
    /// </summary>
    public string buffDes;
    /// <summary>
    /// 附加概率
    /// </summary>
    public int percentage = 100;
    /// <summary>
    /// 最大叠加层数
    /// </summary>
    public int maxOverlayCount = 1;
    /// <summary>
    /// 持续回合数
    /// </summary>
    public int durationRoundCount = 1;
    /// <summary>
    /// Buff触发时机
    /// </summary>
    public E_BuffTriggerType triggerType;
    [Header("----------")]
    /// <summary>
    /// Buff类型(泛)
    /// </summary>
    public E_BuffType buffType;
    public E_BuffDetailType buffDetailType;
    public E_DEBuffDetailType dEBuffDetailType;
    public E_DamageBuffDetailType damageBuffDetailType;
    public E_ControlBuffDetailType controlBuffDetailType;
    [Header("----------")]
    /// <summary>
    /// Buff倍率
    /// </summary>
    public int rate;
    /// <summary>
    /// Buff特效名
    /// </summary>
    public string buffEffectName;
    /// <summary>
    /// 音效名
    /// </summary>
    public string audioName;

}
