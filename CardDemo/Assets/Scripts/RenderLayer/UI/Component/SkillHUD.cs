using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillHUD : MonoBehaviour
{
    public Image imgSkill;
    public Text txtSkillName;
    public CanvasGroup canvasGroup;

    private Vector3 tempPos;

    public void Open(Sprite img, string name)
    {
        //记录初始位置
        if(tempPos != Vector3.zero)
            this.transform.localPosition = tempPos;
        else
            tempPos = this.transform.localPosition;

        //更改内容
        canvasGroup.alpha = 1;
        imgSkill.sprite = img;
        txtSkillName.text = name;

        //设置显示
        this.gameObject.SetActive(true);

        //动画
        this.transform.DOLocalMoveX(-285,0.6f).SetDelay(0.4f);
        this.canvasGroup.DOFade(0, 0.6f).SetDelay(0.8f);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
