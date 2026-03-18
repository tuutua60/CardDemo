using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TogHeroSlot:MonoBehaviour
{
    public Toggle toggle;
    public Image heroIcon;
    public Text txtName;
    public Text txtSeatID;

    public int heroID;
    /// <summary>
    /// T1 -- 座位ID
    /// T2 -- 是否选中
    /// </summary>
    private UnityAction<int,bool> OnValueChange;

    public void Initialize(int heroID,string txtName, UnityAction<int,bool> onValueChange)
    {
        this.txtName.text = txtName;
        this.heroID = heroID;
        heroIcon.sprite = ResourcesMgr.Instance.Load<Sprite>("Texture/" + heroID);
        txtSeatID.gameObject.SetActive(false);
        this.OnValueChange = onValueChange;
        RegisterEvent();
    }
    private void RegisterEvent()
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener((value) =>
        {
            if (value) txtSeatID.gameObject.SetActive(true);
            else txtSeatID.gameObject.SetActive(false);
            // 当Toggle状态改变时，调用回调，并传入自己的英雄ID和当前状态
            OnValueChange?.Invoke(this.heroID, value);

        });
    }

    /// <summary>
    /// 更新座位号的显示
    /// </summary>
    /// <param name="seatIndex">座位索引（从0开始）。如果为-1，则表示未被选中。</param>
    public void UpdateSeatDisplay(int seatIndex)
    {
        bool isSelected = seatIndex != -1;
        txtSeatID.gameObject.SetActive(isSelected);
        if (isSelected)
        {
            // 座位号通常从1开始，所以+1
            txtSeatID.text = (seatIndex + 1).ToString();
        }
    }

    public void SetToggleState(bool isOn)
    {
        toggle.isOn = isOn;
    }
}
