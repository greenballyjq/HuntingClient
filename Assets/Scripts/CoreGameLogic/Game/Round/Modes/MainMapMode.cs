using Cysharp.Threading.Tasks;
using GameFramework.Audio;
using GameFramework.Manager;
using GameFramework.UI;
using UnityEngine;

/// <summary>
/// 主地图玩法模式
/// </summary>
public class MainMapMode : IGameplayMode
{
    private const float StartCountdownDuration = 8f;
    private const float MeatFullCountdownDuration = 10f;
    private const float MeatFullTipDuration = 10f;
    private static readonly string[] StartCountdownTexts =
        { "5", "4", "3", "2", "1", "准备..", "开始.", "战斗!!" };
    private static readonly string[] MeatFullCountdownTexts =
        { "10", "9", "8", "7", "6", "5", "4", "3", "2", "1" };
    /// <summary>
    /// 主地图已用时间（秒）
    /// </summary>
    private float _elapsedTime;
    public float ElapsedTime => _elapsedTime;

    private RoundFlow _roundFlow;
    private EventManager _eventManager;
    private UIManager _uiManager;
    private HuntingConfigManager _configManager;
    private PlayerControlManager _playerControlManager;
    private AudioManager _audioManager;

    public MainMapMode(RoundFlow roundFlow)
    {
        BindServices();
        SubscribeEvents();

        _roundFlow = roundFlow;
    }

    public async UniTask EnterAsync()
    {
        _elapsedTime = 0f;

        // 设置玩法规则
        SetPlayRules();

        // 切换到主地图游玩界面
        var uiGameplay = await _uiManager.OpenAsync<UIGameplay>();
        uiGameplay.SwitchToMode(EGameplayMode.MainMap);

        // 切换到摇杆操作
        _playerControlManager.SwitchToJoystick();

        // 禁止游玩界面操作
        uiGameplay.SetClickable(false);

        var mapRef = _configManager.MapRefSo.Get(_roundFlow.RoundContext.MapData.ID);
        _audioManager.Play(mapRef?.Ambience);
    }

    public async UniTask PresentAsync()
    {
        var uiGameplay = _uiManager.Get<UIGameplay>();
        var roleRef = _configManager.RoleRefSo.Get(_roundFlow.RoundContext.RoleData.ID);
        _audioManager.Play(roleRef?.Opening);

        // 播放倒计时动画
        var uiCountDown = await _uiManager.OpenAsync<UICountdown>();
        await uiCountDown.PlayCountdownAsync(StartCountdownTexts, StartCountdownDuration);

        // 恢复游玩界面操作
        uiGameplay.SetClickable(true);

        var mapBgm = _configManager.MapRefSo.Get(_roundFlow.RoundContext.MapData.ID)?.Bgm;
        _audioManager.PlayMusic(mapBgm, 0f);
    }

    public UniTask ExitAsync()
    {
        UnsubscribeEvents();
        return UniTask.CompletedTask;
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
    private void BindServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _configManager = GameServiceLocator.ConfigManager;
        _audioManager = GameServiceLocator.AudioManager;
        _playerControlManager = GameServiceLocator.GetRoundManager<PlayerControlManager>();
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
    private void SubscribeEvents()
    {
        _eventManager.AddListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnsubscribeEvents()
    {
        _eventManager.RemoveListener(MeatEvents.MeatScaleCompleted, OnMeatScaleCompleted);
    }

    /// <summary>
    /// 肉条刻度完成事件回调
    /// </summary>
    private async void OnMeatScaleCompleted(MeatScaleCompletedEventArgs args)
    {
        var uiGameplay = _uiManager.Get<UIGameplay>();
        if (args.CompletedScaleCount >= args.TotalScaleCount)
        {
            // 播放倒计时动画
            var uiCountDown = await _uiManager.OpenAsync<UICountdown>();
            uiGameplay.PlayTip("肉条已满，即将结算！", Color.red, MeatFullTipDuration,false);
            await uiCountDown.PlayCountdownAsync(MeatFullCountdownTexts, MeatFullCountdownDuration);

            // 开始结算
            _roundFlow.StartSettlement();
            if (_roundFlow.RoundContext.HasHiddenMap)
                await _uiManager.OpenAsync<UIPopupEnterHiddenMap>();
            else
                await _uiManager.OpenAsync<UIPopupNormalSettlement>();
        }
        else
        {
            // 播放狩猎进度提示动画
            int percent = args.CompletedScaleCount * 100 / args.TotalScaleCount;
            uiGameplay.PlayTip($"狩猎进度已完成{percent}%", Color.yellow, 6f);
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