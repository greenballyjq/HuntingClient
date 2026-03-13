using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 瞄准镜道具处理器
/// </summary>
public class PropAimAssistHandler : BasePropHandler
{
    /// <summary>
    /// 道具参数
    /// </summary>
    private PropAimAssist _propParam;

    /// <summary>
    /// 瞄准镜预制体
    /// </summary>
    private GameObject _aimAssisPrefab;

    /// <summary>
    /// 瞄准器实例
    /// </summary>
    private GameObject _aimAssisInstance;

    /// <summary>
    /// 瞄准器控制器
    /// </summary>
    private AimAssistController _aimAssisController;

    private EventManager _eventManager;
    private UIManager _uiManager;
    private PlayerControlManager _playerControlManager;

    protected override void OnInit() 
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        EffectManager = GameServiceLocator.EffectManager;
        _playerControlManager = GameServiceLocator.GetRoundManager<PlayerControlManager>();

        _propParam = ConfigManager.GetPropAimAssist(PropData.ParamTableID);
        _aimAssisPrefab = ConfigManager.PropRefSo.GetPropEffectPrefab(PropData.ID);
    }

    protected override UniTask OnPropStart()
    {
        RegisterEvents();

        _aimAssisInstance = EffectManager.PlayLoop(_aimAssisPrefab);

        _aimAssisController = _aimAssisInstance.GetComponent<AimAssistController>();
        _aimAssisController.SetTarget(null);

        var _uiGameplay = _uiManager.GetUI<UIGameplay>("UIGameplay");
        _uiGameplay.PlayTipAnimationAsync("点击动物自动瞄准射击", Color.green, PropData.Duration).Forget();

        _playerControlManager.SwitchToAimAssist();

        return UniTask.CompletedTask;
    }

    protected override void OnPropUpdate(float dt)
    {
        _aimAssisController.UpdatePosition(dt);
    }


    protected override void OnPropEnd()
    {
        _playerControlManager.SwitchToJoystick();

        EffectManager.Stop(_aimAssisInstance, EffectStopMode.Graceful);

        _aimAssisInstance = null;

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
        _aimAssisController.SetTarget(args.Target?.transform);
    }

    /// <summary>
    /// 目标已丢失事件回调
    /// </summary>
    private void OnTargetLost(TargetLostEventArgs args)
    {
        _aimAssisController.SetTarget(null);
    }

    /// <summary>
    /// 目标已切换事件回调
    /// </summary>
    private void OnTargetChanged(TargetChangedEventArgs args)
    {
        _aimAssisController.SetTarget(args.NewTarget?.transform);
    }

    #endregion
}
