using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using GameFramework.Core.Pool;
using GameFramework.Manager;
using Hunting.Events;
using Hunting.Game.Animal;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹行为
/// </summary>
public class BulletBehavior : MonoBehaviour, IPoolItem
{
    /// <summary>
    /// 子弹数据
    /// </summary>
    private Bullet _bulletData;

    /// <summary>
    /// 子弹运行时上下文
    /// </summary>
    private BulletRuntimeContext _bulletRuntimeContext;

    /// <summary>
    /// 子弹效果
    /// </summary>
    private IBulletEffect _bulletEffect;

    /// <summary>
    /// 移动方向
    /// </summary>
    private Vector3 _moveDirection;

    /// <summary>
    /// 子弹最大飞行时长 TODO: 临时待调整
    /// </summary>
    private const float MaxLifetime = 10f;

    /// <summary>
    /// 生命周期计时 TODO: 临时待调整
    /// </summary>
    private float _lifeTimer;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 特效管理器
    /// </summary>
    private EffectManager _effectManager => GameServiceLocator.GetFrameworkManager<EffectManager>();

    #region 对象池接口
    public void OnSpawned()
    {
        gameObject.SetActive(true);
    }

    public void OnDespawned()
    {
        _lifeTimer = 0;
        _moveDirection = Vector3.zero;
        _bulletRuntimeContext = null;
        _bulletEffect = null;

        gameObject.SetActive(false);
    }
    #endregion

    /// <summary>
    /// 子弹初始化
    /// </summary>
    /// <param name="bulletData">子弹数据</param>
    /// <param name="finalDamage">最终伤害</param>
    public void Init(Bullet bulletData, float finalDamage)
    {
        // 初始化数据
        _bulletData = bulletData;
        _moveDirection = transform.forward;
        _lifeTimer = MaxLifetime;

        _bulletRuntimeContext = new BulletRuntimeContext
        {
            FinalDamage = finalDamage
        };

        // 根据子弹类型创建效果
        _bulletEffect = BulletEffectFactory.CreateEffect(bulletData.BulletType, bulletData);
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    public void DoUpdate(float dt)
    {
        UpdateMovement();
        UpdateLifetime();
        CheckCollision();
    }

    #region 私有方法
    /// <summary>
    /// 更新移动
    /// </summary>
    private void UpdateMovement()
    {
        transform.position += _moveDirection * _bulletData.MoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 更新生命周期
    /// </summary>
    private void UpdateLifetime()
    {
        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f)
        {
            TriggerBulletDestroyed(new BulletDestroyedEventArgs
            {
                BulletData = _bulletData,
                Bullet = this
            });
        }
    }

    /// <summary>
    /// 检查碰撞
    /// </summary>
    private void CheckCollision()
    {
        // 使用射线检测当前帧移动路径
        float moveDistance = _bulletData.MoveSpeed * Time.deltaTime;
        if (Physics.Raycast(transform.position, _moveDirection, out RaycastHit hit, moveDistance, LayerMask.GetMask("Animal")))
        {
            // 尝试获取IDamageable组件（支持所有可攻击的目标）
            if (hit.collider.TryGetComponent(out IDamageable damageable))
                HandleHit(damageable, hit.point, hit.normal);
        }
    }

    /// <summary>
    /// 处理命中可攻击目标
    /// </summary>
    /// <param name="damageable">命中的可攻击目标</param>
    /// <param name="hitPoint">命中位置</param>
    /// <param name="hitNormal">命中法线</param>
    private void HandleHit(IDamageable damageable, Vector3 hitPoint, Vector3 hitNormal)
    {
        // 对目标造成伤害
        damageable.TakeDamage(_bulletRuntimeContext.FinalDamage, hitPoint, hitNormal);
        
        // 播放击中特效
        PlayHitEffect(hitPoint);
        
        // 执行效果，获取所有命中的目标
        List<IDamageable> hitAnimals = _bulletEffect.OnHit(_bulletRuntimeContext, new BulletHitInfo { 
            HitPoint = hitPoint,
            PrimaryTarget = damageable,
        });

        // 触发命中事件
        TriggerBulletHit(new BulletHitEventArgs
        {
            Bullet = this,
            BulletData = _bulletData,
            HitPoint = hitPoint,
            Targets = hitAnimals
        });

        // 触发销毁事件
        TriggerBulletDestroyed(new BulletDestroyedEventArgs
        {
            BulletData = _bulletData,
            Bullet = this
        });
    }

    /// <summary>
    /// 播放击中特效
    /// </summary>
    /// <param name="hitPoint">命中位置</param>
    private void PlayHitEffect(Vector3 hitPoint)
    {
        _effectManager.PlayOneShotAsync(_bulletData.EffectPrefabResourcePath, hitPoint, Quaternion.identity).Forget();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 触发子弹销毁事件
    /// </summary>
    private void TriggerBulletDestroyed(BulletDestroyedEventArgs args)
    {
        _eventManager.Trigger(BulletEvents.BulletDestroyed,args );
    }

    /// <summary>
    /// 触发子弹命中事件
    /// </summary>
    private void TriggerBulletHit(BulletHitEventArgs args)
    {
        _eventManager.Trigger(BulletEvents.BulletHit, args);
    }
    #endregion
}
