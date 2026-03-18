using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 瞄准镜道具处理器
/// </summary>
public class PropAimAssistHandler : BasePropHandler
{
    /// <summary>
    /// 瞄准镜预制体
    /// </summary>
    private GameObject _aimAssisPrefab;

    /// <summary>
    /// 手势引导预制体
    /// </summary>
    private GameObject _handGuidePrefab;

    /// <summary>
    /// 瞄准器控制器
    /// </summary>
    private AimAssistController _aimAssisController;

    /// <summary>
    /// 手势引导控制器
    /// </summary>
    private AimAssistHandGuideController _handGuideController;

    /// <summary>
    /// 是否有目标
    /// </summary>
    private bool _hasTarget;

    private EventManager _eventManager;
    private UIManager _uiManager;
    private AnimalManager _animalManager;
    private PlayerControlManager _playerControlManager;
    
    protected override void OnInit()
    {
        _eventManager = GameServiceLocator.EventManager;
        _uiManager = GameServiceLocator.UIManager;
        _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
        _playerControlManager = GameServiceLocator.GetRoundManager<PlayerControlManager>();
        
        _aimAssisPrefab = ConfigManager.PropRefSo.GetPropPrefab(PropData.ID);
        _handGuidePrefab = ConfigManager.PropRefSo.GetPropEffectPrefab(PropData.ID);
    }

    protected override UniTask OnPropStart()
    {
        RegisterEvents();

        _hasTarget = false;

         _aimAssisController = EffectManager.PlayLoop(_aimAssisPrefab).GetComponent<AimAssistController>();
        _aimAssisController.SetTarget(null);

        _handGuideController = Object.Instantiate(_handGuidePrefab).GetComponent<AimAssistHandGuideController>();
        _handGuideController.SetVisible(false);

        var _uiGameplay = _uiManager.GetUI<UIGameplay>("UIGameplay");
        _uiGameplay.PlayTip("点击动物自动瞄准射击", Color.green, PropData.Duration);

        _playerControlManager.SwitchToAimAssist();

        return UniTask.CompletedTask;
    }

    protected override void OnPropUpdate(float dt)
    {
        _aimAssisController.UpdatePosition(dt);

        if (_hasTarget)
            return;

        var guideTarget = GetGuideTarget();
        var shouldShow = guideTarget != null;
        _handGuideController.SetVisible(shouldShow);
        if (shouldShow)
            _handGuideController.UpdatePosition(guideTarget.Collider.bounds.center, dt);
    }

    protected override void OnPropEnd()
    {
        _playerControlManager.SwitchToJoystick();

        EffectManager.Stop(_aimAssisController.gameObject, EffectStopMode.Graceful);
        _aimAssisController = null; 

        Object.Destroy(_handGuideController.gameObject);
        _handGuideController = null;

        UnregisterEvents();
    }

    #region 私有方法
    /// <summary>
    /// 获取手势引导目标（最近、活着、屏幕内的动物）
    /// </summary>
    private BaseAnimalBehaviour GetGuideTarget()
    {
        var animal = _animalManager.GetNearestVisibleAnimal(Player.position);
        if (animal == null || animal.Health.IsDead)
            return null;
        return animal;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _eventManager.AddListener(PlayerControlEvents.TargetSelected, OnTargetSelected);
        _eventManager.AddListener(PlayerControlEvents.TargetLost, OnTargetLost);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(PlayerControlEvents.TargetSelected, OnTargetSelected);
        _eventManager.RemoveListener(PlayerControlEvents.TargetLost, OnTargetLost);
    }

    /// <summary>
    /// 目标选中事件回调
    /// </summary>
    private void OnTargetSelected(TargetSelectedEventArgs args)
    {
        _hasTarget = true;
        _aimAssisController.SetTarget(args.Target?.transform);
        _handGuideController.SetVisible(false);
    }

    /// <summary>
    /// 目标丢失事件回调
    /// </summary>
    private void OnTargetLost(TargetLostEventArgs args)
    {
        _hasTarget = false;
        _aimAssisController.SetTarget(null);
    }
    #endregion
}
