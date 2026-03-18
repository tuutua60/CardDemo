using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    /// <summary>
    /// Image控件
    /// </summary>
    private Image imgHero;
    /// <summary>
    /// Mask父对象
    /// </summary>
    private Transform fatherTrans;
    /// <summary>
    /// 英雄ID
    /// </summary>
    private int heroID;
    /// <summary>
    /// 英雄图标
    /// </summary>
    private Sprite heroIcon;
    /// <summary>
    /// 在Model中的位置索引
    /// </summary>
    public int slotIndex;

    private void Awake()
    {
        imgHero = GetComponent<Image>();
    }

    public void Initialize(int heroID,Transform fatherTrans, int slotIndex)
    {
        this.heroID = heroID;
        this.fatherTrans = fatherTrans;
        this.slotIndex = slotIndex;
        ChangeHero(heroID,imgHero.sprite);
    }

    private void ChangeHero(int heroID,Sprite heroIcon)
    {
        this.heroIcon = heroIcon;
        this.heroID = heroID;
        //根据英雄ID，设置图片
        imgHero.sprite = ResourcesMgr.Instance.Load<Sprite>("Texture/" + heroID);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DebugMgr.Log("开始拖曳"+eventData.pointerDrag.name);
        // 提升到父级的父级，使其可以自由移动并显示在顶层
        this.transform.SetParent(fatherTrans.parent, true);
        // 拖拽开始时，禁用自身的射线检测，这样才能检测到它下方的物体
        imgHero.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        eventData.pointerDrag.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DebugMgr.Log("结束拖曳" + eventData.pointerDrag.name);
        // 拖拽结束后，恢复自身的射线检测能力
        imgHero.raycastTarget = true;
        GameObject objectUnderPointer = eventData.pointerCurrentRaycast.gameObject;
        if (objectUnderPointer != null && objectUnderPointer != this.gameObject)
        {
            DragableImage otherHeroImage = objectUnderPointer.GetComponent<DragableImage>();
            //如果拖到另一个英雄图标上
            if (otherHeroImage != null)
            {
                DebugMgr.Log("拖到了另一个英雄上: " + otherHeroImage.heroID);
                //交换双方图标和英雄ID
                Sprite tempSprite = this.imgHero.sprite;
                int tempHeroID = this.heroID;
                this.ChangeHero(otherHeroImage.heroID, otherHeroImage.heroIcon);
                otherHeroImage.ChangeHero(tempHeroID, tempSprite);
                //触发交换位置事件给model更新数据
                EventCenter.Instance.EventTrigger<HeroSwapEvent>(EventConfig.HeroSwap,new HeroSwapEvent()
                {
                    fromIndex = this.slotIndex,
                    toIndex = otherHeroImage.slotIndex
                });
            }
            //如果拖到空位置上
            else if (otherHeroImage == null && objectUnderPointer.name=="posMask")
            {
                DebugMgr.Log("拖到了空位置上");
                EventCenter.Instance.EventTrigger<HeroMoveSlotEvent>(EventConfig.HeroMoveSlotEvent, new HeroMoveSlotEvent()
                {
                    heroID = this.heroID,
                    toIndex = int.Parse(objectUnderPointer.transform.parent.name) - 1,
                    fromIndex = this.slotIndex
                });
            }
            //如果拖到卡槽，则把英雄放到卡槽，同时座位变空
            else if (otherHeroImage == null && objectUnderPointer.CompareTag("HeroSlot"))
            {
                DebugMgr.Log("拖到了英雄选择列表区域");
                // 触发英雄取消选择事件
                EventCenter.Instance.EventTrigger<HeroUnselectEvent>(EventConfig.HeroUnselectEvent, new HeroUnselectEvent()
                {
                    fromIndex = this.slotIndex,
                    heroID = this.heroID
                });

                // 触发事件后，这个拖拽图标的生命周期就结束了，将它销毁/回收到对象池
                // 注意：这里的回收逻辑将由SelectHeroPanel在监听到事件后处理，这里不需要再写
                // 为了防止闪烁，可以先隐藏自己
                this.gameObject.SetActive(false);
                return; // 提前返回，不再执行后面的SetParent逻辑
            }
        }
        // 如果没有拖到任何有效区域，则返回原位
        this.transform.SetParent(fatherTrans, false);
        this.transform.localPosition = Vector3.zero;

    }

    /// <summary>
    /// 由外部控制器调用，用于更新该图标的槽位信息
    /// </summary>
    public void UpdateSlot(int newSlotIndex, Transform newFather)
    {
        this.slotIndex = newSlotIndex;
        this.fatherTrans = newFather;
    }
}
