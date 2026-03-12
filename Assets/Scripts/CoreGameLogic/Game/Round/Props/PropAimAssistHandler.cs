using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 指哪打哪道具处理器
/// </summary>
public class PropAimAssistHandler : BasePropHandler
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

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager;

    /// <summary>
    /// 特效管理器
    /// </summary>
    private EffectManager _effectManager;

    /// <summary>
    /// 玩家控制管理器
    /// </summary>
    private PlayerControlManager _playerControlManager;

    /// <summary>
    /// 构造函数
    /// </summary>
    public PropAimAssistHandler()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _effectManager = GameServiceLocator.EffectManager;
        _playerControlManager = GameServiceLocator.GetRoundManager<PlayerControlManager>();
    }

    /// <summary>
    /// 道具开始钩子
    /// </summary>
    protected override UniTask OnPropStart(Prop propData)
    {
        RegisterEvents();

        GameObject effectPrefab = _configManager.PropRefSo.GetPropEffectPrefab(propData.ID);
        _effectInstance = _effectManager.PlayLoop(effectPrefab);

        _effectController = _effectInstance.GetComponent<AimAssistEffectController>();
        _effectController.SetTarget(null);

        _uiManager.GetUI<UIGameplay>("UIGameplay").PlayTipAnimationAsync("点击动物自动瞄准射击", Color.green, propData.Duration).Forget();

        _playerControlManager.SwitchToAimAssist();

        return UniTask.CompletedTask;
    }

    /// <summary>
    /// 道具更新钩子
    /// </summary>
    protected override void OnPropUpdate(float dt)
    {
        if (_currentTarget != null && _currentTarget.gameObject == null)
        {
            _currentTarget = null;
            _effectController.SetTarget(null);
        }

        _effectController?.UpdatePosition(dt);
    }

    /// <summary>
    /// 道具结束钩子
    /// </summary>
    protected override void OnPropEnd()
    {
        _playerControlManager.SwitchToJoystick();

        _effectManager.Stop(_effectInstance, EffectStopMode.Graceful);
        _effectInstance = null;

        _currentTarget = null;

        UnregisterEvents();
    }

    #region 私有方法

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
