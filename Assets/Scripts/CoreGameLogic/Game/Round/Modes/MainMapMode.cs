using Cysharp.Threading.Tasks;
using UnityEngine;
using CoreGameLogic.Managers.AppManagers;
using GameFramework.Manager;

/// <summary>
/// 主地图玩法模式
/// </summary>
public class MainMapMode : IGameplayMode
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    private RoundContext _roundContext;

    /// <summary>
    /// 主地图已用时间（秒）
    /// </summary>
    private float _elapsedTime;
    public float ElapsedTime => _elapsedTime;

    private RoundFlow _roundFlow;
    private EventManager _eventManager;
    private UIManager _uiManager;
    private EffectManager _effectManager;
    private HuntingSoundManager _soundManager;    
    private PlayerControlManager _playerControlManager;
    private SettlementManager _settlementManager;

    public MainMapMode(RoundContext context, RoundFlow roundFlow)
    {
        RegisterServices();
        RegisterEvents();

        _roundContext = context;
        _roundFlow = roundFlow;
    }

    public async UniTask EnterAsync()
    {
        _elapsedTime = 0f;

        // 设置玩法规则
        SetPlayRules();

        // 切换到主地图界面
        var uiGameplay = _uiManager.GetUI<UIGameplay>("UIGameplay");
        uiGameplay.SwitchToMode(EGameplayMode.MainMap);

        // 禁止游玩界面操作
        uiGameplay.SetClickable(false);

        // 切换到摇杆操作
        _playerControlManager.SwitchToJoystick();

        // 播放倒计时动画
        var uiCountDown = await _uiManager.OpenUIAsync<UICountdown>("UICountdown", UIManager.UILayer.Fixed);
        await uiCountDown.PlayCountdownAsync(new[] { "5", "4", "3", "2", "1", "准备..", "开始.", "战斗!!" }, 8f);

        // 恢复游玩界面操作
        uiGameplay.SetClickable(true);
    }

    public async UniTask ExitAsync(EGameplayMode? nextMode)
    {
        UnregisterEvents();

        switch (nextMode)
        {
            case EGameplayMode.HiddenMap:
                {
                    // 播放假结算面板动画
                    var uiPopupSettlementSnowFake = _uiManager.GetUI<UIPopupSettlementSnowFake>("UIPopupSettlementSnowFake");
                    await UniTask.WhenAll(
                        uiPopupSettlementSnowFake.PlayWindowShakeAsync(),
                        uiPopupSettlementSnowFake.PlayButtonGlowAsync()
                    );

                    // 播放爆炸特效和音效
                    await _effectManager.PlayOneShotAsync("Assets/Arts/Prefabs/Effects/Settlement_Explosion");
                    _soundManager.PlaySettlementPanelExplosion();
                    await UniTask.Delay(200); // 等待200毫秒模拟爆炸动画
                    _uiManager.CloseUI("UIPopupSettlementSnowFake");

                    // 播放淡入动画与音效
                    _soundManager.PlayLightTransition();
                    var uiLoading = await _uiManager.OpenUIAsync<UILoading>("UILoading", UIManager.UILayer.Loading);
                    await uiLoading.PlayFadeInAsync();
                }
                break;
        }
    }

    public void DoUpdate(float dt)
    {
        _elapsedTime += dt;

        TriggerTimeUpdated(new TimeUpdatedEventArgs 
        {
            Seconds = _elapsedTime, IsCountdown = false 
        });
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _effectManager = GameServiceLocator.EffectManager;
        _soundManager = GameServiceLocator.GetAppManager<HuntingSoundManager>();
        _playerControlManager = GameServiceLocator.GetRoundManager<PlayerControlManager>();
        _settlementManager = GameServiceLocator.GetRoundManager<SettlementManager>();
    }

    /// <summary>
    /// 设置玩法规则
    /// </summary>
    private void SetPlayRules()
    {
        _roundFlow.SetPlayRule<ITimingRule>(new ElapsedTimingRule (this));
        _roundFlow.SetPlayRule<IUIGameplayComponentVisibilityRule>(new MainMapComponentVisibilityRule());
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
    }

    /// <summary>
    /// 肉条刻度完成事件回调
    /// </summary>
    private async void OnMeatScaleCompleted(MeatScaleCompletedEventArgs args)
    {
        var uiGameplay = _uiManager.GetUI<UIGameplay>("UIGameplay");
        if (args.CompletedScaleCount >= args.TotalScaleCount)
        {
            // 播放倒计时动画
            var uiCountDown = await _uiManager.OpenUIAsync<UICountdown>("UICountdown", UIManager.UILayer.Fixed);
            await UniTask.WhenAll(
                uiGameplay.PlayTipAnimationAsync("肉条已满，即将结算！", Color.red, 10f),
                uiCountDown.PlayCountdownAsync(new[] { "10", "9", "8", "7", "6", "5", "4", "3", "2", "1" }, 10f)
            );

            // 开始结算
            _roundFlow.StartSettlement();
            if (_roundContext.HasHiddenMap)
                await _uiManager.OpenUIAsync<UIPopupSettlementSnowFake>("UIPopupSettlementSnowFake", UIManager.UILayer.PopUp);
            else
                await _uiManager.OpenUIAsync<UIPopupSettlementNormal>("UIPopupSettlementNormal", UIManager.UILayer.PopUp);
            _settlementManager.CalculateReward();
        }
        else
        {
            // 播放狩猎进度提示动画
            int percent = args.CompletedScaleCount * 100 / args.TotalScaleCount;
            uiGameplay.PlayTipAnimationAsync($"狩猎进度已完成{percent}%", Color.yellow, 6f).Forget();
        }
    }

    /// <summary>
    /// 触发时间更新事件
    /// </summary>
    private void TriggerTimeUpdated(TimeUpdatedEventArgs args)
    {
        _eventManager.Trigger(RoundEvents.TimeUpdated, args);
    }
    #endregion
}