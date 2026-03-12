using CoreGameLogic.Managers.AppManagers;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 技能武器类
/// </summary>
public class SkillWeapon : MonoBehaviour
{
    private HuntingSoundManager _soundManager;
    private BulletManager _bulletManager;
    private AnimalManager _animalManager;

    private const int DEFAULT_BULLT_ID = 1;
    private const float MIN_LOCK_DISTANCE = 0f;
    private const float MAX_LOCK_DISTANCE = 30f;
    private const float SCREEN_CHECK_INTERVAL = 0.1f;
    private const float ROTATION_SPEED = 60f;

    /// <summary>
    /// 开火点
    /// </summary>
    [SerializeField] private Transform _firePoint;

    /// <summary>
    /// 射击间隔（秒）
    /// </summary>
    private float _fireInterval;

    /// <summary>
    /// 开火计时器
    /// </summary>
    private float _fireTimer;

    /// <summary>
    /// 当前目标
    /// </summary>
    private BaseAnimalBehaviour _currentTarget;

    /// <summary>
    /// 屏幕检测计时器
    /// </summary>
    private float _screenCheckTimer;

    public void DoUpdate(float dt)
    {
        if (_currentTarget != null)
        {
            if (IsTargetLost(dt))
                _currentTarget = null;
            else
                TryFire(dt);
        }
        else
        {
            var animal = _animalManager.GetNearestVisibleAnimal(_firePoint.position);
            if (animal != null && IsInRange(animal))
            {
                _currentTarget = animal;
                TryFire(dt);
            }
        }
    }

    #region 公共方法
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="fireInterval">射击间隔（秒）</param>
    public void Init(float fireInterval)
    {
        RegisterServices();

        _fireInterval = fireInterval;
        _fireTimer = 0f;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServices()
    {
        _soundManager = GameServiceLocator.GetAppManager<HuntingSoundManager>();
        _bulletManager = GameServiceLocator.GetRoundManager<BulletManager>();
        _animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
    }

    /// <summary>
    /// 目标是否已丢失
    /// </summary>
    private bool IsTargetLost(float dt)
    {
        if (_currentTarget.Health.IsDead)
            return true;

        if (!IsInRange(_currentTarget))
            return true;

        _screenCheckTimer += dt;
        if (_screenCheckTimer >= SCREEN_CHECK_INTERVAL)
        {
            _screenCheckTimer = 0f;
            if (!ScreenUtils.IsVisible(_currentTarget.transform,Camera.main))
                return true;
        }

        return false;
    }

    /// <summary>
    /// 目标是否在攻击范围内
    /// </summary>
    private bool IsInRange(BaseAnimalBehaviour animal)
    {
        float sq = Vector3.SqrMagnitude(animal.transform.position - _firePoint.position);
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

        _fireTimer += dt;
        if (_fireTimer < _fireInterval)
            return;

        _fireTimer = 0f;

        Fire();
    }

    /// <summary>
    /// 瞄准
    /// </summary>
    private void Aim(float dt)
    {
        Vector3 origin = _firePoint.position;
        Vector3 target = _currentTarget.transform.position;
        Vector3 velocity = _currentTarget.Moveable.CurrentSpeed * _currentTarget.Moveable.CurrentMoveDirection;
        float projectileSpeed = _bulletManager.GetBullet(DEFAULT_BULLT_ID).MoveSpeed;

        var leadDir = Ballistics.CalculateLeadDirection(origin, target, velocity, projectileSpeed);
        Vector3 direction = leadDir ?? (target - origin).normalized;
        direction.y = 0f;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, ROTATION_SPEED * dt);
    }

    /// <summary>
    /// 开火
    /// </summary>
    private void Fire()
    {
        _bulletManager.SpawnBullet(DEFAULT_BULLT_ID, _firePoint.position, _firePoint.forward);
        _soundManager.PlayFireSound();
    }
    #endregion
}
