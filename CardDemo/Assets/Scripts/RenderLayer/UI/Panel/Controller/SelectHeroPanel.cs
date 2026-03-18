using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectHeroPanel : BasePanel
{
    private SelectHeroView view;
    private SelectHeroModel model = new SelectHeroModel();

    /// <summary>
    /// 英雄可拖拽图标字典
    /// </summary>
    private Dictionary<int, DragableImage> dragableImageDic = new Dictionary<int, DragableImage>();
    /// <summary>
    /// 英雄卡槽字典
    /// </summary>
    private Dictionary<int, TogHeroSlot> heroSlotDic = new Dictionary<int, TogHeroSlot>();
    public override void ShowMe()
    {
        base.ShowMe();
        view = GetComponent<SelectHeroView>();
        RegisterEvent();
        model.Initialize();
        GenerateHeroSlot();
    }
    private void RegisterEvent()
    {
        view.btnStart.onClick.AddListener(StartGame);
        view.btnQuit.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        EventCenter.Instance.AddEventListener<HeroSwapEvent>(EventConfig.HeroSwap,OnHeroSwap);
        EventCenter.Instance.AddEventListener<HeroUnselectEvent>(EventConfig.HeroUnselectEvent,OnHeroUnselect);
        EventCenter.Instance.AddEventListener<HeroMoveSlotEvent>(EventConfig.HeroMoveSlotEvent, OnHeroMoveSlot);
    }

    /// <summary>
    /// 英雄拖拽完成
    /// </summary>
    /// <param name="args"></param>
    private void OnHeroSwap(HeroSwapEvent args)
    {
        int tempID = model.playerList[args.fromIndex];
        model.playerList[args.fromIndex] = model.playerList[args.toIndex];
        model.playerList[args.toIndex] = tempID;
        //更新卡槽座位显示
        heroSlotDic[model.playerList[args.fromIndex]].UpdateSeatDisplay(args.fromIndex);
        heroSlotDic[model.playerList[args.toIndex]].UpdateSeatDisplay(args.toIndex);
    }

    private void OnHeroUnselect(HeroUnselectEvent args)
    {
        heroSlotDic[args.heroID].SetToggleState(false);
    }

    private void HeroUnselect(int heroID)
    {
        // 找到该英雄在列表中的位置
        int heroIndex = model.playerList.IndexOf(heroID);
        if (heroIndex != -1)
        {
            // 将该位置设置为空位（-1）
            model.playerList[heroIndex] = -1;
            //移除可拖拽图标
            RemoveDragableImage(heroIndex);
        }
    }

    private void OnHeroMoveSlot(HeroMoveSlotEvent args)
    {
        //确保目标位置是空的
        if (model.playerList[args.toIndex] != -1)
        {
            Debug.LogError($"尝试将英雄 {args.heroID} 移动到非空槽位 {args.toIndex}！");
            return;
        }

        //更新数据模型
        model.playerList[args.toIndex] = args.heroID;
        model.playerList[args.fromIndex] = -1; // 原位置变为空

        //更新UI引用
        if (dragableImageDic.TryGetValue(args.fromIndex, out DragableImage movedImage))
        {
            //从旧索引移除
            dragableImageDic.Remove(args.fromIndex);
            //添加到新索引
            dragableImageDic.Add(args.toIndex, movedImage);

            //更新 DragableImage 自身的内部状态
            movedImage.UpdateSlot(args.toIndex, view.playerPosList[args.toIndex]);
        }

        //更新下方英雄选择卡槽的UI状态
        if (heroSlotDic.TryGetValue(args.heroID, out TogHeroSlot slot))
        {
            slot.UpdateSeatDisplay(args.toIndex);
        }

    }

    /// <summary>
    /// 点击btnStart时调用
    /// </summary>
    private void StartGame()
    {
        //将Model的数据传出去
        //如果玩家列表存在-1(自定规则)，说明场上还有空位置，直接return
        //测试
        //if (model.playerList.Contains(-1)) return;
        EventCenter.Instance.EventTrigger<SelectHeroEvent>(EventConfig.OnBattleStart, new SelectHeroEvent()
        {
            playerList = model.playerList,
            enemyList = model.enemyList
        });
        UIManager.Instance.HidePanel<SelectHeroPanel>();
    }

    /// <summary>
    /// 生成可拖拽的英雄图标
    /// </summary>
    private void GenerateDragableImage(int index)
    {
        Transform tmpTrans = view.playerPosList[index];
        int tmpInt = model.playerList[index];
        ResourcesMgr.Instance.LoadAsync<GameObject>(AssetDataPath.HUDPath + "DragableImage", (obj) =>
        {
            //设置父对象
            obj.transform.SetParent(tmpTrans, false);
            obj.transform.localPosition = Vector3.zero;

            //初始化DragableImage
            DragableImage dragableImage = obj.GetComponent<DragableImage>();
            dragableImage.Initialize(tmpInt, tmpTrans, index);
            dragableImageDic.Add(index, dragableImage);
        });
    }

    private void RemoveDragableImage(int index)
    {
        if (dragableImageDic.TryGetValue(index, out DragableImage dragableImage))
        {
            dragableImageDic.Remove(index);
            Destroy(dragableImage.gameObject);
        }
    }

    /// <summary>
    /// 生成英雄卡槽
    /// </summary>
    private void GenerateHeroSlot()
    {
        //先读表得到数据
        var heroDataDic = BinaryDataMgr.Instance.GetTable<T_HeroDataContainer>().dataDic;
        foreach (var item in heroDataDic.Values)
        {
            var config = item;
            //生成英雄卡槽
            PoolMgr.Instance.GetObject(AssetDataPath.HUDPath + "togHeroSlot", (o) =>
            {
                //设置父对象
                o.transform.SetParent(view.slotContentTrans,false);

                TogHeroSlot slot = o.GetComponent<TogHeroSlot>();
                //传入英雄名字和回调
                slot.Initialize(config.id, config.name, OnHeroSlotToggled);
                // 将创建的slot存起来，方便后续查找和更新
                heroSlotDic.Add(config.id, slot);
            });
        }
    }

    private void OnHeroSlotToggled(int heroID, bool isOn)
    {
        if (isOn)
        {
            // --- 添加英雄 ---
            // 找到第一个空位（值为-1）
            int emptyIndex = model.playerList.IndexOf(-1);
            if (emptyIndex != -1)
            {
                // 在空位上放入英雄ID
                model.playerList[emptyIndex] = heroID;
                //生成可拖拽图标
                GenerateDragableImage(emptyIndex);
                //改变座位ID
                heroSlotDic[heroID].UpdateSeatDisplay(emptyIndex);
            }
            else
            {
                // 如果没有空位，则取消当前Toggle的选中状态
                Debug.Log("队伍已满，无法添加！");
                heroSlotDic[heroID].SetToggleState(false);
            }
        }
        else
        {
            // --- 移除英雄 ---
            HeroUnselect(heroID);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventCenter.Instance.RemoveEventListener<HeroSwapEvent>(EventConfig.HeroSwap, OnHeroSwap);
    }
}
