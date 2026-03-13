using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 辅助瞄准控制处理器
/// </summary>
public class AimAssistControlHandler : BaseControlHandler
{
    /// <summary>
    /// 屏幕检测计时器
    /// </summary>
    private float _screenCheckTimer;

    /// <summary>
    /// 玩家是否选择过目标
    /// </summary>
    private bool _hasPlayerSelectedTarget;

    /// <summary>
    /// 当前目标
    /// </summary>
    private BaseAnimalBehaviour _currentTarget;

    /// <summary>
    /// 平滑目标速度
    /// </summary>
    private Vector3 _smoothedTargetVelocity;

    private EventManager _eventManager;
    private HuntingConfigManager _configManager;
    private AnimalManager _animalManager;

    private const float MIN_LOCK_DISTANCE = 0f;
    private const float MAX_LOCK_DISTANCE = 50f;
    private const float SCREEN_CHECK_INTERVAL = 0.1f;
    private const float MIN_YAW_ANGLE = -80f;
    private const float MAX_YAW_ANGLE = 80f;
    private const float ROTATION_SPEED = 120f;
    private const float VELOCITY_SMOOTH_SPEED = 6f;

    protected override void OnInit()
    {
        _eventManager = GameServiceLocator.EventManager;
        _configManager = GameServiceLocator.ConfigManager;
        _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
    }

    protected override void OnControlStart()
    {
        RegisterEvents();

        PlayerWeapon = WeaponManager.PlayerWeapon;

        _hasPlayerSelectedTarget = false;
        _currentTarget = null;
        _screenCheckTimer = 0f;
        _smoothedTargetVelocity = Vector3.zero;

        InputManager.SwitchToSelectTargetMode();
    }

    protected override void OnControlUpdate(float dt)
    {
        if (_currentTarget != null)
        {
            if (IsTargetLost(dt))
            {
                BaseAnimalBehaviour lost = _currentTarget;
                _currentTarget = null;
                TriggerTargetLost(new TargetLostEventArgs 
                {
                    LostTarget = lost 
                });
            }
            else
            {
                TryFire(dt);
            }
        }
        else if (_hasPlayerSelectedTarget)
        {
            var animal = _animalManager.GetNearestVisibleAnimal(PlayerWeapon.transform.position);
            if (animal != null && IsInRange(animal))
            {
                _currentTarget = animal;
                _smoothedTargetVelocity = animal.Moveable.CurrentSpeed * animal.Moveable.CurrentMoveDirection;
                TriggerTargetSelected(new TargetSelectedEventArgs 
                { 
                    Target = animal 
                });
                TryFire(dt);
            }
        }
    }

    protected override void OnControlEnd()
    {
        if (_currentTarget != null)
        {
            BaseAnimalBehaviour lost = _currentTarget;
            _currentTarget = null;
            TriggerTargetLost(new TargetLostEventArgs 
            {
                LostTarget = lost 
            });
        }

        UnregisterEvents();
    }

    #region 私有方法
    /// <summary>
    /// 目标是否已丢失
    /// </summary>
    private bool IsTargetLost(float dt)
    {
        _screenCheckTimer += dt;
        if (_screenCheckTimer >= SCREEN_CHECK_INTERVAL)
        {
            _screenCheckTimer = 0f;
            if (!ScreenUtils.IsVisible(_currentTarget.transform, CameraManager.MainCamera))
                return true;
        }

        if (!IsInRange(_currentTarget)) 
            return true;

        if (_currentTarget.Health.IsDead) 
            return true;

        if (_currentTarget == null) 
            return true;
        
        return false;
    }

    /// <summary>
    /// 目标是否在攻击范围内
    /// </summary>
    private bool IsInRange(BaseAnimalBehaviour animal)
    {
        float sq = Vector3.SqrMagnitude(animal.transform.position - PlayerWeapon.FirePointPosition);
        float minSq = MIN_LOCK_DISTANCE * MIN_LOCK_DISTANCE;
        float maxSq = MAX_LOCK_DISTANCE * MAX_LOCK_DISTANCE;
        return sq >= minSq && sq <= maxSq;
    }

