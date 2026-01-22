using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 指哪打哪控制处理器
/// </summary>
public class AimAssistHandler : IPlayerControlHandler
{
    /// <summary>
    /// 主武器
    /// </summary>
    private MainWeapon _weapon;

    /// <summary>
    /// 当前锁定的目标
    /// </summary>
    private Transform _currentTarget;

    /// <summary>
    /// 最小锁定距离
    /// </summary>
    private float _minLockDistance;

    /// <summary>
    /// 最大锁定距离
    /// </summary>
    private float _maxLockDistance;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager = GameServiceLocator.EventManager;

    /// <summary>
    /// 相机管理器
    /// </summary>
    private CameraManager _cameraManager => GameServiceLocator.GetAppManager<CameraManager>();

    /// <summary>
    /// 输入管理器
    /// </summary>
    private InputManager _inputManager => GameServiceLocator.GetAppManager<InputManager>();

    /// <summary>
    /// 武器管理器
    /// </summary>
    private WeaponManager _weaponManager => GameServiceLocator.GetRoundManager<WeaponManager>();

    public void OnControlStart()
    {
        _inputManager.SwitchToSelectTargetMode();
        RegisterEvents();
    }

    public void OnControlUpdate(float deltaTime)
    {
        // TODO: 待以后实现LoadingManager后移除
        if (_weapon == null)
        {
            _weapon = _weaponManager.GetMainWeapon();
            if (_weapon == null)
                return;
        }

        // 检查是否有目标
        if (_currentTarget == null)
            return;

        // 获取目标的碰撞器中心点
        Collider targetCollider = _currentTarget.GetComponent<Collider>();
        Vector3 targetPosition = targetCollider.bounds.center;

        // 检查目标距离
        float distance = Vector3.Distance(_weapon.transform.position, targetPosition);
        if (distance < _minLockDistance || distance > _maxLockDistance)
        {
            Transform lostTarget = _currentTarget;
            _currentTarget = null;
            TriggerTargetLost(new TargetLostEventArgs
            {
                LostTarget = lostTarget,
                Reason = ETargetLostReason.DistanceExceeded
            });
            return;
        }

        // 控制武器朝向目标
        _weapon.SetAimTarget(targetPosition);

        // 持续射击
        _weapon.Fire();
    }

    public void OnControlEnd()
    {
        UnregisterEvents();
        if (_currentTarget != null)
        {
            Transform lostTarget = _currentTarget;
            _currentTarget = null;
            TriggerTargetLost(new TargetLostEventArgs
            {
                LostTarget = lostTarget,
                Reason = ETargetLostReason.ControlEnded
            });
        }
    }

    #region 公共方法
    /// <summary>
    /// 设置最小锁定距离
    /// </summary>
    /// <param name="distance">最小锁定距离</param>
    public void SetMinLockDistance(float distance)
    {
        _minLockDistance = distance;
    }

    /// <summary>
    /// 设置最大锁定距离
    /// </summary>
    /// <param name="distance">最大锁定距离</param>
    public void SetMaxLockDistance(float distance)
    {
        _maxLockDistance = distance;
    }
    #endregion

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
            if (_weapon != null)
            {
                float distance = Vector3.Distance(_weapon.transform.position, targetPosition);
                if (distance < _minLockDistance || distance > _maxLockDistance)
                    return;
            }

            AnimalBehaviour animal = hit.transform.GetComponent<AnimalBehaviour>();

            if (_currentTarget == null)
            {
                // 首次选择目标
                _currentTarget = newTarget;
                TriggerTargetSelected(new TargetSelectedEventArgs
                {
                    Target = newTarget,
                    Animal = animal
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
                    Animal = animal
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
        _eventManager.AddListener(AnimalEvents.AnimalDying, OnAnimalDying);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        _inputManager.OnTargetSelected -= HandleTargetSelected;
        _eventManager.RemoveListener(AnimalEvents.AnimalDying, OnAnimalDying);
    }

    /// <summary>
    /// 动物进入死亡事件回调
    /// </summary>
    private void OnAnimalDying(AnimalDyingEventArgs args)
    {
        // 当死亡的动物是当前锁定目标时清除
        if (_currentTarget != null && args.Animal.transform == _currentTarget)
        {
            Transform lostTarget = _currentTarget;
            _currentTarget = null;
            TriggerTargetLost(new TargetLostEventArgs
            {
                LostTarget = lostTarget,
                Reason = ETargetLostReason.TargetDied
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
    /// 触发目标已切换事件
    /// </summary>
    private void TriggerTargetChanged(TargetChangedEventArgs args)
    {
        _eventManager.Trigger(PlayerControlEvents.TargetChanged, args);
    }
    #endregion
}