using cfg.HuntingConfig;
using GameFramework.Core;
using GameFramework.Core.Pool;
using Hunting.Events;
using Hunting.Game.Animal;
using Hunting.Game.Bullets.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace Hunting.Game.Bullets
{
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
        /// 子弹最大飞行时长 TODO: 之后配置化
        /// </summary>
        private const float MaxLifetime = 10f;

        /// <summary>
        /// 生命周期计时 TODO: 之后配置化
        /// </summary>
        private float _lifeTimer;

        /// <summary>
        /// 预制体资源路径
        /// </summary>
        public string PrefabPath { get; set; }

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 子弹初始化
        /// </summary>
        /// <param name="bulletData">子弹数据</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="direction">子弹方向</param>
        /// <param name="finalDamage">最终伤害</param>
        public void Init(Bullet bulletData, Vector3 startPosition, Vector3 direction, float finalDamage)
        {
            Debug.LogWarning("init");

            // 初始化数据
            _bulletData = bulletData;
            _moveDirection = direction.normalized;
            _lifeTimer = MaxLifetime;
            
            _bulletRuntimeContext = new BulletRuntimeContext
            {
                FinalDamage = finalDamage
            };

            // 根据子弹类型创建效果
            _bulletEffect = BulletEffectFactory.CreateEffect(bulletData.BulletType, bulletData);
        }

        private void Update()
        {
            Debug.LogWarning("Update");
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
                if (hit.collider.TryGetComponent(out AnimalBehavior animal))
                    HandleHit(animal, hit.point);
            }
        }

        /// <summary>
        /// 处理命中
        /// </summary>
        /// <param name="animal">命中的动物</param>
        /// <param name="hitPoint">命中位置</param>
        private void HandleHit(AnimalBehavior animal, Vector3 hitPoint)
        {
            // 执行效果，获取所有命中的动物
            List<AnimalBehavior> hitAnimals = _bulletEffect.OnHit(_bulletRuntimeContext, new BulletHitInfo { 
                HitPoint = hitPoint,
                PrimaryTarget = animal,
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
        #endregion

        #region 事件相关
        /// <summary>
        /// 触发子弹销毁事件
        /// </summary>
        private void TriggerBulletDestroyed(BulletDestroyedEventArgs args)
        {
            Event.Trigger(BulletEvents.BulletDestroyed,args );
        }

        /// <summary>
        /// 触发子弹命中事件
        /// </summary>
        private void TriggerBulletHit(BulletHitEventArgs args)
        {
            Event.Trigger(BulletEvents.BulletHit, args);
        }
        #endregion

        /// <summary>
        /// 对象从池子取出后调用
        /// </summary>
        public void OnSpawned()
        {
            Debug.LogWarning("OnSpawned");
        }

        /// <summary>
        /// 对象回收到池子之前调用
        /// </summary>
        public void OnDespawned()
        {
            _lifeTimer = 0;
            _moveDirection = Vector3.zero;
            _bulletRuntimeContext = null;
        }
    }
}
