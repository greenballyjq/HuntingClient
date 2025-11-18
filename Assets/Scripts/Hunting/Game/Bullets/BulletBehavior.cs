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
        private EventManager Event => GameServiceLocator.Event;

        #region 对象池接口
        public string PrefabPath { get; set; }

        public void OnSpawned()
        {
            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            _lifeTimer = 0;
            _moveDirection = Vector3.zero;

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

        private void Update()
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
    }
}
