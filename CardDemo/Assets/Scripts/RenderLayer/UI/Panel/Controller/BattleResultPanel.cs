using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResultPanel : BasePanel
{
    private BattleResultView view;
    private BattleResultModel model;
    protected override void Awake()
    {
        base.Awake();
        view = GetComponent<BattleResultView>();
        model = new BattleResultModel();
        RegisterEvent();
    }
    public void Initialize(bool isPlayerWin)
    {
        Time.timeScale = 1f;
        model.isPlayerWin = isPlayerWin;
        view.txtResult.text = model.isPlayerWin ? "胜利" : "失败";
        DelayCallMgr.Instance.ClearList();
    }

    private void RegisterEvent()
    {
        view.btnRecall.onClick.AddListener(() =>
        {
            GameManager.Instance.RecallGame();
            UIManager.Instance.HidePanel<BattleResultPanel>();
        });
        view.btnRestart.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<BattleResultPanel>();
            UIManager.Instance.ShowPanel<SelectHeroPanel>();
        });
    }
}
