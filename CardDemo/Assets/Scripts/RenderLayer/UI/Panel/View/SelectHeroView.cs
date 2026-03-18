using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectHeroView : MonoBehaviour
{
    //开始按钮
    public Button btnStart;

    public Button btnQuit;
    /// <summary>
    /// 英雄可拖拽位置列表
    /// </summary>
    public List<Transform> playerPosList;
    /// <summary>
    /// 英雄卡槽父对象
    /// </summary>
    public Transform slotContentTrans;
}
