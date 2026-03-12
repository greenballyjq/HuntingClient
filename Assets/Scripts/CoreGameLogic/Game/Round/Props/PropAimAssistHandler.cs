using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 指哪打哪道具处理器
/// </summary>
public class PropAimAssistHandler : IPropHandler
{
    /// <summary>
    /// 当前目标
    /// </summary>
    private Transform _currentTarget;

    /// <summary>
    /// 特效实例
    /// </summary>
    private GameObject _effectInstance;

    /// <summary>
    /// 特效控制器
    /// </summary>
    private AimAssistEffectController _effectController;

    private EventManager _eventManager;
    private UIManager _uiManager;
    private EffectManager _effectManager;
    private HuntingConfigManager _configManager;
    private PlayerControlManager _playerControlManager;

    public PropAimAssistHandler()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _configManager = GameServiceLocator.ConfigManager;
        _effectManager = GameServiceLocator.EffectManager;
        _playerControlManager = GameServiceLocator.GetRoundManager<PlayerControlManager>();
    }

    /// <summary>
    /// 道具效果开始
    /// </summary>
    public async void OnPropStart(PropContext context)
    {
        RegisterEvents();

        // 读取配置参数
        PropAimAssist parameter = _configManager.GetPropAimAssist(context.PropData.ParamTableID);

        // 获取特效路径
        string effectPath = parameter.EffectPrefabPath;

        // 播放循环特效
        _effectInstance = await _effectManager.PlayLoopAsync(effectPath);

        // 获取特效控制器组件
        _effectController = _effectInstance.GetComponent<AimAssistEffectController>();
        _effectController.SetTarget(null);

        // 播放道具使用文字提示动画
        _uiManager.GetUI<UIGameplay>("UIGameplay").PlayTipAnimationAsync("点击动物自动瞄准射击", Color.green, context.PropData.Duration).Forget();

        // 切换到指哪打哪模式
        _playerControlManager.SwitchToAimAssist();
    }

    /// <summary>
    /// 道具效果更新
    /// </summary>
    public void OnPropUpdate(PropContext context, float dt)
    {
        // 检查目标是否仍然有效
        if (_currentTarget != null && _currentTarget.gameObject == null)
        {
            _currentTarget = null;
            _effectController.SetTarget(null);
        }

        _effectController?.UpdatePosition(dt);
    }

    /// <summary>
    /// 道具效果结束
    /// </summary>
    public void OnPropEnd(PropContext context)
    {
        // 切换回摇杆控制模式
        _playerControlManager.SwitchToJoystick();

        // 停止并回收特效
        _effectManager.Stop(_effectInstance, EffectStopMode.Graceful);
        _effectInstance = null;

        _currentTarget = null;

        UnregisterEvents();
    }

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(PlayerControlEvents.TargetSelected, OnTargetSelected);
        _eventManager.AddListener(PlayerControlEvents.TargetLost, OnTargetLost);
        _eventManager.AddListener(PlayerControlEvents.TargetChanged, OnTargetChanged);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(PlayerControlEvents.TargetSelected, OnTargetSelected);
        _eventManager.RemoveListener(PlayerControlEvents.TargetLost, OnTargetLost);
        _eventManager.RemoveListener(PlayerControlEvents.TargetChanged, OnTargetChanged);
    }

    /// <summary>
    /// 目标已选中事件回调
    /// </summary>
    private void OnTargetSelected(TargetSelectedEventArgs args)
    {
        _currentTarget = args.Target;
        _effectController.SetTarget(_currentTarget);
    }

    /// <summary>
    /// 目标已丢失事件回调
    /// </summary>
    private void OnTargetLost(TargetLostEventArgs args)
    {
        if (_currentTarget == args.LostTarget)
        {
            _currentTarget = null;
            _effectController.SetTarget(null);
        }
    }

    /// <summary>
    /// 目标已切换事件回调
    /// </summary>
    private void OnTargetChanged(TargetChangedEventArgs args)
    {
        _currentTarget = args.NewTarget;
        _effectController.SetTarget(_currentTarget);
    }
    #endregion
}