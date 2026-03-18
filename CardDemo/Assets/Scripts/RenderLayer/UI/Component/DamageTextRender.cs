using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextRender : BaseRenderObject
{

    private Text txtDamage;
    private CanvasGroup canvasGroup;

    public void Initialize(HeroLogic hero,VInt damage)
    {
        txtDamage = this.GetComponent<Text>();
        txtDamage.text = damage.RawInt.ToString();
        this.transform.SetParent(UIManager.Instance.GetPanel<HUDPanel>().transform);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(hero.LogicPosition.vec3) + new Vector3(0, 150);
        this.transform.position = screenPos;
        canvasGroup.DOFade(0.1f, 1f);
        this.transform.DOMove(screenPos + new Vector2(Random.Range(-100, 100), Random.Range(100, 250)), 1f).OnComplete(() =>
        {
            if(this != null) PoolMgr.Instance.PushObject(this.gameObject.name, this.gameObject);
        });
    }
}
