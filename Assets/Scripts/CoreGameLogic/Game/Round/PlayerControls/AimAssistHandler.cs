using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 指哪打哪控制处理器
/// </summary>
public class AimAssistHandler : BasePlayerControlHandler
{
    private EventManager _eventManager;
    private CameraManager _cameraManager;
    private InputManager _inputManager;
    private WeaponManager _weaponManager;

    private const float MIN_LOCK_DISTANCE = 0f;
    private const float MAX_LOCK_DISTANCE = 50f;

    /// <summary>
    /// 玩家武器
    /// </summary>
    private PlayerWeapon _playerWeapon;

    /// <summary>
    /// 当前目标
    /// </summary>
    private Transform _currentTarget;

    public AimAssistHandler()
    {
        _eventManager = GameServiceLocator.EventManager;
        _cameraManager = GameServiceLocator.GetAppManager<CameraManager>();
        _inputManager = GameServiceLocator.GetAppManager<InputManager>();
        _weaponManager = GameServiceLocator.GetRoundManager<WeaponManager>();
    }

    protected override void OnControlStart()
    {
        RegisterEvents();
        _inputManager.SwitchToSelectTargetMode();

        _playerWeapon = _weaponManager.PlayerWeapon;
    }

    protected override void OnControlUpdate(float deltaTime)
    {
        // 检查是否有目标
        if (_currentTarget == null)
            return;

        // 获取目标的碰撞器中心点
        Collider targetCollider = _currentTarget.GetComponent<Collider>();
        Vector3 targetPosition = targetCollider.bounds.center;

        // 检查目标距离
        float distance = Vector3.Distance(_playerWeapon.transform.position, targetPosition);
        if (distance < MIN_LOCK_DISTANCE || distance > MAX_LOCK_DISTANCE)
        {
            Transform lostTarget = _currentTarget;
            _currentTarget = null;
            TriggerTargetLost(new TargetLostEventArgs
            {
                LostTarget = lostTarget,
            });
            return;
        }

        // 控制武器朝向目标
        _playerWeapon.SetAimTarget(targetPosition);

        // 持续射击
        _playerWeapon.TryFire();
    }

    protected override void OnControlEnd()
    {
        UnregisterEvents();
        if (_currentTarget != null)
        {
            Transform lostTarget = _currentTarget;
            _currentTarget = null;
            TriggerTargetLost(new TargetLostEventArgs
            {
                LostTarget = lostTarget
            });
        }
    }

    #region 私有方法
    /// <summary>
    /// 处理目标选择
    /// </summary>
    private void HandleTargetSelected()
    {
        // 从屏幕坐标发射射线进行射线检测
        Vector2 screenPos = _inputManager.PointerScreenPosition;
        Ray ray = _cameraManager.MainCamera.ScreenPointToRay(screenPos);

        int layerMask = LayerMask.GetMask("Animal");
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Transform newTarget = hit.transform;
            
            // 获取目标的碰撞器中心点
            Collider targetCollider = newTarget.GetComponent<Collider>();
            Vector3 targetPosition = targetCollider != null ? targetCollider.bounds.center : newTarget.position;
            
            // 检查距离是否在范围内
            if (_playerWeapon != null)
            {
                float distance = Vector3.Distance(_playerWeapon.transform.position, targetPosition);
                if (distance < MIN_LOCK_DISTANCE || distance > MAX_LOCK_DISTANCE)
                    return;
            }

            BaseAnimalBehaviour animal = hit.transform.GetComponent<BaseAnimalBehaviour>();

            if (_currentTarget == null)
            {
                // 首次选择目标
                _currentTarget = newTarget;
                TriggerTargetSelected(new TargetSelectedEventArgs
                {
                    Target = newTarget,
                });
            }
            else if (_currentTarget != newTarget)
            {
                // 切换目标
                Transform oldTarget = _currentTarget;
                _currentTarget = newTarget;
                TriggerTargetChanged(new TargetChangedEventArgs
                {
                    OldTarget = oldTarget,
                    NewTarget = newTarget
                });
                TriggerTargetSelected(new TargetSelectedEventArgs
                {
                    Target = newTarget,
                });
            }
        }
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        _inputManager.OnTargetSelected += HandleTargetSelected;
        _eventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _inputManager.OnTargetSelected -= HandleTargetSelected;
        _eventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    /// <summary>
    /// 动物进入死亡事件回调
    /// </summary>
    private void OnAnimalEnteredDeath(AnimalEnteredDeathEventArgs args)
    {
        // 当死亡的动物是当前锁定目标时清除
        if (_currentTarget != null && args.Animal.transform == _currentTarget)
        {
            Transform lostTarget = _currentTarget;
            _currentTarget = null;
            TriggerTargetLost(new TargetLostEventArgs
            {
                LostTarget = lostTarget,
            });
        }
    }

    /// <summary>
    /// 触发目标已选中事件
    /// </summary>
    private void TriggerTargetSelected(TargetSelectedEventArgs args)
    {
        _eventManager.Trigger(PlayerControlEvents.TargetSelected, args);
    }

    /// <summary>
    /// 触发目标已丢失事件
    /// </summary>
    private void TriggerTargetLost(TargetLostEventArgs args)
    {
        _eventManager.Trigger(PlayerControlEvents.TargetLost, args);
    }

    /// <summary>
    /// 触发目标切换事件
    /// </summary>
    private void TriggerTargetChanged(TargetChangedEventArgs args)
    {
        _eventManager.Trigger(PlayerControlEvents.TargetChanged, args);
    }
    #endregion
}