    /// <summary>
    /// 尝试开火
    /// </summary>
    private void TryFire(float dt)
    {
        Aim(dt);
        Fire();
    }

    /// <summary>
    /// 瞄准
    /// </summary>
    private void Aim(float dt)
    {
        if (_currentTarget == null)
            return;

        Vector3 origin = PlayerWeapon.FirePointPosition;
        Vector3 target = _currentTarget.transform.position;
        Vector3 velocity = _currentTarget.Moveable.CurrentSpeed * _currentTarget.Moveable.CurrentMoveDirection;
        _smoothedTargetVelocity = Vector3.Lerp(_smoothedTargetVelocity, velocity, VELOCITY_SMOOTH_SPEED * dt);

        float projectileSpeed = _configManager.GetBullet(PlayerWeapon.CurrentBulletID).MoveSpeed;
        var leadDir = Ballistics.CalculateLeadDirection(origin, target, _smoothedTargetVelocity, projectileSpeed);
        Vector3 direction = leadDir ?? (target - origin).normalized;
        direction.y = 0f;

        float yaw = Vector3.SignedAngle(Vector3.forward, direction.normalized, Vector3.up);
        yaw = Mathf.Clamp(yaw, MIN_YAW_ANGLE, MAX_YAW_ANGLE);
        direction = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;

        Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion smoothedRot = Quaternion.RotateTowards(PlayerWeapon.transform.rotation, targetRot, ROTATION_SPEED * dt);
        PlayerWeapon.transform.rotation = smoothedRot;
    }

    /// <summary>
    /// 开火
    /// </summary>
    private void Fire()
    {
        PlayerWeapon.TryFire();
    }

    /// <summary>
    /// 处理目标选择
    /// </summary>
    private void HandleTargetSelected()
    {
        Vector2 screenPos = InputManager.PointerScreenPosition;
        Ray ray = CameraManager.MainCamera.ScreenPointToRay(screenPos);
        int layerMask = LayerMask.GetMask("Animal");

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            return;

        BaseAnimalBehaviour animal = hit.collider.GetComponentInParent<BaseAnimalBehaviour>();
        if (animal == null || animal.Health.IsDead || !IsInRange(animal))
            return;

        _hasPlayerSelectedTarget = true;

        if (_currentTarget == null)
        {
            _currentTarget = animal;
            _smoothedTargetVelocity = animal.Moveable.CurrentSpeed * animal.Moveable.CurrentMoveDirection;

            TriggerTargetSelected(new TargetSelectedEventArgs 
            { 
                Target = animal 
            });
        }
        else if (_currentTarget != animal)
        {
            BaseAnimalBehaviour oldTarget = _currentTarget;
            _currentTarget = animal;
            _smoothedTargetVelocity = animal.Moveable.CurrentSpeed * animal.Moveable.CurrentMoveDirection;

            TriggerTargetChanged(new TargetChangedEventArgs 
            { 
                OldTarget = oldTarget, 
                NewTarget = animal 
            });

            TriggerTargetSelected(new TargetSelectedEventArgs 
            { 
                Target = animal 
            });
        }
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        InputManager.OnTargetSelected += HandleTargetSelected;
        _eventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    private void UnregisterEvents()
    {
        InputManager.OnTargetSelected -= HandleTargetSelected;
        _eventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalEnteredDeath);
    }

    /// <summary>
    /// 动物进入死亡事件回调
    /// </summary>
    private void OnAnimalEnteredDeath(AnimalEnteredDeathEventArgs args)
    {
        if (_currentTarget != null && args.Animal == _currentTarget)
        {
            BaseAnimalBehaviour lost = _currentTarget;
            _currentTarget = null;
            TriggerTargetLost(new TargetLostEventArgs {
                LostTarget = lost 
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