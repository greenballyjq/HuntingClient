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
    /// 最小锁定距离
    /// </summary>
    private float _minLockDistance;

    /// <summary>
    /// 最大锁定距离
    /// </summary>
    private float _maxLockDistance;

    /// <summary>
    /// 当前锁定的目标
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

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager => GameServiceLocator.UIManager;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 特效管理器
    /// </summary>
    private EffectManager _effectManager => GameServiceLocator.EffectManager;

    /// <summary>
    /// 玩家控制管理器
    /// </summary>
    private PlayerControlManager _playerControlManager => GameServiceLocator.GetRoundManager<PlayerControlManager>();

    /// <summary>
    /// 道具效果开始
    /// </summary>
    public async void OnPropStart(PropContext context)
    {
        RegisterEvents();

        // 读取配置参数
        PropAimAssist parameter = _configManager.GetPropAimAssist(context.PropData.ParamTableID);
        _minLockDistance = parameter.MinLockDistance;
        _maxLockDistance = parameter.MaxLockDistance;

        // 获取特效路径
        string effectPath = parameter.EffectPrefabPath;

        // 播放循环特效
        _effectInstance = await _effectManager.PlayLoopAsync(effectPath, Vector3.zero, Quaternion.identity);

        // 获取特效控制器组件
        _effectController = _effectInstance.GetComponent<AimAssistEffectController>();
        _effectController.SetTarget(null);

        // 播放道具使用文字提示动画
        _uiManager.GetUI<UIGameplay>("UIGameplay").PlayPropUseTipAnimationAsync().Forget();

        // 切换到指哪打哪模式
        _playerControlManager.SwitchToAimAssist(_minLockDistance, _maxLockDistance);
    }

    /// <summary>
    /// 道具效果更新
    /// </summary>
    public void OnPropUpdate(PropContext context, float dt)
    {
        if (_effectController == null)
            return;

        // 检查目标是否仍然有效
        if (_currentTarget != null && _currentTarget.gameObject == null)
        {
            _currentTarget = null;
            _effectController.SetTarget(null);
        }

        // 由道具管理器驱动更新
        _effectController.UpdatePosition(dt);
    }

    /// <summary>
    /// 道具效果结束
    /// </summary>
    public void OnPropEnd(PropContext context)
    {
        // 切换回默认射击模式
        _playerControlManager.SwitchToDefaultShooting();

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