using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundPanel : BasePanel
{
    private RoundView view;
    private RoundModel model = new RoundModel();

    protected override void Awake()
    {
        base.Awake();
        view = GetComponent<RoundView>();
        RegisterEvent();
    }

    public override void ShowMe()
    {
        showDeltaTime = 2f;
        base.ShowMe();
        model.Initialize();
        view.Initialize();
    }

    private void RegisterEvent()
    {
        view.btnPause.onClick.AddListener(() =>
        {
            Time.timeScale = Time.timeScale == 0 ? model.nowSpeed : 0;
        });
        view.btnChangeSpeed.onClick.AddListener(() =>
        {
            model.nowSpeed++;
            if (model.nowSpeed > 5)
            {
                model.nowSpeed = 1;
            }
            view.txtSpeed.text = $"X{model.nowSpeed}";
            if (Time.timeScale != 0) Time.timeScale = model.nowSpeed;
        });
        view.btnSkip.onClick.AddListener(() =>
        {
            DebugMgr.LogWarning("跳过战斗");
            //--todo 联调服务端实现跳过逻辑

            //向服务器发送跳过请求
            NetMgr.Instance.SendMsg(new SkipRequestMsg());
        });
        EventCenter.Instance.AddEventListener<RoundStartEvent>(EventConfig.OnRoundStart, UpdateRoundCount);
    }

    private void UpdateRoundCount(RoundStartEvent args)
    {
        view.txtRoundCount.text = args.round.ToString();
    }

    private void Update()
    {
        // 每帧更新当前逻辑帧数显示
        model.nowLogicFrame = FrameConfig.CurrentFrameID;
        view.txtLogicFrame.text = model.nowLogicFrame.ToString();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventCenter.Instance.RemoveEventListener<RoundStartEvent>(EventConfig.OnRoundStart, UpdateRoundCount);
    }
}
